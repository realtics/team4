#include "DB.h"
#include <iostream>

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
	const char* query = "SELECT * FROM game";
	int state = mysql_query(&_conn, query);
	if (state == 0)
	{
		_pSqlRes = mysql_store_result(&_conn);
		if (_pSqlRes != nullptr)//SELECT가 아닌 나머지는 NULL
		{
			int numCol = mysql_num_fields(_pSqlRes); //필드수 출력
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
