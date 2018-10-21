using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyJSON;

struct NamePacket
{
    public string type;
    public string command;
    public string name;
};

struct ChooseSeatPacket
{
    public string type;
    public string command;
    public int seatID;
}

public class WebService{

    private static WebService instance = new WebService();

    private WebService()
    {
        
    }
    public static WebService getInstance()
    {
        return instance;
    }
    public void setSeatsStatusCallBack(NetworkCallBack callBack)
    {
        NetWorkServices.getInstance().addListenerForSyncMessage("seatStatus", callBack);
    }

    public void setName(string name, NetworkCallBack callBack)
    {
        NetWorkServices netWorkServices = NetWorkServices.getInstance();
        NamePacket packet = new NamePacket();
        packet.name = name;
        packet.type = "CGI";
        packet.command = "setName";
        string jsonStr = JSON.Dump(packet);
        Variant jsonObj = JSON.Load(jsonStr);
        netWorkServices.sendCGIRequest(jsonObj, callBack);
    }

    public void chooseSeat(int seatId, NetworkCallBack callBack)
    {
        NetWorkServices netWorkServices = NetWorkServices.getInstance();
        ChooseSeatPacket packet = new ChooseSeatPacket();
        packet.type = "CGI";
        packet.command = "choose";
        packet.seatID = seatId;
        string jsonStr = JSON.Dump(packet);
        Variant jsonObj = JSON.Load(jsonStr);
        netWorkServices.sendCGIRequest(jsonObj, callBack);
    }
}
