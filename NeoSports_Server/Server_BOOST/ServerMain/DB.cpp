#include "DB.h"

#include <iostream>
#include <boost/lexical_cast.hpp>

using namespace std;

const static char* defaultFarmInfo_0 = "vCwAAB+LCAAAAAAAAAutmstOG0EQRf+l17Og+t2zz4I/IIpYTLCJLDnGsgcEQvx7sowAybfI2Xh5VTM+fafGxz9ew/Fhd1jD/Bquz99+H9eXMK+nx+0UbsJ8NYXvfz/fprC+HLdhDj+XzX45bMIUnpbTbll3D4frw2b7HGZ7mz6Lul/253+zDMyKStaVlpXArAxeYwGzKpjVwKwO3vuBZJnOvZglcS9mSdxfvPemcy9mMdwbyL2B3BvIvYHcG8h9BPs+gtxHsO8j2PcR5D7q3ItzSdyLWQz3EeQ+6txfnCuBfZ/APSeB3Cew75POvTgX0/cJ5D6B3CedezGL6fsM9n0G+z6D3Gew7zPY9xns+wxyn3XuxSyG+wxyX8C+LyD3BeS+gNwXsO8LyH0B9/sCcl/APaeAe04Fua8g9xV8r63gnlNB7ut77n+dlvP561MxbV/BLaeCbV/Btm8g9Q3c7htIfQPbvr2n/u5xv+6elnX79TSm7xtIfgPJb2DfN5D8Du73Hez7DpLfwb7vYN938L22g3tOB7nvYON3kPsBNv4AuR8g9wPkfoDcD5D7AXI/wP1+gNwPcL83Utiaw9hethQOZatOxmw75pC2ahiz7ZhD26phDP/mELfqtwmdAFTdou4Wlbeovf2gb/9r4TfU4KIK1+Fw1cuEJC5pcc2hcdUw6AyQItccJvcyGqTKNdLlmkPmqpNB/2Jw6Fw1DHoKkELXHEZXDYNOAOl0zSF11TDoGeDQumoYdAIcYlf9NqFngEPtqmHMr55Gyl1z2F11MuhNwOF31cmgNwHS8BqpeM3leG//AIvH+w28LAAA";
const static char* defaultFarmInfo_1 = "8QwAAB+LCAAAAAAAAAut1s0KgkAUhuF7OetZNP7rvkV3UEQLqREENZkmQsR7T5hNi6gR3o3g5uU5m0/Ps4z3dnBSzXJ47PvRTVI5+zRKjlLtlJzW56Kkr52xbd1JJa/WGlHiptGsb9e7HYyVRX0LNXX3+CyVQEn/N91MUz87F5j6iQpNRZwq4lQxp4o5VcKpEk6VcqqUU2WcKuNUOafKOVXBqQpOVXKqklPpgGnf2GJc5LyD+67Bgdfgwmtw4jW48Tpg5EM/+AEjv+kvRIMHkq2IaPkTY5CFtDwr4VIpeCHS8qwMZCEtz8pBFtLyrAJkhbUub81yI5/xDAAA";
const static char* defaultFarmInfo_4 = "WgEAAB+LCAAAAAAAAAuLrlYqyM/MK1GyqlbyLHbNLSipVLJKS8wpTtVRilCyMtNRigSStTpKJZUFqUpWhrU6xGgwgmswJaTB0BiswxSuw4yQDhOwBgu4BnNCGizBGozhGkwIugmswZx4G6CeMEeyIhYAOiazV1oBAAA=";

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

	stmt = mysql_stmt_init(&_conn);
	if (!stmt)
	{
		fprintf(stderr, " mysql_stmt_init(), out of memory\n");
		exit(0);
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
	query = "INSERT INTO user(clientID,sessionID) values(?,?)";
	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : InsertUser error" << std::endl;
		return 0;
	}
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)clientID;
	bind[0].is_null = 0;
	bind[0].length = 0;

	bind[1].buffer_type = MYSQL_TYPE_LONG;
	bind[1].buffer = (char*)&data;
	bind[1].is_null = 0;
	bind[1].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : InsertUser error" << std::endl;
		return 0;
	}

	if (mysql_stmt_execute(stmt))
	{
		ErrorCheck();
		std::cout << "DB : InsertUser error" << std::endl;
		return 0;
	}
	InsertDefaultFarmInfo(*clientID);
	return *clientID;
}

