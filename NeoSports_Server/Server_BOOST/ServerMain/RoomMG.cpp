#include <iostream>

#include "RoomMG.h"

RoomMG::RoomMG()
{
	roomVec.reserve(MAX_ROOM_COUNT);

	for (int i = 0; i < MAX_ROOM_COUNT; i++)
	{
		ROOM* room = new ROOM();
		room->Init();
		roomVec.push_back(room);
	}
}

int RoomMG::SearchRoom(int gameIndex)
{
	int i = 0;
	for (auto iter = roomVec.begin(); iter != roomVec.end(); iter++)
	{
		if ((*iter)->isGammingRoom == false && (*iter)->gameIndex == gameIndex)
			return i;

		i++;
	}

	i = 0;
	for (auto iter = roomVec.begin(); iter != roomVec.end(); iter++)
	{
		if ((*iter)->isGammingRoom == false && (*iter)->gameIndex == ROOM_HOST::EMPTY_ROOM
			&& (*iter)->superSessionID == ROOM_HOST::EMPTY_ROOM)
			return i;

		i++;
	}
	std::cout << "��ã�� ����" << std::endl;
	return FAIL_ROOM_SERCH;
}

int RoomMG::MakeRoom(int gameIndex, int sessionID, int charIndex)
{
	int roomNum = SearchRoom(gameIndex);
	if ((roomNum != FAIL_ROOM_SERCH))
	{
		if (roomVec[roomNum]->superSessionID == ROOM_HOST::EMPTY_ROOM)
		{
			roomVec[roomNum]->gameIndex = gameIndex;
			SetRoomChar(roomNum, charIndex);
			roomVec[roomNum]->superSessionID = sessionID;
			std::cout << sessionID << " Ŭ�� "
				<< roomNum << " ���濡 " << gameIndex << " ���� ����. " << std::endl;
			return ROOM_HOST::MAKE_ROOM;
		}

		else if (roomVec[roomNum]->superSessionID != ROOM_HOST::EMPTY_ROOM)
		{
			SetRoomChar(roomNum, charIndex);
			roomVec[roomNum]->sessionID = sessionID;
			std::cout << sessionID << " Ŭ�� "
				<< roomNum << " ���濡 " << gameIndex << " ���� ����. " << std::endl;
			roomVec[roomNum]->isGammingRoom = true;
			std::cout << roomNum << "���� " << gameIndex << "�� ���� ����" << std::endl;
			return ROOM_HOST::ENTER_ROOM;
		}

	}
	std::cout << "����� ����" << std::endl;
	return FAIL_ROOM_SERCH;
}

void RoomMG::SetRoomChar(int roomIndex, int charIndex)
{
	for (int i = 0; i < MAX_CHAR_IN_ROOM; i++)
	{
		if (roomVec[roomIndex]->charIndex[i] == CHAR_INDEX::EMPTY_CHAR)
		{
			roomVec[roomIndex]->charIndex[i] = charIndex;
			return;
		}
	}
}

int RoomMG::GetRoomChar(int roomIndex, int playerIndex)
{
	return roomVec[roomIndex]->charIndex[playerIndex];
}

int RoomMG::GetRoomNum(int sessionID)
{
	int roomNum = 0;

	for (int i=0; i<MAX_ROOM_COUNT; i++)
	{
		if (roomVec[i]->superSessionID == sessionID ||
			roomVec[i]->sessionID == sessionID)
		{
			roomNum = i;
			return roomNum;
		}
	}

	std::cout << "�������� ã�µ� ���� �߽��ϴ�" << std::endl;
	return FAIL_ROOM_SERCH;
}

int RoomMG::GetSuperSessonID(int roomNum)
{
	return roomVec[roomNum]->superSessionID;
}

int RoomMG::GetSessonID(int roomNum)
{
	return roomVec[roomNum]->sessionID;

}


