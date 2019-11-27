#include "DB.h"
#include <iostream>
#include <boost/lexical_cast.hpp>

using namespace std;

DB::DB()
{
	_Init();
}

DB::~DB()
{
	mysql_close(&_conn);
}

void DB::_Init()
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
		std::cout << "mysql_real_connect Error" << std::endl;
	}

	if (mysql_select_db(&_conn, "neosports"))
	{
		cout << "mysql_select_db Error : " << mysql_errno(&_conn)
			<< " : " << mysql_error(&_conn) << endl;
	}
}

void DB::SelectQuery()
{
	//TDOD : 랭킹 조회할떄 사용
	//const char* query = "SELECT * FROM game";
	//int state = mysql_query(&_conn, query);
	//if (state == 0)
	//{
	//	_pSqlRes = mysql_store_result(&_conn);
	//	if (_pSqlRes != nullptr)//SELECT가 아닌 나머지는 NULL
	//	{
	//		int numCol = mysql_num_fields(_pSqlRes); //필드수 출력
	//		while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
	//		{
	//			for (int i = 0; i < numCol; i++)
	//			{
	//				cout << _sqlRow[i] << " ";
	//				cout << endl;
	//			}
	//		}
	//		mysql_free_result(_pSqlRes);
	//	}

	//	else
	//	{
	//		int errNo = mysql_errno(&_conn);
	//		if (errNo != 0)
	//		{
	//			cout << "Error" << endl;
	//		}
	//	}
	//}
}

void DB::Insert(int sessionID)
{
	std::string query = "INSERT INTO game(clientNum) values('";
	std::string temp = boost::lexical_cast<std::string> (sessionID);
	query += temp;
	temp = "')";
	query += temp;
	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		std::cout << "clientNum이 이미 있다.(NotNull)" << std::endl;
		return;
	}
	std::cout << "ClientNum DB INSERT" << std::endl;
}

void DB::Delete(int sessionID)
{
	std::string query = "DELETE FROM game WHERE clientNum =";
	std::string temp = boost::lexical_cast<std::string> (sessionID);
	query += temp;
	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		std::cout << "DELETE error" << std::endl;
		return;
	}
	std::cout << "DELETE DB ROW" << std::endl;
}


void DB::Update(int sessionID, GAME_INDEX gameIndex, int addScore)
{
	LockGuard upDateLockGuard(_upDateLock);

	if (mysql_query(&_conn, "SELECT * FROM game") != 0)
	{
		std::cout << "DB Update mysql_query error" << std::endl;
		return;
	}
	_pSqlRes = mysql_store_result(&_conn);
	int numCol = mysql_num_fields(_pSqlRes); //필드수 출력

	switch (gameIndex)
	{
	case EMPTY_GAME:
		break;

	case ROPE_PULL:
	{
		while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
		{
			//_sqlRow인덱스 0 = sessonID, 2 = winRecord
			if (boost::lexical_cast<int>(_sqlRow[0]) == sessionID)
			{
				int temp = boost::lexical_cast<int>(_sqlRow[2]);
				temp+= addScore;
				std::string aa = "UPDATE game SET winRecord = '";
				aa += boost::lexical_cast<std::string>(temp);
				std::string tempStr = "' WHERE clientNum LIKE '";
				aa += tempStr;
				tempStr = boost::lexical_cast<std::string>(sessionID);
				aa += tempStr;
				tempStr = "'";
				aa += tempStr;

				if (mysql_query(&_conn, aa.c_str()) != 0)
				{
					std::cout << "Update mysql_query error" << std::endl;
					return;
				}
			}
		}

		break;
	}
	case ROPE_JUMP:
		break;
	case BASKET_BALL:
		break;
	default:
		break;
	}
}

