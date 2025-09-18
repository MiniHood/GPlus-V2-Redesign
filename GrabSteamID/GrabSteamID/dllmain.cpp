// pipe_comm.cpp
// Compile as DLL (x86 or x64 to match target process)
#include <windows.h>
#include <string>
#include <vector>
#include <thread>
#include <chrono>
#include <iostream>
#include <unordered_map>
#include <functional>
#include <mutex>

// ---------- Helpers ----------
static bool ReadExact(HANDLE h, void* buffer, DWORD length)
{
    DWORD total = 0;
    BYTE* buf = static_cast<BYTE*>(buffer);
    while (total < length)
    {
        DWORD read = 0;
        if (!ReadFile(h, buf + total, length - total, &read, NULL))
            return false;
        if (read == 0) // remote closed
            return false;
        total += read;
    }
    return true;
}

static bool WriteExact(HANDLE h, const void* buffer, DWORD length)
{
    DWORD total = 0;
    const BYTE* buf = static_cast<const BYTE*>(buffer);
    while (total < length)
    {
        DWORD written = 0;
        if (!WriteFile(h, buf + total, length - total, &written, NULL))
            return false;
        if (written == 0)
            return false;
        total += written;
    }
    return true;
}

static bool SendMessageToHost(HANDLE h, const std::string& msg)
{
    uint32_t len = static_cast<uint32_t>(msg.size());
    BYTE header[4];
    header[0] = (BYTE)(len & 0xFF);
    header[1] = (BYTE)((len >> 8) & 0xFF);
    header[2] = (BYTE)((len >> 16) & 0xFF);
    header[3] = (BYTE)((len >> 24) & 0xFF);

    if (!WriteExact(h, header, 4)) return false;
    if (!WriteExact(h, msg.data(), len)) return false;
    return true;
}

// Naive JSON string extractor (demo-only)
static bool ExtractJsonStringValue(const std::string& json, const std::string& key, std::string& outValue)
{
    std::string pattern = "\"" + key + "\"";
    size_t pos = json.find(pattern);
    if (pos == std::string::npos) return false;
    pos = json.find(':', pos + pattern.size());
    if (pos == std::string::npos) return false;
    size_t q1 = json.find('"', pos);
    if (q1 == std::string::npos) return false;
    size_t q2 = json.find('"', q1 + 1);
    if (q2 == std::string::npos) return false;
    outValue = json.substr(q1 + 1, q2 - (q1 + 1));
    return true;
}

// Helper: build a JSON response with RequestId and Result (simple)
static std::string MakeResponse(const std::string& requestId, const std::string& result)
{
    return std::string("{\"RequestId\":\"") + requestId + "\",\"Result\":\"" + result + "\"}";
}

// ---------- Command registry ----------
using CommandHandler = std::function<void(const std::string& requestId, const std::string& payload, HANDLE pipe)>;

static std::unordered_map<std::string, CommandHandler> g_CommandHandlers;
static std::mutex g_CommandMutex;

static void RegisterCommand(const std::string& name, CommandHandler handler)
{
    std::lock_guard<std::mutex> lk(g_CommandMutex);
    g_CommandHandlers[name] = std::move(handler);
}

static bool DispatchCommand(const std::string& name, const std::string& requestId, const std::string& payload, HANDLE pipe)
{
    std::lock_guard<std::mutex> lk(g_CommandMutex);
    auto it = g_CommandHandlers.find(name);
    if (it == g_CommandHandlers.end()) return false;
    try
    {
        it->second(requestId, payload, pipe);
    }
    catch (...)
    {
        // best-effort: report error back
        SendMessageToHost(pipe, MakeResponse(requestId, "handler_exception"));
    }
    return true;
}

