#pragma once

#include<mysql.h>

class DB
{
public:
	void querySelect();

private:
	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;

	DB();
	~DB();

	void _Init();
};

