#pragma once
#include <Windows.h>
#include "RoomProtocol.h"

const unsigned short PORT_NUMBER = 31400;

const int MAX_RECEIVE_BUFFER_LEN = 512;

const int MAX_NAME_LEN = 17;
const int MAX_MESSAGE_LEN = 129;

enum PACKET_INDEX
{
	REQ_IN = 1,
	MULTI_ROOM, //Ŭ�󿡼� �����ϱ� �������� ���� ����ų� ���������� ������

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

struct PACKET_MAKE_ROOM
{
	PACKET_HEADER header;

};

//ó�� Ŭ�� �������� �� Ŭ���� �̸��� ����
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

//Ŭ�󿡰� ���Ӽ������� �˷���
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

//���ӵ��ִ� Ŭ���� �޽����� ����
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

//���� ���� ä���� �ߴ��� Ŭ��鿡�� �ѷ���
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