#include <iostream>

#include "RoomMG.h"

RoomMG::RoomMG()
{
	_roomVec.reserve(MAX_ROOM_COUNT);

	for (int i = 0; i < MAX_ROOM_COUNT; i++)
	{
		ROOM* room = new ROOM();
		room->Init();
		_roomVec.push_back(room);
	}
}

int RoomMG::_SearchRoom(int gameIndex)
{
	int i = 0;
	for (auto iter = _roomVec.begin(); iter != _roomVec.end(); iter++)
	{
		if ((*iter)->isGammingRoom == false && (*iter)->gameIndex == gameIndex)
			return i;

		i++;
	}

	i = 0;
	for (auto iter = _roomVec.begin(); iter != _roomVec.end(); iter++)
	{
		if ((*iter)->isGammingRoom == false && (*iter)->gameIndex == ROOM_HOST::EMPTY_ROOM
			&& (*iter)->superSessionID == ROOM_HOST::EMPTY_ROOM)
			return i;

		i++;
	}
	std::cout << "방찾기 실패" << std::endl;
	return FAIL_ROOM_SERCH;
}

int RoomMG::_MakeRoom(int gameIndex, int sessionID, int charIndex)
{
	int roomNum = _SearchRoom(gameIndex);
	if ((roomNum != FAIL_ROOM_SERCH))
	{
		if (_roomVec[roomNum]->superSessionID == ROOM_HOST::EMPTY_ROOM)
		{
			_roomVec[roomNum]->gameIndex = gameIndex;
			_SetRoomChar(roomNum, charIndex);
			_roomVec[roomNum]->superSessionID = sessionID;
			std::cout << sessionID << " 클라가 "
				<< roomNum << " 번방에 " << gameIndex << " 게임 생성. " << std::endl;
			return ROOM_HOST::MAKE_ROOM;
		}

		else if (_roomVec[roomNum]->superSessionID != ROOM_HOST::EMPTY_ROOM)
		{
			_SetRoomChar(roomNum, charIndex);
			_roomVec[roomNum]->sessionID = sessionID;
			std::cout << sessionID << " 클라가 "
				<< roomNum << " 번방에 " << gameIndex << " 게임 참가. " << std::endl;
			_roomVec[roomNum]->isGammingRoom = true;
			std::cout << roomNum << "번방 " << gameIndex << "번 게임 시작" << std::endl;
			return ROOM_HOST::ENTER_ROOM;
		}

	}
	std::cout << "방생성 실패" << std::endl;
	return FAIL_ROOM_SERCH;
}

void RoomMG::_SetRoomChar(int roomIndex, int charIndex)
{
	for (int i = 0; i < MAX_CHAR_IN_ROOM; i++)
	{
		if (_roomVec[roomIndex]->charIndex[i] == CHAR_INDEX::EMPTY_CHAR)
		{
			_roomVec[roomIndex]->charIndex[i] = charIndex;
			return;
		}
	}
}

int RoomMG::_GetRoomChar(int roomIndex, int playerIndex)
{
	return _roomVec[roomIndex]->charIndex[playerIndex];
}

int RoomMG::_GetRoomNum(int sessionID)
{
	int roomNum = 0;

	for (int i=0; i<MAX_ROOM_COUNT; i++)
	{
		if (_roomVec[i]->superSessionID == sessionID ||
			_roomVec[i]->sessionID == sessionID)
		{
			roomNum = i;
			return roomNum;

		}
	}

	std::cout << "룸정보를 찾는데 실패 했습니다" << std::endl;
	return FAIL_ROOM_SERCH;
}

int RoomMG::_GetSuperSessonID(int roomNum)
{
	return _roomVec[roomNum]->superSessionID;
}

int RoomMG::_GetSessonID(int roomNum)
{
	return _roomVec[roomNum]->sessionID;

}


