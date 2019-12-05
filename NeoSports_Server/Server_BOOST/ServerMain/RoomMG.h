#pragma once
#include <vector>
#include "Protocol.h"

const int MAX_CHAR_IN_ROOM = 2; //�� ���� �ִ� �������� �÷��̾�

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
	int charIndex[MAX_CHAR_IN_ROOM]; //�濡 �ִ� �÷��̾� �θ��� ĳ���� �ε���
	int superSessionID; //���� Ŭ���̾�Ʈ
	int sessionID; //������ Ŭ���̾�Ʈ(����)
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

