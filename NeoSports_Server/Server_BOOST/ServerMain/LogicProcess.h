#pragma once
#include "ThreadProtocol.h"
#include "Protocol.h"

using namespace std;

class Server;
class LogicProcess
{
public:
	void Init(Server* server);
	void StopProcess();
	void ProcessPacket();

private:
	Server* _serverPtr;
	
	void _PostSend(const bool bImmediately, const int size, char* dataPtr);

	string _SerializationJson(PACKET_INDEX packetIndex, const char* pakcet);
};