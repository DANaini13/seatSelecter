package com.chillyRoom.logicLayer;
import com.chillyRoom.networkLayer.LongConnectionManager;
import com.chillyRoom.networkLayer.LongConnectionSocket;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Map;

public class CommandDispatcher {

    public LongConnectionSocket caller;

    public void dispatchCommand(JSONObject header, CompletionHandler handler) throws JSONException {
        if(!header.getString("type").equals("CGI")) {
            return;
        }
        JSONObject object = new JSONObject();
        object.put("type", "CGI");
        object.put("command", header.getString("command"));
        LongConnectionManager manager = LongConnectionManager.getServerManager();
        switch (header.getString("command")) {
            case "setName":
                if(SeatServices.checkIfNameExist(header.getString("name"))) {
                    object.put("result", 1);
                    manager.addNameByServer(caller, header.getString("name"));
                    JSONObject broadcastObj = new JSONObject();
                    broadcastObj.put("command", "seatStatus");
                    broadcastObj.put("type", "SYNC");
                    for(Map.Entry<Integer, String> pair:SeatServices.getSeatsStatus().entrySet()){
                        broadcastObj.put("" + pair.getKey(), pair.getValue());
                    }
                    caller.sendPack(broadcastObj.toString());
                }else {
                    object.put("result", 0);
                }
                handler.response(object);
                break;
            case "choose":
                if(SeatServices.chooseSeat(header.getInt("seatID"), manager.getNameByServer(caller))) {
                    object.put("result", "success");
                    JSONObject broadcastObj = new JSONObject();
                    broadcastObj.put("command", "seatStatus");
                    broadcastObj.put("type", "SYNC");
                    for(Map.Entry<Integer, String> pair:SeatServices.getSeatsStatus().entrySet()){
                        broadcastObj.put("" + pair.getKey(), pair.getValue());
                    }
                    manager.sendBroadCast(broadcastObj.toString());
                }else {
                    object.put("result", "座位已经被占用啦!");
                }
                handler.response(object);
                break;
        }
    }

    public void dispatchCommand(JSONObject header) throws JSONException {
        if(header.getString("type").equals("PUSH")) {
            System.out.println(header);
        }
    }
}
