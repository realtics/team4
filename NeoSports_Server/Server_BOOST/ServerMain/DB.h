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
	int InsertUser(int clientID,int sessionID);
	void DeleteUser(int clientID);

	void UpdateWinRecord(int clientID, GAME_INDEX gameIndex, int addScore);
	void UpdataUserTable(int clientID, int sessionID);

	void Rank(GAME_INDEX gameIndex, RANK rank[]);
	std::string orderByRank(std::string tableName, GAME_INDEX gameIndex, std::string Column);

	int GetClientID(int sessionID);

private:
	DB() {};
	DB(const DB& other);
	static DB* _instance;

	Lock _upDateLock;
	Lock _userUpDateLock;

	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;

};

