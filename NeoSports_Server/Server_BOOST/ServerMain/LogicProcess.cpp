#include "LogicProcess.h"
#include "Protocol.h"
#include "Server.h"
#include "Json.h"
#include "DB.h"
#include "ThreadHandler.h"
#include "Time.h"

#include <boost/lexical_cast.hpp>
#include <typeinfo>
#include <iostream>

#define FARM_INFO_MAX_COUNT 5

LogicProcess::LogicProcess(Server* serverPtr, ThreadHandler* threadHandler)
	:_serverPtr(serverPtr), _threadHandler(threadHandler)
{

}

LogicProcess::~LogicProcess()
{

}

void LogicProcess::StopProcess()
{
	while (!_threadHandler->IsEmptyPacketQueue())
	{
		ProcessPacket();
	}
}

void LogicProcess::ProcessPacket()
{
	while (true) //TODO : bool변수 만들기
	{
		//_threadHandler->pushPakcetQueueLock.Enter();
		if (_threadHandler->IsEmptyPacketQueue())
		{
			Sleep(1);
			//_threadHandler->pushPakcetQueueLock.Leave();
		}
		else if (!_threadHandler->IsEmptyPacketQueue())
		{
			PacketData packetData = _threadHandler->GetPakcetDataQueueFront();
			//_threadHandler->pushPakcetQueueLock.Leave();

			const int sessionID = packetData.sessionID;
			const char* data = packetData.data;

			PACKET_HEADER* header = (PACKET_HEADER*)data;

			switch (header->packetIndex)
			{
			case PACKET_INDEX::REQ_IN:
			{
				//accept 처리
				_serverPtr->ProcessReqInPacket(sessionID, data);
				//clientID조회를 위해 clientID값을 얻어온다
				PACKET_REQ_IN* recvPakcet = (PACKET_REQ_IN*)data;
				if (recvPakcet->clientID == 0)
				{
					//clientID 조회 및 전송
					PACKET_RES_IN sendPacket;
					sendPacket.Init();
					int temp = DB::GetInstance()->InsertUser(&recvPakcet->clientID, sessionID);
					sendPacket.clientID = temp;

					std::string aa = _SerializationJson(PACKET_INDEX::RES_IN, (const char*)&sendPacket);
					_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
				}
				int clientID = recvPakcet->clientID;

				if (recvPakcet->clientID != 0)
				{
					DB::GetInstance()->UpdataUserTable(clientID, sessionID);
				}

				DB::GetInstance()->SetNameTable(clientID, recvPakcet->name);

				break;
			}

			case PACKET_INDEX::REQ_RES_GOLD:
			{
				PACKET_REQ_RES_GOLD sendPacket;
				sendPacket.Init();
				int clientID = DB::GetInstance()->GetClientID(sessionID);
				int gold = DB::GetInstance()->GetGold(clientID);
				DB::GetInstance()->SetGold(clientID, gold);
				sendPacket.gold = gold;

				std::string aa = _SerializationJson(PACKET_INDEX::REQ_RES_GOLD, (const char*)&sendPacket);
				_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
				break;
			}

			case PACKET_INDEX::REQ_RES_MOVE:
			{
				int roomNum = _serverPtr->GetRoomNum(sessionID);
				ROOM room;
				room.Init();

				if (roomNum != FAIL_ROOM_SERCH)
				{
					room = (*_serverPtr->GetRoomInfo(roomNum));
				}

				if (room.gameMG != nullptr)
				{
					PACKET_REQ_RES_MOVE* packet = (PACKET_REQ_RES_MOVE*)data;
					std::cout << packet->positionX << "," << packet->positionY << std::endl;
					std::cout << "sss" << std::endl;

					std::string aa = _SerializationJson(PACKET_INDEX::REQ_RES_MOVE, (const char*)packet);
					std::cout << aa << std::endl;

					int superSessionIdTemp = room.superSessionID;
					int sessionIdTemp = room.sessionID;

					//방장이면 방장이 아닌 플레이어에게 보내고 방장이 아니면 방장에게만 보냄
					if (sessionID == superSessionIdTemp)
						_serverPtr->PostSendSession(sessionIdTemp, false, aa.length(), (char*)aa.c_str());
					else if (sessionID == sessionIdTemp)
						_serverPtr->PostSendSession(superSessionIdTemp, false, aa.length(), (char*)aa.c_str());
				}
				else
					std::cout << "Session : ProcessPacket : gameMG가 null입니다." << std::endl;
				break;

				break;
			}

			case PACKET_INDEX::REQ_RES_BASKET_BALL_GAME:
			{
				int roomNum = _serverPtr->GetRoomNum(sessionID);
				ROOM room;
				room.Init();

				if (roomNum != FAIL_ROOM_SERCH)
				{
					room = (*_serverPtr->GetRoomInfo(roomNum));
				}

				if (room.gameMG != nullptr)
				{
					PACKET_REQ_RES_BASKET_BALL_GAME* packet = (PACKET_REQ_RES_BASKET_BALL_GAME*)data;

					std::string aa = _SerializationJson(PACKET_INDEX::REQ_RES_BASKET_BALL_GAME, (const char*)packet);

					int superSessionIdTemp = room.superSessionID;
					int sessionIdTemp = room.sessionID;

					//방장이면 방장이 아닌 플레이어에게 보내고 방장이 아니면 방장에게만 보냄
					if (sessionID == superSessionIdTemp)
						_serverPtr->PostSendSession(sessionIdTemp, false, aa.length(), (char*)aa.c_str());
					else if (sessionID == sessionIdTemp)
						_serverPtr->PostSendSession(superSessionIdTemp, false, aa.length(), (char*)aa.c_str());
				}
				else
					std::cout << "Session : ProcessPacket : gameMG가 null입니다." << std::endl;
				break;
			}

			case PACKET_INDEX::REQ_SAVE_FARM:
			{
				PACKET_REQ_RES_FARM* packet = (PACKET_REQ_RES_FARM*)data;
				int clientID = DB::GetInstance()->GetClientID(sessionID);
				DB::GetInstance()->SetFarmInfo(clientID, packet->farmInfoJSON, packet->farmIndex);

				break;
			}

			case PACKET_INDEX::REQ_ENTER_FARM:
			{
				PACKET_REQ_ENTER_FARM* enterPacket = (PACKET_REQ_ENTER_FARM*)data;
				int clientID = enterPacket->clientID;

				int farmIndex[FARM_INFO_MAX_COUNT] = { -1,-1,-1,-1,-1 };
				std::string jsonArr[FARM_INFO_MAX_COUNT] = { "", };
				DB::GetInstance()->GetFarmInfo(clientID, jsonArr, farmIndex);

				for (int i = 0; i < FARM_INFO_MAX_COUNT; i++)
				{
					PACKET_REQ_RES_FARM sendPacket;
					sendPacket.Init();
					sendPacket.packetIndex = PACKET_INDEX::RES_ENTER_FARM;
					sendPacket.farmIndex = (FARM_INDEX)farmIndex[i];
					memcpy(&sendPacket.farmInfoJSON, jsonArr[i].c_str(), strlen(jsonArr[i].c_str()));

					std::string aa = _SerializationJson(PACKET_INDEX::RES_ENTER_FARM, (const char*)&sendPacket);
					_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
					Sleep(100); //패킷 전송이 뭉쳐서 가서 임시로 딜레이를 줌
				}
				break;
			}

			case PACKET_INDEX::REQ_TIME:
			{
				//PACKET_RES_NOW_TIME* packet = (PACKET_RES_NOW_TIME*)data;
				PACKET_RES_NOW_TIME sendPacket;
				sendPacket.header.packetIndex = PACKET_INDEX::RES_NOW_TIME;
				sendPacket.header.packetSize = sizeof(PACKET_RES_NOW_TIME);

				std::string tempTimeStr = currentDateTime();
				int strLen = strlen(tempTimeStr.data());
				memcpy(&sendPacket.time, tempTimeStr.data(), sizeof(strLen));

				std::string aa = _SerializationJson(PACKET_INDEX::RES_NOW_TIME, (const char*)&sendPacket);
				_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
				break;
			}

			case PACKET_INDEX::REQ_CHECK_CLIENT_ID:
			{
				PACKET_REQ_CHECK_CLIENT_ID* packetQCC = (PACKET_REQ_CHECK_CLIENT_ID*)data;

				PACKET_RES_CHECK_CLIENT_ID packetSCC;
				packetSCC.Init();

				bool isClientID = DB::GetInstance()->CheckClientID(packetQCC->clientID);
				packetSCC.isClientID = isClientID;

				std::string aa = _SerializationJson(PACKET_INDEX::RES_CHECK_CLIENT_ID, (const char*)&packetSCC);
				_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
				break;
			}
			case PACKET_INDEX::REQ_MULTI_ROOM:
			{
				PACKET_REQ_MULTI_ROOM* packet = (PACKET_REQ_MULTI_ROOM*)data;

				int mrTemp = _serverPtr->MakeRoom(packet->gameIndex, sessionID, packet->charIndex);

				if (mrTemp == ROOM_HOST::ENTER_ROOM) //도전자 입장이면 스타트패킷생성후 방장과 도전자에게 전송
				{
					_serverPtr->SetGameMG(false, sessionID, packet->gameIndex);
					int roomNum = _serverPtr->GetRoomNum(sessionID);

					ROOM room;
					room.Init();
					room = *(_serverPtr->GetRoomInfo(roomNum));

					int superSessionIdTemp = room.superSessionID;
					int sessionIdTemp = room.sessionID;

					PACKET_START_GAME* startPacket = new PACKET_START_GAME;
					startPacket->header.packetIndex = PACKET_INDEX::RES_START_GAME;
					startPacket->header.packetSize = sizeof(PACKET_START_GAME);
					startPacket->superCharID = (CHAR_INDEX)room.charIndex[0]; //방장의 캐릭터
					startPacket->charID = (CHAR_INDEX)room.charIndex[1]; //도전자의 캐릭터
					strcpy(startPacket->superName, _serverPtr->GetSuperSessionName(superSessionIdTemp).c_str());
					strcpy(startPacket->name, _serverPtr->GetSessionName(sessionIdTemp).c_str());

					startPacket->gameIndex = room.gameMG->GetCurGame();

					std::string aa = _SerializationJson(PACKET_INDEX::RES_START_GAME, (const char*)startPacket);

					_serverPtr->PostSendSession(superSessionIdTemp, false, aa.length(), (char*)aa.c_str());
					_serverPtr->PostSendSession(sessionIdTemp, false, aa.length(), (char*)aa.c_str());
					break;
				}
				else if (mrTemp == ROOM_HOST::MAKE_ROOM)
				{
					_serverPtr->SetGameMG(true, sessionID, packet->gameIndex);

					PACKET_ROOM_INFO sendPacket;
					sendPacket.header.packetIndex = PACKET_INDEX::RES_ROOM_INFO;
					sendPacket.header.packetSize = sizeof(PACKET_ROOM_INFO);
					sendPacket.roomInfo = (ROOM_HOST)mrTemp; //받는 클라입장에서 자신이 방장인지 구별

					std::string aa = _SerializationJson(PACKET_INDEX::RES_ROOM_INFO, (const char*)&sendPacket);
					_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
					break;
				}
			}

			case PACKET_INDEX::REQ_INIT_ROOM:
			{
				_serverPtr->ProcessInitRoomPacket(sessionID, data);
			}
			break;

			case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
			{
				int roomNum = _serverPtr->GetRoomNum(sessionID);
				ROOM room;
				room.Init();

				if (roomNum != FAIL_ROOM_SERCH)
				{
					room = (*_serverPtr->GetRoomInfo(roomNum));
				}

				//클라에서 x버튼이나 게임중 메뉴의 yes,no버튼 클릭할때도
				//게임로직 패킷이 보내져서 예외처리 해주는중
				if (roomNum == FAIL_ROOM_SERCH ||
					room.curGame == GAME_INDEX::EMPTY_GAME)
				{
					std::cout << "(already Init)." << std::endl;
					break;
				}

				if (room.gameMG != nullptr)
				{
					PACKET_REQ_RES_ROPE_PULL_GAME* packet = (PACKET_REQ_RES_ROPE_PULL_GAME*)data;
					room.gameMG->SetRopePos(packet->ropePos);
					float ropePos = room.gameMG->GetRopePos();

					PACKET_REQ_RES_ROPE_PULL_GAME resPacket;
					resPacket.header.packetIndex = PACKET_INDEX::REQ_RES_ROPE_PULL_GAME;
					resPacket.header.packetSize = sizeof(PACKET_REQ_RES_ROPE_PULL_GAME);
					resPacket.ropePos = ropePos;

					std::string aa = _SerializationJson(PACKET_INDEX::REQ_RES_ROPE_PULL_GAME, (const char*)&resPacket);

					int superSessionIdTemp = room.superSessionID;
					int sessionIdTemp = room.sessionID;

					_serverPtr->PostSendSession(superSessionIdTemp, false, aa.length(), (char*)aa.c_str());
					_serverPtr->PostSendSession(sessionIdTemp, false, aa.length(), (char*)aa.c_str());
				}
				else
					std::cout << "Session : ProcessPacket : gameMG가 null입니다." << std::endl;
			}
			break;

			case PACKET_INDEX::REQ_RANK:
			{
				PACKET_REQ_RANK* packet = (PACKET_REQ_RANK*)data;
				RANK rank[MAX_RANK_COUNT]
					= { { NULL,NULL },{ NULL,NULL },{ NULL,NULL },{ NULL,NULL }, { NULL,NULL }, };

				DB::GetInstance()->Rank(packet->gameIndex, rank);
				PACKET_RES_RANK resRankPacket;
				resRankPacket.header.packetIndex = PACKET_INDEX::RES_RANK;
				resRankPacket.header.packetSize = sizeof(PACKET_RES_RANK);
				for (int i = 0; i < MAX_RANK_COUNT; i++)
				{
					if (rank[i].clientID != NULL)
					{
						resRankPacket.rank[i].clientID = rank[i].clientID;
						resRankPacket.rank[i].winRecord = rank[i].winRecord;
					}
				}
				std::string aa = _SerializationJson(PACKET_INDEX::RES_RANK, (const char*)&resRankPacket);
				_serverPtr->PostSendSession(sessionID, false, aa.length(), (char*)aa.c_str());
			}
			break;

			/*case REQ_CHAT:
			{
				PACKET_REQ_CHAT* packet = (PACKET_REQ_CHAT*)data;

				PACKET_NOTICE_CHAT sendPacket;
				sendPacket.Init();
				strncpy_s(sendPacket.szName, MAX_NAME_LEN, _sessionVec[sessionID]->GetName(), MAX_NAME_LEN - 1);
				strncpy_s(sendPacket.szMessage, MAX_MESSAGE_LEN, packet->szMessage, MAX_MESSAGE_LEN - 1);

				size_t totalSessioncount = _sessionVec.size();

				for (int i = 0; i < totalSessioncount; i++)
				{
					if (_sessionVec[i]->Socket().is_open())
					{
						_sessionVec[i]->PostSend(false, sendPacket.packetSize, (char*)&sendPacket);
					}
				}
			}
			break;*/
			}
			_threadHandler->PopPacketQueue();
		}
	}
}

