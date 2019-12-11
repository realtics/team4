#include "ServerSession.h"
#include "Server.h"
#include "Json.h"
#include "ThreadHandler.h"

#include <iostream>

Session::Session(int sessionID, boost::asio::io_context& io_service, Server* serverPtr)
	:_socket(io_service),
	_sessionId(sessionID),
	_serverPtr(serverPtr)
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

void Session::Init(ThreadHandler* threadHandle)
{
	_packetBufferMark = 0;
	_threadHandler = threadHandle;
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
void Session::_PushPacketQueue(const int sessionId, const char* data)
{
	PacketData packetData(sessionId, data);
	_threadHandler->PushPacketQueue(packetData);
	_threadHandler->SetEventsObject();
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
			std::cout << "Session : Client Out" << std::endl;
		}
		else
		{
			std::cout << "Session : error No : " << error.value() << " error Message : " << error.message() <<
				std::endl;
		}
		_serverPtr->CloseSession(_sessionId);
	}
	else
	{
		/*TODO : 역직렬화가 끝난후에 TCP Byte처리는 의미가 없어 보인다
		이미 다 받고 역직렬화를 했기때문?
		역직렬화 하기전에 TCp Byte처리로 변경 필요*/
		_DeSerializationJson(_receiveBuffer.data());
		LockGuard pushPakcetQueue(_pushPakcetQueue);
		_PushPacketQueue(_sessionId, &_packetBuffer[0]); 
		//int packetData = _packetBufferMark + bytesTransferred;
		//int readData = 0;
		//PACKET_HEADER* header = (PACKET_HEADER*)&_packetBuffer[readData];

		//while (packetData > 0)
		//{
		//	if (packetData < sizeof(PACKET_HEADER))
		//	{
		//		break;
		//	}

		//	if (header->packetSize <= packetData)
		//	{
		//		//TODO : Byte처리 수정후 락부분 수정
		//		/*LockGuard pushPakcetQueue(_pushPakcetQueue);
		//		_PushPacketQueue(_sessionId, &_packetBuffer[readData]);*/

		//		packetData -= header->packetSize;
		//		readData += header->packetSize;
		//	}
		//	else
		//		break;
		//}

		//if (packetData > 0)
		//{
		//	char tempBuffer[MAX_RECEIVE_BUFFER_LEN] = { 0, };
		//	memcpy(&tempBuffer[0], &_packetBuffer[readData], packetData);
		//	memcpy(&_packetBuffer[0], &tempBuffer[0], packetData);
		//}

		//_packetBufferMark = packetData;

		PostReceive();
	}
}


int Session::GetSessionID()
{
	return _sessionId;
}

void Session::SetName(const char* name)
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

	case PACKET_INDEX::REQ_MULTI_ROOM:
	{
		PACKET_REQ_MULTI_ROOM packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = children.get<int>("packetSize");
		packet.gameIndex = (GAME_INDEX)ptRecv.get<int>("gameIndex");
		packet.charIndex = (CHAR_INDEX)ptRecv.get<int>("charIndex");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_INIT_ROOM:
	{
		PACKET_REQ_INIT_ROOM packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = children.get<int>("packetSize");
		packet.gameIndex = (GAME_INDEX)ptRecv.get<int>("gameIndex");
		packet.isEndGame = ptRecv.get<bool>("isEndGame");
		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
	{
		PACKET_REQ_RES_ROPE_PULL_GAME packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = children.get<int>("packetSize");
		packet.ropePos = ptRecv.get<float>("ropePos");
		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_RANK:
	{
		PACKET_REQ_RANK packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = children.get<int>("packetSize");
		packet.gameIndex = (GAME_INDEX)ptRecv.get<int>("gameIndex");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	default:
		std::cout << "Session : _DeSerializationJson() : Packet HeaderIndex is not. : " << headerIndex
			<< std::endl;
		break;
	}
}