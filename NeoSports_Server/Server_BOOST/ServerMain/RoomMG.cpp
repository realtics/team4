#include <iostream>

#include "RoomMG.h"

RoomMG::RoomMG()
{
	_roomVec.reserve(MAX_ROOM_COUNT);

	for (int i = 0; i < MAX_ROOM_COUNT; i++)
	{
		ROOM* room = new ROOM();
		_roomVec.push_back(room);
	}
}

int RoomMG::_SearchRoom(int gameIndex)
{
	for (auto iter = _roomVec.begin(); iter != _roomVec.end(); iter++)
	{
		int i = 0;
		/*ROOM��ü�� gameIndex�� ����̸鼭 ã������ �����ε��� �϶�
		���° �������� ����*/
		if ((*iter)->gameIndex == ROOM_INDEX::EMPTY_ROOM && ((*iter)->gameIndex == gameIndex))
			return i;
		i++;
	}
	return FAIL_ROOM_SERCH;
}

int RoomMG::_MakeRoom(int gameIndex, int sessionID)
{
	int roomNum = _SearchRoom(gameIndex);
	if ((roomNum != FAIL_ROOM_SERCH) && _roomVec[roomNum]->isEmptyRoom == true)
	{
		_roomVec[roomNum]->gameIndex = gameIndex;
		_roomVec[roomNum]->superSessionID = sessionID;
		_roomVec[roomNum]->isEmptyRoom = false;
		std::cout << sessionID << " Ŭ�� "
			<< roomNum << " ���濡 " << gameIndex << " ���� ����. " << std::endl;
		return ROOM_INDEX::MAKE_ROOM;
	}

	else if ((roomNum != FAIL_ROOM_SERCH) && _roomVec[roomNum]->isEmptyRoom == false)
	{
		_roomVec[roomNum]->gameIndex = gameIndex;
		_roomVec[roomNum]->sessionID = sessionID;
		std::cout << sessionID << " Ŭ�� "
			<< roomNum << " ���濡 " << gameIndex << " ���� ����. " << std::endl;
		return ROOM_INDEX::ENTER_ROOM;
	}

	return FAIL_ROOM_SERCH;
}
