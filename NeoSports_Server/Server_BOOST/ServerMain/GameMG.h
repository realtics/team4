#pragma once
//��Ŭ�����ȿ� ������ ���� ������ ���۵� �뿡�� ���������� ���� �ϴ� Ŭ����
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