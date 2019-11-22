#pragma once
#include <vector>
#include "Protocol.h"

const int MAX_CHAR_IN_ROOM = 2; //�� ���� �ִ� �������� �÷��̾�

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
	int gameIndex; //��ü�� ���� ������ ������ ����
	int charIndex[MAX_CHAR_IN_ROOM]; //�濡 �ִ� �÷��̾� �θ��� ĳ���� �ε���
	int superSessionID; //���� Ŭ���̾�Ʈ
	int sessionID; //������ Ŭ���̾�Ʈ(����)
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

