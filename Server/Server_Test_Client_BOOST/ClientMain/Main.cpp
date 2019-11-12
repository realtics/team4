#include<iostream>
#include<boost/asio.hpp>
#include<boost/bind.hpp>

const char SERVER_IP[] = "192.168.1.119";
const unsigned short PORT_NUM = 31400;

class TCP_Client
{
public:
	TCP_Client(boost::asio::io_context& io_service) :_io_service(io_service),
		_socket(io_service), _nSeqnumber(0) {}

	void Connect(boost::asio::ip::tcp::endpoint& endpoint)
	{
		_socket.async_connect(endpoint, boost::bind(&TCP_Client::handle_connect,
			this,
			boost::asio::placeholders::error));
	};

private:
	boost::asio::io_context& _io_service;
	boost::asio::ip::tcp::socket _socket;
	int _nSeqnumber;
	std::array<char, 128> _receiveBuffer;
	std::string _writeMessage;

	void PostWrite()
	{
		if (!(_socket.is_open()))
		{
			return;
		}
		if (_nSeqnumber > 7)
		{
			_socket.close();
			return;
		}
		++_nSeqnumber;

		char szMessage[128] = { 0, };
		sprintf_s(szMessage, 128 - 1, "%d ", _nSeqnumber);

		_writeMessage = szMessage;

		boost::asio::async_write(_socket, boost::asio::buffer(_writeMessage),
			boost::bind(&TCP_Client::handle_write, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred)
		);
		PostReceive();
	}

	void PostReceive()
	{
		memset(&_receiveBuffer, '\0', sizeof(_receiveBuffer));

		_socket.async_read_some(boost::asio::buffer(_receiveBuffer),
			boost::bind(&TCP_Client::handle_receive, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred)
		);
	}

	void handle_connect(const boost::system::error_code& error)
	{
		if (error)
		{
			std::cout << "connet failed : " << error.message() << std::endl;
		}
		else
		{
			std::cout << "connected" << std::endl;
			PostWrite();
		}
	}
	void handle_receive(const boost::system::error_code& error, size_t bytesTransferred)
	{
		if (error)
		{
			if (error == boost::asio::error::eof)
				std::cout << "서버와 연결 끊김" << std::endl;
			else
				std::cout << "error No : " << error.value() << " error Message : " <<
				error.message() << std::endl;
		}
		else
		{
			const std::string strRecvMessage = _receiveBuffer.data();
			std::cout << "받은 메시지 : " << strRecvMessage << " 받은 크기 : " <<
				bytesTransferred << std::endl;
			PostWrite();
		}
	}
	void handle_write(const boost::system::error_code&, size_t)
	{

	}
};

int main()
{
	boost::asio::io_service io;
	boost::asio::ip::tcp::endpoint endPoint(boost::asio::ip::address::
		from_string(SERVER_IP), PORT_NUM);

	TCP_Client client(io);
	client.Connect(endPoint);
	io.run();

	std::cout << "네트워크 접속 종료" << std::endl;

	return 0;
}