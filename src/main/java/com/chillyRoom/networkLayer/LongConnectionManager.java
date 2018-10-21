package com.chillyRoom.networkLayer;
import com.chillyRoom.logicLayer.SeatServices;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class LongConnectionManager extends Thread {
    private static LongConnectionManager serverManager = null;

    public static synchronized LongConnectionManager getServerManager(int port) {
        if(serverManager == null) {
            serverManager = new LongConnectionManager(port);
            serverManager.connectionMap = new HashMap<>();
            serverManager.lock = new ReentrantLock();
            serverManager.start();
        }
        return serverManager;
    }

    public static synchronized LongConnectionManager getServerManager() {
        return serverManager;
    }

    private final int port;
    private Integer serverID = 0;

    LongConnectionManager(int port) {
        this.port = port;
    }

    public void run() {
        try {
            ServerSocket serverSocket = new ServerSocket(port);
            while (true) {
                Socket server = serverSocket.accept();
                LongConnectionSocket newLongConnectionSocket = LongConnectionSocket.create(server, serverID);
                lock.lock();
                connectionMap.put(newLongConnectionSocket, "NULL");
                lock.unlock();
                ++serverID;
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private HashMap<LongConnectionSocket, String> connectionMap;
    private Lock lock;

    public void sendBroadCast(String packet) {
        for(Map.Entry<LongConnectionSocket, String> pair:connectionMap.entrySet()){
            pair.getKey().sendPack(packet);
        }
    }

    public void removeServerIfExist(LongConnectionSocket socket) {
        if(connectionMap.containsKey(socket)) {
            lock.lock();
            connectionMap.remove(socket);
            lock.unlock();
        }
    }

    public String getNameByServer(LongConnectionSocket socket) {
        return connectionMap.get(socket);
    }

    public void addNameByServer(LongConnectionSocket socket, String name) {
        lock.lock();
        connectionMap.put(socket, name);
        lock.unlock();
    }
}