void DB::InsertGameInfo(int clientID, GAME_INDEX gameIndex, int winRecord)
{
	std::string query = "";
	query = "INSERT INTO gameInfo values(?,?,?)";

	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : InsertGameInfo error" << std::endl;
		return;
	}
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)&clientID;
	bind[0].is_null = 0;
	bind[0].length = 0;

	bind[1].buffer_type = MYSQL_TYPE_LONG;
	bind[1].buffer = (char*)&gameIndex;
	bind[1].is_null = 0;
	bind[1].length = 0;

	bind[2].buffer_type = MYSQL_TYPE_LONG;
	bind[2].buffer = (char*)&winRecord;
	bind[2].is_null = 0;
	bind[2].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : InsertGameInfo error" << std::endl;
		return;
	}

	if (mysql_stmt_execute(stmt))
	{
		ErrorCheck();
		std::cout << "DB : InsertGameInfo error" << std::endl;
		return;
	}
	std::cout << "DB : INSERT GameInfo" << std::endl;
}

void DB::InitUserTable()
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
			std::string query = "UPDATE name SET name = ? WHERE clientID = ?";

			if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
			{
				ErrorCheck();
				std::cout << "DB : SetNameTable error" << std::endl;
				return;
			}

			memset(bind, 0, sizeof(bind));
			bind[0].buffer_type = MYSQL_TYPE_STRING;
			bind[0].buffer = (char*)name.c_str();
			bind[0].buffer_length = strlen(name.c_str());
			bind[0].is_null = 0;
			bind[0].length = 0;

			bind[1].buffer_type = MYSQL_TYPE_LONG;
			bind[1].buffer = (char*)&clientID;
			bind[1].is_null = 0;
			bind[1].length = 0;

			if (mysql_stmt_bind_param(stmt, bind))
			{
				ErrorCheck();
				std::cout << "DB : SetNameTable error" << std::endl;
				return;
			}

			if (mysql_stmt_execute(stmt))
			{
				ErrorCheck();
				std::cout << "DB : SetNameTable error" << std::endl;
				return;
			}
			return;
		}
	}

	std::string query = "";
	query = "INSERT INTO name(clientID,name) values(?,?)";

	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : SetNameTable error" << std::endl;
		return;
	}
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)&clientID;
	bind[0].is_null = 0;
	bind[0].length = 0;

	bind[1].buffer_type = MYSQL_TYPE_STRING;
	bind[1].buffer = (char*)&name;
	bind[1].buffer_length = strlen(name.c_str());
	bind[1].is_null = 0;
	bind[1].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : SetNameTable INSERT error" << std::endl;
		return;
	}

	if (mysql_stmt_execute(stmt))
	{
		ErrorCheck();
		std::cout << "DB : SetNameTable INSERT error" << std::endl;
		return;
	}
	std::cout << "DB : INSERT Name" << std::endl;
	mysql_free_result(_pSqlRes);
}

void DB::DeleteUser(int clientID)
{
	std::string query = "DELETE FROM user WHERE clientID = ?";

	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : DeleteUser error" << std::endl;
		return;
	}
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)&clientID;
	bind[0].is_null = 0;
	bind[0].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : DeleteUser error" << std::endl;
		return;
	}

	if (mysql_stmt_execute(stmt))
	{
		ErrorCheck();
		std::cout << "DB : DeleteUser error" << std::endl;
		return;
	}
	std::cout << "DB : DELETE ROW" << std::endl;
}

void DB::UpdataUserTable(int clientID, int sessionID)
{
	std::string query = "UPDATE user SET sessionID = ? WHERE clientID = ?";
	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : UpdataUserTable error" << std::endl;
		return;
	}
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)&sessionID;
	bind[0].is_null = 0;
	bind[0].length = 0;

	bind[1].buffer_type = MYSQL_TYPE_LONG;
	bind[1].buffer = (char*)&clientID;
	bind[1].is_null = 0;
	bind[1].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : UpdataUserTable error" << std::endl;
		return;
	}

	if (mysql_stmt_execute(stmt))
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
		std::cout << "DB : GetFarmInfo clientID == 0 " << std::endl;
		return;
	}

	std::string temp = "SELECT * FROM farmInfo WHERE clientID = '";
	temp += boost::lexical_cast<std::string>(clientID);
	temp += "'";

	mysql_close(&_conn);
	Init();

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

