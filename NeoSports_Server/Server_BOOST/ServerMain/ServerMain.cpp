//19.11.15�� �۾�����

#include "Server.h"

const int MAX_SESSION_COUNT = 100;

int main()
{
	boost::asio::io_context io_service;

	Server server(io_service);
	server.Init(MAX_SESSION_COUNT);
	server.Start();

	io_service.run();

	std::cout << "��Ʈ��ũ ����" << std::endl;

	return 0;
}