static void InitCommands()
{
    // Ping -> reply Pong
    RegisterCommand("Ping", [](const std::string& id, const std::string&, HANDLE pipe) {
        SendMessageToHost(pipe, MakeResponse(id, "Pong"));
        });

    // Quit -> try graceful close (WM_CLOSE) then fallback to ExitProcess
    RegisterCommand("Quit", [](const std::string& id, const std::string&, HANDLE pipe) {
        // respond immediately
        SendMessageToHost(pipe, MakeResponse(id, "quitting"));

        // find top-level visible window for this process and post WM_CLOSE
        DWORD pid = GetCurrentProcessId();
        HWND found = NULL;
        EnumWindows([](HWND hwnd, LPARAM lParam) -> BOOL {
            DWORD wpid = 0;
            GetWindowThreadProcessId(hwnd, &wpid);
            if (wpid == (DWORD)lParam && IsWindowVisible(hwnd) && GetWindow(hwnd, GW_OWNER) == NULL)
            {
                *((HWND*)(&lParam)) = hwnd; // store hwnd in lParam (works here)
                return FALSE;
            }
            return TRUE;
            }, (LPARAM)pid);

        HWND mainWnd = (HWND)pid; // lambda stored found hwnd back into lParam
        if (mainWnd != NULL && IsWindow(mainWnd))
        {
            PostMessageW(mainWnd, WM_CLOSE, 0, 0);
            // give it a short moment for graceful shutdown
            std::this_thread::sleep_for(std::chrono::milliseconds(250));
        }
        else
        {
            // fallback: force exit
            ExitProcess(0);
        }
        });

    // You can register more commands here:
    // RegisterCommand("DoThing", [](const std::string& id, const std::string& payload, HANDLE pipe) { ... });
}

// ---------- Pipe thread ----------
DWORD WINAPI PipeThread(LPVOID lpParameter)
{
    DWORD pid = GetCurrentProcessId();
    std::wstring pipeName = L"\\\\.\\pipe\\gplus_comm_pipe_" + std::to_wstring(pid);

    // init commands before connecting
    InitCommands();

    while (true)
    {
        HANDLE hPipe = CreateFileW(
            pipeName.c_str(),
            GENERIC_READ | GENERIC_WRITE,
            0,
            NULL,
            OPEN_EXISTING,
            0,
            NULL);

        if (hPipe != INVALID_HANDLE_VALUE)
        {
            // optional handshake: send hello with pid
            SendMessageToHost(hPipe, std::string("{\"type\":\"hello\",\"pid\":") + std::to_string(pid) + "}");

            while (true)
            {
                BYTE header[4];
                if (!ReadExact(hPipe, header, 4))
                    break;

                uint32_t payloadLen = (uint32_t)header[0]
                    | ((uint32_t)header[1] << 8)
                    | ((uint32_t)header[2] << 16)
                    | ((uint32_t)header[3] << 24);

                if (payloadLen == 0)
                    continue;

                std::vector<char> buf(payloadLen + 1);
                if (!ReadExact(hPipe, buf.data(), payloadLen))
                    break;

                buf[payloadLen] = '\0';
                std::string message(buf.data(), payloadLen);

                // parse basic fields: RequestId and Type
                std::string requestId;
                if (!ExtractJsonStringValue(message, "RequestId", requestId))
                {
                    // unsolicited / malformed: ack
                    SendMessageToHost(hPipe, std::string("{\"Error\":\"Missing RequestId\"}"));
                    continue;
                }

                std::string type;
                if (!ExtractJsonStringValue(message, "Type", type))
                {
                    // no type -> treat as echo request
                    SendMessageToHost(hPipe, MakeResponse(requestId, "no_type"));
                    continue;
                }

                // dispatch to registered handler
                if (!DispatchCommand(type, requestId, message, hPipe))
                {
                    // unknown command
                    SendMessageToHost(hPipe, std::string("{\"RequestId\":\"") + requestId + "\",\"Error\":\"UnknownCommand\"}");
                }
            }

            CloseHandle(hPipe);
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(200));
    }

    return 0;
}

// ---------- DllMain ----------
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        // Create a background thread to handle pipe comms.
        // Keep DllMain work minimal.
        HANDLE h = CreateThread(NULL, 0, PipeThread, NULL, 0, NULL);
        if (h) CloseHandle(h);
    }
    break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
