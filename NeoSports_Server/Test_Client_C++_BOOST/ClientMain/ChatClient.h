#pragma once

#include <iostream>
#include <deque>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/thread.hpp>

#include"Protocol.h"

class ChatClient
{
public:
	ChatClient(boost::asio::io_context& io_service);
	~ChatClient();

	bool IsConnecting();
	void SetLoginTrue();
	bool IsLogin();

	void Connect(boost::asio::ip::tcp::endpoint endpoint);
	void Close();

	void PostSend(const bool immediately, const int size, char* data);
		

private:
	boost::asio::io_service& _ioService;
	boost::asio::ip::tcp::socket _socket;

	std::array<char, 512> _receiveBuffer;

	int _packetBufferMark;
	char _packetBuffer[MAX_RECEIVE_BUFFER_LEN * 2];

	CRITICAL_SECTION _lock;
	std::deque< char* > _sendDataDeq;

	bool _isLogin;

	void _PostReceive();
	void _ConnectHandle(const boost::system::error_code& error);
	void _WriteHandle(const boost::system::error_code& error, size_t bytesTransferred);
	void _ReceiveHandle(const boost::system::error_code& error, size_t bytesTransferred);
	void _ProcessPacket(const char* data);
};

