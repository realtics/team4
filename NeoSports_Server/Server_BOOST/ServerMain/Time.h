#pragma once

#include <iostream>
#include <string>
#include <stdio.h>
#include <time.h>

// ����ð��� string type���� return�ϴ� �Լ�
std::string currentDateTime() {
	time_t     now = time(0); //���� �ð��� time_t Ÿ������ ����
	struct tm  tstruct;
	char       buf[80];
	tstruct = *localtime(&now);
	strftime(buf, sizeof(buf), "%Y-%m-%d.%X", &tstruct); // YYYY-MM-DD.HH:mm:ss ������ ��Ʈ��

	return buf;
}