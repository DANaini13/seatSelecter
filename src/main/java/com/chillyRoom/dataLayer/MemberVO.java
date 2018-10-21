package com.chillyRoom.dataLayer;

import java.sql.*;

public class MemberVO {
    private Connection connection = null;
    private String tableName = "names";

    public MemberVO() {
        try {
            Class.forName("org.sqlite.JDBC");
            connection = DriverManager.getConnection("jdbc:sqlite:" + "data.db");
            Statement statement = connection.createStatement();
            ResultSet rs = statement.executeQuery("SELECT * FROM sqlite_master" +
                                                        " WHERE type = 'table'" +
                                                        " AND name = \"" + tableName + "\"");

            if(rs.next()) {
                statement.close();
                return;
            }
            statement.execute("CREATE TABLE " + tableName + "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                " name VARCHAR(50))");
            statement.close();
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public boolean checkIfNameInTable(String name) {
        try {
            Statement statement = connection.createStatement();
            ResultSet rs = statement.executeQuery("SELECT * FROM " + tableName + " WHERE name = \"" + name + "\"");
            System.out.println("SELECT * FROM " + tableName + " WHERE name = \"" + name + "\"");
            if(rs.next()) {
                statement.close();
                return true;
            }
            statement.close();
            return false;
        } catch (SQLException e) {
            return false;
        }
    }
}
