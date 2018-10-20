package com.chillyRoom.seatSelecterServer;
import com.chillyRoom.networkLayer.LongConnectionManager;
import org.json.JSONException;
import org.json.JSONObject;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
//        System.out.println( "Hello World!" );
//        JSONObject obj = new JSONObject();
//        try {
//            obj.append("count", 32);
//            for(int i=0; i<32; ++i) {
//                obj.append("" + i, i%3==0);
//            }
//        } catch (JSONException e) {
//            e.printStackTrace();
//        }
//        System.out.println(obj.toString());
        LongConnectionManager manager = LongConnectionManager.getServerManager(8888);
    }
}
