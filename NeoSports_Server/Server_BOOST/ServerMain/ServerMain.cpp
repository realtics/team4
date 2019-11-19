#include "Server.h"
#include <boost/thread/thread.hpp>
#include <process.h>
#include <boost/bind.hpp>

const int MAX_SESSION_COUNT = 100;

int main()
{
	std::vector<std::thread> thread_pool{};

	boost::asio::io_context io_service;

	Server server(io_service);
	server.Init(MAX_SESSION_COUNT);

	SYSTEM_INFO systemInfo;
	GetSystemInfo(&systemInfo);
	server.Start();

	for (int i = 0; i < systemInfo.dwNumberOfProcessors; i++)
	{
		boost::thread thread(boost::bind(&boost::asio::io_service::run,
			&io_service));
		thread.join();
	}

	std::cout << "네트워크 종료" << std::endl;

	return 0;
}