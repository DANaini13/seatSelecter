package com.chillyRoom.dataLayer;

import java.sql.*;

public class SeatsVO {
    private Connection connection = null;
    private String tableName = "seats";

    public SeatsVO() {
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
            statement.execute("CREATE TABLE " + tableName + "(id INTEGER PRIMARY KEY, " +
                    " name VARCHAR(50))");
            for(int i=0; i<32; ++i) {
                statement.execute("INSERT INTO " + tableName + "(id, name) VALUES (" + i + ", 'NULL')");
            }
            statement.close();
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public boolean setSeatName(int id, String name) {
        try {
            Statement statement = connection.createStatement();
            ResultSet rs = statement.executeQuery("SELECT * FROM " + tableName + " WHERE id = " + id);
            if(rs.next()) {
                statement.close();
                return false;
            }
            rs = statement.executeQuery("SELECT * FROM " + tableName + " WHERE name = " + "'" + name +"'");
            if (rs.next()) {
                statement.executeUpdate("UPDATE " + tableName + " SET name = 'NULL' WHERE id = " + rs.getInt("id"));
            }
            statement.executeUpdate("UPDATE " + tableName + " SET name = '"+ name +"' WHERE id = " + id);
            statement.close();
            return true;
        } catch (SQLException e) {
            e.printStackTrace();
            return false;
        }
    }

    public ResultSet getSeatsStatus() {
        ResultSet rs = null;
        try {
            Statement statement = connection.createStatement();
            rs = statement.executeQuery("SELECT * FROM " + tableName);
        } catch (SQLException e) {
            e.printStackTrace();
        }
        return rs;
    }
}
