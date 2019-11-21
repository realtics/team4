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
    public Byte[] buffer;
    public Socket workingSocket;
    public AsyncObject(Int32 bufferSize)
    {
        this.buffer = new Byte[bufferSize];
    }
}

public enum PACKET_INDEX
{
    REQ_IN = 1,
};

public struct PACKET_REQ_IN
{
    public PACKET_HEADER header;
    public string name;
};

public enum EPacketRoomIndex
{
    RopePullGame,
    RopeJumpGame,
    BasketBallGame,
};
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

public struct TempPacket
{
    public PACKET_HEADER header;
};

public class NetworkManager : Singleton<NetworkManager>
{
    const string IpAdress = "192.168.1.119";
    const string LoopbackAdress = "127.0.0.1";
    const int PortNumber = 31400;

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

    public void SendRequsetRoom(EPacketRoomIndex roomIndex)
    {
        if (_sock == null)
            Connect();

        PACKET_HEADER headerPacket = MakeHeaderPacket(PACKET_INDEX.REQ_IN);
        PACKET_REQ_IN packet = new PACKET_REQ_IN { header = headerPacket, name = roomIndex.ToString() };

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
        int packetSize = Marshal.SizeOf<PACKET_REQ_IN>();
        PACKET_HEADER headerPacket;
        headerPacket = new PACKET_HEADER(packetIndex, packetSize);
        return headerPacket;
    }

    //To Do :클라이언트 Receive 비동기처리 
    void ReciveFromSeverPacket()
    {
        byte[] bufRecv = new byte[128]; //수신을 위해 바이트단위로 변환
        int n = _sock.Receive(bufRecv);
        Debug.Log("recv");
        Debug.Log(n);

        //string recvData = Encoding.UTF8.GetString(bufRecv, 0, n);
        //int bufLen = Encoding.Default.GetBytes(bufRecv);
        int bufLen = bufRecv.Length;
        string recvData = Encoding.UTF8.GetString(bufRecv, 0, n);
        Debug.Log(recvData);

        var data = JsonConvert.DeserializeObject<TempPacket>(recvData);
        //if (data.header.packetIndex == 101) //JsonExample
        //{
        //    var packetTemp = JsonConvert.DeserializeObject<JsonExample>(recvData);
        //    Debug.Log(packetTemp.Data1);
        //    Debug.Log(packetTemp.Data2);
        //}

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
            _sock.Connect(new IPEndPoint(IPAddress.Parse(LoopbackAdress), PortNumber));
            //_sock.Connect(new IPEndPoint(IPAddress.Parse(IpAdress), PortNumber));

            //Async 추가 작성 
            {
                _receiveHandler = new AsyncCallback(HandleDataRecive);
                _sock.NoDelay = true; //nagle off
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
        Debug.Log("AsyncReceive");
        AsyncObject ao = (AsyncObject)ar.AsyncState;
        Int32 recvBytes = ao.workingSocket.EndReceive(ar);
        Debug.Log( "receiveByte size:"+recvBytes);

        if (recvBytes > 0)
        {
            //receive처리 
            string recvData = Encoding.UTF8.GetString(ao.buffer, 0, recvBytes);
            
            //To Do: Json 처리
            //var data = JsonConvert.DeserializeObject<TempPacket>(recvData);
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