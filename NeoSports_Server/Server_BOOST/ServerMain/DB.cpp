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

}

void DB::_Init()
{
	if (mysql_init(&_conn) == nullptr)
	{
		std::cout << "MySql Init Error" << std::endl;
	}

	if (mysql_real_connect(
		&_conn, "localhost", //������
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
	const char* query = "SELECT * FROM game";
	int state = mysql_query(&_conn, query);
	if (state == 0)
	{
		_pSqlRes = mysql_store_result(&_conn);
		if (_pSqlRes != nullptr)//SELECT�� �ƴ� �������� NULL
		{
			int numCol = mysql_num_fields(_pSqlRes); //�ʵ�� ���
			while ((_sqlRow = mysql_fetch_row(_pSqlRes)) != nullptr)
			{
				for (int i = 0; i < numCol; i++)
				{
					cout << _sqlRow[i] << " ";
					cout << endl;
				}
			}
			mysql_free_result(_pSqlRes);
		}

		else
		{
			int errNo = mysql_errno(&_conn);
			if (errNo != 0)
			{
				cout << "Error" << endl;
			}
		}
		mysql_close(&_conn);
	}
}

void DB::Insert()
{
	std::string query = "INSERT INTO";

}

void DB::Update(int sessionID, GAME_INDEX gameIndex, int addScore)
{
	switch (gameIndex)
	{
	case EMPTY_GAME:
		break;

	case ROPE_PULL:
	{
		std::string query = "SELECT winRecord FROM game WHERE clientNum = '";
		std::string temp = boost::lexical_cast<std::string>(sessionID);
		query += temp;
		query += "'";
		std::cout << query << std::endl;

		int curScored = mysql_query(&_conn, query.c_str());
		std::cout << curScored << std::endl;

		std::string updateQuery = "UPDATE game SET winRecord = '";
		updateQuery += (curScored + addScore);
		updateQuery += "' WHERE clientNum = '";
		updateQuery += temp;
		updateQuery += "'";
		std::cout << updateQuery << std::endl;

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

