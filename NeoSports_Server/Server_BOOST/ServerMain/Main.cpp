#include "Server.h"
#include "LogicProcess.h"

#include <process.h>
#include <boost/bind.hpp>
#include <boost/thread/thread.hpp>

const int MAX_SESSION_COUNT = 100;

int main()
{
	boost::asio::io_context io_service;

	Server server(io_service);
	server.Init(MAX_SESSION_COUNT);

	SYSTEM_INFO systemInfo;
	GetSystemInfo(&systemInfo);

	server.Start();

	/*JSON������ ������ȭ ���ִ� ��Ƽ������
	  ������ȭ �� LogicThread�� �����͸� ������ �̱۾������ ó���Ѵ�*/
	boost::thread_group tg;
	for (int i = 0; i < systemInfo.dwNumberOfProcessors; i++)
	{
		tg.create_thread(boost::bind(&boost::asio::io_service::run,
			&io_service));
	}

	LogicProcess logicProcess(&server);
	tg.create_thread(boost::bind(&LogicProcess::ProcessPacket, &logicProcess));

	tg.join_all();

	logicProcess.StopProcess();
	std::cout << "End Network" << std::endl;

	return 0;
}