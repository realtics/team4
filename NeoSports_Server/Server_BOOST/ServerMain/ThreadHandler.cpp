#include "ThreadHandler.h"
#include <iostream>

bool ThreadHandler::IsEmptyPacketQueue()
{
	return _packetQue.empty();
}

PacketData ThreadHandler::GetPakcetDataQueueFront()
{
	return _packetQue.front();
}

void ThreadHandler::PopPacketQueue()
{
	_packetQue.pop();
}

void ThreadHandler::PushPacketQueue(PacketData packetData)
{
	_packetQue.push(packetData);
}

int ThreadHandler::GetPakcetQueueSize()
{
	return _packetQue.size();
}
