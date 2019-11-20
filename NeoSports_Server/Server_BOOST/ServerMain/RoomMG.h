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
	int gameIndex; //��ü�� ���� ������ ������ ����
	int superSessionID; //���� Ŭ���̾�Ʈ
	int sessionID; //������ Ŭ���̾�Ʈ
private:
};

class RoomMG
{
public:
	RoomMG();
private:
	std::vector<ROOM*> _roomVec;

	int _SearchRoom(int roomIndex);
	int _MakeRoom(int roomIndexk, int sessionID);
};

