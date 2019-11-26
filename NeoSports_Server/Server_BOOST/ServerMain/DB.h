#pragma once

#include<mysql.h>
#include "Protocol.h"

class DB
{
public:
	DB();
	~DB();

	void SelectQuery();
	void Insert();
	void Update(int sessionID, GAME_INDEX gameIndex, int addScore);

private:
	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;


	void _Init();
};

