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
	void CreateEvents(HANDLE eventsHandle,bool resetMode)
	{
		eventsHandle = CreateEvent(NULL, FALSE, resetMode, NULL);
		if (eventsHandle == NULL)
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

	static ThreadHandler* GetInstance()
	{
		if (_instance == nullptr)
		{
			_instance = new ThreadHandler;
		}
		return _instance;
	}

private:
	ThreadHandler() {};
	ThreadHandler(const ThreadHandler& other);
	static ThreadHandler* _instance;
	
	HANDLE _packetQueueEvents;
	queue<PacketData> _packetQue;
};