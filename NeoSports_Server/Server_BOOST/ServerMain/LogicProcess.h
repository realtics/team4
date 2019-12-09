#pragma once
#include <queue>
#include "Protocol.h"

using namespace std;

struct PacketData
{
	const int sessionID;
	const char* data;

	PacketData(const int sessionId,const char* packet)
		: sessionID(sessionId), data(packet)
	{

	}
};

class Server;
class LogicProcess
{
public:
	void Init(Server* server);
	void StopProcess();
	void PushPacketQueue(const int sessionId, const char* data);
	void ProcessPacket();

private:
	Server* _serverPtr;
	queue<PacketData> _packetQue;
	
	string _SerializationJson(PACKET_INDEX packetIndex, const char* pakcet);
};