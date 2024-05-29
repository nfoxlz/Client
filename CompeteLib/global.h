#pragma once
#include "pch.h"

#include <string>
#include <stdio.h>
#include <comdef.h>

inline const char* stringToPChar(std::string str)
{
	const char* buf = str.c_str();
	size_t len = strlen(buf) + 1;
	char* result = new char[len];
	strcpy_s(result, len, buf);
	return result;
}

inline const char* copyPChar(const char* str)
{
	size_t len = strlen(str) + 1;
	char* result = new char[len];
	strcpy_s(result, len, str);
	return result;
}

const char* bstrToConstChar(BSTR bstr);