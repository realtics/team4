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

void Server::ProcessPacket(const int sessionID, const char* data)
{
	PACKET_HEADER* header = (PACKET_HEADER*)data;

	switch (header->packetIndex)
	{
	case PACKET_INDEX::REQ_IN:
	{
		PACKET_REQ_IN* packet = (PACKET_REQ_IN*)data;
		_sessionVec[sessionID]->SetNanme(packet->name);

		std::cout << "Server : Client accept. Name : " << _sessionVec[sessionID]->GetName() << std::endl;
		db.Insert(_sessionVec[sessionID]->GetName());
	}
	break;

	case PACKET_INDEX::REQ_MULTI_ROOM:
	{
		PACKET_REQ_MULTI_ROOM* packet = (PACKET_REQ_MULTI_ROOM*)data;

		int mrTemp = roomMG.MakeRoom(packet->gameIndex, sessionID, packet->charIndex);

		PACKET_ROOM_INFO sendPacket;
		sendPacket.header.packetIndex = PACKET_INDEX::ROOM_INFO;
		sendPacket.header.packetSize = sizeof(PACKET_ROOM_INFO);
		sendPacket.roomInfo = (ROOM_HOST)mrTemp; //받는 클라입장에서 자신이 방장인지 구별

		if (mrTemp == ROOM_HOST::ENTER_ROOM) //들어가는 입장이면 스타트패킷생성후 전송해야함
		{
			int roomNum = roomMG.GetRoomNum(sessionID);
			int superSessionIdTemp = roomMG.GetSuperSessonID(roomNum);
			int sessionIdTemp = roomMG.GetSessonID(roomNum);

			PACKET_START_GAME startPacket;
			startPacket.header.packetIndex = PACKET_INDEX::START_GAME;
			startPacket.header.packetSize = sizeof(PACKET_START_GAME);
			startPacket.superCharID = (CHAR_INDEX)roomMG.GetRoomChar(roomNum, 0);
			startPacket.charID = (CHAR_INDEX)roomMG.GetRoomChar(roomNum, 1);
			strcpy(startPacket.superName, _sessionVec[superSessionIdTemp]->GetName());
			strcpy(startPacket.name, _sessionVec[sessionIdTemp]->GetName());

			std::string aa = _SerializationJson(PACKET_INDEX::START_GAME, (const char*)&startPacket);

			_sessionVec[superSessionIdTemp]->PostSend(false, aa.length(), (char*)aa.c_str());
			_sessionVec[sessionIdTemp]->PostSend(false, aa.length(), (char*)aa.c_str());

			return;
		}

		std::string aa = _SerializationJson(PACKET_INDEX::ROOM_INFO, (const char*)&sendPacket);
		_sessionVec[sessionID]->PostSend(false, aa.length(), (char*)aa.c_str());
	}
	break;

	case PACKET_INDEX::REQ_INIT_ROOM:
	{
		PACKET_REQ_INIT_ROOM* packet = (PACKET_REQ_INIT_ROOM*)data;

		int roomNum = roomMG.GetRoomNum(sessionID);
		if (roomNum == FAIL_ROOM_SERCH)
			break;

		if (packet->isEndGame)
		{
			std::cout << roomNum << "Room " << packet->gameIndex << "End Game. Winner : "
				<< sessionID << std::endl;
			db.Update(_sessionVec[sessionID]->GetName(), packet->gameIndex, 1);
		}

		roomMG.roomVec[roomNum]->Init();
	}
	break;

	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
	{
		//LockGuard ropeLockGuard(_ropePullLock);
		int roomNum = roomMG.GetRoomNum(sessionID);

		//클라에서 x버튼이나 게임중 메뉴의 yes,no버튼 클릭할때도
		//게임로직 패킷이 보내져서 예외처리 해주는중
		if (roomNum == FAIL_ROOM_SERCH ||
			roomMG.roomVec[roomNum]->superSessionID == GAME_INDEX::EMPTY_GAME)
		{
			std::cout << "(already Init)." << std::endl;
			break;
		}

		PACKET_REQ_RES_ROPE_PULL_GAME* packet = (PACKET_REQ_RES_ROPE_PULL_GAME*)data;
		roomMG.roomVec[roomNum]->gameMG.SetRopePos(packet->ropePos);
		float ropePos = roomMG.roomVec[roomNum]->gameMG.GetRopePos();

		PACKET_REQ_RES_ROPE_PULL_GAME resPacket;;
		resPacket.header.packetIndex = PACKET_INDEX::REQ_RES_ROPE_PULL_GAME;
		resPacket.header.packetSize = sizeof(PACKET_REQ_RES_ROPE_PULL_GAME);
		resPacket.ropePos = ropePos;

		std::string aa = _SerializationJson(PACKET_INDEX::REQ_RES_ROPE_PULL_GAME, (const char*)&resPacket);

		int superSessionIdTemp = roomMG.GetSuperSessonID(roomNum);
		int sessionIdTemp = roomMG.GetSessonID(roomNum);

		_sessionVec[superSessionIdTemp]->PostSend(false, aa.length(), (char*)aa.c_str());
		_sessionVec[sessionIdTemp]->PostSend(false, aa.length(), (char*)aa.c_str());
	}
	break;

	case PACKET_INDEX::REQ_RANK:
	{
		PACKET_REQ_RANK* packet = (PACKET_REQ_RANK*)data;
		RANK rank[MAX_RANK_COUNT];

		db.Rank(packet->gameIndex, rank); //배열 포인터를 전달하고싶다
		PACKET_RES_RANK resRankPacket;
		resRankPacket.header.packetIndex = PACKET_INDEX::RES_RANK;
		resRankPacket.header.packetSize = sizeof(PACKET_RES_RANK);
		for (int i = 0; i < MAX_RANK_COUNT; i++)
		{
				strcpy(resRankPacket.rank[i].name, rank[i].name);
				resRankPacket.rank[i].winRecord = rank[i].winRecord;
		}
		std::string aa = _SerializationJson(PACKET_INDEX::RES_RANK, (const char*)&resRankPacket);
		_sessionVec[sessionID]->PostSend(false, aa.length(), (char*)aa.c_str());
	}
	break;

	/*case REQ_CHAT:
	{
		PACKET_REQ_CHAT* packet = (PACKET_REQ_CHAT*)data;

		PACKET_NOTICE_CHAT sendPacket;
		sendPacket.Init();
		strncpy_s(sendPacket.szName, MAX_NAME_LEN, _sessionVec[sessionID]->GetName(), MAX_NAME_LEN - 1);
		strncpy_s(sendPacket.szMessage, MAX_MESSAGE_LEN, packet->szMessage, MAX_MESSAGE_LEN - 1);

		size_t totalSessioncount = _sessionVec.size();

		for (int i = 0; i < totalSessioncount; i++)
		{
			if (_sessionVec[i]->Socket().is_open())
			{
				_sessionVec[i]->PostSend(false, sendPacket.packetSize, (char*)&sendPacket);
			}
		}
	}
	break;*/

	return;
	}
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

std::string Server::_SerializationJson(int packetIndex, const char* packet)
{
	std::string sendStr;


	switch (packetIndex)
	{
	case PACKET_INDEX::ROOM_INFO:
	{
		PACKET_ROOM_INFO* roomInfoPacket = new PACKET_ROOM_INFO;
		memcpy(&roomInfoPacket, &packet, sizeof(PACKET_ROOM_INFO));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", roomInfoPacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_ROOM_INFO));
		ptSend.add_child("header", ptSendHeader);

		ptSend.put<int>("roomInfo", (ROOM_HOST)roomInfoPacket->roomInfo);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}

	case PACKET_INDEX::START_GAME:
	{
		PACKET_START_GAME* startGamePacket = new PACKET_START_GAME;
		memcpy(&startGamePacket, &packet, sizeof(PACKET_START_GAME));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", startGamePacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_START_GAME));
		ptSend.add_child("header", ptSendHeader);

		ptSend.put<int>("superCharID", (CHAR_INDEX)startGamePacket->superCharID);
		ptSend.put<int>("charID", (CHAR_INDEX)startGamePacket->charID);

		ptSend.put<char*>("superName", startGamePacket->superName);
		ptSend.put<char*>("name", startGamePacket->name);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}

	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
	{
		PACKET_REQ_RES_ROPE_PULL_GAME* ropePullPacket = new PACKET_REQ_RES_ROPE_PULL_GAME;
		memcpy(&ropePullPacket, &packet, sizeof(PACKET_REQ_RES_ROPE_PULL_GAME));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", ropePullPacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_START_GAME));
		ptSend.add_child("header", ptSendHeader);

		ptSend.put<float>("ropePos", ropePullPacket->ropePos);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}
	break;

	case PACKET_INDEX::RES_RANK:
	{
		PACKET_RES_RANK* resRankPacket = new PACKET_RES_RANK;
		memcpy(&resRankPacket, &packet, sizeof(PACKET_RES_RANK));
		boost::property_tree::ptree ptSend;

		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", resRankPacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_RES_RANK));
		ptSend.add_child("header", ptSendHeader);

		boost::property_tree::ptree ptSendRankArr;
		boost::property_tree::ptree arr[MAX_RANK_COUNT];
		for (int i = 0; i < MAX_RANK_COUNT; i++)
		{
			arr[i].put("name", resRankPacket->rank[i].name);
			arr[i].put("winRecord", resRankPacket->rank[i].winRecord);
			ptSendRankArr.push_back(std::make_pair("", arr[i]));
		}
		ptSend.add_child("rank", ptSendRankArr);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}
	break;

	}
}