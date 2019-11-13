#include "ChatClient.h"

const char SERVER_IP[] = "192.168.1.119";
const unsigned short PORT_NUM = 31400;



int main()
{
	boost::asio::io_service io;
	boost::asio::ip::tcp::endpoint endPoint(boost::asio::ip::address::
		from_string(SERVER_IP), PORT_NUM);

	ChatClient client(io);
	client.Connect(endPoint);

	boost::thread thread(boost::bind(&boost::asio::io_context::run, &io));

	char sizeInputMessage[MAX_MESSAGE_LEN * 2] = { 0, };

	while (std::cin.getline(sizeInputMessage, MAX_MESSAGE_LEN))
	{
		if (strnlen_s(sizeInputMessage, MAX_MESSAGE_LEN) == 0)
		{
			break;
		}

		if (client.IsConnecting() == false)
		{
			std::cout << "서버와 연결되지 않음" << std::endl;
			continue;
		}

		if (client.IsLogin() == false)
		{
			PACKET_REQ_IN sendPacket;
			sendPacket.Init();
			strncpy_s(sendPacket.szName, MAX_NAME_LEN, sizeInputMessage, MAX_NAME_LEN - 1);
			client.PostSend(false, sendPacket.packetSize, (char*)&sendPacket);
		}

		else
		{
			PACKET_REQ_CHAT sendPacket;
			sendPacket.Init();
			strncpy_s(sendPacket.szMessage, MAX_MESSAGE_LEN, sizeInputMessage, MAX_MESSAGE_LEN - 1);

			client.PostSend(false, sendPacket.packetSize, (char*)&sendPacket);
		}
	}
	
	io.stop();
	client.Close();
	thread.join();

	std::cout << "클라이언트를 종료해 주세요" << std::endl;
	return 0;
}