#pragma once

#include <iostream>
#include <vector>
#include <deque>
#include <algorithm>
#include <string>

#include "RoomMG.h"
#include "ServerSession.h"
#include "DB.h"

class Server
{
public:
	Server(boost::asio::io_context& io_service);
	~Server();

	void Init(const int maxSessionCount);
	void Start();
	void CloseSession(const int sessionID);
	void ProcessReqInPacket(const int sessionID, const char* data);

	int MakeRoom(GAME_INDEX gameIndex, int sessionID, CHAR_INDEX charIndex);
	int GetRoomNum(int sessionID);
	//GetsuperSissonID
	//GetsessuonID

	//void SetSessionName(int sessionID, std::string name);


private:
	DB _db;
	RoomMG _roomMG;

	Lock _acceptLock;
	Lock _ropePullLock;

	int _seqNumber;
	bool _isAccepting;

	boost::asio::ip::tcp::acceptor _acceptor;

	std::vector<Session*> _sessionVec;
	std::deque<int> _sessionDeq;

	bool _PostAccept();
	void _AcceptHandle(Session* session, const boost::system::error_code& error);

	//std::string _SerializationJson(int packetIndex, const char* pakcet);

};

