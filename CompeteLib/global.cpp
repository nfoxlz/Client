#include "pch.h"

#include <comdef.h>

const char* bstrToConstChar(BSTR bstr)
{
    // 分配一个新的BSTR字符串
    BSTR newBstr = SysAllocString(bstr);

    // 获取BSTR字符串的长度
    int len = WideCharToMultiByte(CP_UTF8, 0, newBstr, -1, NULL, 0, NULL, NULL);

    // 分配一个足够大的缓冲区来存储转换后的字符串
    char* buffer = new char[len];

    // 将BSTR字符串转换为多字节字符集
    WideCharToMultiByte(CP_UTF8, 0, newBstr, -1, buffer, len, NULL, NULL);

    // 将转换后的多字节字符集复制到const char*指针
    const char* result = buffer;

    // 释放分配的BSTR字符串
    SysFreeString(newBstr);

    return result;
}

extern "C" __declspec(dllexport) void releaseString(const char* str)
{
	delete[] str;
}
