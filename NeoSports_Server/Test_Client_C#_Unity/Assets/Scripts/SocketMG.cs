﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

using System.Text;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum ROOM_INDEX
{
    EMPTY_ROOM = 11,

    ENTER_ROOM,
    MAKE_ROOM,
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

//멀티게임을 요청한 클라에게 보내는 패킷
public struct PACKET_ROOM_INFO
{
    public PACKET_HEADER header;
    public int roomInfo; //방을 만든건지 들어간건지의 정보
    public int superCharInfo; //방장의 캐릭터
    public int charInfo; //도전자의 캐릭터 정보
};

public struct PACKET_HEADER
{
    public int packetIndex;
    public int packetSize;
}
public struct TempPacket
{
    public PACKET_HEADER header;
}

public struct PACKET_REQ_IN
{
    public PACKET_HEADER header;
    public string name;
};

public struct PACKET_MULTI_ROOM
{
    public PACKET_HEADER header;
    public int gameIndex;
    public int charIndex;
};

public struct RANK
{
    public string name;
    public int winRecord;
};

public struct PACKET_RES_RANK
{
    public PACKET_HEADER header;
    public List<RANK> rank;
};
public struct PACKET_REQ_RANK
{
    public PACKET_HEADER header;
    public GAME_INDEX gameIndex;
};

public class SocketMG : MonoBehaviour
{
    private Socket sock = null;
    // Start is called before the first frame update
    void Start()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        if (sock == null)
        {
            Debug.Log("소켓생성 실패");
        }
        sock.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31400));

        {
            var headerPacket = new PACKET_HEADER { packetIndex = 206, packetSize = 10 };
            var p = new PACKET_MULTI_ROOM { header = headerPacket, gameIndex = 21};
            string json;
            json = JsonConvert.SerializeObject(p); //객체를 json직렬화 
            json += '\0';//서버에서 널문자까지 읽기 위해 널문자붙이기
            byte[] bufSend = new byte[128]; //전송을 위해 바이트단위로 변환
            bufSend = Encoding.UTF8.GetBytes(json);
            sock.Send(bufSend);
        }


        byte[] bufRecv = new byte[300]; //수신을 위해 바이트단위로 변환
        int n = sock.Receive(bufRecv);

        int bufLen = bufRecv.Length;

        string recvData = Encoding.UTF8.GetString(bufRecv, 0, n);
        Debug.Log(recvData);

        var data = JsonConvert.DeserializeObject<TempPacket>(recvData);
        if (data.header.packetIndex == 207) //JsonExample
        {
            JObject jobj = JObject.Parse(recvData);
            PACKET_RES_RANK packetTemp;
            packetTemp = JsonConvert.DeserializeObject<PACKET_RES_RANK>(recvData);
            Debug.Log(recvData);

            for (int i = 0; i < 5; i++)
            {
                Debug.Log(packetTemp.rank[i].name);
                Debug.Log(packetTemp.rank[i].winRecord);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}