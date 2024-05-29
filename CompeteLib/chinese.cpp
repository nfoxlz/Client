#include "pch.h"
#include "global.h"

const std::string numeralList[] = { "��", "Ҽ", "��", "��", "��", "��", "½", "��", "��", "��" };

extern "C"
{
	__declspec(dllexport) const char* convertNumerals(const double numerals, const unsigned short digit)
	{
		std::string str = std::to_string(numerals);

		size_t pos = str.find('.');
		if (pos >= 0)
		{
			size_t len = pos + digit + 1;
			if (len < str.length())
				str = str.substr(0, len);
		}

		std::string buffer;
		if (numerals < 0)   // ��������
			buffer.append("��");

		for (char c : str)
			buffer.append('.' == c ? "��" : numeralList[c - 48]);

		return stringToPChar(buffer);
	}

	__declspec(dllexport) const char* convertAmount(const double amount)
	{
		static std::string list[] = { "��", "ʰ", "��", "Ǫ", "��", "ʰ", "��", "Ǫ", "��", "ʰ", "��", "Ǫ", "��", "ʰ", "��", "Ǫ", "��", "ʰ", "��", "Ǫ", "��", "ʰ", "��", "Ǫ", "��", "ʰ", "Ԫ", "��", "��" };

		std::string str = std::to_string(abs(amount) * 100);

		size_t pos = str.find('.');
		if (pos >= 0)
			str = str.substr(0, pos);

		std::string buffer;
		buffer.append("��");
		if (amount < 0)   // ��������
			buffer.append("��");

		size_t digit = 29 - str.size();
		for (char c : str)
		{
			buffer.append(numeralList[c - 48]);
			buffer.append(list[digit]);           // �����λ��
			digit++;
		}

		return stringToPChar(buffer);
	}
}