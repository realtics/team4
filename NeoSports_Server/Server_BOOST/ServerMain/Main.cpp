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

	/*JSON파일을 역직렬화 해주는 멀티쓰레드
	  역직렬화 후 LogicThread로 데이터를 보내서 싱글쓰레드로 처리한다*/
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