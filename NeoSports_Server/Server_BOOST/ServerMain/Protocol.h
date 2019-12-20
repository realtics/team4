#pragma once
#include <iostream>

const unsigned short PORT_NUMBER = 31400;

const int MAX_RECEIVE_BUFFER_LEN = 1024;

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
	REQ_INIT_ROOM,
	REQ_TIME,
	REQ_ENTER_FARM,
	REQ_SAVE_FARM,
	REQ_RES_GOLD,

	RES_IN, //Ŭ�󿡰� clientID�� �Ҵ��ϴ� ��Ŷ
	RES_START_GAME,
	RES_ROOM_INFO,
	RES_NOW_TIME,

	//�ٴٸ���� ��Ŷ
	REQ_RES_ROPE_PULL_GAME,

	//�󱸰��ӿ� ��Ŷ
	REQ_RES_BASKET_BALL_GAME,

	//���ӷ�ŷ
	REQ_RANK,
	RES_RANK,

	//Ŭ��� ����ϰ� ���� ���� �ε����� (ä�ÿ�)
	REQ_CHAT,
	NOTICE_CHAT,
	//
};

enum FARM_INDEX
{
	LAND = 300,
	ROAD,
	PRODUCT,
	DECORATION,
	GARBAGE
};


struct RANK
{
	int clientID;
	int winRecord;
};

struct PACKET_HEADER
{
	int packetIndex;
	int packetSize;
};

struct PACKET_REQ_ENTER_FARM : public PACKET_HEADER
{
	int clientID;

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_ENTER_FARM;
		packetSize = sizeof(PACKET_REQ_ENTER_FARM);
	}
};

struct PACKET_REQ_RES_GOLD : public PACKET_HEADER
{
	int gold;

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_RES_GOLD;
		packetSize = sizeof(PACKET_REQ_RES_GOLD);
	}
};

struct PACKET_REQ_RES_BASKET_BALL_GAME : public PACKET_HEADER
{
	float power;
	float angleX;
	float angleY;

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_RES_BASKET_BALL_GAME;
		packetSize = sizeof(PACKET_REQ_RES_BASKET_BALL_GAME);
		power = 0.0f;
		angleX = 0.0f;
		angleY = 0.0f;
	}
};

struct PACKET_REQ_RES_FARM : public PACKET_HEADER
{
	FARM_INDEX farmIndex;
	char farmInfoJSON[1024] = {0,};

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_SAVE_FARM;
		packetSize = sizeof(PACKET_REQ_RES_FARM);
	}
};

struct PACKET_RES_IN : public PACKET_HEADER
{
	int clientID;

	void Init()
	{
		packetIndex = PACKET_INDEX::RES_IN;
		packetSize = sizeof(PACKET_RES_IN);
		clientID = 0;
	}
};

struct PACKET_REQ_UNKNOWN //������ ���� ��û�� �ϴ� ��Ŷ
{
	PACKET_HEADER header;
};

struct PACKET_RES_NOW_TIME
{
	PACKET_HEADER header;
	char time[30];
};

struct PACKET_REQ_RANK
{
	PACKET_HEADER header;
	GAME_INDEX gameIndex;
};

struct PACKET_RES_RANK
{
	PACKET_HEADER header;
	RANK rank[MAX_RANK_COUNT]
		= { { NULL,NULL },{ NULL,NULL },{ NULL,NULL },{ NULL,NULL }, { NULL,NULL }, };;
};

//Ŭ�� ��Ƽ���� ��ư�� ��������
struct PACKET_REQ_MULTI_ROOM
{
	PACKET_HEADER header;
	GAME_INDEX gameIndex;
	int charIndex;
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
	int superCharID; //������ ĳ����
	int charID;
	GAME_INDEX gameIndex;

	char superName[12];
	char name[12];
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
	int clientID;
	char name[MAX_NAME_LEN];

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_IN;
		packetSize = sizeof(PACKET_REQ_IN);
		//memset(name,0,MAX_NAME_LEN);
	}
};

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