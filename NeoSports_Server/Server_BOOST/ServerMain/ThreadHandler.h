#pragma once
//Thread 끼리 통신을 위한 이벤트를 관리하는 핸들러역할 클래스

#include <Windows.h>
#include <iostream>
#include <queue>
#include "Lock.h"

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

class ThreadHandler
{
public:
	ThreadHandler() {};
	void CreateEvents(bool resetMode)
	{
		
	}
	PacketData GetPakcetDataQueueFront();
	bool IsEmptyPacketQueue();

	void PopPacketQueue();
	void PushPacketQueue(PacketData packetData);

	int GetPakcetQueueSize();

	Lock pushPakcetQueueLock;

private:
	
	queue<PacketData> _packetQue;
};