#pragma once
#include <vector>
#include "Protocol.h"

const int MAX_CHAR_IN_ROOM = 2; //한 방의 최대 게임중인 플레이어

class ROOM
{
public:
	void Init()
	{
		superSessionID = ROOM_HOST::EMPTY_ROOM;
		sessionID = ROOM_HOST::EMPTY_ROOM;
		curGame = GAME_INDEX::EMPTY_GAME;
		for (int i = 0; i < MAX_CHAR_IN_ROOM; i++)
		{
			charIndex[i] = CHAR_INDEX::EMPTY_CHAR;
		}

		isGammingRoom = false;
	}
	bool isGammingRoom;
	GAME_INDEX curGame;
	int charIndex[MAX_CHAR_IN_ROOM]; //방에 있는 플레이어 두명의 캐릭터 인덱스
	int superSessionID; //방장 클라이언트
	int sessionID; //접속한 클라이언트(상대방)
private:
};

class RoomMG
{
public:
	RoomMG();
	int SearchRoom(GAME_INDEX roomIndex);
	int MakeRoom(GAME_INDEX gameIndexk, int sessionID, CHAR_INDEX charIndex);

	int GetRoomChar(int roomIndex, int playerIndex);
	void SetRoomChar(int roomIndex, CHAR_INDEX charIndex);

	int GetRoomNum(int sessionID);

	int GetSuperSessonID(int roomNum);
	int GetSessonID(int roomNum);

	ROOM* GetRoomInfo(int roomNum);
	void InitRoom(int roomNum);

private:
	std::vector<ROOM*> _roomVec;

};

