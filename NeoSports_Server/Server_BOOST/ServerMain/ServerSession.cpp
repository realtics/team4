#include "ServerSession.h"
#include "ChattingServer.h"
#include "Json.h"
#include <iostream>

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
			std::cout << "클라와 연결 끊김" << std::endl;
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
		////TODO : JSON파싱부분.함수화 시키기
		//역직렬화
		char* temp = DeSerializationJson(_receiveBuffer.data());
		memcpy(&_packetBuffer[_packetBufferMark], temp, bytesTransferred);

		int packetData = _packetBufferMark + bytesTransferred;
		int readData = 0;

		//직렬화
		/*TEST_PACKET testPakcet;
		testPakcet.header.packetIndex = 2;
		testPakcet.header.packetSize = sizeof(TEST_PACKET);
		testPakcet.Data1 = 1000;
		testPakcet.Data2 = "mesaage";

		std::string recvTemp;

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex",testPakcet.header.packetIndex);
		ptSendHeader.put<int>("packetSize",testPakcet.header.packetSize);
		ptSend.add_child("header", ptSendHeader);
		ptSend.put<int>("Data1", testPakcet.Data1);
		ptSend.put<std::string>("Data2", testPakcet.Data2);

		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		std::string sendStr = os.str();
		std::cout << sendStr << std::endl;
		PostSend(false, std::strlen(sendStr.c_str()), (char*)sendStr.c_str());*/
		
		/////////

		while (packetData > 0)
		{
			if (packetData < sizeof(PACKET_HEADER))
			{
				break;
			}

			PACKET_HEADER* header = (PACKET_HEADER*)&_packetBuffer[readData];

			if (header->packetSize >= packetData)
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


//template<typename T>
char* Session::DeSerializationJson(char* jsonStr)
{
	boost::property_tree::ptree ptRecv;
	std::istringstream is(jsonStr);
	boost::property_tree::read_json(is, ptRecv);

	boost::property_tree::ptree& children = ptRecv.get_child("header");
	int headerIndex = children.get<int>("packetIndex");

	switch (headerIndex)
	{
	case 101:
	{
		TEST_PACKET* packet = new TEST_PACKET;
		packet->header.packetIndex = 101;
		packet->header.packetSize = children.get<int>("packetSize");
		packet->Data1 = ptRecv.get<int>("Data1");
		packet->Data2 = ptRecv.get<std::string>("Data2");

		char* p = reinterpret_cast<char*>(&packet);
		std::cout <<p << std::endl;
		return (char*)&packet;
	}

	}
}
