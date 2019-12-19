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
	int InsertUser(int* clientID, int sessionID);
	void DeleteUser(int clientID);

	void InsertGameInfo(int clientID, GAME_INDEX gameIndex,int winRecord);
	void SetNameTable(int clientID, std::string name);

	void UpdateWinRecord(int clientID, GAME_INDEX gameIndex, int addScore);
	void UpdataUserTable(int clientID, int sessionID);

	void Rank(GAME_INDEX gameIndex, RANK rank[]);
	std::string orderByRank(std::string tableName, GAME_INDEX gameIndex, std::string Column);

	std::string GetFarmInfo(int clientID);
	void SetFarmInfo(int clientID, std::string farmJson, FARM_INDEX farmIndex);
	void InsertFarmInfo(int clientID, std::string farmJson, FARM_INDEX farmIndex);

	int GetClientID(int sessionID);
	int GetGold(int clientID);
	void SetGold(int clientID,int gold);

	void ErrorCheck();

private:
	DB() {};
	DB(const DB& other);
	static DB* _instance;

	Lock _upDateLock;
	Lock _userUpDateLock;
	Lock _userGoldUpdateLock;

	MYSQL		_conn;
	MYSQL_RES* _pSqlRes;
	MYSQL_ROW	_sqlRow;

};

