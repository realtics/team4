#include "ServerSession.h"
#include "Server.h"
#include "Json.h"
#include "ThreadHandler.h"

#include <iostream>

std::string Session::_staticRecvBufHeader = ""; //Thread공유 버퍼
std::string Session::_staticRecvBuf = ""; //Thread공유 버퍼
std::atomic<int> Session::_readMarkAtomic = 0;


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
		//recv ByteStream정리 필요///////////////////////////////////////////////////////////
		////받은 JSON의 총길이를 알 수 있는 값이 저장되있는 위치
		/*int jsonStrLen = 47;
		_readData = strlen(_receiveBuffer.data());
		if (_readData < jsonStrLen)
		{
			{
				LockGuard recvLock(_recvLock);
				_staticRecvBufHeader += _receiveBuffer.data();
			}
			_readMarkAtomic.fetch_add(_readData);
			PostReceive();
			return;
		}
		{
			LockGuard recvLock(_recvLock);
			_staticRecvBuf = _receiveBuffer.data();
		}
		boost::property_tree::ptree ptRecv;
		std::istringstream is(_staticRecvBuf);
		{
			LockGuard recvLock(_recvLock);
			_staticRecvBufHeader = "";
		}
		boost::property_tree::read_json(is, ptRecv);
		boost::property_tree::ptree& children = ptRecv.get_child("header");
		int packetSize = children.get<int>("packetSize");

		_readData = 0;
		while (strlen(_receiveBuffer.data()) < packetSize)
		{
			_readData = strlen(_receiveBuffer.data());
			{
				LockGuard recvLock(_recvLock);
				_staticRecvBuf += _receiveBuffer.data();
			}
			_readMarkAtomic.fetch_add(_readData);
			PostReceive();
			return;
		}
		{
			LockGuard recvLock(_recvLock);
			_readMarkAtomic.exchange(0);
		}*/
		/////////////////////////////////////////////////////////////////////////////
		_DeSerializationJson(_receiveBuffer.data());
		{
			LockGuard pushPakcetQueueLock(_threadHandler->pushPakcetQueueLock);
			_PushPacketQueue(_sessionId, &_packetBuffer[0]);
		}

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
		packet.clientID = ptRecv.get<int>("clientID");
		strcpy(packet.name, ptRecv.get<std::string>("name").c_str());

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_SET_GOLD:
	{
		PACKET_REQ_RES_GOLD packet;
		packet.Init();
		packet.gold = ptRecv.get<int>("gold");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_GET_GOLD:
	{
		PACKET_REQ_RES_GOLD packet;
		packet.Init();
		packet.packetIndex = PACKET_INDEX::REQ_GET_GOLD;

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_CHECK_CLIENT_ID:
	{
		PACKET_REQ_CHECK_CLIENT_ID packet;
		packet.packetIndex = headerIndex;
		packet.packetSize = 0;
		packet.clientID = ptRecv.get<int>("clientID");
		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_RES_BASKET_BALL_GAME:
	{
		PACKET_REQ_RES_BASKET_BALL_GAME packet;
		packet.packetIndex = headerIndex;
		packet.packetSize = children.get<int>("packetSize");
		packet.power = ptRecv.get<float>("power");
		packet.angleX = ptRecv.get<float>("angleX");
		packet.angleY = ptRecv.get<float>("angleY");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_MULTI_ROOM:
	{
		PACKET_REQ_MULTI_ROOM packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = children.get<int>("packetSize");
		packet.gameIndex = (GAME_INDEX)ptRecv.get<int>("gameIndex");
		boost::property_tree::ptree& charInfo = ptRecv.get_child("charInfo");
		packet.charInfo.charIndex = charInfo.get<int>("charIndex");
		packet.charInfo.Item = charInfo.get<int>("item");

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

	case PACKET_INDEX::REQ_TIME:
	{
		PACKET_REQ_NULL_DATA packet;
		packet.header.packetIndex = headerIndex;
		packet.header.packetSize = 0; //요청확인만 하면 되므로 사이즈 불필요

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_ENTER_FARM:
	{
		PACKET_REQ_ENTER_FARM packet;
		packet.Init();
		packet.clientID = ptRecv.get<int>("clientID");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_RES_MOVE:
	{
		PACKET_REQ_RES_MOVE packet;
		packet.Init();
		packet.positionX = ptRecv.get<float>("positionX");
		packet.positionY = ptRecv.get<float>("positionY");
		packet.positionZ = ptRecv.get<float>("positionZ");

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	case PACKET_INDEX::REQ_SAVE_FARM:
	{
		PACKET_REQ_RES_FARM packet;
		packet.packetIndex = headerIndex;
		packet.packetSize = children.get<int>("packetSize");
		packet.farmIndex = (FARM_INDEX)ptRecv.get<int>("saveIndex");

		std::string temp = ptRecv.get<std::string>("saveData");
		memcpy(&packet.farmInfoJSON, temp.c_str(), strlen(temp.c_str())); //size = ??

		memcpy(&_packetBuffer[_packetBufferMark], (char*)&packet, sizeof(packet));
		break;
	}

	default:
		std::cout << "Session : _DeSerializationJson() : Packet HeaderIndex is not. : " << headerIndex
			<< std::endl;
		break;
	}
}