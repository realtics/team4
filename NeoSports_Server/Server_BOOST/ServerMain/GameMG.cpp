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

GAME_INDEX GameMG::GetCurGame()
{
	return _curGame;
}



void GameMG::Init()
{
	_curGame = GAME_INDEX::EMPTY_GAME;
	_ropePos = 0;
}

void GameMG::SetRopePos(float ropePos)
{
		_ropePos += ropePos;

}

float GameMG::GetRopePos()
{
	return _ropePos;
}