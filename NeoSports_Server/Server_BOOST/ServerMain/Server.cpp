#include "Server.h"
#include "Json.h"


Server::Server(boost::asio::io_context& io_service) : _acceptor(io_service,
	boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), PORT_NUMBER))
{
	_isAccepting = false;
}

Server::~Server()
{
	for (int i = 0; i < _sessionVec.size(); i++)
	{
		if (_sessionVec[i]->Socket().is_open())
		{
			_sessionVec[i]->Socket().close();
		}

		delete _sessionVec[i];
	}
}

void Server::Init(const int maxSessionCount)
{
	for (int i = 0; i < maxSessionCount; i++)
	{
		Session* session = new Session(i, (boost::asio::io_context&)_acceptor.get_executor().context(), this);
		_sessionVec.push_back(session);
		_sessionDeq.push_back(i);
	}
}

void Server::Start()
{
	std::cout << "Server Start..." << std::endl;
	DB::GetInstance()->Init();

	_PostAccept();
}

void Server::CloseSession(const int sessionID)
{
	std::cout << "Server : Client out. Session ID : " << sessionID << std::endl;

	_sessionVec[sessionID]->Socket().close();
	_sessionDeq.push_back(sessionID);

	if (_isAccepting == false)
	{
		_PostAccept();
	}
}

int Server::MakeRoom(GAME_INDEX gameIndex, int sessionID, CHAR_INDEX charIndex)
{
	return _roomMG.MakeRoom(gameIndex, sessionID, charIndex);
}
int Server::GetRoomNum(int sessionID)
{
	return _roomMG.GetRoomNum(sessionID);
}

int Server::GetSuperSessionID(int roomNum)
{
	return _roomMG.GetSuperSessonID(roomNum);
}

int Server::GetSessionID(int roomNum)
{
	return _roomMG.GetSessonID(roomNum);
}

ROOM* Server::GetRoomInfo(int roomNum)
{
	return _roomMG.GetRoomInfo(roomNum);
}

std::string Server::GetSuperSessionName(int sessionID)
{
	return _sessionVec[sessionID]->GetName();
}

void Server::PostSendSession(int sessionID, const bool Immediately, const int size, char* data)
{
	_sessionVec[sessionID]->PostSend(Immediately,size,data);
}

std::string Server::GetSessionName(int sessionID)
{
	return _sessionVec[sessionID]->GetName();
}

void Server::InitRoom(int roomNum)
{
	_roomMG.InitRoom(roomNum);
}


void Server::ProcessReqInPacket(const int sessionID, const char* data)
{
	PACKET_REQ_IN* packet = (PACKET_REQ_IN*)data;
	_sessionVec[sessionID]->SetNanme(packet->name);

	std::cout << "Server : Client accept. Name : " << _sessionVec[sessionID]->GetName() << std::endl;
	DB::GetInstance()->Insert(_sessionVec[sessionID]->GetName());
}

bool Server::_PostAccept()
{
	if (_sessionDeq.empty())
	{
		_isAccepting = false;
		return false;
	}

	_isAccepting = true;
	int sessionId = _sessionDeq.front();

	_sessionDeq.pop_front();
	_acceptor.async_accept(_sessionVec[sessionId]->Socket(),
		boost::bind(&Server::_AcceptHandle, this,
			_sessionVec[sessionId], boost::asio::placeholders::error)
	);

	return true;
}

void Server::_AcceptHandle(Session* session, const boost::system::error_code& error)
{
	LockGuard acceptLockGuard(_acceptLock);

	if (!error)
	{
		std::cout << "Server : Accept : Entered Client. SessionID : " << session->GetSessionID() << std::endl;

		session->Init();
		session->PostReceive();

		_PostAccept();
	}

	else
	{
		std::cout << "error No : " << error.value() << " error Message : " << error.message()
			<< std::endl;
	}
}

