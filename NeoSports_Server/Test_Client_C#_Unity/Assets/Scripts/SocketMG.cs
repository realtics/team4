using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

using System.Text;
using System;

using Newtonsoft.Json;

public class JsonExample
{
    public int Data1;
    public String Data2;
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


        string json = "{'Data1' : 1, 'Data2' : 'Test' }";
        char temp = '\0'; //서버에서 널문자까지 읽기위해 널문자붙이기!
        json += temp;

        byte[] buf = new byte[128];
        buf = Encoding.UTF8.GetBytes(json);
        sock.Send(buf);

        //var data = JsonConvert.DeserializeObject<JsonExample>(json);

        //Debug.Log(data.Data1);
        //Debug.Log(data.Data2);

        //data.Data1 = 10;
        //data.Data2 = "Hi!!";
        //json = JsonConvert.SerializeObject(data);

        //Debug.Log(json);
    }

    // Update is called once per frame
    void Update()
    {

    }
}