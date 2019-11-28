using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //single은 PlayManager에서,멀티는 네트워크에서


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
	const int TimeOutCode = 100060;
	const int bufferSize = 512;

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
		
		instance = this;
		isOwnHost = false;
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

	public bool IsSinglePlay()
	{
		if (SceneManager.GetActiveScene().name == SceneName.NetworkBasketBallSceneName)
			return false;
		else if (SceneManager.GetActiveScene().name == SceneName.NetworkRopeGameSceneName)
			return false;
		return true;
	}

	public void SendRequestRoom(GAME_INDEX roomIndex)
	{
		if (_sock == null)
			Connect();

		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_MULTI_ROOM);
		PACKET_REQ_MULTI_ROOM packet = new PACKET_REQ_MULTI_ROOM
		{
			header = headerPacket,
			gameIndex = roomIndex,
			charIndex = (CHAR_INDEX)InventoryManager.Instance.CurrentCharacter.Type//(int)CHAR_INDEX.CHICK 
		};

		SendToServerPacket(packet);
	}

	public void SendRequestRopePull(float pullPower)
	{
		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_RES_ROPE_PULL_GAME);
		PACKET_REQ_RES_ROPE_PULL_GAME packet = new PACKET_REQ_RES_ROPE_PULL_GAME
		{
			header = headerPacket,
			ropePos = pullPower,
		};
		SendToServerPacket(packet);
	}

	public void SendRequsetRank(GAME_INDEX currentGameIndex)
	{
		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_RANK);
		PACKET_REQ_RANK packet = new PACKET_REQ_RANK
		{
			header = headerPacket,
			gameIndex = currentGameIndex,
		};
		SendToServerPacket(packet);
	}

    public void SendRequestExitRoom(GAME_INDEX roomIndex, bool isRoomEndGame)
    {
        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_INIT_ROOM);
        PACKET_REQ_INIT_ROOM packet = new PACKET_REQ_INIT_ROOM
        {
            header = headerPacket,
            gameIndex = roomIndex,
            isEndGame = isRoomEndGame,
        };
        SendToServerPacket(packet);
    }

	void SendToServerPacket(object value)
	{
		string jsonBuffer;
		jsonBuffer = JsonConvert.SerializeObject(value); //객체를 json직렬화 
		jsonBuffer += '\0';//서버에서 널문자까지 읽기 위해 널문자붙이기
		byte[] bufSend = new byte[bufferSize]; //전송을 위해 바이트단위로 변환
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
			case PACKET_INDEX.REQ_MULTI_ROOM:
				{
					int packetSize = Marshal.SizeOf<PACKET_REQ_MULTI_ROOM>();
					PACKET_HEADER headerPacket;
					headerPacket = new PACKET_HEADER(packetIndex, packetSize);
					return headerPacket;
				}
            case PACKET_INDEX.REQ_INIT_ROOM:
                {
                    int packetSize = Marshal.SizeOf<PACKET_REQ_INIT_ROOM>();
                    PACKET_HEADER headerPacket;
                    headerPacket = new PACKET_HEADER(packetIndex, packetSize);
                    return headerPacket;
                }
			case PACKET_INDEX.REQ_RANK:
				{
					int packetSize = Marshal.SizeOf<PACKET_REQ_RANK>();
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
				var result = _sock.BeginConnect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber), null, null);
				bool success = result.AsyncWaitHandle.WaitOne(1000, true);
				if (success)
				{
					//_sock.EndConnect(result);
				}
				else
				{
					_sock.Close();
					throw new SocketException(TimeOutCode);
				}
			}
			else
			{
				var result = _sock.BeginConnect(new IPEndPoint(IPAddress.Parse(IpAdress), PortNumber), null, null);
				bool success = result.AsyncWaitHandle.WaitOne(1000, true);
				if (success)
				{
					//_sock.EndConnect(result);
				}
				else
				{
					_sock.Close();
					throw new SocketException(TimeOutCode);
				}
			}

			//Async 추가 작성 
			{
				_receiveHandler = new AsyncCallback(HandleDataRecive);
				_sock.NoDelay = true; //nagle off
				AsyncObject ao = new AsyncObject(bufferSize);
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
			if (se.ErrorCode == TimeOutCode)
				a.text = "Server is not running";
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

			
			var headerData = JsonConvert.DeserializeObject<HeaderPacket>(recvData);
			
			//recevie 처리를 큐잉으로 대체. 
			PacketQueue.Instance.networkQueue.Enqueue(new NetworkQueueData( headerData.header.packetIndex,recvData));
		}
		ao.workingSocket.BeginReceive(ao.buffer, 0, ao.buffer.Length
			, SocketFlags.None, _receiveHandler, ao);
	}

	void ExitProgram()
	{
		Debug.Log("Call Exit");
		Application.Quit();
	}

}