//std::string Server::_SerializationJson(int packetIndex, const char* packet)
//{
//	std::string sendStr;
//
//
//	switch (packetIndex)
//	{
//	case PACKET_INDEX::ROOM_INFO:
//	{
//		PACKET_ROOM_INFO* roomInfoPacket = new PACKET_ROOM_INFO;
//		memcpy(&roomInfoPacket, &packet, sizeof(PACKET_ROOM_INFO));
//
//		boost::property_tree::ptree ptSend;
//		boost::property_tree::ptree ptSendHeader;
//		ptSendHeader.put<int>("packetIndex", roomInfoPacket->header.packetIndex);
//		ptSendHeader.put<int>("packetSize", sizeof(PACKET_ROOM_INFO));
//		ptSend.add_child("header", ptSendHeader);
//
//		ptSend.put<int>("roomInfo", (ROOM_HOST)roomInfoPacket->roomInfo);
//
//		std::string recvTemp;
//		std::ostringstream os(recvTemp);
//		boost::property_tree::write_json(os, ptSend, false);
//		sendStr = os.str();
//		return sendStr;
//	}
//
//	case PACKET_INDEX::START_GAME:
//	{
//		PACKET_START_GAME* startGamePacket = new PACKET_START_GAME;
//		memcpy(&startGamePacket, &packet, sizeof(PACKET_START_GAME));
//
//		boost::property_tree::ptree ptSend;
//		boost::property_tree::ptree ptSendHeader;
//		ptSendHeader.put<int>("packetIndex", startGamePacket->header.packetIndex);
//		ptSendHeader.put<int>("packetSize", sizeof(PACKET_START_GAME));
//		ptSend.add_child("header", ptSendHeader);
//
//		ptSend.put<int>("superCharID", (CHAR_INDEX)startGamePacket->superCharID);
//		ptSend.put<int>("charID", (CHAR_INDEX)startGamePacket->charID);
//
//		ptSend.put<char*>("superName", startGamePacket->superName);
//		ptSend.put<char*>("name", startGamePacket->name);
//
//		std::string recvTemp;
//		std::ostringstream os(recvTemp);
//		boost::property_tree::write_json(os, ptSend, false);
//		sendStr = os.str();
//		return sendStr;
//	}
//
//	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
//	{
//		PACKET_REQ_RES_ROPE_PULL_GAME* ropePullPacket = new PACKET_REQ_RES_ROPE_PULL_GAME;
//		memcpy(&ropePullPacket, &packet, sizeof(PACKET_REQ_RES_ROPE_PULL_GAME));
//
//		boost::property_tree::ptree ptSend;
//		boost::property_tree::ptree ptSendHeader;
//		ptSendHeader.put<int>("packetIndex", ropePullPacket->header.packetIndex);
//		ptSendHeader.put<int>("packetSize", sizeof(PACKET_START_GAME));
//		ptSend.add_child("header", ptSendHeader);
//
//		ptSend.put<float>("ropePos", ropePullPacket->ropePos);
//
//		std::string recvTemp;
//		std::ostringstream os(recvTemp);
//		boost::property_tree::write_json(os, ptSend, false);
//		sendStr = os.str();
//		return sendStr;
//	}
//	break;
//
//	case PACKET_INDEX::RES_RANK:
//	{
//		PACKET_RES_RANK* resRankPacket = new PACKET_RES_RANK;
//		memcpy(&resRankPacket, &packet, sizeof(PACKET_RES_RANK));
//		boost::property_tree::ptree ptSend;
//
//		boost::property_tree::ptree ptSendHeader;
//		ptSendHeader.put<int>("packetIndex", resRankPacket->header.packetIndex);
//		ptSendHeader.put<int>("packetSize", sizeof(PACKET_RES_RANK));
//		ptSend.add_child("header", ptSendHeader);
//
//		boost::property_tree::ptree ptSendRankArr;
//		boost::property_tree::ptree arr[MAX_RANK_COUNT];
//		for (int i = 0; i < MAX_RANK_COUNT; i++)
//		{
//			arr[i].put("name", resRankPacket->rank[i].name);
//			arr[i].put("winRecord", resRankPacket->rank[i].winRecord);
//			ptSendRankArr.push_back(std::make_pair("", arr[i]));
//		}
//		ptSend.add_child("rank", ptSendRankArr);
//
//		std::string recvTemp;
//		std::ostringstream os(recvTemp);
//		boost::property_tree::write_json(os, ptSend, false);
//		sendStr = os.str();
//		return sendStr;
//	}
//	break;
//
//	}
//}