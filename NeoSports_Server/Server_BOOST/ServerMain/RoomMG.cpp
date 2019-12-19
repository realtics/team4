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

void RoomMG::SetGameMG(int sessionID, GameMG* gameMg)
{
	if (gameMg != nullptr)
	{
		int roomNum = GetRoomNum(sessionID);
		_roomVec[roomNum]->gameMG = gameMg;
	}
	else if (gameMg == nullptr)
	{
		int temp = GetRoomNum(sessionID);
		_roomVec[temp]->gameMG->Init();
	}
}


int RoomMG::SearchRoom(GAME_INDEX gameIndex)
{
	int i = 0;
	for (auto iter = _roomVec.begin(); iter != _roomVec.end(); iter++)
	{
		if ((*iter)->isGammingRoom == false && (*iter)->curGame == gameIndex)
			return i;

		i++;
	}

	i = 0;
	for (auto iter = _roomVec.begin(); iter != _roomVec.end(); iter++)
	{
		if ((*iter)->isGammingRoom == false && (*iter)->curGame == GAME_INDEX::EMPTY_GAME)
			return i;

		i++;
	}
	std::cout << "Room : SearchRoom : Fail Room Search" << std::endl;
	return FAIL_ROOM_SERCH;
}

int RoomMG::MakeRoom(GAME_INDEX gameIndex, int sessionID, int charIndex)
{
	int roomNum = SearchRoom(gameIndex);
	if ((roomNum != FAIL_ROOM_SERCH))
	{
		if (_roomVec[roomNum]->superSessionID == ROOM_HOST::EMPTY_ROOM)
		{
			_roomVec[roomNum]->curGame = gameIndex;
			SetRoomChar(roomNum, charIndex);
			_roomVec[roomNum]->superSessionID = sessionID;
			std::cout << "Room : " << sessionID << " Client "
				<< roomNum << " RoomNum " << gameIndex << " Make Game. " << std::endl;
			return ROOM_HOST::MAKE_ROOM;
		}

		else if (_roomVec[roomNum]->superSessionID != ROOM_HOST::EMPTY_ROOM)
		{
			SetRoomChar(roomNum, charIndex);
			_roomVec[roomNum]->sessionID = sessionID;
			std::cout << sessionID << " Client "
				<< roomNum << " Room " << gameIndex << " Enter Game. " << std::endl;
			_roomVec[roomNum]->isGammingRoom = true;
			std::cout << roomNum << " RoomNum " << gameIndex << " Game Start" << std::endl;
			return ROOM_HOST::ENTER_ROOM;
		}
	}
	std::cout << "Room : MakeRoom : Fail Make Room" << std::endl;
	return FAIL_ROOM_SERCH;
}

void RoomMG::SetRoomChar(int roomIndex, int charIndex)
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

int RoomMG::GetRoomChar(int roomIndex, int playerIndex)
{
	return _roomVec[roomIndex]->charIndex[playerIndex];
}

int RoomMG::GetRoomNum(int sessionID)
{
	int roomNum = 0;

	for (int i = 0; i < MAX_ROOM_COUNT; i++)
	{
		if (_roomVec[i]->superSessionID == sessionID ||
			_roomVec[i]->sessionID == sessionID)
		{
			roomNum = i;
			return roomNum;
		}
	}

	std::cout << "Room : GetRoomNum : Fail Search Room" << std::endl;
	return FAIL_ROOM_SERCH;
}

int RoomMG::GetSuperSessonID(int roomNum)
{
	return _roomVec[roomNum]->superSessionID;
}

int RoomMG::GetSessonID(int roomNum)
{
	return _roomVec[roomNum]->sessionID;

}

ROOM* RoomMG::GetRoomInfo(int roomNum)
{
	if (roomNum != FAIL_ROOM_SERCH)
	{
		ROOM* room = new ROOM;
		room->Init();
		room = _roomVec[roomNum];

		return room;
	}
}

void RoomMG::InitRoom(int roomNum)
{
	_roomVec[roomNum]->Init();
}
