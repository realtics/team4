using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

using System.Text;
using System;

using Newtonsoft.Json;

public enum ROOM_INDEX
{
    EMPTY_ROOM = 0,
    ENTER_ROOM,

    MAKE_ROOM,
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
    public int charInfo; //상대 플레이어의 캐릭터 정보
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
    public String name;
};

public struct PACKET_MULTI_ROOM
{
    public PACKET_HEADER header;
    public int gameIndex;
    public int charIndex;
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
        //sock.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.119"), 31400));
        sock.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31400));

        //TODO : 패킷을 제이슨으로 직렬화,역직렬화 시키는 함수 작성하기
        {
            var headerPacket = new PACKET_HEADER { packetIndex = 201, packetSize = 10 };
            var p = new PACKET_MULTI_ROOM { header = headerPacket, gameIndex = 0, charIndex = 0 };
            string json;
            json = JsonConvert.SerializeObject(p); //객체를 json직렬화 
            json += '\0';//서버에서 널문자까지 읽기 위해 널문자붙이기
            byte[] bufSend = new byte[128]; //전송을 위해 바이트단위로 변환
            bufSend = Encoding.UTF8.GetBytes(json);
            sock.Send(bufSend);
        }


        byte[] bufRecv = new byte[128]; //수신을 위해 바이트단위로 변환
        int n = sock.Receive(bufRecv);
        Debug.Log("recv");
        Debug.Log(n);

        //string recvData = Encoding.UTF8.GetString(bufRecv, 0, n);
        //int bufLen = Encoding.Default.GetBytes(bufRecv);
        int bufLen = bufRecv.Length;
        string recvData = Encoding.UTF8.GetString(bufRecv, 0, n);
        Debug.Log(recvData);

        var data = JsonConvert.DeserializeObject<TempPacket>(recvData);
        if (data.header.packetIndex == 202) //JsonExample
        {
            var packetTemp = JsonConvert.DeserializeObject<PACKET_ROOM_INFO>(recvData);
            Debug.Log(packetTemp.roomInfo); //방번호
            Debug.Log(packetTemp.charInfo); //입장한 캐릭터 번호
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}