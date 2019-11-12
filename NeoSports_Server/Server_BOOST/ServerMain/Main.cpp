#include <iostream>
#include <algorithm>
#include <string>
#include <list>
#include <boost/bind.hpp>
#include <boost/asio.hpp>

const char SERVER_IP[] = "192.168.1.119";
const unsigned short PORT_NUM = 31400;

class Session
{
public:
	Session(boost::asio::io_service& io_service) : _socket(io_service) {}
	boost::asio::ip::tcp::socket& Socket()
	{
		return _socket;
	}
	void PostReceive()
	{
		memset(&_receiveBuffer, '\0', sizeof(_receiveBuffer));
		_socket.async_read_some
		(
			boost::asio::buffer(_receiveBuffer),
			boost::bind(&Session::_handleReceive, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred)
		);
	}
private:
	boost::asio::ip::tcp::socket _socket;
	std::array<char, 128> _receiveBuffer;
	std::string _writeMessage;
	
	void _handleWrite(const boost::system::error_code&, size_t) 
	{

	}
	void _handleReceive(const boost::system::error_code& error, size_t bytesTransferred)
	{
		if (error)
		{
			if (error == boost::asio::error::eof)
			{
				std::cout << "클라와 연결 끊김" << std::endl;
			}
			else
			{
				const std::string strRecvMessage = _receiveBuffer.data();
				std::cout << "받은 메시지 : " << strRecvMessage << ", 받은 크기 : " <<
					bytesTransferred << std::endl;
				char szMessage[128] = { 0, };
				sprintf_s(szMessage, 128 - 1, "Re : %s", strRecvMessage.c_str());
				_writeMessage = szMessage;

				boost::asio::async_write(_socket, boost::asio::buffer(_writeMessage),
					boost::bind(&Session::_handleWrite, this,
						boost::asio::placeholders::error,
						boost::asio::placeholders::bytes_transferred)
				);
				PostReceive();
			}
		}
	}
};

class TCP_Server
{
public:
	TCP_Server(boost::asio::io_context& io_service) : _acceptor(io_service,
		boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(),
			PORT_NUM))
	{
		_pSession = nullptr;
		StartAccept();
	}

	~TCP_Server()
	{
		if (_pSession != nullptr)
		{
			delete _pSession;
		}
	}
private:
	int _nSeqNumber;
	boost::asio::ip::tcp::acceptor _acceptor;
	Session* _pSession;

	void StartAccept()
	{
		std::cout << "클라이언트 접속 대기..." << std::endl;
		_pSession = new Session((boost::asio::io_service&)_acceptor.get_executor().context());
		_acceptor.async_accept(_pSession->Socket(),
			boost::bind(&TCP_Server::handle_accept,
				this,
				_pSession,
				boost::asio::placeholders::error)
		);
		std::cout << _pSession->Socket().local_endpoint() << std::endl;
		std::cout << _pSession->Socket().local_endpoint() << std::endl;
	}

	void handle_accept(Session* pSession, const boost::system::error_code& error)
	{
		if (!error)
		{
			std::cout << "클라 접속 " << std::endl;
			pSession->PostReceive();
		}
	}
};

int main()
{
	boost::asio::io_context io_service;
	TCP_Server server(io_service);
	io_service.run();

	std::cout << "네트워크 접속 종료" << std::endl;

	return 0;
}