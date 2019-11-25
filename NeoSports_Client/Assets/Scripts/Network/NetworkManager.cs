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

public class NetworkManager : Singleton<NetworkManager>
{
    const string IpAdress = "192.168.1.119";
    const string LoopbackAdress = "127.0.0.1";
    const int PortNumber = 31400;

    public bool isLoopBack;
    [HideInInspector]
    public bool isOwnHost;
	[HideInInspector]
	public CHAR_INDEX superCharIndex;
	[HideInInspector]
	public CHAR_INDEX charIndex;


	Socket _sock = null;
    AsyncCallback _receiveHandler;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
		isOwnHost = false;
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void SendNickName(string playerNickName)
    {
        if (_sock == null)
            Connect(); 
        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_IN);
        PACKET_REQ_IN packet = new PACKET_REQ_IN { header = headerPacket, name = playerNickName };

        SendToServerPacket(packet);
    }

    public void SendRequsetRoom(GAME_INDEX roomIndex)
    {
        if (_sock == null)
            Connect();

        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.MULTI_ROOM);
        PACKET_MULTI_ROOM packet = new PACKET_MULTI_ROOM 
		{ header = headerPacket, gameIndex = roomIndex,
            charIndex = (CHAR_INDEX)InventoryManager.Instance.CurrentCharacter.Type//(int)CHAR_INDEX.CHICK 
		};
		
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
                //_sock.Connect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber));
				var result = _sock.BeginConnect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber),null,null);
				bool success = result.AsyncWaitHandle.WaitOne(1000, true);
				if (success)
				{
					//_sock.EndConnect(result);
				}
				else 
				{
					_sock.Close();
					throw new SocketException(100060);
				}
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
					Debug.Log(packetdata.charIndex);
					break;
				}
			case (int)PACKET_INDEX.ROOM_INFO:
				{
					var packetdata = JsonConvert.DeserializeObject<PACKET_ROOM_INFO>(recvData);
					Debug.Log(packetdata.roomInfo);

					if (packetdata.roomInfo == ROOM_INDEX.MAKE_ROOM)
						isOwnHost = true;
					if (packetdata.roomInfo == ROOM_INDEX.ENTER_ROOM)
						isOwnHost = false;
				
					break;
				}
			case (int)PACKET_INDEX.REQ_IN:
				{
					var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_IN>(recvData);
					Debug.Log(packetdata.name);
					break;
				}
			case (int)PACKET_INDEX.START_GAME:
				{
					var packetdata = JsonConvert.DeserializeObject<PACKET_START_GAME>(recvData);

					//씬이 바뀌기전에 StartGame 패킷이 먼저와서 싱글톤 RopeGameManager를 찾을 수 없어. 
					//NetworkManager에서 정보를 갖고 있는 것으로 대체.
					//RopePullGame.RopePullGameManager.Instance.CreateCharacters(packetdata.superCharID, packetdata.charID);
					superCharIndex = packetdata.superCharID;
					charIndex = packetdata.charID;

					break;
				}
			default:
				return;
		}
	}
}