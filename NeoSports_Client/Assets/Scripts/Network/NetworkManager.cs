using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

using System.Text;
using System;

using Newtonsoft.Json;

public class AsyncObject
{
    public byte[] buffer;
    public Socket workingSocket;
    public AsyncObject(Int32 bufferSize)
    {
        this.buffer = new byte[bufferSize];
    }
}

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

public struct HeaderPacket
{
    public PACKET_HEADER header;
};


public enum PACKET_INDEX
{
    REQ_IN = 200,
    MULTI_ROOM, //클라에서 같이하기 눌렀을때 방을 만들거나 방이있으면 접속함
    ROOM_INFO,
};

public enum CHAR_INDEX
{
    EMPTY_CHAR = 100,
    CHICK,
    JELLY,
};

public enum ROOM_INDEX
{
    EMPTY_ROOM = 0,
    ENTER_ROOM,
    MAKE_ROOM,
    ROPE_PULL,
    ROPE_JUMP,
    BASKET_BALL,
};

public class NetworkManager : Singleton<NetworkManager>
{
    const string IpAdress = "192.168.1.119";
    const string LoopbackAdress = "127.0.0.1";
    const int PortNumber = 31400;

    public bool isLoopBack;

    Socket _sock = null;
    AsyncCallback _receiveHandler;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void SendNickName(string playerNickName)
    {
        if (_sock == null)
            Connect();
        //TO DO : 서버에게 패킷으로 플레이어가 결정한 별명 보내주기 
        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_IN);
        PACKET_REQ_IN packet = new PACKET_REQ_IN { header = headerPacket, name = playerNickName };

        SendToServerPacket(packet);
    }

    public void SendRequsetRoom(ROOM_INDEX roomIndex)
    {
        if (_sock == null)
            Connect();

        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.MULTI_ROOM);
        PACKET_MULTI_ROOM packet = new PACKET_MULTI_ROOM { header = headerPacket, gameIndex = (int)roomIndex,
            charIndex = (int)CHAR_INDEX.CHICK };

        SendToServerPacket(packet);
    }

    void SendToServerPacket(object value)
    {
        string jsonBuffer;
        jsonBuffer = JsonConvert.SerializeObject(value); //객체를 json직렬화 
        jsonBuffer += '\0';//서버에서 널문자까지 읽기 위해 널문자붙이기
        byte[] bufSend = new byte[128]; //전송을 위해 바이트단위로 변환
        bufSend = Encoding.UTF8.GetBytes(jsonBuffer);
        _sock.Send(bufSend);
    }

    PACKET_HEADER MakeHeaderPacket(PACKET_INDEX packetIndex)
    {
        switch (packetIndex)
        {
            case PACKET_INDEX.REQ_IN:
                {
                    int packetSize = Marshal.SizeOf<PACKET_REQ_IN>();
                    PACKET_HEADER headerPacket;
                    headerPacket = new PACKET_HEADER(packetIndex, packetSize);
                    return headerPacket;
                }
            case PACKET_INDEX.MULTI_ROOM:
                {
                    int packetSize = Marshal.SizeOf<PACKET_MULTI_ROOM>();
                    PACKET_HEADER headerPacket;
                    headerPacket = new PACKET_HEADER(packetIndex, packetSize);
                    return headerPacket;
                }
            default:
                {
                    int packetSize = Marshal.SizeOf<PACKET_REQ_IN>();
                    PACKET_HEADER headerPacket;
                    headerPacket = new PACKET_HEADER(packetIndex, packetSize);
                    return headerPacket;
                }
        }
    }

    void Connect()
    {
        _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        if (_sock == null)
        {
            Debug.Log("소켓생성 실패");
            PopupManager.PopupData a;
            a.text = "Socket Fail";
            a.okFlag = true;
            a.callBack = ExitProgram;
            PopupManager.Instance.ShowPopup(a);
            return;
        }
        try
        {
            if (isLoopBack)
            {
                _sock.Connect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber));
            }
            else
            {
                _sock.Connect(new IPEndPoint(IPAddress.Parse(IpAdress), PortNumber));
            }
            //Async 추가 작성 
            {
                _receiveHandler = new AsyncCallback(HandleDataRecive);
                //_sock.NoDelay = true; //nagle off
                AsyncObject ao = new AsyncObject(128);
                ao.workingSocket = _sock;
                _sock.BeginReceive(ao.buffer, 0, ao.buffer.Length,
                   SocketFlags.None, _receiveHandler, ao);
            }
        }
        catch (SocketException se)
        {
            Debug.Log(se.Message);
            PopupManager.PopupData a;
            a.text = se.Message;
            a.okFlag = true;
            a.callBack = ExitProgram;
            PopupManager.Instance.ShowPopup(a);
            return;
        }
    }

    void HandleDataRecive(IAsyncResult ar)
    {
        AsyncObject ao = (AsyncObject)ar.AsyncState;
        Int32 recvBytes = ao.workingSocket.EndReceive(ar);

        if (recvBytes > 0)
        {
			//receive처리 
			byte[] recvBuf = new byte[recvBytes];
			Array.Copy(ao.buffer, recvBuf, recvBytes);

			string recvData = Encoding.UTF8.GetString(recvBuf, 0, recvBytes);

			//To Do: Json 처리
			var headerData = JsonConvert.DeserializeObject<HeaderPacket>(recvData);
			ProcessReceivePacket(headerData.header.packetIndex, recvData);
		}
        ao.workingSocket.BeginReceive(ao.buffer, 0, ao.buffer.Length
            , SocketFlags.None, _receiveHandler, ao);
    }

    void ExitProgram()
    {
        Debug.Log("Call Exit");
        Application.Quit();
    }

	void ProcessReceivePacket(int pakcetIndex, string recvData)
	{
		switch (pakcetIndex)
		{
			case (int)PACKET_INDEX.MULTI_ROOM:
				{
					var packetdata = JsonConvert.DeserializeObject<PACKET_MULTI_ROOM>(recvData);
					Debug.Log(packetdata.gameIndex);
					break;
				}
			case (int)PACKET_INDEX.ROOM_INFO:
				{
					break;
				}
			case (int)PACKET_INDEX.REQ_IN:
				{
					break;
				}
			default:
				return;
		}
	}
}