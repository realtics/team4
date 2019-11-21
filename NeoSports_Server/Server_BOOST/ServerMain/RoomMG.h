#pragma once
#include <vector>

#include "RoomProtocol.h"
#include "CharProtocol.h"
const int MAX_CHAR_IN_ROOM = 2; //�� ���� �ִ� �������� �÷��̾�
class ROOM
{
public:
	ROOM()
	{
		gameIndex = ROOM_INDEX::EMPTY_ROOM;
		superSessionID = ROOM_INDEX::EMPTY_ROOM;
		sessionID = ROOM_INDEX::EMPTY_ROOM;

		for (int i = 0; i < MAX_CHAR_IN_ROOM; i++)
		{
			charIndex[i] = CHAR_INDEX::EMPTY_CHAR;
		}

		isEmptyRoom = true;
	}
	bool isEmptyRoom;
	int gameIndex; //��ü�� ���� ������ ������ ����
	int charIndex[MAX_CHAR_IN_ROOM]; //�濡 �ִ� �÷��̾� �θ��� ĳ���� �ε���
	int superSessionID; //���� Ŭ���̾�Ʈ
	int sessionID; //������ Ŭ���̾�Ʈ
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

