#include "Server.h"
#include "Json.h"
#include "DB.h"

const int MAX_GAME_MG_COUNT = 10;

Server::Server(boost::asio::io_context& io_service) :
	_acceptor(io_service
	, boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), PORT_NUMBER))
{
	_isAccepting = false;

	for (int i = 0; i < MAX_GAME_MG_COUNT; i++)
	{
		_gameMGPool.push_back(new GameMG);
		_gameMGPool[i]->Init();
	}
}

Server::~Server()
{
	for (int i = 0; i < _sessionVec.size(); i++)
	{
		if (_sessionVec[i]->Socket().is_open())
		{
			_sessionVec[i]->Socket().close();
		}

		delete _sessionVec[i];
	}
}

void Server::Init(const int maxSessionCount)
{
	for (int i = 0; i < maxSessionCount; i++)
	{
		Session* session = new Session(i, (boost::asio::io_context&)_acceptor.get_executor().context(), this);
		_sessionVec.push_back(session);
		_sessionDeq.push_back(i);
	}
}

void Server::Start()
{
	std::cout << "Server Start..." << std::endl;

	DB::GetInstance()->Init();

	_PostAccept();
}

void Server::CloseSession(const int sessionID)
{
	std::cout << "Server : Client out. Session ID : " << sessionID << std::endl;

	_sessionVec[sessionID]->Socket().close();
	_sessionDeq.push_back(sessionID);

	if (_isAccepting == false)
	{
		_PostAccept();
	}
}


int Server::MakeRoom(GAME_INDEX gameIndex, int sessionID, CHAR_INDEX charIndex)
{
	return _roomMG.MakeRoom(gameIndex, sessionID, charIndex);
}
int Server::GetRoomNum(int sessionID)
{
	return _roomMG.GetRoomNum(sessionID);
}

int Server::GetSuperSessionID(int roomNum)
{
	return _roomMG.GetSuperSessonID(roomNum);
}

int Server::GetSessionID(int roomNum)
{
	return _roomMG.GetSessonID(roomNum);
}

ROOM* Server::GetRoomInfo(int roomNum)
{
	return _roomMG.GetRoomInfo(roomNum);
}

std::string Server::GetSuperSessionName(int sessionID)
{
	return _sessionVec[sessionID]->GetName();
}

void Server::PostSendSession(int sessionID, const bool Immediately, const int size, char* data)
{
	_sessionVec[sessionID]->PostSend(Immediately, size, data);
}

std::string Server::GetSessionName(int sessionID)
{
	return _sessionVec[sessionID]->GetName();
}

void Server::InitRoom(int roomNum)
{
	_roomMG.InitRoom(roomNum);
}

void Server::GetGameMG(bool isSuperSession, int  sessionID, GAME_INDEX gameIndex)
{
	for (auto iter = _gameMGPool.begin(); iter != _gameMGPool.end(); iter++)
	{
		if (isSuperSession == true && (*iter)->GetCurGame() == GAME_INDEX::EMPTY_GAME)
		{
			GameMG* gameMG = (*iter);
			gameMG->SetCurGame(gameIndex);
			_sessionVec[sessionID]->SetGameMG(gameMG);
			break;
		}
		else if (!isSuperSession && (*iter)->GetCurGame() == gameIndex)
		{
			GameMG* gameMG = (*iter);
			_sessionVec[sessionID]->SetGameMG(gameMG);
			break;
		}
	}
}


void Server::ProcessReqInPacket(const int sessionID, const char* data)
{
	PACKET_REQ_IN* packet = (PACKET_REQ_IN*)data;
	_sessionVec[sessionID]->SetName(packet->name);

	std::cout << "Server : Client accept. Name : " << _sessionVec[sessionID]->GetName() << std::endl;
	DB::GetInstance()->Insert(_sessionVec[sessionID]->GetName());
}

void Server::ProcessInitRoomPacket(const int sessionID, const char* data)
{
	PACKET_REQ_INIT_ROOM* packet = (PACKET_REQ_INIT_ROOM*)data;

	int roomNum = GetRoomNum(sessionID);
	if (roomNum == FAIL_ROOM_SERCH)
		return;

	if (packet->isEndGame)
	{
		std::cout << roomNum << " Room " << packet->gameIndex << " End Game. Winner : "
			<< sessionID << std::endl;
		int addWinRecord = 1;
		DB::GetInstance()->Update(GetSessionName(sessionID), packet->gameIndex, addWinRecord);
	}
	int superSessionID = _roomMG.GetSuperSessonID(roomNum);

	_sessionVec[superSessionID]->InitGameMG();
	_sessionVec[superSessionID]->SetGameMG(nullptr);

	if (packet->gameIndex != GAME_INDEX::EMPTY_GAME)
	{
		int challengerSessionID = _roomMG.GetSessonID(roomNum);
		_sessionVec[challengerSessionID]->InitGameMG();
		_sessionVec[challengerSessionID]->SetGameMG(nullptr);
	}

	InitRoom(roomNum);
}


bool Server::_PostAccept()
{
	if (_sessionDeq.empty())
	{
		_isAccepting = false;
		return false;
	}

	_isAccepting = true;
	int sessionId = _sessionDeq.front();

	_sessionDeq.pop_front();
	_acceptor.async_accept(_sessionVec[sessionId]->Socket(),
		boost::bind(&Server::_AcceptHandle, this,
			_sessionVec[sessionId], boost::asio::placeholders::error)
	);

	return true;
}

void Server::_AcceptHandle(Session* session, const boost::system::error_code& error)
{
	LockGuard acceptLockGuard(_acceptLock);

	if (!error)
	{
		std::cout << "Server : Accept : Entered Client. SessionID : " << session->GetSessionID() << std::endl;

		session->Init();
		session->PostReceive();

		_PostAccept();
	}

	else
	{
		std::cout << "error No : " << error.value() << " error Message : " << error.message()
			<< std::endl;
	}
}