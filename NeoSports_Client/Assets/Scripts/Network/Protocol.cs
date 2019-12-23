using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using FarmGame;

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

public enum FARM_INDEX
{
	LAND = 300,
	ROAD,
	PRODUCT,
	DECORATION,
	GARBAGE
}

public enum PACKET_INDEX
{
    REQ_IN = 200,

    REQ_MULTI_ROOM, //클라에서 같이하기 눌렀을때 방을 만들거나 방이있으면 접속함
    REQ_INIT_ROOM,
    REQ_TIME,
    REQ_ENTER_FARM,
    REQ_SAVE_FARM,
	REQ_RES_GOLD,
	REQ_RES_MOVE,
	REQ_CHECK_CLIENT_ID,

	RES_IN,
    RES_START_GAME,
    RES_ROOM_INFO,
    RES_NOW_TIME,
	RES_NULL_CLIENT_ID,
	RES_ENTER_FARM,

    //줄다리기용 패킷
    REQ_RES_ROPE_PULL_GAME,

	REQ_RES_BASKET_BALL_GAME,

    //게임랭킹
    REQ_RANK,
    RES_RANK,

    //클라와 통신하고 있지 않은 인덱스들 (채팅용)
    REQ_CHAT,
    NOTICE_CHAT,
	//
};

#region ClientToServerPacket
[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_IN
{
    public PACKET_HEADER header;
	public int clientID;
	public string name;
};
[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_INIT_ROOM
{
    public PACKET_HEADER header;
    public int gameIndex;
    public bool isEndGame;
};

//줄다리기 게임 데이터 패킷
[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_RES_ROPE_PULL_GAME
{
    public PACKET_HEADER header;
    public float ropePos;
};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_MULTI_ROOM
{
    public PACKET_HEADER header;
    public int gameIndex;
    public int charIndex;

    public PACKET_REQ_MULTI_ROOM(PACKET_HEADER packetHeader, GAME_INDEX _gameIndex, int _charIndex)
    {
        header = packetHeader;
        gameIndex = (int)_gameIndex;
        charIndex = _charIndex;
    }

};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_RANK
{
    public PACKET_HEADER header;
    public int gameIndex;
};


[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_RES_FARM
{
    public PACKET_HEADER header;
	public MapData.ESaveType saveIndex;
    public string saveData;
};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_UNKNOWN //데이터 없이 요청만 하는 패킷
{
    PACKET_HEADER header;
};

public struct PACKET_REQ_ENTER_FARM
{
	public PACKET_HEADER header;
	public int clientID;
}

public struct PACKET_REQ_RES_GOLD
{
	public PACKET_HEADER header;
	public int gold;
}

public struct PACKET_REQ_RES_BASKET_BALL_GAME
{
	public PACKET_HEADER header;
	public float power;
	public float angleX;
	public float angleY;
}

public struct PACKET_REQ_RES_MOVE
{
	public PACKET_HEADER header;
	public float positionX;
	public float positionY;
	public float positionZ;
}
#endregion

[StructLayout(LayoutKind.Sequential)]
struct PACKET_RES_NOW_TIME
{
	public PACKET_HEADER header;
	public string time;
};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_RES_IN
{
    public PACKET_HEADER header;
    public int clientID;
};

[StructLayout(LayoutKind.Sequential)]
public struct RANK
{
    public string name;
    public int winRecord;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PACKET_RES_RANK
{
    public PACKET_HEADER header;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public RANK[] rank;
};

[StructLayout(LayoutKind.Sequential)]
public struct HeaderPacket
{
    public PACKET_HEADER header;
};

[StructLayout(LayoutKind.Sequential)]
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

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_START_GAME
{
    public PACKET_HEADER header;
    public int superCharID; //방장의 캐릭터
    public int charID;
    public GAME_INDEX gameIndex;

    public string superName;
    public string name;
};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_ROOM_INFO
{
    public PACKET_HEADER header;
    public ROOM_INDEX roomInfo; //방을 만든건지 들어간건지의 정보
};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_REQ_CHECK_CLIENT_ID
{
	public PACKET_HEADER header;
	public int clientID;
};

[StructLayout(LayoutKind.Sequential)]
public struct PACKET_RES_CHECK_CLIENT_ID
{
	public PACKET_HEADER header;
	public bool isClientID;
};
