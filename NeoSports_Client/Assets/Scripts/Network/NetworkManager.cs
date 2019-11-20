﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

using System.Text;
using System;

using Newtonsoft.Json;

public struct PACKET_HEADER
{
    public int packetIndex;
    public int packetSize;
}

public struct JsonExample
{
    public PACKET_HEADER header;
    public int Data1;
    public String Data2;
}
public struct TempPacket
{
    public PACKET_HEADER header;
}

public class NetworkManager : Singleton<NetworkManager>
{
    const string IpAdress = "192.168.1.119";
    const string LoopbackAdress = "127.0.0.1";
    const int PortNumber = 31400;

    Socket _sock = null;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

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
        if (_sock.Connected)
        {
            SendToServerPacket();
            ReciveFromSeverPacket();
        }
    }

    void SendToServerPacket()
    {
        //TODO : 패킷을 제이슨으로 직렬화,역직렬화 시키는 함수 작성하기
        {
            var packetHeader = new PACKET_HEADER { packetIndex = 101, packetSize = 10 };
            var jsonPacket = new JsonExample { header = packetHeader, Data1 = 2, Data2 = "Hi" };
            string json;
            json = JsonConvert.SerializeObject(jsonPacket); //객체를 json직렬화
            json += '\0'; //서버에서 널문자까지 읽기 위해 널문자붙이기
            byte[] bufSend = new byte[128]; //전송을 위해 바이트단위로 변환
            bufSend = Encoding.UTF8.GetBytes(json);
            _sock.Send(bufSend);
        }
    }

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
        if (data.header.packetIndex == 101) //JsonExample
        {
            var packetTemp = JsonConvert.DeserializeObject<JsonExample>(recvData);
            Debug.Log(packetTemp.Data1);
            Debug.Log(packetTemp.Data2);
        }

    }

    void ExitProgram()
    {
        Application.Quit();
    }
}