bool DB::CheckClientID(int clientID)
{
	if (clientID == 0)
		return false;
	std::string temp = "SELECT clientID FROM user WHERE clientID = '";
	temp += boost::lexical_cast<std::string>(clientID);
	temp += "'";

	if (mysql_query(&_conn, temp.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : CheckClientID mysql_query error" << std::endl;
		return false;
	}
	_pSqlRes = mysql_store_result(&_conn);
	int rowCount = mysql_num_rows(_pSqlRes);

	if (rowCount <= 0 || rowCount == NULL)
		return false;
	else
		return true;
}

void DB::SetFarmInfo(int clientID, std::string farmJson, FARM_INDEX farmIndex)
{
	if (clientID == 0)
	{
		std::cout << "DB : SetFarmInfo clientID == 0 " << std::endl;
		return;
	}

	if (mysql_query(&_conn, "SELECT * FROM farmInfo") != 0)
	{
		ErrorCheck();
		std::cout << "DB : SetFarmInfo mysql_query error" << std::endl;
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
				std::string query = "UPDATE farmInfo SET infoJson = '";
				query += farmJson;
				query += "' WHERE clientID = '";
				query += boost::lexical_cast<std::string>(clientID);
				query += "' AND farmIndex = '";
				query += boost::lexical_cast<std::string>(farmIndex);
				query += "'";

				if (mysql_query(&_conn, query.c_str()) != 0)
				{
					ErrorCheck();
					std::cout << "DB : SetFarmInfo mysql_query error" << std::endl;
					return;
				}

				std::cout << "DB : SetFarmInfo" << std::endl;
				return;
			}
		}
		/*std::string query = "UPDATE farmInfo SET infoJson = ? WHERE clientID = ? AND farmIndex = ?";

		if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
		{
			ErrorCheck();
			std::cout << "DB : SetFarmInfo error" << std::endl;
			return;
		}
		memset(bind, 0, sizeof(bind));
		bind[0].buffer_type = MYSQL_TYPE_STRING;
		bind[0].buffer = (char*)&farmJson;
		bind[0].buffer_length = strlen(farmJson.c_str());
		bind[0].is_null = 0;
		bind[0].length = 0;

		bind[1].buffer_type = MYSQL_TYPE_LONG;
		bind[1].buffer = (char*)&clientID;
		bind[1].is_null = 0;
		bind[1].length = 0;

		bind[2].buffer_type = MYSQL_TYPE_LONG;
		bind[2].buffer = (char*)&farmIndex;
		bind[2].is_null = 0;
		bind[2].length = 0;

		if (mysql_stmt_bind_param(stmt, bind))
		{
			ErrorCheck();
			std::cout << "DB : SetFarmInfo error" << std::endl;
			return;
		}

		if (mysql_stmt_execute(stmt))
		{
			ErrorCheck();
			std::cout << "DB : SetFarmInfo error" << std::endl;
			return;
		}
		return;*/
	}
	InsertFarmInfo(clientID, farmJson, farmIndex);
}

void DB::InsertDefaultFarmInfo(int clientID)
{
	InsertFarmInfo(clientID, defaultFarmInfo_0, FARM_INDEX::LAND);
	InsertFarmInfo(clientID, defaultFarmInfo_1, FARM_INDEX::ROAD);
	InsertFarmInfo(clientID, defaultFarmInfo_4, FARM_INDEX::GARBAGE);
}


