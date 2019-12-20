#include "DB.h"
#include "Protocol.h"

#include <iostream>
#include <boost/lexical_cast.hpp>

using namespace std;

DB* DB::_instance = nullptr;

DB::~DB()
{
	mysql_close(&_conn);
}
void DB::ErrorCheck()
{
	int errorNo = mysql_errno(&_conn);
	std::cout << errorNo << std::endl;
	std::cout << mysql_error(&_conn) << std::endl;
}

void DB::Init()
{
	if (mysql_init(&_conn) == nullptr)
	{
		std::cout << "MySql Init Error" << std::endl;
	}

	if (mysql_real_connect(
		&_conn, "localhost", //루프백
		"root", "!qhdksxl0212",
		"neosports", 3306, NULL, 0) == nullptr)
	{
		std::cout << "DB : mysql_real_connect Error" << std::endl;
	}

	if (mysql_select_db(&_conn, "neosports"))
	{
		ErrorCheck();
		cout << "DB : mysql_select_db Error : " << mysql_errno(&_conn)
			<< " : " << mysql_error(&_conn) << endl;
	}
}

void DB::SelectQuery()
{

}

int DB::InsertUser(int* clientID, int data)
{
	if (clientID == NULL)
	{
		ErrorCheck();
		std::cout << "DB : InsertUser &clientID is NULL" << std::endl;
		return -1;
	}
	if (mysql_query(&_conn, "SELECT * FROM user") != 0)
	{
		ErrorCheck();
		std::cout << "DB : InsertUser mysql_query error" << std::endl;
		return -1;
	}
	_pSqlRes = mysql_store_result(&_conn);
	int maxCount = mysql_num_rows(_pSqlRes);
	*clientID = maxCount++;

	std::string query = "";
	query = "INSERT INTO user(clientID,sessionID) values('";
	query += boost::lexical_cast<std::string>(*clientID);;
	query += "','";
	query += boost::lexical_cast<std::string>(data);
	query += "')";

	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : INSERT ClientID error" << std::endl;
		return -1;
	}
	std::cout << "DB : INSERT ClientID" << std::endl;
	return *clientID;
}

void DB::InsertGameInfo(int clientID, GAME_INDEX gameIndex, int winRecord)
{
	std::string query = "";
	query = "INSERT INTO gameInfo values('";
	query += boost::lexical_cast<std::string>(clientID);;
	query += "','";
	query += boost::lexical_cast<std::string>(gameIndex);
	query += "','";
	query += boost::lexical_cast<std::string>(winRecord);
	query += "')";

	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : INSERT GameInfo error" << std::endl;
		return;
	}
	std::cout << "DB : INSERT GameInfo" << std::endl;
}

void DB::InitUser()
{
	std::string query = "UPDATE user Set sessionID = '-1' WHERE sessionID != '-1'";
	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : InitUser sessionID error" << std::endl;
		return;
	}
}


void DB::SetNameTable(int clientID, std::string name)
{
	// INSERT 먼저 해보고 PROMARY KEY오류가 나면 이미 있는 clientID이므로 그떄 while하는게 좋을듯
	if (mysql_query(&_conn, "SELECT * FROM name") != 0)
	{
		ErrorCheck();
		std::cout << "DB : SetNameTable mysql_query error" << std::endl;
		return;
	}
	_pSqlRes = mysql_store_result(&_conn);

	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 0 = DB의 칼럼(clientID), 1 = name
		if (_sqlRow[0] == boost::lexical_cast<std::string>(clientID))
		{
			std::string aa = "UPDATE name SET name = '";
			aa += name;
			std::string tempStr = "' WHERE clientID = '";
			aa += tempStr;
			aa += boost::lexical_cast<std::string>(clientID);;
			aa += "'";

			if (mysql_query(&_conn, aa.c_str()) != 0)
			{
				ErrorCheck();
				std::cout << "DB : SetNameTable Name error" << std::endl;
				return;
			}
			return;
		}
	}

	std::string query = "";
	query = "INSERT INTO name(clientID,name) values('";
	query += boost::lexical_cast<std::string>(clientID);;
	query += "','";
	query += name;
	query += "')";

	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : INSERT Name error" << std::endl;
		return;
	}
	std::cout << "DB : INSERT Name" << std::endl;
}

