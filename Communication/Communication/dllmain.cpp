#define WIN32_LEAN_AND_MEAN
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <thread>
#include <mutex>
#include <queue>
#include <string>
#include <iostream>
#include "json.hpp"

// --- SDK headers ---
#include "CLuaInterface.h"
#include "CLuaShared.h"
#include "CGlobalVarsBase.h"
#include "CClientEntityList.h"
#include "CEngineClient.h"
#include "CVar.h"
#include "memory.h"
#include "CHLClient.h"

#pragma comment(lib, "ws2_32.lib")

#define SERVER_IP "127.0.0.1"
#define SERVER_PORT 8080

using json = nlohmann::json;

// --- Globals ---
SOCKET sock;
std::mutex luaMutex;
std::queue<std::string> luaQueue;
std::mutex luaQueueMutex;
bool luaReady = false;

// --- Debug helper ---
void DebugLog(const std::string& msg)
{
    OutputDebugStringA((msg + "\n").c_str());
    std::cout << msg << std::endl;
}

// --- Lazy Accessors ---
CLuaShared* GetLuaShared()
{
    static CLuaShared* luaShared = nullptr;
    if (!luaShared) {
        HMODULE hLuaShared = GetModuleHandleA("lua_shared.dll");
        if (hLuaShared) {
            auto CreateInterface = reinterpret_cast<CLuaShared* (*)(const char*, int*)>(
                GetProcAddress(hLuaShared, "CreateInterface"));
            if (CreateInterface) {
                luaShared = CreateInterface("LUASHARED003", nullptr);
            }
        }
    }
    return luaShared;
}

CLuaInterface* GetLua()
{
    static CLuaInterface* lua = nullptr;
    if (!lua) {
        auto shared = GetLuaShared();
        if (shared) {
            lua = shared->GetLuaInterface(static_cast<unsigned char>(LuaInterfaceType::LUA_CLIENT));
        }
    }
    return lua;
}

CCvar* GetCVar()
{
    static CCvar* cvar = nullptr;
    if (!cvar) {
        cvar = (CCvar*)GetInterface("vstdlib.dll", "VEngineCvar007");
    }
    return cvar;
}

CEngineClient* GetEngineClient()
{
    static CEngineClient* engine = nullptr;
    if (!engine) {
        engine = (CEngineClient*)GetInterface("engine.dll", "VEngineClient015");
    }
    return engine;
}

CClientEntityList* GetClientEntityList()
{
    static CClientEntityList* ents = nullptr;
    if (!ents) {
        ents = (CClientEntityList*)GetInterface("client.dll", "VClientEntityList003");
    }
    return ents;
}

CHLClient* GetHLClient()
{
    static CHLClient* client = nullptr;
    if (!client) {
        client = (CHLClient*)GetInterface("client.dll", "VClient017");
    }
    return client;
}

CGlobalVarsBase* GetGlobalVars()
{
    static CGlobalVarsBase* globals = nullptr;
    if (!globals) {
        auto chl = GetHLClient();
        if (chl) {
            globals = GetVMT<CGlobalVarsBase>((uintptr_t)chl, 0, 0x94);
        }
    }
    return globals;
}

const char* GetServerName()
{
    if (!GetCVar()) return "(unknown)";
    ConVar* hostCvar = GetCVar()->FindVar("hostname");
    if (hostCvar && hostCvar->pszValueStr)
        return hostCvar->pszValueStr;
    return "(unknown)";
}

// --- Command response helper ---
void SendJson(const json& j)
{
    std::string msg = j.dump() + "\n";
    DebugLog("[SendJson] " + msg);
    send(sock, msg.c_str(), static_cast<int>(msg.size()), 0);
}

void SendCommandResponse(const std::string& command, const std::string& result, bool success)
{
    DWORD pid = GetCurrentProcessId();
    DebugLog("[SendCommandResponse] Command=" + command + " Success=" + std::to_string(success) + " Result=" + result);

    json resp = {
        {"PID", pid},
        {"Responses", { {
            {"Command", command},
            {"Result", result},
            {"Success", success}
        }}}
    };
    SendJson(resp);
}

