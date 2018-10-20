package com.chillyRoom.networkLayer;
import com.chillyRoom.logicLayer.CommandDispatcher;
import org.json.JSONException;
import org.json.JSONObject;
import java.io.*;
import java.net.Socket;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class LongConnectionSocket extends Thread {
    private final Socket server;
    private Lock lock = new ReentrantLock();

    static public LongConnectionSocket create(Socket server, int id) {
        LongConnectionSocket newLongConnectionSocket = new LongConnectionSocket(server);
        newLongConnectionSocket.start();
        return newLongConnectionSocket;
    }

    private LongConnectionSocket(Socket server) {
        this.server = server;
    }

    private void startSendHeartBeat() {
        Timer timer = new Timer();
        timer.schedule(new TimerTask() {
            @Override
            public void run() {
                sendPack("0");
            }
        }, 0, 5*1000);
    }

    private void setUpNewMessageListener() {
        try {
            DataInputStream in = new DataInputStream(server.getInputStream());
            Timer timer = new Timer();
            timer.schedule(new TimerTask() {
                @Override
                public void run() {
                    try {
                        BufferedReader reader = new BufferedReader(new InputStreamReader(in));
                        String buffer = null;
                        buffer = reader.readLine();
                        if(buffer == null)
                            return;
                        JSONObject header = null;
                        header = new JSONObject(buffer);
                        CommandDispatcher commandDispatcher = new CommandDispatcher();
                        commandDispatcher.dispatchCommand(header, response -> {
                            JSONObject jsonObject = new JSONObject();
                            sendPack(response.toString());
                        });
                        System.out.println(buffer);
                    } catch (JSONException e) {
                       // e.printStackTrace();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }

                }
            }, 0, 100);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void run() {
        startSendHeartBeat();
        setUpNewMessageListener();
    }

    void sendPack(String pack) {
        try {
            lock.lock();
            PrintWriter writer = new PrintWriter(server.getOutputStream());
            writer.print(pack);
            writer.flush();
            lock.unlock();
        } catch (IOException e) {
            return;
        }
    }

}
