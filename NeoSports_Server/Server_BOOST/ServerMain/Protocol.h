#pragma once
#include <iostream>

const unsigned short PORT_NUMBER = 31400;

const int MAX_RECEIVE_BUFFER_LEN = 512;

const int MAX_NAME_LEN = 17;
const int MAX_MESSAGE_LEN = 129;

const int MAX_ROOM_COUNT = 20;
const int FAIL_ROOM_SERCH = -101;

const int MAX_RANK_COUNT = 5;

	
enum ROOM_HOST
{
	EMPTY_ROOM = 11,

	MAKE_ROOM, //�������μ� �濡 ����
	ENTER_ROOM,
};

enum GAME_INDEX
{
	EMPTY_GAME = 20,
	ROPE_PULL,
	ROPE_JUMP,
	BASKET_BALL,
};

enum CHAR_INDEX
{
	EMPTY_CHAR = 100,
	CHICK,
	JELLY,
};

enum PACKET_INDEX
{
	REQ_IN = 200,

	REQ_MULTI_ROOM, //Ŭ�󿡼� �����ϱ� �������� ���� ����ų� ���������� ������
	START_GAME,
	REQ_INIT_ROOM,
	ROOM_INFO,

	//�ٴٸ���� ��Ŷ
	REQ_RES_ROPE_PULL_GAME,

	//���ӷ�ŷ
	REQ_RANK,
	RES_RANK,

	//Ŭ��� ����ϰ� ���� ���� �ε����� (ä�ÿ�)
	RES_IN,
	REQ_CHAT,
	NOTICE_CHAT,
	//
};

struct RANK
{
	char name[12];
	int winRecord;
};

struct PACKET_HEADER
{
	int packetIndex;
	int packetSize;
};

struct PACKET_REQ_RANK
{
	PACKET_HEADER header;
	GAME_INDEX gameIndex;
};

struct PACKET_RES_RANK
{
	PACKET_HEADER header;
	RANK rank[MAX_RANK_COUNT];
};

//Ŭ�� ��Ƽ���� ��ư�� ��������
struct PACKET_REQ_MULTI_ROOM
{
	PACKET_HEADER header;
	GAME_INDEX gameIndex;
	CHAR_INDEX charIndex;
};

struct PACKET_REQ_INIT_ROOM
{
	PACKET_HEADER header;
	GAME_INDEX gameIndex;
	bool isEndGame;
};

struct PACKET_START_GAME
{
	PACKET_HEADER header;
	CHAR_INDEX superCharID; //������ ĳ����
	CHAR_INDEX charID;

	char superName[12];
	char name[12];

	~PACKET_START_GAME() {};
};

//�ٴٸ��� ���� ������ ��Ŷ
struct PACKET_REQ_RES_ROPE_PULL_GAME
{
	PACKET_HEADER header;
	float ropePos;
};

//��Ƽ������ ��û�� Ŭ�󿡰� ������ ��Ŷ
struct PACKET_ROOM_INFO
{
	PACKET_HEADER header;
	ROOM_HOST roomInfo; //���� ������� �������� ����
};

//ó�� Ŭ�� �������� �� Ŭ���� �̸��� ����
struct PACKET_REQ_IN : public PACKET_HEADER
{
	char name[MAX_NAME_LEN];

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_IN;
		packetSize = sizeof(PACKET_REQ_IN);
		//memset(name,0,MAX_NAME_LEN);
	}
};

//Ŭ�󿡰� ���Ӽ������� �˷���
//struct PACKET_RES_IN : public PACKET_HEADER
//{
//	bool isSuccess;
//
//	void Init()
//	{
//		packetIndex = PACKET_INDEX::RES_IN;
//		packetSize = sizeof(PACKET_RES_IN);
//		isSuccess = false;
//	}
//};
//
////���ӵ��ִ� Ŭ���� �޽����� ����
//struct PACKET_REQ_CHAT : public PACKET_HEADER
//{
//	char szMessage[MAX_MESSAGE_LEN];
//
//	void Init()
//	{
//		packetIndex = PACKET_INDEX::REQ_CHAT;
//		packetSize = sizeof(PACKET_REQ_CHAT);
//		ZeroMemory(szMessage, MAX_MESSAGE_LEN);
//	}
//};
//
////���� ���� ä���� �ߴ��� Ŭ��鿡�� �ѷ���
//struct PACKET_NOTICE_CHAT : public PACKET_HEADER
//{
//	char szName[MAX_NAME_LEN];
//	char szMessage[MAX_MESSAGE_LEN];
//
//	void Init()
//	{
//		packetIndex = PACKET_INDEX::NOTICE_CHAT;
//		packetSize = sizeof(PACKET_NOTICE_CHAT);
//		ZeroMemory(szName, MAX_NAME_LEN);
//		ZeroMemory(szMessage, MAX_MESSAGE_LEN);
//	}
//};