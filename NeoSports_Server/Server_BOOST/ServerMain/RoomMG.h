#pragma once
#include <vector>
#include "Protocol.h"
#include "GameMG.h"

const int MAX_CHAR_IN_ROOM = 2; //�� ���� �ִ� �������� �÷��̾�

class ROOM
{
public:
	void Init()
	{
		superSessionID = ROOM_HOST::EMPTY_ROOM;
		sessionID = ROOM_HOST::EMPTY_ROOM;

		for (int i = 0; i < MAX_CHAR_IN_ROOM; i++)
		{
			charIndex[i] = CHAR_INDEX::EMPTY_CHAR;
		}

		isGammingRoom = false;
		gameMG.Init();
	}
	bool isGammingRoom;
	int charIndex[MAX_CHAR_IN_ROOM]; //�濡 �ִ� �÷��̾� �θ��� ĳ���� �ε���
	int superSessionID; //���� Ŭ���̾�Ʈ
	int sessionID; //������ Ŭ���̾�Ʈ(����)

	GameMG gameMG;
private:
};

class RoomMG
{
public:
	RoomMG();
	int SearchRoom(int roomIndex);
	int MakeRoom(int gameIndexk, int sessionID, int charIndex);

	int GetRoomChar(int roomIndex, int playerIndex);
	void SetRoomChar(int roomIndex, int charIndex);

	int GetRoomNum(int sessionID);

	int GetSuperSessonID(int roomNum);
	int GetSessonID(int roomNum);

	ROOM* GetRoomInfo(int roomNum);
	void InitRoom(int roomNum);

private:
	std::vector<ROOM*> _roomVec;

};

