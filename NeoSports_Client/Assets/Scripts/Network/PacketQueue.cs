﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;
using FarmGame;

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
	public object queueLock = new object();
	public Queue<NetworkQueueData> networkQueue = new Queue<NetworkQueueData>();

	[HideInInspector]
	public int superCharIndex;
	[HideInInspector]
	public int charIndex;
	[HideInInspector]
	public int superCharEquip;
	[HideInInspector]
	public int gusetCharEquip;
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
			NetworkQueueData networkData;
			//lock (queueLock)
			{
				networkData = networkQueue.Dequeue();
			}
			
			ProcessReceivePacket((PACKET_INDEX)networkData.packetIndex, networkData.recvData);
		}
	}

	void ProcessReceivePacket(PACKET_INDEX pakcetIndex, string recvData)
	{
		switch (pakcetIndex)
		{
			case PACKET_INDEX.RES_ROOM_INFO:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_ROOM_INFO>(recvData);
				if (packetdata.roomInfo == ROOM_INDEX.MAKE_ROOM)
				{
					superCharIndex = InventoryManager.instance.CurrentCharacter.Type;
					NetworkManager.Instance.isOwnHost = true;
					NetworkManager.Instance.SendRequsetRank(packetdata.gameIndex);
					SceneManager.LoadScene(SceneName.WaitGameSceneName);
				}
				else
				{
					NetworkManager.Instance.isOwnHost = false;
				}
				break;
			}
			case PACKET_INDEX.REQ_IN:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_IN>(recvData);
				break;
			}
			case PACKET_INDEX.RES_START_GAME:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_START_GAME>(recvData);
				superCharIndex = packetdata.superCharInfo.charIndex;
				charIndex = packetdata.charInfo.charIndex;
				superCharEquip = packetdata.superCharInfo.item;
				gusetCharEquip = packetdata.charInfo.item;
				superName = packetdata.superName.ToString();
				guestName = packetdata.name.ToString();
				ChangeNetworkScene(packetdata.gameIndex);

				break;
			}
			case PACKET_INDEX.REQ_RES_ROPE_PULL_GAME:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_RES_ROPE_PULL_GAME>(recvData);
				RopePullGame.RopePullRope.Instance.UpdateNetworkRopePostion(packetdata.ropePos);
				break;
			}
			case PACKET_INDEX.RES_RANK:
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
			case PACKET_INDEX.RES_IN:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_RES_IN>(recvData);
				ResourceManager.Instance.SetClientId(packetdata.clientID);
				MainMenu.NickNameManager.Instance.SetUserIdInfoText(packetdata.clientID);
				break;
			}
			case PACKET_INDEX.REQ_RES_BASKET_BALL_GAME:
			{
				var packetdata =JsonConvert.DeserializeObject<PACKET_REQ_RES_BASKET_BALL_GAME>(recvData);
					BasketBallGame.BasketBallGameManager.Instance.NetworkShootOtherPlayer(packetdata.power, packetdata.angleX, packetdata.angleY);
				break;
			}
			case PACKET_INDEX.REQ_RES_MOVE:
			{
				var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_RES_MOVE>(recvData);
					BasketBallGame.BasketBallGameManager.Instance.NetworkMoveOtherPlayer(packetdata.positionX, packetdata.positionY, packetdata.positionZ);
				break;
			}
			case PACKET_INDEX.RES_NULL_CLIENT_ID:
			{
				var packetData = JsonConvert.DeserializeObject<PACKET_RES_CHECK_CLIENT_ID>(recvData);
				PlayManager.Instance.ProcessClientIdCheckResult(packetData.isClientID);
				break;
			}
			case PACKET_INDEX.RES_ENTER_FARM:
			{
				var packetData = JsonConvert.DeserializeObject<PACKET_REQ_RES_FARM>(recvData);

                if(packetData.saveData == null)
                    {
                        packetData.saveData = string.Empty;
                    }
				FriendFarmManager.Instance.LoadFarmSaveDatas(packetData.saveData, packetData.saveIndex);
				break;
			}
			case PACKET_INDEX.REQ_SET_GOLD:
			{
				var packetData = JsonConvert.DeserializeObject<PACKET_REQ_RES_GOLD>(recvData);
				ResourceManager.Instance.AddGoldResource(packetData.gold);
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
