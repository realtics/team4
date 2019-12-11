#pragma once
//Thread 끼리 통신을 위한 이벤트를 관리하는 핸들러역할 클래스

#include <Windows.h>
#include <iostream>
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

class ThreadHandler
{
public:
	ThreadHandler() { _packetQueueEvents = 0; };
	void CreateEvents(bool resetMode)
	{
		_packetQueueEvents = CreateEvent(NULL, resetMode, FALSE, NULL);
		if (_packetQueueEvents == NULL)
		{
			cout << "LogicProcess : packetQueueEvents : error " << endl;
		}
	}
	PacketData GetPakcetDataQueueFront();
	HANDLE GetPacketQueueEvents();
	bool IsEmptyPacketQueue();

	//_packetQueueEvents만 바꿔주는중
	void SetEventsObject();
	void PopPacketQueue();
	void PushPacketQueue(PacketData packetData);
private:
	
	HANDLE _packetQueueEvents;
	queue<PacketData> _packetQue;
};