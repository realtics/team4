#include "Server.h"
#include "Json.h"


Server::Server(boost::asio::io_context& io_service) : _acceptor(io_service,
	boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), PORT_NUMBER))
{
	_isAccepting = false;
	db.SelectQuery();
	db.Update(-1, GAME_INDEX::ROPE_PULL, 1);
	db.SelectQuery();
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
	std::cout << "��������..." << std::endl;

	_PostAccept();
}

void Server::CloseSession(const int sessionID)
{
	std::cout << "Ŭ���̾�Ʈ ���� ����. ���� ID: " << sessionID << std::endl;

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

		std::cout << "Ŭ������ Name : " << _sessionVec[sessionID]->GetName() << std::endl;
	}
	break;

	case PACKET_INDEX::MULTI_ROOM:
	{
		PACKET_REQ_MULTI_ROOM* packet = (PACKET_REQ_MULTI_ROOM*)data;

		int mrTemp = roomMG.MakeRoom(packet->gameIndex, sessionID, packet->charIndex);

		PACKET_ROOM_INFO sendPacket;
		sendPacket.header.packetIndex = PACKET_INDEX::ROOM_INFO;
		sendPacket.header.packetSize = sizeof(PACKET_ROOM_INFO);
		sendPacket.roomInfo = (ROOM_HOST)mrTemp; //�޴� Ŭ�����忡�� �ڽ��� �������� ����

		if (mrTemp == ROOM_HOST::ENTER_ROOM) //���� �����̸� ��ŸƮ��Ŷ������ �����ؾ���
		{
			int roomNum = roomMG.GetRoomNum(sessionID);
			int superSessionIdTemp = roomMG.GetSuperSessonID(roomNum);
			int sessionIdTemp = roomMG.GetSessonID(roomNum);

			PACKET_START_GAME startPacket;
			startPacket.header.packetIndex = PACKET_INDEX::START_GAME;
			startPacket.header.packetSize = sizeof(PACKET_START_GAME);
			startPacket.superCharID = (CHAR_INDEX)roomMG.GetRoomChar(roomNum, 0);
			startPacket.charID = (CHAR_INDEX)roomMG.GetRoomChar(roomNum, 1);

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

		if (packet->isEndGame)
		{
			std::cout << roomNum << "���� " << packet->gameIndex << "���� ����. ���� "
				<< sessionID << std::endl;

			//DB�߰��� ���Ӱ�� ���� �߰��ҽ� ��ġ
		}

		roomMG._roomVec[roomNum]->Init();
	}
	break;

	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
	{
		LockGuard ropeLockGuard(_ropePullLock);

		PACKET_REQ_RES_ROPE_PULL_GAME* packet = (PACKET_REQ_RES_ROPE_PULL_GAME*)data;
		int roomNum = roomMG.GetRoomNum(sessionID);
		roomMG._roomVec[roomNum]->gameMG.SetRopePos(packet->ropePos);
		float ropePos = roomMG._roomVec[roomNum]->gameMG.GetRopePos();

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
		std::cout << "Ŭ������. SessionID : " << session->GetSessionID() << std::endl;

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
		//ptSend.put<int>("charInfo", roomInfoPacket->charInfo);

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

	}
}