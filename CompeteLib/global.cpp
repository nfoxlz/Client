#include "pch.h"

#include <comdef.h>

const char* bstrToConstChar(BSTR bstr)
{
    // ����һ���µ�BSTR�ַ���
    BSTR newBstr = SysAllocString(bstr);

    // ��ȡBSTR�ַ����ĳ���
    int len = WideCharToMultiByte(CP_UTF8, 0, newBstr, -1, NULL, 0, NULL, NULL);

    // ����һ���㹻��Ļ��������洢ת������ַ���
    char* buffer = new char[len];

    // ��BSTR�ַ���ת��Ϊ���ֽ��ַ���
    WideCharToMultiByte(CP_UTF8, 0, newBstr, -1, buffer, len, NULL, NULL);

    // ��ת����Ķ��ֽ��ַ������Ƶ�const char*ָ��
    const char* result = buffer;

    // �ͷŷ����BSTR�ַ���
    SysFreeString(newBstr);

    return result;
}

extern "C" __declspec(dllexport) void releaseString(const char* str)
{
	delete[] str;
}