void LogicProcess::memcpyStr(std::string* str1, const int str2)
{
	std::string temp = boost::lexical_cast<std::string>(str2);
	int temps = str1->size();
	int temps2 = temp.size();
	for (temps2; temps2 >= 0; temps2--, temps--)
	{
		str1[0][temps - 1] = temp[temps2 - 1];
		if (temps2 - 1 <= 0)
			break;
	}
}


std::string LogicProcess::_SerializationJson(PACKET_INDEX packetIndex, const char* packet, std::string jsonLength)
{
	std::string sendStr = "";

	switch (packetIndex)
	{

	case PACKET_INDEX::RES_IN:
	{
		PACKET_RES_IN resInPacket;
		memcpy(&resInPacket, packet, sizeof(PACKET_RES_IN));

		boost::property_tree::ptree ptSendRI;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", resInPacket.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);
		ptSendRI.add_child("header", ptSendHeader);

		ptSendRI.put<int>("clientID", resInPacket.clientID);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendRI, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}

		return sendStr;
	}

	case PACKET_INDEX::REQ_RES_GOLD:
	{
		PACKET_REQ_RES_GOLD goldPacket;
		memcpy(&goldPacket, packet, sizeof(PACKET_REQ_RES_GOLD));

		boost::property_tree::ptree ptSendG;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", goldPacket.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendG.add_child("header", ptSendHeader);

		ptSendG.put<int>("gold", goldPacket.gold);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendG, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}

		return sendStr;
	}

	case PACKET_INDEX::RES_CHECK_CLIENT_ID:
	{
		PACKET_RES_CHECK_CLIENT_ID chckClientIdPacket;
		memcpy(&chckClientIdPacket, packet, sizeof(PACKET_RES_CHECK_CLIENT_ID));

		boost::property_tree::ptree ptSendCC;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", chckClientIdPacket.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendCC.add_child("header", ptSendHeader);

		ptSendCC.put<bool>("isClientID", chckClientIdPacket.isClientID);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendCC, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}
		return sendStr;

		break;
	}

	case PACKET_INDEX::REQ_RES_MOVE:
	{
		PACKET_REQ_RES_MOVE movePacket;
		movePacket.Init();
		memcpy(&movePacket, packet, sizeof(PACKET_REQ_RES_MOVE));

		boost::property_tree::ptree ptSendMove;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", movePacket.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendMove.add_child("header", ptSendHeader);

		ptSendMove.put<float>("positonX", movePacket.positionX);
		ptSendMove.put<float>("positonY", movePacket.positionY);
		ptSendMove.put<float>("positonZ", movePacket.positionZ);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendMove, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}

		return sendStr;
	}

	case PACKET_INDEX::REQ_RES_BASKET_BALL_GAME:
	{
		PACKET_REQ_RES_BASKET_BALL_GAME bascetBallPacket;
		bascetBallPacket.Init();
		memcpy(&bascetBallPacket, packet, sizeof(PACKET_REQ_RES_BASKET_BALL_GAME));

		boost::property_tree::ptree ptSendBB;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", bascetBallPacket.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendBB.add_child("header", ptSendHeader);

		ptSendBB.put<float>("power", bascetBallPacket.power);
		ptSendBB.put<float>("angleX", bascetBallPacket.angleX);
		ptSendBB.put<float>("angleY", bascetBallPacket.angleY);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendBB, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}

		return sendStr;
	}

	case PACKET_INDEX::RES_ROOM_INFO:
	{
		PACKET_ROOM_INFO roomInfoPacket;
		memset(&roomInfoPacket, 0, sizeof(PACKET_ROOM_INFO));
		memcpy(&roomInfoPacket, packet, sizeof(PACKET_ROOM_INFO));

		boost::property_tree::ptree ptSendRI;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", roomInfoPacket.header.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendRI.add_child("header", ptSendHeader);

		ptSendRI.put<int>("roomInfo", (ROOM_HOST)roomInfoPacket.roomInfo);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendRI, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}

		return sendStr;
	}

	case PACKET_INDEX::RES_NOW_TIME:
	{
		PACKET_RES_NOW_TIME timePacket;
		memset(&timePacket, 0, sizeof(PACKET_RES_NOW_TIME));
		memcpy(&timePacket, packet, sizeof(PACKET_RES_NOW_TIME));

		boost::property_tree::ptree ptSendT;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", timePacket.header.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendT.add_child("header", ptSendHeader);

		std::string tempTimeStr = currentDateTime();
		char tempTimChar[30];
		memcpy(&tempTimChar, tempTimeStr.data(), sizeof(strlen(tempTimeStr.data())));
		ptSendT.put<char*>("time", tempTimChar);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendT, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}
		return sendStr;
	}

	case PACKET_INDEX::RES_ENTER_FARM:
	{
		PACKET_REQ_RES_FARM farmInfoPacket;
		farmInfoPacket.Init();
		memcpy(&farmInfoPacket, packet, sizeof(PACKET_REQ_RES_FARM));

		boost::property_tree::ptree ptSendFarmInfo;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", farmInfoPacket.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendFarmInfo.add_child("header", ptSendHeader);

		ptSendFarmInfo.put<int>("saveIndex", (FARM_INDEX)farmInfoPacket.farmIndex);
		ptSendFarmInfo.put<char*>("saveData", farmInfoPacket.farmInfoJSON);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendFarmInfo, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}
		return sendStr;
	}

	case PACKET_INDEX::RES_START_GAME:
	{
		PACKET_START_GAME startGamePacket;
		memset(&startGamePacket, 0, sizeof(PACKET_START_GAME));
		memcpy(&startGamePacket, packet, sizeof(PACKET_START_GAME));

		boost::property_tree::ptree ptSendSGP;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", startGamePacket.header.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendSGP.add_child("header", ptSendHeader);

		ptSendSGP.put<int>("superCharID", (CHAR_INDEX)startGamePacket.superCharID);
		ptSendSGP.put<int>("charID", (CHAR_INDEX)startGamePacket.charID);

		ptSendSGP.put<char*>("superName", startGamePacket.superName);
		ptSendSGP.put<char*>("name", startGamePacket.name);

		ptSendSGP.put<int>("gameIndex", (GAME_INDEX)startGamePacket.gameIndex);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendSGP, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}
		return sendStr;
	}

	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
	{
		PACKET_REQ_RES_ROPE_PULL_GAME ropePullPacket;
		memset(&ropePullPacket, 0, sizeof(PACKET_REQ_RES_ROPE_PULL_GAME));
		memcpy(&ropePullPacket, packet, sizeof(PACKET_REQ_RES_ROPE_PULL_GAME));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", ropePullPacket.header.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSend.add_child("header", ptSendHeader);

		ptSend.put<float>("ropePos", ropePullPacket.ropePos);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}
		return sendStr;
	}

	case PACKET_INDEX::RES_RANK:
	{
		PACKET_RES_RANK resRankPacket;
		memset(&resRankPacket, 0, sizeof(PACKET_RES_RANK));
		memcpy(&resRankPacket, packet, sizeof(PACKET_RES_RANK));
		boost::property_tree::ptree ptSendRR;

		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", resRankPacket.header.packetIndex);
		ptSendHeader.put<std::string>("packetSize", jsonLength);

		ptSendRR.add_child("header", ptSendHeader);

		boost::property_tree::ptree ptSendRankArr;
		boost::property_tree::ptree arr[MAX_RANK_COUNT];
		for (int i = 0; i < MAX_RANK_COUNT; i++)
		{
			arr[i].put("name", resRankPacket.rank[i].clientID);
			arr[i].put("winRecord", resRankPacket.rank[i].winRecord);
			ptSendRankArr.push_back(std::make_pair("", arr[i]));
		}
		ptSendRR.add_child("rank", ptSendRankArr);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSendRR, false);
		sendStr = os.str();

		if (jsonLength == "0000")
		{
			int jsonLengthTemp = sendStr.length();
			memcpyStr(&jsonLength, jsonLengthTemp);
			sendStr = _SerializationJson(packetIndex, packet, jsonLength);
		}
		return sendStr;
	}

	}
}