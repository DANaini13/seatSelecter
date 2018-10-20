using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

//收到消息后的委托回调
public delegate void ReceiveCallBack(string content);

public class LOSocket
{

    //可以创建的Socket端口类型
    public enum LOSocketType
    {
        CLIENT = 0,
        SERVER = 1,
    }
    #region --------取消构造器
    private LOSocket()
    {
    }

    #endregion

    #region --------公共代码
    //通过静态方法获取不同的端口类型
    public static LOSocket GetSocket(LOSocket.LOSocketType type)
    {
        LOSocket socket = null;


        switch (type)
        {
            case LOSocketType.CLIENT:
                {
                    //创建一个新的客户端
                    socket = new LOSocket();
                    break;
                }
            case LOSocketType.SERVER:
                {
                    //获取服务端
                    socket = GetServer();
                    break;
                }
        }

        return socket;
    }

    #endregion
    #region --------客户端部分代码
    private Socket clientSocket;

    //声明客户端收到服务端返回消息后的回调委托函数
    private ReceiveCallBack clientReceiveCallBack;
    //用来存储服务端返回的消息数据
    byte[] Buffer = new byte[1024];

    //初始化客户端Socket信息
    public void InitClient(string ip, int port, ReceiveCallBack ccb)
    {
        this.clientReceiveCallBack = ccb;
        this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress address = IPAddress.Parse(ip);
        IPEndPoint ep = new IPEndPoint(address, port);
        try
        {
            this.clientSocket.Connect(ep);
        }catch (SocketException e)
        {
            throw e;
        }
        
        //开始异步等待接收服务端消息
        this.clientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new System.AsyncCallback(ReceiveFromServer), this.clientSocket);
    }

    //收到服务端返回消息后的回调函数
    void ReceiveFromServer(System.IAsyncResult ar)
    {
        //获取当前正在工作的Socket对象
        Socket worker = ar.AsyncState as Socket;
        int ByteRead = 0;
        try
        {
            //接收完毕消息后的字节数
            ByteRead = worker.EndReceive(ar);
        }
        catch (System.Exception ex)
        {
            this.clientReceiveCallBack(ex.ToString());
        }
        if (ByteRead > 0)
        {
            string Content = Encoding.Default.GetString(Buffer);
            //通过回调函数将消息返回给调用者
            this.clientReceiveCallBack(Content);
        }
        //继续异步等待接受服务器的返回消息
        worker.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new System.AsyncCallback(ReceiveFromServer), worker);
    }

    //客户端主动发送消息
    public void SendMessage(string message)
    {
        if (message == null)
            return;

        message += "\r\n";
        byte[] sendData = Encoding.UTF8.GetBytes(message);

        //异步发送消息请求
        clientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new System.AsyncCallback(SendToServer), clientSocket);
    }
    //发送消息结束的回调函数
    void SendToServer(System.IAsyncResult ar)
    {
        Socket worker = ar.AsyncState as Socket;
        worker.EndSend(ar);
    }

    #endregion


    #region -------服务器部分代码
    //服务器端收到消息的存储空间
    byte[] ReceiveBuffer = new byte[1024];
    //服务器收到消息后的回调委托
    private ReceiveCallBack callback;

    //单例模式  
    private static LOSocket serverSocket;
    private static LOSocket GetServer()
    {
        if (serverSocket == null)
        {
            serverSocket = new LOSocket();
        }
        return serverSocket;
    }

    //初始化服务器信息
    public void InitServer(ReceiveCallBack cb)
    {
        this.callback = cb;
        // 1.
        Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 2.
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 2222);
        // 3.
        server_socket.Bind(endPoint);
        // 4.
        server_socket.Listen(10);
        // 5.开始异步等待客户端的请求链接
        server_socket.BeginAccept(new System.AsyncCallback(Accept), server_socket);

        this.callback("开启服务器" + endPoint.ToString());
    }

    //接受到客户端的链接请求后的回调函数
    void Accept(System.IAsyncResult ar)
    {
        //获取正在工作的Socket对象 
        Socket socket = ar.AsyncState as Socket;
        //存储异步操作的信息，以及用户自定义的数据  
        Socket worker = socket.EndAccept(ar);

        SocketError error;

        //开始异步接收客户端发送消息内容
        worker.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, new System.AsyncCallback(Receive), worker);
        //继续异步等待新的客户端链接请求
        socket.BeginAccept(new System.AsyncCallback(Accept), socket);

    }
    //服务端收到客户端的消息后的回调函数
    void Receive(System.IAsyncResult ar)
    {
        //获取正在工作的Socket对象
        Socket worker = ar.AsyncState as Socket;
        int ByteRead = 0;
        try
        {
            ByteRead = worker.EndReceive(ar);
        }
        catch (System.Exception ex)
        {
            this.callback(ex.ToString());
        }
        if (ByteRead > 0)
        {
            string Content = Encoding.Default.GetString(ReceiveBuffer);
            this.callback(Content);
        }
        //继续异步等待客户端的发送消息请求
        worker.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, new System.AsyncCallback(Receive), worker);
    }
    #endregion
}