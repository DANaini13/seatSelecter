using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Timers;


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
    private Timer reconnectionTimer = null;
    private Timer heartBeatTimer = null;
    private bool connectionAlive = false;

    private void tryConnect()
    {
        try
        {
            Debug.Log("================= start connecting to server. ==================");
            connectionStatus = ConnectionStatus.connecting;
            client.InitClient(address, port, onNewMessage);
            connectionStatus = ConnectionStatus.connected;
            Debug.Log("================= connect successfully. ==================");
            connectionAlive = true;
            startCheckHeartBeat();
        } catch (SocketException e)
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
            reconnectionTimer = new Timer();
        }
        reconnectionTimer.Enabled = true;
        reconnectionTimer.Interval = 1000;
        reconnectionTimer.Elapsed += new ElapsedEventHandler(tryReconnect);
        reconnectionTimer.Start();
    }

    private void startCheckHeartBeat()
    {
        if(heartBeatTimer == null)
        {
            heartBeatTimer = new Timer();
        }
        heartBeatTimer.Enabled = true;
        heartBeatTimer.Interval = 10000;
        heartBeatTimer.Elapsed += new ElapsedEventHandler(checkHeartBeat);
        heartBeatTimer.Start();
    }

    private void tryReconnect(object source, ElapsedEventArgs e)
    {
        if(connectionStatus == ConnectionStatus.connected)
        {
            Debug.Log("================= reconnect successfully. ==================");
            reconnectionTimer.Enabled = false;
            reconnectionTimer.Stop();
            return;
        }
        tryConnect();
        Debug.Log("================= reconnecting... ==================");
    }

    private void onNewMessage(string message)
    {
        if (message.StartsWith("0"))
        {
            Debug.Log("================= hear beat ==================");
            connectionAlive = true;
            return;
        }
        callBack(message);
    }

    private void checkHeartBeat(object source, ElapsedEventArgs e)
    {
        if (connectionStatus != ConnectionStatus.connected)
            return;
        if (connectionAlive)
        {
            connectionAlive = false;
            return;
        }
        connectionStatus = ConnectionStatus.reconnecting;
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

    public bool sendMessage(string message)
    {
        if (connectionStatus != ConnectionStatus.connected)
            return false;
        Debug.Log("==================================================send message");
        client.SendMessage(message);
        return true;
    }
}

public class WebService{
    private static WebService instance = new WebService();

    public static WebService getInstance()
    {
        return instance;
    }

    private WebService()
    {
        socketClient = new SocketClient("192.168.0.105", 8888, (string content) => {
        });
    }

    private SocketClient socketClient;

}
