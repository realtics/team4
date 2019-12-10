#pragma once
//Thread ���� ����� ���� �̺�Ʈ�� �����ϴ� �ڵ鷯���� Ŭ����

#include <Windows.h>
#include <queue>

using namespace std;

struct PacketData
{
	const int sessionID;
	const char* data;

	PacketData(const int sessionId, const char* packet)
		: sessionID(sessionId), data(packet)
	{

	}
};

queue<PacketData> packetQue;

HANDLE packetQueueEvents;