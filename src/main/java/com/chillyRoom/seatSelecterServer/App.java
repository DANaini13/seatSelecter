package com.chillyRoom.seatSelecterServer;
import com.chillyRoom.dataLayer.MemberVO;
import com.chillyRoom.dataLayer.SeatsVO;
import com.chillyRoom.networkLayer.LongConnectionManager;
import org.json.JSONException;
import org.json.JSONObject;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        LongConnectionManager manager = LongConnectionManager.getServerManager(8888);
    }
}
