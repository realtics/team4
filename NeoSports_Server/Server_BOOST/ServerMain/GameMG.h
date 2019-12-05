#pragma once
//룸클래스안에 선언해 놓고 게임이 시작된 룸에서 게임진행을 관리 하는 클래스 
//session에서 GameMG까지올려면 session -> server -> roomMG -> room -> GameMG 인데 따로 빼는것을 생각해야할것같다
#include "Protocol.h"

class GameMG
{
private:
	GAME_INDEX _curGame;

	float _ropePos;

public:
	GameMG();
	~GameMG();

	void SetCurGame(GAME_INDEX gameIndex);
	GAME_INDEX GetCurGame();

	void Init();

	void SetRopePos(float ropePos);
	int GetRopePos();
};