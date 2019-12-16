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
		std::cout << "DB::InsertUser &clientID is NULL" << std::endl;
		return -1;
	}
	if (mysql_query(&_conn, "SELECT * FROM user") != 0)
	{
		std::cout << "DB : Update mysql_query error" << std::endl;
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
		std::cout << "DB : INSERT GameInfo error" << std::endl;
		return;
	}
	std::cout << "DB : INSERT GameInfo" << std::endl;
}

void DB::SetNameTable(int clientID, std::string name)
{
	std::string query = "";
	query = "INSERT INTO name(clientID,name) values('";
	query += boost::lexical_cast<std::string>(clientID);;
	query += "','";
	query += name;
	query += "')";

	if (mysql_query(&_conn, query.c_str()) != 0)
	{
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
		std::cout << "DB : UpdataUserTable error" << std::endl;
		return;
	}

	std::cout << "DB : UPDATE sessionID of clientID" << std::endl;
}

int DB::GetClientID(int sessionID)
{
	if (mysql_query(&_conn, "SELECT * FROM user") != 0)
	{
		std::cout << "DB : Update mysql_query error" << std::endl;
		return -1;
	}
	_pSqlRes = mysql_store_result(&_conn);

	while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	{
		//_sqlRow인덱스 1 = DB의 칼럼(sessionID)
		//_sqlRow인덱스 0 = DB의 칼럼(clientID)
		if (_sqlRow[1] == boost::lexical_cast<std::string>(sessionID))
		{
			return boost::lexical_cast<int>(_sqlRow[0]);;
		}
	}
}

void DB::UpdateWinRecord(int clientID, GAME_INDEX gameIndex, int addScore)
{
	LockGuard upDateLockGuard(_upDateLock);

	if (mysql_query(&_conn, "SELECT * FROM gameInfo") != 0)
	{
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
			rank[rowNum].winRecord = boost::lexical_cast<int>(_sqlRow[2]);
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