#pragma once
//Thread ���� ����� ���� �̺�Ʈ�� �����ϴ� �ڵ鷯���� Ŭ����

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

	//_packetQueueEvents�� �ٲ��ִ���
	void SetEventsObject();
	void PopPacketQueue();
	void PushPacketQueue(PacketData packetData);
private:
	
	HANDLE _packetQueueEvents;
	queue<PacketData> _packetQue;
};