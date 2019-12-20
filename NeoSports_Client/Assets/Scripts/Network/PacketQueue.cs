using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;

public struct NetworkQueueData
{
	public int packetIndex;
	public string recvData;

	public NetworkQueueData(int _packetIndex, string _recvData)
	{
		packetIndex = _packetIndex;
		recvData = _recvData;
	}
}

public class PacketQueue : Singleton<PacketQueue>
{
	public Queue<NetworkQueueData> networkQueue = new Queue<NetworkQueueData>();

	[HideInInspector]
	public int superCharIndex;
	[HideInInspector]
	public int charIndex;
	[HideInInspector]
	public string superName;
	[HideInInspector]
	public string guestName;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		superCharIndex = InventoryManager.EmptyCharType;
		charIndex = InventoryManager.EmptyCharType;
		DontDestroyOnLoad(this);
	}

	void FixedUpdate()
	{
		if (networkQueue.Count > 0)
		{
			var networkData = networkQueue.Dequeue();
			ProcessReceivePacket(networkData.packetIndex, networkData.recvData);
		}
	}

	void ProcessReceivePacket(int pakcetIndex, string recvData)
	{
		switch (pakcetIndex)
		{
			case (int)PACKET_INDEX.RES_ROOM_INFO:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_ROOM_INFO>(recvData);
				if (packetdata.roomInfo == ROOM_INDEX.MAKE_ROOM)
				{
					superCharIndex = InventoryManager.instance.CurrentCharacter.Type;
					NetworkManager.Instance.isOwnHost = true;
					//To DO: 현재는 임시시스템. 
		
					NetworkManager.Instance.SendRequsetRank(GAME_INDEX.ROPE_PULL);
					SceneManager.LoadScene(SceneName.WaitGameSceneName);
				}
				else
				{
					NetworkManager.Instance.isOwnHost = false;
				}


				break;
			}
			case (int)PACKET_INDEX.REQ_IN:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_IN>(recvData);
				break;
			}
			case (int)PACKET_INDEX.RES_START_GAME:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_START_GAME>(recvData);


				
				superCharIndex = packetdata.superCharID;
				charIndex = packetdata.charID;

				superName = packetdata.superName.ToString();
				guestName = packetdata.name.ToString();
				//SceneManager.LoadScene(SceneName.NetworkRopeGameSceneName);
				ChangeNetworkScene(packetdata.gameIndex);
				break;
			}
			case (int)PACKET_INDEX.REQ_RES_ROPE_PULL_GAME:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_RES_ROPE_PULL_GAME>(recvData);
				RopePullGame.RopePullRope.Instance.UpdateNetworkRopePostion(packetdata.ropePos);
				break;
			}
			case (int)PACKET_INDEX.RES_RANK:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_RES_RANK>(recvData);
				foreach (var rankdata in packetdata.rank)
				{
					WaitSceneManager.Instance.AddRankingName(rankdata.name);
					WaitSceneManager.Instance.AddRankingName("\t");
					WaitSceneManager.Instance.AddRankingName(rankdata.winRecord.ToString());
					WaitSceneManager.Instance.AddRankingName("\n");
				}
				break;
			}
			case (int)PACKET_INDEX.RES_IN:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_RES_IN>(recvData);
				ResourceManager.Instance.SetClientId(packetdata.clientID);
				break;
			}
			case (int)PACKET_INDEX.REQ_RES_BASKET_BALL_GAME:
			{
				var packetdata =JsonConvert.DeserializeObject<PACKET_REQ_RES_BASKET_BALL_GAME>(recvData);
					BasketBallGame.BasketBallGameManager.Instance.NetworkShootOtherPlayer(packetdata.power, packetdata.angleX, packetdata.angleY);
				break;
			}
			case (int)PACKET_INDEX.REQ_RES_MOVE:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_RES_MOVE>(recvData);
					BasketBallGame.BasketBallGameManager.Instance.NetworkMoveOtherPlayer(packetdata.positionX, packetdata.positionY);
				break;
			}
	
			default:
				break;
		}
	}

	void ChangeNetworkScene(GAME_INDEX networkGameIndex)
	{
		switch(networkGameIndex)
		{
			case GAME_INDEX.ROPE_PULL:
			{
				SceneManager.LoadScene(SceneName.NetworkRopeGameSceneName);
				break;
			}
			case GAME_INDEX.BASKET_BALL:
			{
				SceneManager.LoadScene(SceneName.NetworkBasketBallSceneName);
				break;
			}
			case GAME_INDEX.ROPE_JUMP:
			{
				break;
			}
			default:
				break;
		}
	}
}
