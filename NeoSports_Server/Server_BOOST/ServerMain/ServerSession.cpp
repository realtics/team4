#include "ServerSession.h"
#include "Server.h"
#include "Json.h"

#include <iostream>

Session::Session(int sessionID, boost::asio::io_context& io_service, Server* serverPtr)
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
		boost::bind(&Session::_ReceiveHandle, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));
}

void  Session::_ReceiveHandle(const boost::system::error_code& error, size_t bytesTransferred)
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
		LockGuard closeLock(_closeLock);
		_serverPtr->CloseSession(_sessionId);
	}
	else
	{
		LockGuard recvLockGuard(_recvLock);
		_DeSerializationJson(_receiveBuffer.data());
		int packetData = _packetBufferMark + bytesTransferred;
		int readData = 0;
		PACKET_HEADER* header = (PACKET_HEADER*)&_packetBuffer[readData];

		while (packetData > 0)
		{
			if (packetData < sizeof(PACKET_HEADER))
			{
				break;
			}

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
	LockGuard sendLockGuard(_sendLock);
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
		boost::bind(&Session::_WriteHandle, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred)
	);
}

void Session::_WriteHandle(const boost::system::error_code& error, size_t butesTransferred)
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


void Session::_DeSerializationJson(char* jsonStr)
{
	_packetBufferMark = 0;

	boost::property_tree::ptree ptRecv;
	std::istringstream is(jsonStr);
	boost::property_tree::read_json(is, ptRecv);

	boost::property_tree::ptree& children = ptRecv.get_child("header");
	int headerIndex = children.get<int>("packetIndex");

	switch (headerIndex)
	{
	case PACKET_INDEX::REQ_IN:
	{
		PACKET_REQ_IN packet;
		packet.packetIndex = headerIndex;
		packet.packetSize = children.get<int>("packetSize");
		strcpy(packet.name, ptRecv.get<std::string>("name").c_str());

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::MULTI_ROOM:
	{
		PACKET_MULTI_ROOM packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = children.get<int>("packetSize");
		packet.gameIndex = ptRecv.get<int>("gameIndex");
		packet.charIndex = ptRecv.get<int>("charIndex");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	default:
		std::cout << "해당 패킷의 HeaderIndex가 없습니다. : " << headerIndex
			<< std::endl;
		break;
	}
}

//std::string Session::_SerializationJson(int packetIndex, const char* packet)
//{
//	std::string sendStr;
//	switch (packetIndex)
//	{
//
//	case PACKET_INDEX::MULTI_ROOM:
//	{
//		PACKET_ROOM_INFO* testPacket = new PACKET_ROOM_INFO;
//		memcpy(&testPacket, &packet, sizeof(PACKET_ROOM_INFO));
//
//		boost::property_tree::ptree ptSend;
//		boost::property_tree::ptree ptSendHeader;
//		ptSendHeader.put<int>("packetIndex", testPacket->header.packetIndex);
//		ptSendHeader.put<int>("packetSize", sizeof(PACKET_ROOM_INFO));
//		ptSend.add_child("header", ptSendHeader);
//		ptSend.put<int>("roomInfo", testPacket->roomInfo);
//		ptSend.put<int>("charInfo", testPacket->charInfo);
//
//		std::string recvTemp;
//		std::ostringstream os(recvTemp);
//		boost::property_tree::write_json(os, ptSend, false);
//		sendStr = os.str();
//		return sendStr;
//	}
//
//	}
//}

