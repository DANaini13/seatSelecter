using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TinyJSON;


class SocketClient
{
    private enum ConnectionStatus
    {
        connectionFailed = 0,
        connected = 1,
        reconnecting = 2,
        connecting = 3
    }

    private TcpClient client = null;
    private ReceiveCallBack callBack = null;
    private string address = null;
    private int port = 0;
    private ConnectionStatus connectionStatus;
    private System.Timers.Timer reconnectionTimer = null;
    private System.Timers.Timer heartBeatTimer = null;
    private System.Timers.Timer recieveTimer = null;
    private bool connectionAlive = false;

    private bool tryConnect()
    {
        try
        {
            Debug.Log("================= start connecting to server. ==================");
            client = new TcpClient();
            client.Connect(IPAddress.Parse(address), port);
            connectionStatus = ConnectionStatus.connected;
            Debug.Log("================= connect successfully. ==================");
            connectionAlive = true;
            return true;
        }
        catch (SocketException e)
        {
            return false;
        }

    }

    private void startReconnect()
    {
        connectionStatus = ConnectionStatus.reconnecting;
        if (reconnectionTimer == null)
        {
            reconnectionTimer = new System.Timers.Timer();
        }
        reconnectionTimer.Enabled = true;
        reconnectionTimer.Interval = 5000;
        reconnectionTimer.Elapsed += new System.Timers.ElapsedEventHandler(tryReconnect);
        reconnectionTimer.Start();
    }

    private void startCheckHeartBeat()
    {
        if (heartBeatTimer == null)
        {
            heartBeatTimer = new System.Timers.Timer();
        }
        heartBeatTimer.Enabled = true;
        heartBeatTimer.Interval = 5000;
        heartBeatTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkHeartBeat);
        heartBeatTimer.Start();
    }

    private void tryReconnect(object source, System.Timers.ElapsedEventArgs e)
    {
        if (connectionStatus == ConnectionStatus.connected)
        {
            Debug.Log("================= reconnect successfully. ==================");
            reconnectionTimer.Enabled = false;
            reconnectionTimer.Stop();
            return;
        }
        connectionStatus = ConnectionStatus.reconnecting;
        if(!tryConnect()) {
            Debug.Log("================= reconnecting... ==================");
        }else {
            startCheckHeartBeat();
            startReceiveMessage();
        }
    }

    private void checkHeartBeat(object source, System.Timers.ElapsedEventArgs e)
    {
        if (connectionStatus != ConnectionStatus.connected)
            return;
        if (connectionAlive)
        {
            connectionAlive = false;
            return;
        }
        startReconnect();
        heartBeatTimer.Enabled = false;
        heartBeatTimer.Stop();
    }

    public SocketClient(string address, int port, ReceiveCallBack callBack)
    {
        this.callBack = callBack;
        this.address = address;
        this.port = port;
        connectionStatus = ConnectionStatus.connecting;
        if(tryConnect()) {
            startCheckHeartBeat();
            startReceiveMessage();
        }else {
            startReconnect();
        }
    }

    private void startReceiveMessage()
    {
        if (recieveTimer == null)
        {
            recieveTimer = new System.Timers.Timer();
        }
        recieveTimer.Enabled = true;
        recieveTimer.Interval = 100;
        recieveTimer.Elapsed += new System.Timers.ElapsedEventHandler(receiveMessage);
        recieveTimer.Start();
    }

    private void receiveMessage(object source, System.Timers.ElapsedEventArgs e)
    {
        if (connectionStatus != ConnectionStatus.connected)
            return;
        byte[] buffer = new byte[2048];
        int byteRead = client.GetStream().Read(buffer, 0, 2048);
        if (byteRead == 0) {
            
        }else {
            string msg = Encoding.UTF8.GetString(buffer, 0, byteRead);
            onMessages(msg);
        }
    }

    public bool sendMessage(Variant message)
    {
        if (connectionStatus != ConnectionStatus.connected)
            return false;
        byte[] buffer = Encoding.UTF8.GetBytes(JSON.Dump(message));
        client.GetStream().Write(buffer, 0, buffer.Length);
        return true;
    }

    private void onMessages(string messages)
    {
        string result = "";
        foreach(char c in messages)
        {
            if (c == '#')
            {
                onNewMessage(result);
                result = "";
                continue;
            }
            result = result + c;
        }
    }

    private void onNewMessage(string message)
    {
        if (message == "heart")
        {
            connectionAlive = true;
            return;
        }
        callBack(message);
    }
}
