#pragma once

#include <iostream>
#include <vector>
#include <deque>
#include <algorithm>
#include <string>

#include "RoomMG.h"
#include "ServerSession.h"
#include "GameMG.h"
#include "ThreadHandler.h"

#include <boost/thread/thread.hpp>

class Server
{
public:
	Server(boost::asio::io_context& io_service, ThreadHandler* threadHandle);
	~Server();

	void Init(const int maxSessionCount);
	void Start();
	void CloseSession(const int sessionID);
	void ProcessReqInPacket(const int sessionID, const char* data);
	void ProcessInitRoomPacket(const int sessionID, const char* data);

	//Room
	int MakeRoom(GAME_INDEX gameIndex, int sessionID, NEO_CHAR_INFO charIndex);
	int GetRoomNum(int sessionID);
	void InitRoom(int roomNum);
	ROOM* GetRoomInfo(int roomNum);

	//Session
	std::string GetSessionName(int sessionID);
	int GetSuperSessionID(int roomNum);
	int GetSessionID(int roomNum);
	std::string GetSuperSessionName(int sessionID);
	void PostSendSession(int sessionID, const bool Immediately, const int size, char* data);

	//GameMG
	void SetGameMG(bool isSuperSession, int sessionID, GAME_INDEX gameIndex);

private:
	ThreadHandler* _threadHanlder;
	RoomMG _roomMG;
	std::vector<GameMG*> _gameMGPool;

	Lock _acceptLock;
	//Lock _ropePullLock;

	int _seqNumber;
	bool _isAccepting;

	boost::asio::ip::tcp::acceptor _acceptor;

	std::vector<Session*> _sessionVec;
	std::deque<int> _sessionDeq;

	bool _PostAccept();
	void _AcceptHandle(Session* session, const boost::system::error_code& error);
};