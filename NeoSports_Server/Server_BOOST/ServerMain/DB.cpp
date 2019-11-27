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

void DB::Insert(std::string name)
{
	std::string query = "INSERT INTO game(name) values('";
	query += name;
	query += "')";
	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		std::cout << "clientNum이 이미 있다.(NotNull)" << std::endl;
		return;
	}
	std::cout << "ClientNum DB INSERT" << std::endl;
}

void DB::Delete(std::string name)
{
	std::string query = "DELETE FROM game WHERE name =";
	query += name;
	if (mysql_query(&_conn, query.c_str()) != 0)
	{
		std::cout << "DELETE error" << std::endl;
		return;
	}
	std::cout << "DELETE DB ROW" << std::endl;
}


void DB::Update(std::string name, GAME_INDEX gameIndex, int addScore)
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
			//_sqlRow인덱스 0 = DB의 칼럼(name), 2 = DB의 칼럼(winRecord)
			if (_sqlRow[0] == name)
			{
				int temp = boost::lexical_cast<int>(_sqlRow[2]);
				temp += addScore;
				std::string aa = "UPDATE game SET winRecord = '";
				aa += boost::lexical_cast<std::string>(temp);
				std::string tempStr = "' WHERE name LIKE '";
				aa += tempStr;
				aa += name;
				aa += "'";

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

