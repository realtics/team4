#include "ServerSession.h"
#include "ChattingServer.h"
#include <iostream>
#include <boost/property_tree/ptree.hpp> 
#include <boost/property_tree/json_parser.hpp>
#include <boost/foreach.hpp>

Session::Session(int sessionID, boost::asio::io_context& io_service, ChatServer* serverPtr)
	:_socket(io_service),
	_sessionId(sessionID)
	, _serverPtr(serverPtr)
{

}

Session::~Session()
{
	while (_sendDataDeq.empty() == false)
	{
		delete[] _sendDataDeq.front();
		_sendDataDeq.pop_front();
	}
}

void Session::Init()
{
	_packetBufferMark = 0;
}

void Session::PostReceive()
{
	_socket.async_read_some(boost::asio::buffer(_receiveBuffer),
		boost::bind(&Session::ReceiveHandle, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred)
	);
}

void  Session::ReceiveHandle(const boost::system::error_code& error, size_t bytesTransferred)
{
	if (error)
	{
		if (error == boost::asio::error::eof)
		{
			std::cout << "Å¬¶ó¿Í ¿¬°á ²÷±è" << std::endl;
		}
		else
		{
			std::cout << "error No : " << error.value() << " error Message : " << error.message() <<
				std::endl;
		}
		_serverPtr->CloseSession(_sessionId);
	}
	else
	{
		memcpy(&_packetBuffer[_packetBufferMark], _receiveBuffer.data(), bytesTransferred);
		int packetData = _packetBufferMark + bytesTransferred;
		int readData = 0;

		////TODO : JSONÆÄ½ÌºÎºÐ.ÇÔ¼öÈ­ ½ÃÅ°±â

		std::string temp;
		temp = _receiveBuffer.data();
		std::cout << temp << std::endl;

		boost::property_tree::ptree pt;
		std::istringstream is(temp);
		boost::property_tree::read_json(is, pt);

		boost::property_tree::ptree& children = pt.get_child("header");
		int headerIndex = children.get<int>("packetIndex");
		int packetSize = children.get<int>("packetSize");

		int jsonData1 = pt.get<int>("Data1");
		std::string jsonData2 = pt.get<std::string>("Data2");

		std::cout << "1´ð½ºÁ¢±Ù Data1 : " << jsonData1 << std::endl;
		std::cout << "1´ð½ºÁ¢±Ù Data2 : " << jsonData2 << std::endl;
		std::cout << "2´ð½ºÁ¢±Ù(header->index) : " << headerIndex << std::endl;
		std::cout << "2´ð½ºÁ¢±Ù(header->packetSize) : " << packetSize << std::endl;



		/////////

		while (packetData > 0)
		{
			if (packetData < sizeof(PACKET_HEADER))
			{
				break;
			}


			PACKET_HEADER* header = (PACKET_HEADER*)&_packetBuffer[readData];

			if (header->packetSize <= packetData)
			{
				_serverPtr->ProcessPacket(_sessionId, &_packetBuffer[readData]);

				packetData -= header->packetSize;
				readData += header->packetSize;
			}
			else
				break;
		}

		if (packetData > 0)
		{
			char tempBuffer[MAX_RECEIVE_BUFFER_LEN] = { 0, };
			memcpy(&tempBuffer[0], &_packetBuffer[readData], packetData);
			memcpy(&_packetBuffer[0], &tempBuffer[0], packetData);
		}

		_packetBufferMark = packetData;

		PostReceive();
	}
}

void Session::PostSend(const bool Immediately, const int size, char* data)
{
	char* sendData = nullptr;
	if (Immediately == false)
	{
		sendData = new char[size];
		memcpy(sendData, data, size);

		_sendDataDeq.push_back(sendData);
	}
	else
	{
		sendData = data;
	}

	if (Immediately == false && _sendDataDeq.size() > 1)
	{
		return;
	}

	boost::asio::async_write(_socket, boost::asio::buffer(sendData, size),
		boost::bind(&Session::WriteHandle, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred)
	);
}

void Session::WriteHandle(const boost::system::error_code& error, size_t butesTransferred)
{
	delete[] _sendDataDeq.front();
	_sendDataDeq.pop_front();

	if (_sendDataDeq.empty() == false)
	{
		char* data = _sendDataDeq.front();

		PACKET_HEADER* header = (PACKET_HEADER*)data;
		PostSend(true, header->packetSize, data);
	}
}

int Session::GetSessionID()
{
	return _sessionId;
}

void Session::SetNanme(const char* name)
{
	_name = name;
}

const char* Session::GetName()
{
	return _name.c_str();
}

