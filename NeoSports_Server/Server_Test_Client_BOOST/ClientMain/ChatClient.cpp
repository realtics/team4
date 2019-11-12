#include "ChatClient.h"

ChatClient::ChatClient(boost::asio::io_service& io_service)
	: _ioService(io_service), _socket(io_service)
{
	_isLogin = false;
	InitializeCriticalSectionAndSpinCount(&_lock, 4000);

}

ChatClient::~ChatClient()
{
	EnterCriticalSection(&_lock);

	while (_sendDataDeq.empty() == false)
	{
		delete[] _sendDataDeq.front();
		_sendDataDeq.pop_front();
	}

	LeaveCriticalSection(&_lock);
	DeleteCriticalSection(&_lock);
}

bool ChatClient::IsConnecting()
{
	return _socket.is_open();
}

void ChatClient::SetLoginTrue()
{
	_isLogin = true;
}

bool ChatClient::IsLogin()
{
	return _isLogin;
}

void ChatClient::Connect(boost::asio::ip::tcp::endpoint endpoint)
{
	_packetBufferMark = 0;

	_socket.async_connect(endpoint,
		boost::bind(&ChatClient::_ConnectHandle, this,
			boost::asio::placeholders::error)
	);
}

void ChatClient::Close()
{
	if (_socket.is_open())
	{
		_socket.close();
	}
}

void ChatClient::PostSend(const bool immediately, const int size, char* data)
{
	char* sendData = nullptr;
	EnterCriticalSection(&_lock);

	if (immediately == false)
	{
		sendData = new char[size];
		memcpy(sendData, data, size);

		_sendDataDeq.push_back(sendData);
	}
	else
		sendData = data;

	if (immediately || _sendDataDeq.size() < 2)
	{
		boost::asio::async_write(_socket, boost::asio::buffer(sendData, size),
			boost::bind(&ChatClient::_WriteHandle, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred)
		);
	}
	LeaveCriticalSection(&_lock);
}

void ChatClient::_PostReceive()
{
	memset(&_receiveBuffer, '\0', sizeof(_receiveBuffer));
	_socket.async_read_some(boost::asio::buffer(_receiveBuffer),
		boost::bind(&ChatClient::_ReceiveHandle, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred)
	);
}

void ChatClient::_ConnectHandle(const boost::system::error_code& error)
{
	if (!error)
	{
		std::cout << "서버접속!" << std::endl;
		std::cout << "Name? : " << std::endl;

		_PostReceive();
	}

	else
	{
		std::cout << "접속 실패.error No : " << error.value() << " error Message : " << error.message()
			<< std::endl;
	}
}

void ChatClient::_WriteHandle(const boost::system::error_code& error, size_t bytesTransferred)
{
	EnterCriticalSection(&_lock);
	delete[] _sendDataDeq.front();
	_sendDataDeq.pop_front();

	char* data = nullptr;

	if (_sendDataDeq.empty() == false)
	{
		data = _sendDataDeq.front();
	}

	LeaveCriticalSection(&_lock);

	if (data != nullptr)
	{
		PACKET_HEADER* header = (PACKET_HEADER*)data;
		PostSend(true, header->packetSize, data);
	}
}

void ChatClient::_ReceiveHandle(const boost::system::error_code& error, size_t bytesTransferred)
{
	if (error)
	{
		if (error == boost::asio::error::eof)
		{
			std::cout << "클라이언트와 연결이 끊어졌습니다" << std::endl;
		}
		else
		{
			std::cout << "error No: " << error.value() << " error Message: " << error.message() << std::endl;
		}

		Close();
	}
	else
	{
		memcpy(&_packetBuffer[_packetBufferMark], _receiveBuffer.data(), bytesTransferred);

		int packetData = _packetBufferMark + bytesTransferred;
		int readData = 0;

		while (packetData > 0)
		{
			if (packetData < sizeof(PACKET_HEADER))
			{
				break;
			}

			PACKET_HEADER* header = (PACKET_HEADER*)&_packetBuffer[readData];

			if (header->packetSize <= packetData)
			{
				_ProcessPacket(&_packetBuffer[readData]);

				packetData -= header->packetSize;
				readData == header->packetSize;
			}
			else
				break;

		}
		if (packetData > 0)
		{
			char tempBuffer[MAX_RECEIVE_BUFFER_LEN] = { 0, };
			memcpy(&tempBuffer[0],&_packetBuffer[readData],packetData);
			memcpy(&_packetBuffer[0],&tempBuffer[0],packetData);
		}

		_packetBufferMark = packetData;

		_PostReceive();
	}
}

void ChatClient::_ProcessPacket(const char* data)
{
	PACKET_HEADER* header = (PACKET_HEADER*)data;

	switch (header->packetIndex)
	{
	case RES_IN:
	{
		PACKET_RES_IN* pPacket = (PACKET_RES_IN*)data;

		SetLoginTrue();

		std::cout << "클라이언트 로그인 성공 ?: " << pPacket->isSuccess << std::endl;
	}
	break;
	case NOTICE_CHAT:
	{
		PACKET_NOTICE_CHAT* pPacket = (PACKET_NOTICE_CHAT*)data;

		std::cout << pPacket->szName << ": " << pPacket->szMessage << std::endl;
	}
	break;
	}
}