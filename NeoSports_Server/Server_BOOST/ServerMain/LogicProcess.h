#pragma once
#include <boost/asio.hpp>
#include <boost/bind.hpp>

#include "Protocol.h"

using namespace std;

class Server;
class LogicProcess
{
public:
	LogicProcess(Server* serverPtr);
	~LogicProcess();

	void StopProcess();
	void ProcessPacket();
private:
	Server* _serverPtr;

	string _SerializationJson(PACKET_INDEX packetIndex, const char* pakcet);
};