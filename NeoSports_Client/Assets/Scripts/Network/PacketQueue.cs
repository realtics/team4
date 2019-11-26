using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine;

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
	public CHAR_INDEX superCharIndex;
	[HideInInspector]
	public CHAR_INDEX charIndex;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		superCharIndex = CHAR_INDEX.EMPTY_CHAR;
		charIndex = CHAR_INDEX.EMPTY_CHAR;
		DontDestroyOnLoad(this);
	}

	void Update()
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
					{
						superCharIndex = (CHAR_INDEX)InventoryManager.instance.CurrentCharacter.Type; 
					}
					SceneManager.LoadScene(SceneName.NetworkRopeGameSceneName);
					
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

					superCharIndex = packetdata.superCharID;
					charIndex = packetdata.charID;

					SceneManager.LoadScene(SceneName.NetworkRopeGameSceneName);

					break;
				}
			case (int)PACKET_INDEX.REQ_RES_ROPE_PULL_GAME:
				{
					var packetdata = JsonConvert.DeserializeObject<PACKET_REQ_RES_ROPE_PULL_GAME>(recvData);
					RopePullGame.RopePullMoveRopeWithKey.Instance.UpdateNetworkRopePostion(packetdata.ropePos);
					
					break;
				}
			default:
				return;
		}
	}
}
