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
    EMPTY_GAME = 20,
    ROPE_PULL,
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

    REQ_MULTI_ROOM, //클라에서 같이하기 눌렀을때 방을 만들거나 방이있으면 접속함
    START_GAME,
    REQ_INIT_ROOM,
    ROOM_INFO,

	//줄다리기용 패킷
	REQ_RES_ROPE_PULL_GAME,

	//게임랭킹
	REQ_RANK,
	RES_RANK,

	//클라와 통신하고 있지 않은 인덱스들 (채팅용)
	RES_IN,
    REQ_CHAT,
    NOTICE_CHAT,
    //
};
#region ClientToServerPacket
public struct PACKET_REQ_IN
{
    public PACKET_HEADER header;
    public string name;
};

public struct PACKET_REQ_INIT_ROOM
{
    public PACKET_HEADER header;
    public GAME_INDEX gameIndex;
    public bool isEndGame;
};

//줄다리기 게임 데이터 패킷
public struct PACKET_REQ_RES_ROPE_PULL_GAME
{
	public PACKET_HEADER header;
	public float ropePos;
};

public struct PACKET_REQ_MULTI_ROOM
{
	public PACKET_HEADER header;
	public GAME_INDEX gameIndex;
	public CHAR_INDEX charIndex;

	public PACKET_REQ_MULTI_ROOM(PACKET_HEADER packetHeader, GAME_INDEX _gameIndex, CHAR_INDEX _charIndex)
	{
		header = packetHeader;
		gameIndex = _gameIndex;
		charIndex = _charIndex;
	}

};
public struct PACKET_REQ_RANK
{
	public PACKET_HEADER header;
	public GAME_INDEX gameIndex;
};
#endregion
public struct RANK
{
	public char[] name;
	public int winRecord;
};

struct PACKET_RES_RANK
{
	PACKET_HEADER header;
	RANK[] rank;
};

public struct HeaderPacket
{
    public PACKET_HEADER header;
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

public struct PACKET_START_GAME
{
    public PACKET_HEADER header;
    public CHAR_INDEX superCharID; //방장의 캐릭터
    public CHAR_INDEX charID;

	public char[] superName;
	public char[] name;
};

public struct PACKET_ROOM_INFO
{
    public PACKET_HEADER header;
    public ROOM_INDEX roomInfo; //방을 만든건지 들어간건지의 정보
    public CHAR_INDEX charInfo; //상대 플레이어의 캐릭터 정보
};