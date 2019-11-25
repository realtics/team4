#pragma once

#include<mysql.h>

class DB
{
public:
	DB();
	~DB();

	void SelectQuery();
	void Insert();
	void Update();

private:
	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;


	void _Init();
};

