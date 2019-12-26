using FarmGame;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement; //single은 PlayManager에서,멀티는 네트워크에서

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
	const string AzureIPAdress = "52.141.2.84";
	string IpAdress = "192.168.1.119";
	const string LoopbackAdress = "127.0.0.1";
	const int PortNumber = 31400;
	const int TimeOutCode = 100060;
	const int bufferSize = 10240;

	public bool isLoopBackServer;
	public bool isAzureServer;

	[HideInInspector]
	public bool isOwnHost;
	[HideInInspector]
	public int superCharIndex;
	[HideInInspector]
	public int charIndex;

	Socket _sock = null;
	AsyncCallback _receiveHandler;

	public bool IsConnected { get; private set; }

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		instance = this;
		DontDestroyOnLoad(this);

		isOwnHost = false;
		IsConnected = false;
		if (isAzureServer)
		{
			IpAdress = AzureIPAdress;
		}
		
		
	}

	void Start()
	{
		if (_sock == null)
		{
			Connect();
		}
	}

	public void SendNickName(string playerNickName)
	{
		if (_sock == null)
		{
			Connect();
		}
		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_IN);
		PACKET_REQ_IN packet = new PACKET_REQ_IN { header = headerPacket, name = playerNickName, clientID  = ResourceManager.Instance.ClientId };

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
		{
			Connect();
		}

		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_MULTI_ROOM);
		PACKET_REQ_MULTI_ROOM packet = new PACKET_REQ_MULTI_ROOM
		(
			headerPacket,
			roomIndex,
			InventoryManager.Instance.CurrentCharacter.Type//(int)CHAR_INDEX.CHICK 
		);

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
			gameIndex = (int)currentGameIndex,
		};
		SendToServerPacket(packet);
	}

    public void SendRequestExitRoom(GAME_INDEX roomIndex, bool isRoomEndGame)
    {
        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_INIT_ROOM);
        PACKET_REQ_INIT_ROOM packet = new PACKET_REQ_INIT_ROOM
        {
            header = headerPacket,
            gameIndex = (int)roomIndex,
            isEndGame = isRoomEndGame,
        };
        SendToServerPacket(packet);
    }

	public void SendRequestFireBall(float firePower, float fireAngleX, float fireAngleY)
	{
		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_RES_BASKET_BALL_GAME);
		PACKET_REQ_RES_BASKET_BALL_GAME packet = new PACKET_REQ_RES_BASKET_BALL_GAME
		{
			header = headerPacket,
			power = firePower,
			angleX = fireAngleX,
			angleY = fireAngleY,
		};
		SendToServerPacket(packet);
	}

	public void SendRequestMove(float posX, float posY,float posZ)
	{
		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_RES_MOVE);
		PACKET_REQ_RES_MOVE packet = new PACKET_REQ_RES_MOVE
		{
			header = headerPacket,
			positionX = posX,
			positionY = posY,
			positionZ = posZ,
		};
		SendToServerPacket(packet);
	}

	public void SendRequestEarnGold(int goldAmount)
	{
		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_SET_GOLD);
		PACKET_REQ_RES_GOLD packet = new PACKET_REQ_RES_GOLD
		{
			header = headerPacket,
			gold = goldAmount,
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

	public bool SendFarmSaveData(MapData.ESaveType index, string data)
	{
		if(_sock == null)
		{
			return false;
		}

		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_SAVE_FARM);
		PACKET_REQ_RES_FARM packet = new PACKET_REQ_RES_FARM
		{
			header = headerPacket,
			saveIndex = (int)index,
			saveData = data
		};

		SendToServerPacket(packet);
		return true;
	}

	public void SendCheckClientIsExist(int clientId)
	{
		if(_sock == null)
		{
			return;
		}

		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_CHECK_CLIENT_ID);
		PACKET_REQ_CHECK_CLIENT_ID packet = new PACKET_REQ_CHECK_CLIENT_ID
		{
			header = headerPacket,
			clientID = clientId
		};

		SendToServerPacket(packet);
	}

	public void SendFriendFarmDataRequest(int clientId)
	{
		if (_sock == null)
		{
			return;
		}

		PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_ENTER_FARM);
		PACKET_REQ_ENTER_FARM packet = new PACKET_REQ_ENTER_FARM
		{
			header = headerPacket,
			clientID = clientId
		};

		SendToServerPacket(packet);
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
			case PACKET_INDEX.REQ_RES_ROPE_PULL_GAME:
				{
					int packetSize = Marshal.SizeOf<PACKET_REQ_RES_ROPE_PULL_GAME>();
					PACKET_HEADER headerPacket;
					headerPacket = new PACKET_HEADER(packetIndex, packetSize);
					return headerPacket;
				}
			case PACKET_INDEX.REQ_RES_BASKET_BALL_GAME:
				{
					int packetSize = Marshal.SizeOf<PACKET_REQ_RES_BASKET_BALL_GAME>();
					PACKET_HEADER headerPacket;
					headerPacket = new PACKET_HEADER(packetIndex, packetSize);
					return headerPacket;
				}
			case PACKET_INDEX.REQ_RES_MOVE:
				{ 
					int packetSize = Marshal.SizeOf<PACKET_REQ_RES_MOVE>();
					PACKET_HEADER headerPacket;
					headerPacket = new PACKET_HEADER(packetIndex, packetSize);
					return headerPacket;
				}
			case PACKET_INDEX.REQ_SET_GOLD:
				{
					int packetSize = Marshal.SizeOf<PACKET_REQ_RES_GOLD>();
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
			if (isLoopBackServer)
			{
				//_sock.Connect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber));
				var result = _sock.BeginConnect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber), null, null);
				bool IsConnected = result.AsyncWaitHandle.WaitOne(1000, true);
				if (IsConnected)
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

				
				IsConnected = result.AsyncWaitHandle.WaitOne(1000, true);
				if (IsConnected)
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
				//_sock.NoDelay = true; //nagle off
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
			a.callBack = null;
			PopupManager.Instance.ShowPopup(a);
			return;
		}
	}

	void HandleDataRecive(IAsyncResult ar)
	{
		try
		{
			AsyncObject ao = (AsyncObject)ar.AsyncState;
			Int32 recvBytes = ao.workingSocket.EndReceive(ar);

            if (recvBytes > Marshal.SizeOf<PACKET_HEADER>()) //bytestream 처리 
			{
				//receive처리 
				byte[] recvBuf = new byte[recvBytes];
				Array.Copy(ao.buffer, recvBuf, recvBytes);

				string recvData = Encoding.UTF8.GetString(recvBuf, 0, recvBytes);
				Debug.Log("Original RecvData: " + recvData);

				while (recvData != string.Empty)
				{
					var headerData = JsonConvert.DeserializeObject<HeaderPacket>(recvData);
					string strData = recvData.Substring(0, headerData.header.packetSize);
					Debug.Log("StrData: " + strData);

					//lock (PacketQueue.Instance.queueLock)
					{
						PacketQueue.Instance.networkQueue.Enqueue(new NetworkQueueData(headerData.header.packetIndex, strData));
					}
					recvData = recvData.Substring(headerData.header.packetSize);
					Debug.Log("Subed RecvData: " + recvData);
				}

				//recevie 처리를 큐잉으로 대체. 
				//PacketQueue.Instance.networkQueue.Enqueue(new NetworkQueueData(headerData.header.packetIndex, recvData));
			}
			ao.workingSocket.BeginReceive(ao.buffer, 0, ao.buffer.Length
				, SocketFlags.None, _receiveHandler, ao);

		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
			return;
		}
	}

	void ExitProgram()
	{
		Debug.Log("Call Exit");
		Application.Quit();
	}

}