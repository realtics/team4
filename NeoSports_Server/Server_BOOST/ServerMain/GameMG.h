#pragma once
//��Ŭ�����ȿ� ������ ���� ������ ���۵� �뿡�� ���������� ���� �ϴ� Ŭ����

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
	float GetRopePos();
};