using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROOM_INDEX
{
	EMPTY_ROOM = 11,

	MAKE_ROOM, //방장으로서 방에 입장
	ENTER_ROOM,
};

public enum GAME_INDEX
{
	ROPE_PULL = 20,
	ROPE_JUMP,
	BASKET_BALL,
};

public enum CHAR_INDEX
{
	EMPTY_CHAR = 100,
	CHICK,
	JELLY,
};

public enum PACKET_INDEX
{
	REQ_IN = 200,
	MULTI_ROOM, //클라에서 같이하기 눌렀을때 방을 만들거나 방이있으면 접속함
	ROOM_INFO,

	//클라와 통신하고 있지 않은 인덱스들 (채팅용)
	RES_IN,
	REQ_CHAT,
	NOTICE_CHAT,
	//
};

public struct PACKET_HEADER
{
	public int packetIndex;
	public int packetSize;

	public PACKET_HEADER(PACKET_INDEX index, int size)
	{
		packetIndex = (int)index;
		packetSize = size;
	}
};

public struct PACKET_REQ_IN
{
	public PACKET_HEADER header;
	public string name;
};

public struct PACKET_END_GAME
{
	PACKET_HEADER header;
	int gameIndex;
};

public struct PACKET_MULTI_ROOM
{
	public PACKET_HEADER header;
	public int gameIndex;
	public int charIndex;
};

public struct PACKET_ROOM_INFO
{
	public PACKET_HEADER header;
	public int roomInfo; //방을 만든건지 들어간건지의 정보
	public int charInfo; //상대 플레이어의 캐릭터 정보
};