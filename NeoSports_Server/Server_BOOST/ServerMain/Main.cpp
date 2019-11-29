//패킷을 처리하는 로직을 분리해야한다(서버가 어느정도 돌아가는걸 확인한후)

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

	std::cout << "End Network" << std::endl;

	return 0;
}