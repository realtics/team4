#pragma once
#include <queue>
#include <map>

#define sessionID int

using namespace std;

class LogicProcess
{
public:
	void ProcessPacket();

	template <typename T>
	void PushPacketQueue(sessionID sessionId, T data);

private:
	template <typename T>
	queue<map<sessionID, T>> _packetQue;
};