void DB::InsertFarmInfo(int clientID, std::string farmJson, FARM_INDEX farmIndex)
{
	std::string query = "INSERT INTO farmInfo VALUES('";
	query += boost::lexical_cast<std::string>(clientID);
	query += "','";
	query += farmJson;
	query += "','";
	query += boost::lexical_cast<std::string>(farmIndex);
	query += "')";

	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : InsertFarmInfo mysql_query error" << std::endl;
		return;
	}
	/*std::string query = "INSERT INTO farmInfo VALUES(?,?,?)";

	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : InsertFarmInfo error" << std::endl;
		return;
	}
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)&clientID;
	bind[0].is_null = 0;
	bind[0].length = 0;

	bind[1].buffer_type = MYSQL_TYPE_STRING;
	bind[1].buffer = (char*)&farmJson;
	bind[1].buffer_length = strlen(farmJson.c_str());
	bind[1].is_null = 0;
	bind[1].length = 0;

	bind[2].buffer_type = MYSQL_TYPE_LONG;
	bind[2].buffer = (char*)&farmIndex;
	bind[2].is_null = 0;
	bind[2].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : InsertFarmInfo error" << std::endl;
		return;
	}

	if (mysql_stmt_execute(stmt))
	{
		ErrorCheck();
		std::cout << "DB : InsertFarmInfo error" << std::endl;
		return;
	}*/
	std::cout << "DB : INSERT InsertFarmInfo" << std::endl;
}

int DB::GetGold(int clientID)
{
	std::string getGoldTemp = "SELECT gold FROM user WHERE clientID= '";
	getGoldTemp += boost::lexical_cast<std::string>(clientID);
	getGoldTemp += "'";

	if (mysql_query(&_conn, getGoldTemp.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : GetGold mysql_query error" << std::endl;
		return -1;
	}
	_pSqlRes = mysql_store_result(&_conn);
	_sqlRow = mysql_fetch_row(_pSqlRes);

	return boost::lexical_cast<int>(_sqlRow[0]);
}

void DB::SetGold(int clientID, int gold)
{
	std::string query = "UPDATE user SET gold = ? WHERE clientID = ?";

	if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
	{
		ErrorCheck();
		std::cout << "DB : SetGold error" << std::endl;
		return;
	}

	int sumGold = gold + GetGold(clientID);
	memset(bind, 0, sizeof(bind));
	bind[0].buffer_type = MYSQL_TYPE_LONG;
	bind[0].buffer = (char*)&sumGold;
	bind[0].is_null = 0;
	bind[0].length = 0;

	bind[1].buffer_type = MYSQL_TYPE_LONG;
	bind[1].buffer = (char*)&clientID;
	bind[1].is_null = 0;
	bind[1].length = 0;

	if (mysql_stmt_bind_param(stmt, bind))
	{
		ErrorCheck();
		std::cout << "DB : SetGold error" << std::endl;
		return;
	}

	if (mysql_stmt_execute(stmt))
	{
		ErrorCheck();
		std::cout << "DB : SetGold error" << std::endl;
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
				std::string query = "UPDATE gameInfo SET winRecord = ? WHERE clientID = ?";
				query += " AND gameIndex = ?";

				if (mysql_stmt_prepare(stmt, query.c_str(), strlen(query.c_str())))
				{
					ErrorCheck();
					std::cout << "DB : UpdateWinRecord error" << std::endl;
					return;
				}
				memset(bind, 0, sizeof(bind));
				bind[0].buffer_type = MYSQL_TYPE_LONG;
				bind[0].buffer = (char*)&temp;
				bind[0].is_null = 0;
				bind[0].length = 0;

				bind[1].buffer_type = MYSQL_TYPE_LONG;
				bind[1].buffer = (char*)&clientID;
				bind[1].is_null = 0;
				bind[1].length = 0;

				bind[2].buffer_type = MYSQL_TYPE_LONG;
				bind[2].buffer = (char*)&gameIndex;
				bind[2].is_null = 0;
				bind[2].length = 0;

				if (mysql_stmt_bind_param(stmt, bind))
				{
					ErrorCheck();
					std::cout << "DB : UpdateWinRecord error" << std::endl;
					return;
				}

				if (mysql_stmt_execute(stmt))
				{
					ErrorCheck();
					std::cout << "DB : UpdateWinRecord error" << std::endl;
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
	rankStr = orderByRank(gameIndex);

	if (mysql_query(&_conn, rankStr.c_str()) != 0)
	{
		ErrorCheck();
		std::cout << "DB : Rank mysql_query error" << std::endl;
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

std::string DB::orderByRank(GAME_INDEX gameIndex)
{
	std::string query = "SELECT clientID, winRecord FROM gameInfo WHERE gameIndex = '";
	query += boost::lexical_cast<std::string>(gameIndex);
	query += "'";
	query += " ORDER BY winRecord DESC LIMIT ";
	query += boost::lexical_cast<std::string>(MAX_RANK_COUNT);

	return query;
}