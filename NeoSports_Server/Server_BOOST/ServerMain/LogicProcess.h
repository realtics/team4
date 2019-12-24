#pragma once
#include <boost/asio.hpp>
#include <boost/bind.hpp>

#include "Protocol.h"
#include "ThreadHandler.h"

using namespace std;

class Server;
class LogicProcess
{
public:
	LogicProcess(Server* serverPtr,ThreadHandler* threadHandler);
	~LogicProcess();

	void StopProcess();
	void ProcessPacket();
private:
	ThreadHandler* _threadHandler;
	Server* _serverPtr;

	string _SerializationJson(PACKET_INDEX packetIndex, const char* pakcet, std::string jsonLength = "0000");
	//str1�� str2���ٰ� ����
	void memcpyStr(std::string* str1, std::string* str2);
};