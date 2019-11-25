#include "GameMG.h"
#include <iostream>

GameMG::GameMG()
{
	Init();
}

GameMG::~GameMG()
{

}

void GameMG::SetCurGame(GAME_INDEX gameIndex)
{
	_curGame = gameIndex;
}


void GameMG::Init()
{
	_curGame = GAME_INDEX::EMPTY_GAME;
	_ropePos = 0;
}

void GameMG::SetRopePos(int ropePos)
{
	_ropePos = ropePos;
}

int GameMG::GetRopePos()
{
	return _ropePos;
}