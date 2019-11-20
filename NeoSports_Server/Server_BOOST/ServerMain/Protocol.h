#pragma once
#include <Windows.h>

const unsigned short PORT_NUMBER = 31400;

const int MAX_RECEIVE_BUFFER_LEN = 512;

const int MAX_NAME_LEN = 17;
const int MAX_MESSAGE_LEN = 129;

enum PACKET_INDEX
{
	REQ_IN = 1,
	RES_IN,
	REQ_CHAT,
	NOTICE_CHAT,
};

struct PACKET_HEADER
{
	int packetIndex;
	int packetSize;
};

struct TEST_PACKET
{
	PACKET_HEADER header;
	int Data1;
	std::string Data2;
};

//처음 클라가 들어왔을때 그 클라의 이름을 받음
struct PACKET_REQ_IN : public PACKET_HEADER
{
	char name[MAX_NAME_LEN];

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_IN;
		packetSize = sizeof(PACKET_REQ_IN);
		ZeroMemory(name, MAX_NAME_LEN);
	}
};

//클라에게 접속성공인지 알려줌
struct PACKET_RES_IN : public PACKET_HEADER 
{
	bool isSuccess;

	void Init()
	{
		packetIndex = PACKET_INDEX::RES_IN;
		packetSize = sizeof(PACKET_RES_IN);
		isSuccess = false;
	}
};

//접속되있는 클라의 메시지를 받음
struct PACKET_REQ_CHAT : public PACKET_HEADER
{
	char szMessage[MAX_MESSAGE_LEN];

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_CHAT;
		packetSize = sizeof(PACKET_REQ_CHAT);
		ZeroMemory(szMessage, MAX_MESSAGE_LEN);
	}
};

//누가 무슨 채팅을 했는지 클라들에게 뿌려줌
struct PACKET_NOTICE_CHAT : public PACKET_HEADER 
{
	char szName[MAX_NAME_LEN];
	char szMessage[MAX_MESSAGE_LEN];

	void Init()
	{
		packetIndex = PACKET_INDEX::NOTICE_CHAT;
		packetSize = sizeof(PACKET_NOTICE_CHAT);
		ZeroMemory(szName, MAX_NAME_LEN);
		ZeroMemory(szMessage, MAX_MESSAGE_LEN);
	}
};