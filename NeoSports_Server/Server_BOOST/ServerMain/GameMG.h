#pragma once
//룸클래스안에 선언해 놓고 게임이 시작된 룸에서 게임진행을 관리 하는 클래스
#include "Protocol.h"

class GameMG
{
private:
	GAME_INDEX _curGame;

	int _ropePos;

public:
	GameMG();
	~GameMG();

	void SetCurGame(GAME_INDEX gameIndex);

	void Init();

	void SetRopePos(int ropePos);
	int GetRopePos();
};