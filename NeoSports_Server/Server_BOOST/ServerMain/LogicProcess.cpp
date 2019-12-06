#include "LogicProcess.h"
#include <typeinfo>
#include <iostream>

template <typename T>
void LogicProcess::PushPacketQueue(sessionID sessionId, T data)
{
	map<sessionID, T> tempMap;
	tempMap.insert(make_pair(sessionId, T));
	_packetQue.push(tempMap);
	cout << "LogicProcess : PushProcessQue : " << typeid(T).name() << " push in packetQue." << endl;
}

void LogicProcess::ProcessPacket()
{
	while (!_packetQue.empty())
	{
		_packetQue.front();
	}
}