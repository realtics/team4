#pragma once
#include <iostream>
#include <deque>

#include <boost/bind.hpp>
#include <boost/asio.hpp>

#include "Protocol.h"
#include "Lock.h"

class Server;
class GameMG;

class Session
{
public:
	Session(int sessionID, boost::asio::io_context& io_service, Server* serverPtr);
	~Session();

	int GetSessionID();
	void Init();
	void PostReceive();
	void PostSend(const bool bImmediately, const int size, char* dataPtr);
	void SetName(const char* namePtr);
	const char* GetName();
	void SetGameMG(GameMG* gameMG);

	boost::asio::ip::tcp::socket& Socket() { return Session::_socket; }

private:
	Lock _sendLock;
	Lock _recvLock;
	Lock _closeLock;
	Lock _ropePullLock;
	
	int _sessionId;
	int _packetBufferMark;
	char _packetBuffer[MAX_RECEIVE_BUFFER_LEN * 2];

	boost::asio::ip::tcp::socket _socket;
	std::array<char, MAX_RECEIVE_BUFFER_LEN> _receiveBuffer;

	std::deque<char*> _sendDataDeq;
	std::string _name;

	Server* _serverPtr;
	GameMG* _gameMG;

	void _ProcessPacket(const int sessionID, const char* data);


	void _WriteHandle(const boost::system::error_code& error, size_t bytesTransferred);
	void _ReceiveHandle(const boost::system::error_code& error, size_t bytesTransferred);

	void _DeSerializationJson(char* jsonStr);
	std::string _SerializationJson(int packetIndex, const char* pakcet);

};

