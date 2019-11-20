#pragma once

#include <iostream>
#include <vector>
#include <deque>
#include <algorithm>
#include <string>

#include "ServerSession.h"

class Server
{
public:
	Server(boost::asio::io_context& io_service);
	~Server();

	void Init(const int maxSessionCount);
	void Start();
	void CloseSession(const int sessionID);
	void ProcessPacket(const int sessionID, const char* data);

private:
	Lock _acceptLock;

	RoomMG _roomMG;

	int _seqNumber;
	bool _isAccepting;

	boost::asio::ip::tcp::acceptor _acceptor;

	std::vector<Session*> _sessionVec;
	std::deque<int> _sessionDeq;

	bool _PostAccept();
	void _AcceptHandle(Session* session, const boost::system::error_code& error);
};

