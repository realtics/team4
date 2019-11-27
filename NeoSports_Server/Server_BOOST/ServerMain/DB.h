#pragma once

#include<mysql.h>
#include "Lock.h"
#include "Protocol.h"

class DB
{
public:
	DB();
	~DB();

	void SelectQuery();
	void Insert(int sessionID);
	void Update(int sessionID, GAME_INDEX gameIndex, int addScore);

private:
	Lock _upDateLock;

	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;

	void _Init();
};

