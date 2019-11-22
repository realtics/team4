#pragma once
#include <vector>
#include "Protocol.h"

const int MAX_CHAR_IN_ROOM = 2; //한 방의 최대 게임중인 플레이어

class ROOM
{
public:
	void Init()
	{
		gameIndex = ROOM_INDEX::EMPTY_ROOM;
		superSessionID = ROOM_INDEX::EMPTY_ROOM;
		sessionID = ROOM_INDEX::EMPTY_ROOM;

		for (int i = 0; i < MAX_CHAR_IN_ROOM; i++)
		{
			charIndex[i] = CHAR_INDEX::EMPTY_CHAR;
		}

		isGammingRoom = false;
	}
	bool isGammingRoom;
	int gameIndex; //객체가 무슨 게임의 방인지 구별
	int charIndex[MAX_CHAR_IN_ROOM]; //방에 있는 플레이어 두명의 캐릭터 인덱스
	int superSessionID; //방장 클라이언트
	int sessionID; //접속한 클라이언트(상대방)
private:
};

class RoomMG
{
public:
	RoomMG();
	int _SearchRoom(int roomIndex);
	int _MakeRoom(int gameIndexk, int sessionID, int charIndex);
	int _GetRoomChar(int roomIndex, int playerIndex);
	void _SetRoomChar(int roomIndex, int charIndex);
	int _GetRoomNum(int sessionID);
	std::vector<ROOM*> _roomVec;
private:

};

