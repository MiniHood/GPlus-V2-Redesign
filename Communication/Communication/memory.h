#pragma once
#include <psapi.h>

// credits to https://github.com/Gaztoof/GMod-SDK/
char* GetRealFromRelative(char* address, int offset, int instructionSize, bool isRelative) // Address must be an instruction, not a pointer! And offset = the offset to the bytes you want to retrieve.
{
#ifdef _WIN64
    isRelative = true;
#endif
    char* instruction = address + offset;
    if (!isRelative)
    {
        return *(char**)(instruction);
    }

    int relativeAddress = *(int*)(instruction);
    char* realAddress = address + instructionSize + relativeAddress;
    return realAddress;
}

typedef PVOID(*CreateInterfaceFn)(const char* Name, int* ReturnCode);
PVOID GetInterface(const char* moduleName, const char* interfaceName)
{
    CreateInterfaceFn createInterface = (CreateInterfaceFn)GetProcAddress(GetModuleHandleA(moduleName), "CreateInterface");
    return createInterface(interfaceName, NULL);
}

static auto generateBadCharTable(std::string_view pattern) noexcept
{
    std::array<std::size_t, 256> table;

    auto lastWildcard = pattern.rfind('?');
    if (lastWildcard == std::string_view::npos)
        lastWildcard = 0;

    const auto defaultShift = (std::max)(std::size_t(1), pattern.length() - 1 - lastWildcard);
    table.fill(defaultShift);

    for (auto i = lastWildcard; i < pattern.length() - 1; ++i)
        table[static_cast<std::uint8_t>(pattern[i])] = pattern.length() - 1 - i;

    return table;
}

const char* findPattern(const char* moduleName, std::string_view pattern, std::string patternName) noexcept
{
    PVOID moduleBase = 0;
    std::size_t moduleSize = 0;
    if (HMODULE handle = GetModuleHandleA(moduleName))
        if (MODULEINFO moduleInfo; GetModuleInformation(GetCurrentProcess(), handle, &moduleInfo, sizeof(moduleInfo)))
        {
            moduleBase = moduleInfo.lpBaseOfDll;
            moduleSize = moduleInfo.SizeOfImage;
        }


    if (moduleBase && moduleSize) {
        int lastIdx = pattern.length() - 1;
        const auto badCharTable = generateBadCharTable(pattern);

        auto start = static_cast<const char*>(moduleBase);
        const auto end = start + moduleSize - pattern.length();

        while (start <= end) {
            int i = lastIdx;
            while (i >= 0 && (pattern[i] == '?' || start[i] == pattern[i]))
                --i;

            if (i < 0)
            {
                return start;
            }

            start += badCharTable[static_cast<std::uint8_t>(start[lastIdx])];
        }
    }

    return 0;
}

template<typename T>
T* GetVMT(uintptr_t address, int index, uintptr_t offset) // Address must be a VTable pointer, not a VTable !
{
#ifdef _WIN64
    uintptr_t step = 3;
    uintptr_t instructionSize = 7;
    uintptr_t instruction = ((*(uintptr_t**)(address))[index] + offset);

    uintptr_t relativeAddress = *(DWORD*)(instruction + step);
    uintptr_t realAddress = instruction + instructionSize + relativeAddress;
    return *(T**)(realAddress);
#else
    uintptr_t instruction = ((*(uintptr_t**)(address))[index] + offset);
    return *(T**)(*(uintptr_t*)(instruction));
#endif
}
template<typename T>
T* GetVMT(uintptr_t address, uintptr_t offset) // This doesn't reads from the VMT, address must be the function's base ! Not a pointer!
{
#ifdef _WIN64
    uintptr_t step = 3;
    uintptr_t instructionSize = 7;
    uintptr_t instruction = address + offset;

    uintptr_t relativeAddress = *(DWORD*)(instruction + step);
    uintptr_t realAddress = instruction + instructionSize + relativeAddress;
    return *(T**)(realAddress);
#else
    uintptr_t instruction = (address + offset);
    return *(T**)(*(uintptr_t*)(instruction));
#endif
}