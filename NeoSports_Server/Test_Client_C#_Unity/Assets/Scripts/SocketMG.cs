using System.Collections;
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

public class SocketMG : MonoBehaviour
{
    private Socket sock = null;
    // Start is called before the first frame update
    void Start()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        if (sock == null)
        {
            Debug.Log("소켓생성 실패");
        }
        //sock.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.119"), 31400));
        sock.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31400));

        //TODO : 패킷을 제이슨으로 직렬화,역직렬화 시키는 함수 작성하기
        var a = new PACKET_HEADER { packetIndex = 1, packetSize = 10 };
        var p = new JsonExample { header = a, Data1 = 2, Data2 = "Hi" };
        string json;
        json = JsonConvert.SerializeObject(p); //객체를 json직렬화
        char temp = '\0'; //서버에서 널문자까지 읽기 위해 널문자붙이기
        json += temp;
        byte[] bufSend = new byte[128]; //전송을 위해 바이트단위로 변환
        bufSend = Encoding.UTF8.GetBytes(json);
        sock.Send(bufSend);


        byte[] bufRecv = new byte[128]; //수신을 위해 바이트단위로 변환
        int n = sock.Receive(bufRecv);
        Debug.Log("recv");

        string recvData = Encoding.UTF8.GetString(bufRecv, 0, n);

        var data = JsonConvert.DeserializeObject<TempPacket>(recvData);
        if (data.header.packetIndex == 2)
        {
            var packetTemp = JsonConvert.DeserializeObject<JsonExample>(json);
            Debug.Log(packetTemp.Data1);
            Debug.Log(packetTemp.Data2);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}