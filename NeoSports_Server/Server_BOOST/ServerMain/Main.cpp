#include "Server.h"
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

	boost::thread_group tg;
	for (int i = 0; i < systemInfo.dwNumberOfProcessors; i++)
	{
		tg.create_thread(boost::bind(&boost::asio::io_service::run,
			&io_service));
	}
	tg.join_all();

	std::cout << "네트워크 종료" << std::endl;

	return 0;
}