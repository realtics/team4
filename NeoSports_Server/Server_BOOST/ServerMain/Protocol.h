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

	MAKE_ROOM, //방장으로서 방에 입장
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

	REQ_MULTI_ROOM, //클라에서 같이하기 눌렀을때 방을 만들거나 방이있으면 접속함
	REQ_INIT_ROOM,
	REQ_TIME,
	REQ_ENTER_FARM,
	REQ_SAVE_FARM,

	RES_IN, //클라에게 clientID를 할당하는 패킷
	RES_START_GAME,
	RES_ROOM_INFO,
	RES_NOW_TIME,

	//줄다리기용 패킷
	REQ_RES_ROPE_PULL_GAME,

	//농구게임용 패킷
	REQ_RES_BASKET_BALL_GAME,

	//게임랭킹
	REQ_RANK,
	RES_RANK,

	//클라와 통신하고 있지 않은 인덱스들 (채팅용)
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

struct PACKET_REQ_RES_BASKET_BALL_GAME : public PACKET_HEADER
{
	float power;
	float angle;

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_RES_BASKET_BALL_GAME;
		packetSize = sizeof(PACKET_REQ_RES_BASKET_BALL_GAME);
	}
};

struct PACKET_REQ_RES_FARM : public PACKET_HEADER
{
	FARM_INDEX farmIndex;
	char farmInfoJSON[1024] = {0,};

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_ENTER_FARM;
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

struct PACKET_REQ_UNKNOWN //데이터 없이 요청만 하는 패킷
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

//클라가 멀티게임 버튼을 눌렀을때
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
	CHAR_INDEX superCharID; //방장의 캐릭터
	CHAR_INDEX charID;
	GAME_INDEX gameIndex;

	char superName[12];
	char name[12];
};

//줄다리기 게임 데이터 패킷
struct PACKET_REQ_RES_ROPE_PULL_GAME
{
	PACKET_HEADER header;
	float ropePos;
};

//멀티게임을 요청한 클라에게 보내는 패킷
struct PACKET_ROOM_INFO
{
	PACKET_HEADER header;
	ROOM_HOST roomInfo; //방을 만든건지 들어간건지의 정보
};

//처음 클라가 들어왔을때 그 클라의 이름을 받음
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
////접속되있는 클라의 메시지를 받음
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
////누가 무슨 채팅을 했는지 클라들에게 뿌려줌
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