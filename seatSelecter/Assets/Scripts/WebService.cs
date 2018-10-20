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

    private void tryConnect()
    {
        try
        {
            connectionStatus = ConnectionStatus.connecting;
            client.InitClient(address, port, callBack);
            connectionStatus = ConnectionStatus.connected;
        } catch (SocketException e)
        {
            connectionStatus = ConnectionStatus.reconnecting;
            startReconnect();
        }
              
    }

    private void startReconnect()
    {
        if(reconnectionTimer == null)
        {
            reconnectionTimer = new Timer();
        }
        reconnectionTimer.Enabled = true;
        reconnectionTimer.Interval = 1000;
        reconnectionTimer.Elapsed += new System.Timers.ElapsedEventHandler(tryReconnect);
        reconnectionTimer.Start();
    }

    private void tryReconnect(object source, ElapsedEventArgs e)
    {
        if(connectionStatus == ConnectionStatus.connected)
        {
            reconnectionTimer.Stop();
            return;
        }
        tryConnect();
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
        if(connectionStatus != ConnectionStatus.connected)
        {
            return false;
        }
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
