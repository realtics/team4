#include "LogicProcess.h"
#include "Protocol.h"
#include "Server.h"
#include "Json.h"
#include "DB.h"

#include <typeinfo>
#include <iostream>

void LogicProcess::Init(Server* server)
{
	_serverPtr = server;
	packetQueueEvents = CreateEvent(NULL, FALSE, TRUE, NULL);
	if (packetQueueEvents == NULL)
	{
		cout << "LogicProcess::Init() : packetQueueEvents : error " << endl;
	}
}

void LogicProcess::StopProcess()
{
	while (!packetQue.empty())
	{
		ProcessPacket();
	}
}

void LogicProcess::_PostSend(const bool Immediately, const int size, char* data)
{
	char* sendData = nullptr;

	if (Immediately == false)
	{
		sendData = new char[size];
		memcpy(sendData, data, size);

		_sendDataDeq.push_back(sendData);
	}
	else
	{
		sendData = data;
	}

	if (Immediately == false && _sendDataDeq.size() > 1)
	{
		return;
	}

	boost::asio::async_write(_socket, boost::asio::buffer(sendData, size),
		boost::bind(&Session::_WriteHandle, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred)
	);
}

void LogicProcess::ProcessPacket()
{
	while (true)
	{
		int retval = WaitForSingleObject(packetQueueEvents, INFINITE);
		if (retval == WAIT_FAILED)
			break;

		std::cout << "Call Logic Thread" << std::endl;
		const int sessionID = packetQue.front().sessionID;
		const char* data = packetQue.front().data;

		PACKET_HEADER* header = (PACKET_HEADER*)data;

		switch (header->packetIndex)
		{
		case PACKET_INDEX::REQ_IN:
		{
			_serverPtr->ProcessReqInPacket(sessionID, data);
		}
		break;

		case PACKET_INDEX::REQ_MULTI_ROOM:
		{
			PACKET_REQ_MULTI_ROOM* packet = (PACKET_REQ_MULTI_ROOM*)data;

			int mrTemp = _serverPtr->MakeRoom(packet->gameIndex, sessionID, packet->charIndex);

			if (mrTemp == ROOM_HOST::ENTER_ROOM) //도전자 입장이면 스타트패킷생성후 방장과 도전자에게 전송
			{
				_serverPtr->GetGameMG(false, sessionID, packet->gameIndex);
				int roomNum = _serverPtr->GetRoomNum(sessionID);

				ROOM room;
				room.Init();
				room = *(_serverPtr->GetRoomInfo(roomNum));

				int superSessionIdTemp = room.superSessionID;
				int sessionIdTemp = room.sessionID;

				PACKET_START_GAME* startPacket = new PACKET_START_GAME;
				startPacket->header.packetIndex = PACKET_INDEX::START_GAME;
				startPacket->header.packetSize = sizeof(PACKET_START_GAME);
				startPacket->superCharID = (CHAR_INDEX)room.charIndex[0]; //방장의 캐릭터
				startPacket->charID = (CHAR_INDEX)room.charIndex[1]; //도전자의 캐릭터
				strcpy(startPacket->superName, _serverPtr->GetSuperSessionName(superSessionIdTemp).c_str());
				strcpy(startPacket->name, _serverPtr->GetSessionName(sessionIdTemp).c_str());

				std::string aa = _SerializationJson(PACKET_INDEX::START_GAME, (const char*)startPacket);

				_serverPtr->PostSendSession(superSessionIdTemp, false, aa.length(), (char*)aa.c_str());
				PostSend(false, aa.length(), (char*)aa.c_str());
				return;
			}
			_serverPtr->GetGameMG(true, sessionID, packet->gameIndex);

			PACKET_ROOM_INFO sendPacket;
			sendPacket.header.packetIndex = PACKET_INDEX::ROOM_INFO;
			sendPacket.header.packetSize = sizeof(PACKET_ROOM_INFO);
			sendPacket.roomInfo = (ROOM_HOST)mrTemp; //받는 클라입장에서 자신이 방장인지 구별


			std::string aa = _SerializationJson(PACKET_INDEX::ROOM_INFO, (const char*)&sendPacket);
			PostSend(false, aa.length(), (char*)aa.c_str());
		}
		break;

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
			room = (*_serverPtr->GetRoomInfo(roomNum));

			//클라에서 x버튼이나 게임중 메뉴의 yes,no버튼 클릭할때도
			//게임로직 패킷이 보내져서 예외처리 해주는중
			if (roomNum == FAIL_ROOM_SERCH ||
				_gameMG->GetCurGame() == GAME_INDEX::EMPTY_GAME)
			{
				std::cout << "(already Init)." << std::endl;
				break;
			}

			if (_gameMG != nullptr)
			{

				PACKET_REQ_RES_ROPE_PULL_GAME* packet = (PACKET_REQ_RES_ROPE_PULL_GAME*)data;
				_gameMG->SetRopePos(packet->ropePos);
				float ropePos = _gameMG->GetRopePos();

				PACKET_REQ_RES_ROPE_PULL_GAME resPacket;;
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
			RANK rank[MAX_RANK_COUNT];

			DB::GetInstance()->Rank(packet->gameIndex, rank);//배열 포인터를 전달하고싶다
			PACKET_RES_RANK resRankPacket;
			resRankPacket.header.packetIndex = PACKET_INDEX::RES_RANK;
			resRankPacket.header.packetSize = sizeof(PACKET_RES_RANK);
			for (int i = 0; i < MAX_RANK_COUNT; i++)
			{
				strcpy(resRankPacket.rank[i].name, rank[i].name);
				resRankPacket.rank[i].winRecord = rank[i].winRecord;
			}
			std::string aa = _SerializationJson(PACKET_INDEX::RES_RANK, (const char*)&resRankPacket);
			PostSend(false, aa.length(), (char*)aa.c_str());
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

		return;
		}

	}
}

std::string LogicProcess::_SerializationJson(PACKET_INDEX packetIndex, const char* packet)
{
	std::string sendStr;


	switch (packetIndex)
	{
	case PACKET_INDEX::ROOM_INFO:
	{
		PACKET_ROOM_INFO* roomInfoPacket = new PACKET_ROOM_INFO;
		memcpy(&roomInfoPacket, &packet, sizeof(PACKET_ROOM_INFO));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", roomInfoPacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_ROOM_INFO));
		ptSend.add_child("header", ptSendHeader);

		ptSend.put<int>("roomInfo", (ROOM_HOST)roomInfoPacket->roomInfo);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}

	case PACKET_INDEX::START_GAME:
	{
		PACKET_START_GAME* startGamePacket = new PACKET_START_GAME;
		memcpy(&startGamePacket, &packet, sizeof(PACKET_START_GAME));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", startGamePacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_START_GAME));
		ptSend.add_child("header", ptSendHeader);

		ptSend.put<int>("superCharID", (CHAR_INDEX)startGamePacket->superCharID);
		ptSend.put<int>("charID", (CHAR_INDEX)startGamePacket->charID);

		ptSend.put<char*>("superName", startGamePacket->superName);
		ptSend.put<char*>("name", startGamePacket->name);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}

	case PACKET_INDEX::REQ_RES_ROPE_PULL_GAME:
	{
		PACKET_REQ_RES_ROPE_PULL_GAME* ropePullPacket = new PACKET_REQ_RES_ROPE_PULL_GAME;
		memcpy(&ropePullPacket, &packet, sizeof(PACKET_REQ_RES_ROPE_PULL_GAME));

		boost::property_tree::ptree ptSend;
		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", ropePullPacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_START_GAME));
		ptSend.add_child("header", ptSendHeader);

		ptSend.put<float>("ropePos", ropePullPacket->ropePos);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}
	break;

	case PACKET_INDEX::RES_RANK:
	{
		PACKET_RES_RANK* resRankPacket = new PACKET_RES_RANK;
		memcpy(&resRankPacket, &packet, sizeof(PACKET_RES_RANK));
		boost::property_tree::ptree ptSend;

		boost::property_tree::ptree ptSendHeader;
		ptSendHeader.put<int>("packetIndex", resRankPacket->header.packetIndex);
		ptSendHeader.put<int>("packetSize", sizeof(PACKET_RES_RANK));
		ptSend.add_child("header", ptSendHeader);

		boost::property_tree::ptree ptSendRankArr;
		boost::property_tree::ptree arr[MAX_RANK_COUNT];
		for (int i = 0; i < MAX_RANK_COUNT; i++)
		{
			arr[i].put("name", resRankPacket->rank[i].name);
			arr[i].put("winRecord", resRankPacket->rank[i].winRecord);
			ptSendRankArr.push_back(std::make_pair("", arr[i]));
		}
		ptSend.add_child("rank", ptSendRankArr);

		std::string recvTemp;
		std::ostringstream os(recvTemp);
		boost::property_tree::write_json(os, ptSend, false);
		sendStr = os.str();
		return sendStr;
	}
	break;

	}
}