#pragma once
//#include <Windows.h>

const unsigned short PORT_NUMBER = 31400;

const int MAX_RECEIVE_BUFFER_LEN = 512;

const int MAX_NAME_LEN = 17;
const int MAX_MESSAGE_LEN = 129;

const int MAX_ROOM_COUNT = 20;
const int FAIL_ROOM_SERCH = -101;

enum ROOM_INDEX
{
	EMPTY_ROOM = 11,

	MAKE_ROOM, //�������μ� �濡 ����
	ENTER_ROOM,
};

enum GAME_INDEX
{
	ROPE_PULL = 20,
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
	MULTI_ROOM, //Ŭ�󿡼� �����ϱ� �������� ���� ����ų� ���������� ������
	ROOM_INFO,

	//Ŭ��� ����ϰ� ���� ���� �ε����� (ä�ÿ�)
	RES_IN,
	REQ_CHAT,
	NOTICE_CHAT,
	//
};

struct PACKET_HEADER
{
	int packetIndex;
	int packetSize;
};

//Ŭ�� ��Ƽ���� ��ư�� ��������
struct PACKET_MULTI_ROOM
{
	PACKET_HEADER header;
	int gameIndex;
	int charIndex;
};

//��Ƽ������ ��û�� Ŭ�󿡰� ������ ��Ŷ
struct PACKET_ROOM_INFO
{
	PACKET_HEADER header;
	int roomInfo; //���� ������� �������� ����
	int charInfo; //��� �÷��̾��� ĳ���� ����
};

//ó�� Ŭ�� �������� �� Ŭ���� �̸��� ����
struct PACKET_REQ_IN : public PACKET_HEADER
{
	char name[MAX_NAME_LEN];

	void Init()
	{
		packetIndex = PACKET_INDEX::REQ_IN;
		packetSize = sizeof(PACKET_REQ_IN);
		//ZeroMemory(name, MAX_NAME_LEN);
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