void DB::DeleteUser(int clientID)
{
	std::string query = "DELETE FROM user WHERE clientID =";
	query += boost::lexical_cast<std::string>(clientID);
	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		std::cout << "DB : DELETE error" << std::endl;
		return;
	}
	std::cout << "DB : DELETE ROW" << std::endl;
}

void DB::UpdataUserTable(int clientID, int sessionID)
{
	std::string aa = "UPDATE user SET sessionID = ";
	aa += boost::lexical_cast<std::string>(sessionID);
	std::string tempStr = " WHERE clientID = ";
	aa += tempStr;
	aa += boost::lexical_cast<std::string>(clientID);

	if (mysql_query(&_conn, aa.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : UpdataUserTable error" << std::endl;
		return;
	}

	std::cout << "DB : UPDATE sessionID of clientID" << std::endl;
}

int DB::GetClientID(int sessionID)
{
	if (mysql_query(&_conn, "SELECT * FROM user") != 0)
	{
		ErrorCheck();
		std::cout << "DB : Update mysql_query error" << std::endl;
		return -1;
	}
	_pSqlRes = mysql_store_result(&_conn);

	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 0 = DB의 칼럼(clientID)
		//_sqlRow인덱스 1 = DB의 칼럼(sessionID)
		if (_sqlRow[1] == boost::lexical_cast<std::string>(sessionID))
		{
			return boost::lexical_cast<int>(_sqlRow[0]);;
		}
	}
}

void DB::GetFarmInfo(int clientID, std::string json[], int farmIndex[])
{
	if (clientID == 0)
	{
		ErrorCheck();
		std::cout << "DB : GetFarmInfo clientID == 0 " << std::endl;
		return;
	}

	std::string temp = "SELECT * FROM farmInfo WHERE clientID = '";
	temp += boost::lexical_cast<std::string>(clientID);
	temp += "'";

	if (mysql_query(&_conn, temp.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : GetFarmInfo mysql_query error" << std::endl;
		return;
	}
	_pSqlRes = mysql_store_result(&_conn);

	int i = 0;
	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 0 = DB의 칼럼(clientID), 1 == json, 2 == farmIndex
		json[i] = _sqlRow[1];
		farmIndex[i] = boost::lexical_cast<int>(_sqlRow[2]);
		i++;
	}
}

void DB::SetFarmInfo(int clientID, std::string farmJson, FARM_INDEX farmIndex)
{
	if (clientID == 0)
	{
		ErrorCheck();
		std::cout << "DB : SetFarmInfo clientID == 0 " << std::endl;
		return;
	}

	if (mysql_query(&_conn, "SELECT * FROM farmInfo") != 0)
	{
		ErrorCheck();
		std::cout << "DB : GetFarmInfo mysql_query error" << std::endl;
		return;
	}
	_pSqlRes = mysql_store_result(&_conn);

	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 0 = DB의 칼럼(clientID), 2 == farmIndex
		if (_sqlRow[0] == boost::lexical_cast<std::string>(clientID))
		{
			if (_sqlRow[2] == boost::lexical_cast<std::string>(farmIndex))
			{
				std::string aa = "UPDATE farmInfo SET infoJson = '";
				aa += farmJson;
				std::string tempStr = "' WHERE clientID = '";
				aa += tempStr;
				aa += boost::lexical_cast<std::string>(clientID);
				aa += "' AND farmIndex = '";
				aa += boost::lexical_cast<std::string>(farmIndex);
				aa += "'";

				if (mysql_query(&_conn, aa.c_str()) != 0)
				{
					ErrorCheck();
					std::cout << "DB : SetFarmInfo mysql_query error" << std::endl;
					return;
				}
				return;
			}
		}
	}
	InsertFarmInfo(clientID, farmJson, farmIndex);
}

void DB::InsertFarmInfo(int clientID, std::string farmJson, FARM_INDEX farmIndex)
{
	std::string query = "";
	query = "INSERT INTO farmInfo VALUES('";
	query += boost::lexical_cast<std::string>(clientID);
	query += "','";
	query += farmJson;
	query += "','";
	query += boost::lexical_cast<std::string>(farmIndex);
	query += "')";

	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : INSERT FarmInfo error" << std::endl;
		return;
	}
	std::cout << "DB : INSERT InsertFarmInfo" << std::endl;
}

