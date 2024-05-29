#include "pch.h"
#include "global.h"
#include <sstream>

extern "C"
{
	__declspec(dllexport) size_t getLevelLen(const char* code, const char* structure)
	{
		std::string level;
		int levelLen = 0, len = 0, codeLen = strlen(code);
		std::stringstream ss(structure);
		while (std::getline(ss, level, '-'))
		{
			levelLen = std::stoi(level);
			len += levelLen;
			if (len >= codeLen)
				return levelLen;
		}

		return levelLen;
	}

	__declspec(dllexport) int getNextLevelLen(const char* code, const char* structure)
	{
		std::string level;
		int levelLen = 0, len = 0, codeLen = strlen(code);
		std::stringstream ss(structure);
		while (std::getline(ss, level, '-'))
		{
			levelLen = std::stoi(level);
			len += levelLen;
			if (len > codeLen)
				return levelLen;
		}

		return levelLen;
	}
}