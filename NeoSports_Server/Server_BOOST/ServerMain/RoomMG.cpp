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
		/*ROOM객체의 gameIndex가 빈방이면서 찾을려는 게임인덱스 일때
		몇번째 방인지를 리턴*/
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
		std::cout << sessionID << " 클라가 "
			<< roomNum << " 번방에 " << gameIndex << " 게임 생성. " << std::endl;
		return ROOM_INDEX::MAKE_ROOM;
	}

	else if ((roomNum != FAIL_ROOM_SERCH) && _roomVec[roomNum]->isEmptyRoom == false)
	{
		_roomVec[roomNum]->gameIndex = gameIndex;
		_roomVec[roomNum]->sessionID = sessionID;
		std::cout << sessionID << " 클라가 "
			<< roomNum << " 번방에 " << gameIndex << " 게임 참가. " << std::endl;
		return ROOM_INDEX::ENTER_ROOM;
	}

	return FAIL_ROOM_SERCH;
}
