#include "ThreadHandler.h"

bool ThreadHandler::IsEmptyPacketQueue()
{
	return _packetQue.empty();
}


HANDLE ThreadHandler::GetPacketQueueEvents()
{
	return _packetQueueEvents;
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

void ThreadHandler::SetEventsObject()
{
	SetEvent(_packetQueueEvents);
}