// --- Command handler ---
void ProcessCommand(const json& cmd)
{
    if (!cmd.contains("Type")) {
        DebugLog("[ProcessCommand] Missing Type field");
        return;
    }

    std::string type = cmd["Type"];
    DebugLog("[ProcessCommand] Type=" + type);

    try
    {
        if (type == "GetServer")
        {
            DebugLog("[GetServer] Running...");
            std::string name = GetServerName() ? GetServerName() : "(unknown)";
            std::string ip = "(unknown)";

            auto eng = GetEngineClient();
            if (eng && eng->GetNetChannelInfo()) {
                const char* addr = eng->GetNetChannelInfo()->GetAddress();
                if (addr) ip = addr;
            }

            json resp = {
                {"PID", GetCurrentProcessId()},
                {"Command", "GetServer"},
                {"Result", { {"Name", name}, {"IP", ip} }},
                {"Success", true}
            };
            SendJson(resp);
        }
        else if (type == "GetPlayers")
        {
            DebugLog("[GetPlayers] Called");
            auto eng = GetEngineClient();
            auto ents = GetClientEntityList();
            auto globals = GetGlobalVars();

            if (!eng || !ents || !globals) {
                SendCommandResponse("GetPlayers", "Interfaces not ready (not in-game)", false);
                return;
            }

            std::vector<json> players;
            int maxClients = globals->maxClients;
            DebugLog("[GetPlayers] maxClients=" + std::to_string(maxClients));

            for (int i = 1; i <= maxClients; ++i) {
                C_BasePlayer* ent = (C_BasePlayer*)ents->GetClientEntity(i);
                if (!ent) continue;

                player_info_t info;
                if (eng->GetPlayerInfo(i, &info)) {
                    players.push_back({
                        {"Index", i},
                        {"Name", std::string(info.name)},
                        {"UserID", info.userID},
                        {"SteamID", info.guid}
                    });
                    DebugLog("[GetPlayers] Found player: " + std::string(info.name));
                }
            }

            json result = {
                {"PID", GetCurrentProcessId()},
                {"Command", "GetPlayers"},
                {"Players", players},
                {"Success", true}
            };
            SendJson(result);
        }
        else if (type == "Disconnect")
        {
            DebugLog("[Disconnect] Called");
            auto eng = GetEngineClient();
            if (eng) {
                eng->ClientCmd_Unrestricted("disconnect");
                SendCommandResponse("Disconnect", "Issued disconnect", true);
            } else {
                SendCommandResponse("Disconnect", "EngineClient not available", false);
            }
        }
        else if (type == "Connect")
        {
            DebugLog("[Connect] Called");
            if (!cmd.contains("Data")) {
                SendCommandResponse("Connect", "Missing IP", false);
                return;
            }

            std::string address = cmd["Data"].get<std::string>();
            DebugLog("[Connect] Address=" + address);

            auto eng = GetEngineClient();
            if (eng) {
                std::string command = "connect " + address;
                eng->ClientCmd_Unrestricted(command.c_str());
                SendCommandResponse("Connect", "Issued connect to " + address, true);
            } else {
                SendCommandResponse("Connect", "EngineClient not available", false);
            }
        }
    }
    catch (const std::exception& e)
    {
        DebugLog("[ProcessCommand] Exception: " + std::string(e.what()));
        SendCommandResponse(type, e.what(), false);
    }
}

// --- Lua worker ---
void LuaWorker()
{
    DWORD pid = GetCurrentProcessId();
    DebugLog("[LuaWorker] Started, PID=" + std::to_string(pid));

    while (true)
    {
        {
            std::lock_guard<std::mutex> luaLock(luaMutex);
            luaReady = (GetLua() != nullptr);
        }

        json status = { {"PID", pid}, {"LuaReady", luaReady}, {"Responses", json::array()} };
        SendJson(status);

        std::this_thread::sleep_for(std::chrono::milliseconds(100));
    }
}

// --- TCP connection ---
void ConnectToServer()
{
    DebugLog("[ConnectToServer] Starting...");

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        DebugLog("[ConnectToServer] WSAStartup failed");
        return;
    }

    sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (sock == INVALID_SOCKET) {
        DebugLog("[ConnectToServer] Failed to create socket");
        return;
    }

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(SERVER_PORT);
    inet_pton(AF_INET, SERVER_IP, &serverAddr.sin_addr);

    if (connect(sock, (sockaddr*)&serverAddr, sizeof(serverAddr)) != 0) {
        DebugLog("[ConnectToServer] Failed to connect to server");
        closesocket(sock);
        WSACleanup();
        return;
    }

    DWORD pid = GetCurrentProcessId();
    json reg = { {"PID", pid}, {"LuaReady", luaReady} };
    SendJson(reg);

    char buffer[4096];
    while (true) {
        int bytes = recv(sock, buffer, sizeof(buffer) - 1, 0);
        if (bytes <= 0) {
            DebugLog("[ConnectToServer] Connection closed");
            break;
        }

        buffer[bytes] = '\0';
        std::string msg(buffer);
        DebugLog("[ConnectToServer] Received: " + msg);

        try {
            auto j = json::parse(msg);
            if (j.contains("Commands") && j["Commands"].is_array()) {
                for (auto& cmd : j["Commands"])
                    ProcessCommand(cmd);
            }
        }
        catch (const std::exception& e) {
            DebugLog("[ConnectToServer] JSON parse failed: " + std::string(e.what()));
        }
    }

    closesocket(sock);
    WSACleanup();
    DebugLog("[ConnectToServer] Cleanup done");
}

// --- DLL entry ---
void Initialise()
{
    DebugLog("[Initialise] Bootstrap starting...");

    std::thread(ConnectToServer).detach();
    DebugLog("[Initialise] ConnectToServer started");
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        DebugLog("[DllMain] DLL_PROCESS_ATTACH");
        DisableThreadLibraryCalls(hModule);

        std::thread(Initialise).detach();

        std::thread(LuaWorker).detach();
        DebugLog("[Initialise] LuaWorker started");
    }
    return TRUE;
}
