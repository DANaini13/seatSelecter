using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using TinyJSON;

public delegate void NetworkCallBack(Variant content);

class NetWorkServices
{
    private static NetWorkServices instance = new NetWorkServices();

    public static NetWorkServices getInstance()
    {
        return instance;
    }

    private NetWorkServices()
    {
        socketClient = new SocketClient("139.162.27.138", 5613, onNewMessage);
        CGIPackages = new LinkedList<Variant>();
        PUSHPackages = new LinkedList<Variant>();
        CGICallbackMap = new Hashtable();
        PUSHCallbackMap = new Hashtable();
        startNewPackageChecker();
    }

    private SocketClient socketClient;

    private void onNewMessage(string content)
    {
        var jsonObj = JSON.Load(content);
        string type = jsonObj["type"];
        if(type.Equals("CGI"))
        {
            lock(CGIPackages)
            {
                CGIPackages.AddFirst(jsonObj);
            }
        }
        else if(type.Equals("SYNC"))
        {
            lock (PUSHPackages)
            {
                Debug.Log("put packet into the PUSH packages");
                PUSHPackages.AddFirst(jsonObj);
            }
        }
        else
        {
            Debug.Log("服务器傻屌");
        }
    }

    private void startNewPackageChecker()
    {
        var timer = new System.Timers.Timer();
        timer.Interval = 100;
        timer.Enabled = true;
        timer.Elapsed += new System.Timers.ElapsedEventHandler(onCheckPackages);
        timer.Start();
    }

    private void onCheckPackages(object source, System.Timers.ElapsedEventArgs e)
    {
        //CGI part
        var package = CGIPackages.First.Value;
        if(package != null)
        {
            string command = package["command"];
            var callBack = (NetworkCallBack) CGICallbackMap[command];
            if(callBack != null)
            {
                callBack(package);
                lock (CGICallbackMap)
                {
                    CGICallbackMap.Remove(command);
                }
            }
            lock(CGIPackages)
            {
                CGIPackages.RemoveFirst();
            }
        }
        // SYNC part
        package = PUSHPackages.First.Value;
        if (package != null)
        {
            string command = package["command"];
            var callBack = (NetworkCallBack)PUSHCallbackMap[command];
            if (callBack != null)
            {
                Debug.Log("found call back for command " + command);
                callBack(package);
            }
            lock (PUSHPackages)
            {
                PUSHPackages.RemoveFirst();
            }
        }
    }

    //各类缓存资源
    private LinkedList<Variant> CGIPackages;
    private LinkedList<Variant> PUSHPackages;
    private Hashtable CGICallbackMap;
    private Hashtable PUSHCallbackMap;

    public void sendCGIRequest(Variant message, NetworkCallBack callBack)
    {
        lock(CGICallbackMap)
        {
            if (CGICallbackMap.Contains((string)message["command"]))
                CGICallbackMap[(string)message["command"]] = callBack;
            else
                CGICallbackMap.Add((string)message["command"], callBack);
        }
        socketClient.sendMessage(message);
    }

    public void addListenerForSyncMessage(string command, NetworkCallBack callBack)
    {
        lock(PUSHCallbackMap)
        {
            if (PUSHCallbackMap.Contains(command)) {
                PUSHCallbackMap[command] = callBack;
            }
            else
                PUSHCallbackMap.Add(command, callBack);
        }
    }

    public void pushMessage(Variant message)
    {
        socketClient.sendMessage(message);
    }
}
