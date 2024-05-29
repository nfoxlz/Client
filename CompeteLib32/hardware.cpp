#include "pch.h"

extern "C"
{
    __declspec(dllexport) const char* getCpuSerial()
	{
        unsigned int i;
        char cpuSerial[30];
        memset(cpuSerial, 0, sizeof(cpuSerial));
        char* ad = cpuSerial + 24;

        for (i = 0; i < 3; i++)
        {
            _asm
            {
                mov eax, 15 // WMI CPU Serial Number
                mov edx, i
                int 138h
                mov[ad], eax // 只取后四位，因为是16进制数
            }
            cpuSerial[i * 8 - 1] = '-';
        }

        return cpuSerial;
	}
}