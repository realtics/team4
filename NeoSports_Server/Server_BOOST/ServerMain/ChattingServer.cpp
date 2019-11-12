#include "ChattingServer.h"

ChatServer::ChatServer(boost::asio::io_context& io_service) : _acceptor(io_service,
	boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), PORT_NUMBER))
{
	_isAccepting = false;
}

ChatServer::~ChatServer()
{
	for (size_t i = 0; i < _sessionVec.size(); i++)
	{
		if (_sessionVec[i]->Socket().is_open())
		{
			_sessionVec[i]->Socket().close();
		}

		delete _sessionVec[i];
	}
}

void ChatServer::Init(const int maxSessionCount)
{
	for (int i = 0; i < maxSessionCount; i++)
	{
		Session* session = new Session(i, (boost::asio::io_context&)_acceptor.get_executor().context(), this);
		_sessionVec.push_back(session);
		_sessionDeq.push_back(i);
	}
}

void ChatServer::Start()
{
	std::cout << "서버시작..." << std::endl;

	_PostAccept();
}

void ChatServer::CloseSession(const int sessionID)
{
	std::cout << "클라이언트 접속 종료. 세션 ID: " << sessionID << std::endl;

	_sessionVec[sessionID]->Socket().close();
	_sessionDeq.push_back(sessionID);

	if (_isAccepting == false)
	{
		_PostAccept();
	}
}

void ChatServer::ProcessPacket(const int sessionID, const char* data)
{
	PACKET_HEADER* header = (PACKET_HEADER*)data;

	switch (header->packetIndex)
	{
	case REQ_IN:
	{
		PACKET_REQ_IN* packet = (PACKET_REQ_IN*)data;
		_sessionVec[sessionID]->SetNanme(packet->szName);

		std::cout << "클라접속 Name : " << _sessionVec[sessionID]->GetName() << std::endl;

		PACKET_RES_IN sendPacket;
		sendPacket.Init();
		sendPacket.isSuccess = true;

		_sessionVec[sessionID]->PostSend(false, sendPacket.packetSize, (char*)&sendPacket);
	}
	break;

	case REQ_CHAT:
	{
		PACKET_REQ_CHAT* packet = (PACKET_REQ_CHAT*)data;

		PACKET_NOTICE_CHAT sendPacket;
		sendPacket.Init();
		strncpy_s(sendPacket.szName, MAX_NAME_LEN, _sessionVec[sessionID]->GetName(), MAX_NAME_LEN - 1);
		strncpy_s(sendPacket.szMessage, MAX_MESSAGE_LEN, packet->szMessage, MAX_MESSAGE_LEN - 1);

		size_t totalSessioncount = _sessionVec.size();

		for (int i = 0; i < totalSessioncount; i++)
		{
			if (_sessionVec[i]->Socket().is_open());
			{
				_sessionVec[i]->PostSend(false, sendPacket.packetSize, (char*)&sendPacket);
			}
		}
	}
	break;

	return;
	}
}

bool ChatServer::_PostAccept()
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
		boost::bind(&ChatServer::_AcceptHandle, this,
			_sessionVec[sessionId], boost::asio::placeholders::error)
	);

	return true;
}

void ChatServer::_AcceptHandle(Session* session, const boost::system::error_code& error)
{
	if (!error)
	{
		std::cout << "클라접속. SessionID : " << session->GetSessionID() << std::endl;

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