using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
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

    private LOSocket client = null;
    private ReceiveCallBack callBack = null;
    private string address = null;
    private int port = 0;
    private ConnectionStatus connectionStatus;
    private System.Timers.Timer reconnectionTimer = null;
    private System.Timers.Timer heartBeatTimer = null;
    private bool connectionAlive = false;

    private void tryConnect()
    {
        try
        {
            Debug.Log("================= start connecting to server. ==================");
            connectionStatus = ConnectionStatus.connecting;
            client.InitClient(address, port, onMessages);
            connectionStatus = ConnectionStatus.connected;
            Debug.Log("================= connect successfully. ==================");
            connectionAlive = true;
        //    startCheckHeartBeat();
        }
        catch (SocketException e)
        {
            Debug.Log("================= connecting failed, start reconnecting. ==================");
            startReconnect();
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
        tryConnect();
        Debug.Log("================= reconnecting... ==================");
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
        client = LOSocket.GetSocket(LOSocket.LOSocketType.CLIENT);
        this.callBack = callBack;
        this.address = address;
        this.port = port;
        tryConnect();
    }

    public bool sendMessage(Variant message)
    {
        if (connectionStatus != ConnectionStatus.connected)
            return false;
        client.SendMessage(JSON.Dump(message));
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
        if (message.StartsWith("heartBeat"))
        {
            connectionAlive = true;
            return;
        }
        Debug.Log(message);
        callBack(message);
    }
}
