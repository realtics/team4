#pragma once

#include <mysql.h>
#include <iostream>
#include "Lock.h"
#include "Protocol.h"

class DB
{
public:
	static DB* GetInstance()
	{
		if (_instance == nullptr)
		{
			_instance = new DB;
		}

		return _instance;
	}
	~DB();


	void Init();

	void SelectQuery();
	void Insert(std::string name);
	void Delete(std::string name);
	void Update(std::string name, GAME_INDEX gameIndex, int addScore);

	void Rank(GAME_INDEX gameIndex, RANK rank[]);
	std::string orderByRank(std::string tableName, GAME_INDEX gameIndex, std::string Column);

private:
	DB() {};
	DB(const DB& other);
	static DB* _instance;

	Lock _upDateLock;

	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;

};

