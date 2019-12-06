#pragma once
#include <queue>

#define sessionID int

using namespace std;

template <typename PACKET>
struct PacketData
{
	sessionID sessionId;
	PACKET packet;
};

class LogicProcess
{
public:
	void ProcessPacket();

	template <typename T>
	void PushPacketQueue(sessionID sessionId, T data);

private:
	queue<PacketData<int>> _packetQue;
};