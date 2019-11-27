#pragma once

#include <mysql.h>
#include <iostream>
#include "Lock.h"
#include "Protocol.h"

class DB
{
public:
	DB();
	~DB();

	void SelectQuery();
	void Insert(std::string name);
	void Delete(std::string name);
	void Update(std::string name, GAME_INDEX gameIndex, int addScore);

private:
	Lock _upDateLock;

	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;

	void _Init();
};