int DB::GetGold(int clientID)
{
	if (mysql_query(&_conn, "SELECT * FROM user") != 0)
	{
		ErrorCheck();
		std::cout << "DB : Update mysql_query error" << std::endl;
		return -1;
	}
	_pSqlRes = mysql_store_result(&_conn);

	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 0 = DB의 칼럼(clientID), 2 = DB의 칼럼(gold)
		if (_sqlRow[0] == boost::lexical_cast<std::string>(clientID))
		{
			return boost::lexical_cast<int>(_sqlRow[2]);
		}
	}
	return 0;
}

void DB::SetGold(int clientID, int gold)
{
	std::string aa = "UPDATE user SET gold = '";
	aa += boost::lexical_cast<std::string>(gold);
	std::string tempStr = "' WHERE clientID = '";
	aa += tempStr;
	aa += boost::lexical_cast<std::string>(clientID);
	aa += "'";

	if (mysql_query(&_conn, aa.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : Update SetGold mysql_query error" << std::endl;
		return;
	}
}

void DB::UpdateWinRecord(int clientID, GAME_INDEX gameIndex, int addScore)
{
	//LockGuard upDateLockGuard(_upDateLock);
	if (mysql_query(&_conn, "SELECT * FROM gameInfo") != 0)
	{
		ErrorCheck();
		std::cout << "DB : Update mysql_query error" << std::endl;
		return;
	}
	_pSqlRes = mysql_store_result(&_conn);

	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 0 = DB의 칼럼(clientID), 2 = DB의 칼럼(winRecord)
		// 1 = gameIndex
		if (_sqlRow[0] == boost::lexical_cast<std::string>(clientID))
		{
			if (_sqlRow[1] == boost::lexical_cast<std::string>(gameIndex))
			{
				int temp = boost::lexical_cast<int>(_sqlRow[2]);
				temp += addScore;
				std::string aa = "UPDATE gameInfo SET winRecord = '";
				aa += boost::lexical_cast<std::string>(temp);
				std::string tempStr = "' WHERE clientID = '";
				aa += tempStr;
				aa += boost::lexical_cast<std::string>(clientID);
				aa += "' AND ";
				aa += _sqlRow[1];
				aa += " = '";
				aa += boost::lexical_cast<std::string>(gameIndex);
				aa += "'";

				if (mysql_query(&_conn, aa.c_str()) != 0)
				{
					ErrorCheck();
					std::cout << "DB : Update mysql_query error" << std::endl;
					return;
				}
				return;
			}
		}
	}

	//Update할 ROW가 없다면 ROW를 추가한다
	InsertGameInfo(clientID, gameIndex, 1);
}

void DB::Rank(GAME_INDEX gameIndex, RANK rank[])
{
	std::string rankStr = "";
	rankStr = orderByRank("gameInfo", gameIndex, "winRecord");

	if (mysql_query(&_conn, rankStr.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : OrderByRank mysql_query error" << std::endl;
		return;
	}

	_pSqlRes = mysql_store_result(&_conn);
	int numCol = mysql_num_fields(_pSqlRes); //필드수 출력

	int rowNum = 0;
	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		for (int i = 0; i < numCol; i++)
		{
			rank[rowNum].clientID = boost::lexical_cast<int>(_sqlRow[0]);
			rank[rowNum].winRecord = boost::lexical_cast<int>(_sqlRow[1]);
		}
		rowNum++;
	}
}

std::string DB::orderByRank(std::string tableName, GAME_INDEX gameIndex, std::string column)
{
	std::string orderByStr = "SELECT clientID, winRecord FROM ";
	orderByStr += tableName;
	orderByStr += " WHERE gameIndex = ";
	orderByStr += boost::lexical_cast<std::string>(gameIndex);
	orderByStr += " ORDER BY ";
	orderByStr += (column + " DESC");
	orderByStr += " LIMIT ";
	orderByStr += boost::lexical_cast<std::string>(MAX_RANK_COUNT);

	return orderByStr;
}