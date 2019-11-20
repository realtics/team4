#pragma once
#include <vector>

#include "RoomProtocol.h"

class ROOM
{
public:
	ROOM()
	{
		gameIndex = ROOM_INDEX::EMPTY_ROOM;
		superSessionID = ROOM_INDEX::EMPTY_ROOM;
		sessionID = ROOM_INDEX::EMPTY_ROOM;
		isEmptyRoom = true;
	}
	bool isEmptyRoom;
	int gameIndex; //객체가 무슨 게임의 방인지 구별
	int superSessionID; //방장 클라이언트
	int sessionID; //접속한 클라이언트
private:
};

class RoomMG
{
public:
	RoomMG();
	int _SearchRoom(int roomIndex);
	int _MakeRoom(int roomIndexk, int sessionID);
private:
	std::vector<ROOM*> _roomVec;

};

