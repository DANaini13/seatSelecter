package com.chillyRoom.logicLayer;

import com.chillyRoom.dataLayer.MemberVO;
import com.chillyRoom.dataLayer.SeatsVO;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;

public class SeatServices {
    static boolean checkIfNameExist(String name) {
        MemberVO vo = new MemberVO();
        return vo.checkIfNameInTable(name);
    }


    static boolean chooseSeat(int id, String name) {
        SeatsVO vo = new SeatsVO();
        return vo.setSeatName(id, name);
    }

    static HashMap<Integer, String> getSeatsStatus() {
        SeatsVO vo = new SeatsVO();
        HashMap<Integer, String> result = new HashMap<>();
        ResultSet rs = vo.getSeatsStatus();
        try {
            while (rs.next()) {
                result.put(rs.getInt("id"), rs.getString("name"));
            }
        }catch (SQLException e) {
            return result;
        }
        return result;
    }
}
