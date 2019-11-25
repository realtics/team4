#pragma once

#include <iostream>
#include <vector>
#include <deque>
#include <algorithm>
#include <string>

#include "RoomMG.h"
#include "DB.h"
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
	RoomMG roomMG;

private:
	DB db;

	Lock _acceptLock;
	Lock _ropePullLock;

	int _seqNumber;
	bool _isAccepting;

	boost::asio::ip::tcp::acceptor _acceptor;

	std::vector<Session*> _sessionVec;
	std::deque<int> _sessionDeq;

	bool _PostAccept();
	void _AcceptHandle(Session* session, const boost::system::error_code& error);

	std::string _SerializationJson(int packetIndex, const char* pakcet);

};

