#define _CRT_SECURE_NO_WARNINGS
#include <Windows.h>
#include "steam_api.h" // Steamworks SDK headers
#include <stdint.h>
#include <thread>
#include <chrono>
#include <cstdio>

// Structure to send via WM_COPYDATA
struct SteamMessage
{
    uint64_t steamID;
    uint32_t processID;
};

// Sends SteamID + PID to a C# window given its HWND
void SendSteamIDToCSharp(HWND hwnd, uint64_t steamID, uint32_t pid)
{
    SteamMessage msg;
    msg.steamID = steamID;
    msg.processID = pid;

    COPYDATASTRUCT cds;
    cds.dwData = 1; // custom identifier
    cds.cbData = sizeof(msg);
    cds.lpData = &msg;

    SendMessage(hwnd, WM_COPYDATA, 0, (LPARAM)&cds);
}

HWND GetCSharpHWND()
{
    return FindWindowA(nullptr, "Form1");
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        std::thread([]() {
            std::this_thread::sleep_for(std::chrono::seconds(3));

            ISteamUser* user = SteamUser();
            if (user && user->BLoggedOn())
            {
                CSteamID steamID = user->GetSteamID();
                uint32_t pid = GetCurrentProcessId();

                char buffer[128];
                sprintf_s(buffer, "%llu|%u", steamID.ConvertToUint64(), pid);
                OutputDebugStringA(buffer);

                HWND hwnd = GetCSharpHWND();
                if (hwnd)
                {
                    SendSteamIDToCSharp(hwnd, steamID.ConvertToUint64(), pid);
                }
            }
            else
            {
                MessageBoxA(nullptr, "SteamAPI not initialized or not logged in.", "Error", MB_OK);
            }

            }).detach();

        break;
    }
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
