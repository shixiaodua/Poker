using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }
    /// <summary>
    /// 公匙
    /// </summary>
    public string PublicKey = "OceanSever";
    /// <summary>
    /// 私匙
    /// </summary>
    public string SecretKey { get; private set; }
    /// <summary>
    /// 客户端socket
    /// </summary>
    private Socket m_Socket;
    /// <summary>
    /// 读取字节数组
    /// </summary>
    private ByteArray m_ReadBuff;
    /// <summary>
    /// 服务器ip
    /// </summary>
    private string m_Ip = "192.168.1.63";
    /// <summary>
    /// 服务器监听端口
    /// </summary>
    private int m_Port = 8011;

    //链接状态
    /// <summary>
    /// 是否正在连接
    /// </summary>
    private bool m_Connecting = false;
    /// <summary>
    /// 是否正在关闭，要等待消息发送完毕后关闭
    /// </summary>
    private bool m_Closing = false;
    /// <summary>
    /// 线程，处理接受消息
    /// </summary>
    private Thread m_MsgThread;
    private Thread m_HeartThread;
    /// <summary>
    /// 上一次发送心跳包的时间
    /// </summary>
    static long lastPingTime;
    /// <summary>
    /// 上一次收到心跳包回应的时间
    /// </summary>
    static long lastPongTime;
    /// <summary>
    /// 等待发送的消息队列
    /// </summary>
    private Queue<ByteArray> m_WriteQueue;
    /// <summary>
    /// 存储接受的消息
    /// </summary>
    private List<MsgBase> m_MsgList;
    /// <summary>
    /// 存储需要发送给unity处理的消息（非心跳消息），后续会进行分发处理（委托）
    /// </summary>
    private List<MsgBase> m_UnityMsgList;
    /// <summary>
    /// 待处理消息数
    /// </summary>
    private int m_MsgCount = 0;
    /// <summary>
    /// 心跳包间隔时间
    /// </summary>
    public static long m_PingInterval = 30;

    public delegate void EventListener(string str);
    /// <summary>
    /// 不同的连接状态，对应不同的委托对象（绑定方法），为不同连接状态提供不同的处理方法
    /// </summary>
    private Dictionary<NetEvent, EventListener> m_ListenerDic = new Dictionary<NetEvent, EventListener>();
    public delegate void ProtoListener(MsgBase msg);
    /// <summary>
    /// 将协议与方法绑定，后续会根据接收到的协议执行对应的方法
    /// </summary>
    private Dictionary<ProtocolEnum, ProtoListener> m_ProtoDic = new Dictionary<ProtocolEnum, ProtoListener>();
    /// <summary>
    /// 是否掉线（关闭连接），只会在客户端执行关闭方法后为true
    /// </summary>
    private bool m_Diaoxian = false;
    /// <summary>
    /// 是否链接成功过（只要链接成功过就是true，再也不会变成false）
    /// </summary>
    private bool m_IsConnectSuccessed = false;
    private bool m_ReConnect = false;
    /// <summary>
    /// 设备当前的网络连接状态，初始化为未连接网络
    /// </summary>
    private NetworkReachability m_CurNetWork = NetworkReachability.NotReachable;
    /// <summary>
    /// 每隔一秒检查当前网络状态的协程
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckNet()
    {
        m_CurNetWork = Application.internetReachability;//获取当前网络状态
        while (true)
        {
            yield return new WaitForSeconds(1);//等待一秒
            if (m_IsConnectSuccessed)//判断是否连接成功过
            {
                if (m_CurNetWork != Application.internetReachability)//判断当前网络状态是否发生改变，若是改变则尝试重新连接
                {
                    ReConnect();
                    m_CurNetWork = Application.internetReachability;
                }
            }
        }
    }
    /// <summary>
    /// 为不同的连接状态添加对应的委托对象
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="listener"></param>
    public void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        if (m_ListenerDic.ContainsKey(netEvent))
        {
            m_ListenerDic[netEvent] += listener;
        }
        else
        {
            m_ListenerDic[netEvent] = listener;
        }
    }
    /// <summary>
    /// 为不同的连接状态移除指定的委托
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="listener"></param>
    public void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (m_ListenerDic.ContainsKey(netEvent))
        {
            m_ListenerDic[netEvent] -= listener;
            if (m_ListenerDic[netEvent] == null)
            {
                m_ListenerDic.Remove(netEvent);
            }
        }
    }
    /// <summary>
    /// 执行指定的连接状态对应的委托
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="str"></param>
    void FirstEvent(NetEvent netEvent, string str)
    {
        if (m_ListenerDic.ContainsKey(netEvent))
        {
            m_ListenerDic[netEvent](str);
        }
    }

    /// <summary>
    /// 为不同的协议添加对应的委托对象
    /// </summary>
    /// <param name="protocolEnum"></param>
    /// <param name="listener"></param>
    public void AddProtoListener(ProtocolEnum protocolEnum, ProtoListener listener)
    {
        m_ProtoDic[protocolEnum] = listener;
    }
    /// <summary>
    /// 执行指定的协议对应的委托
    /// </summary>
    /// <param name="protocolEnum"></param>
    /// <param name="msgBase"></param>
    public void FirstProto(ProtocolEnum protocolEnum, MsgBase msgBase)
    {
        if (m_ProtoDic.ContainsKey(protocolEnum))
        {
            m_ProtoDic[protocolEnum](msgBase);
        }
    }
    public void Update()
    {
        if (m_Diaoxian && m_IsConnectSuccessed)
        {
            //弹框，确定是否重连
            //重新链接
            ReConnect();
            //退出游戏
            m_Diaoxian = false;
        }

        //断开链接后，链接服务器之后自动登录
        if (!string.IsNullOrEmpty(SecretKey) && m_Socket.Connected && m_ReConnect)//判断如果之前登录过且现在链接成功且执行了重连方法就自动登录
        {
            //在本地保存了我们的账户和token，然后进行判断有无账户和token，

            //使用token登录
            //ProtocolMgr.Login( LoginType.Token, "username", "token",(res, restoken)=> 
            //{
            //    if (res == LoginResult.Success)
            //    {

            //    }
            //    else 
            //    {

            //    }
            //});
            m_ReConnect = false;
        }

        MsgUpdate();
    }
    /// <summary>
    /// 每一帧去执行该方法，执行消息队列中的第一条消息的委托
    /// </summary>
    void MsgUpdate()
    {
        if (m_Socket != null && m_Socket.Connected)//若是当前处在连接状态则去执行消息对应的委托
        {
            if (m_MsgCount == 0) return;
            MsgBase msgBase = null;
            lock (m_UnityMsgList)
            {
                if (m_UnityMsgList.Count > 0)
                {
                    msgBase = m_UnityMsgList[0];
                    m_UnityMsgList.RemoveAt(0);
                    m_MsgCount--;
                }
            }
            if (msgBase != null)
            {
                FirstProto(msgBase.ProtoType, msgBase);
            }
        }
    }
    /// <summary>
    /// 消息处理线程，将心跳消息与其他消息区分
    /// </summary>
    void MsgThread()
    {
        while (m_Socket != null && m_Socket.Connected)
        {
            if (m_MsgList.Count <= 0) continue;

            MsgBase msgBase = null;
            lock (m_MsgList)
            {
                if (m_MsgList.Count > 0)
                {
                    msgBase = m_MsgList[0];
                    m_MsgList.RemoveAt(0);
                }
            }

            if (msgBase != null)
            {
                if (msgBase is MsgPing)
                {
                    lastPongTime = GetTimeStamp();
                    Debug.Log("收到心跳包！！！！！！！");
                    m_MsgCount--;
                }
                else
                {
                    lock (m_UnityMsgList)
                    {
                        m_UnityMsgList.Add(msgBase);
                    }
                }
            }
            else
            {
                break;
            }
        }
    }
    /// <summary>
    /// 心跳包线程，每隔指定时间发送一次心跳包，检测是否断开连接（过长时间未收到心跳）
    /// </summary>
    void PingThread()
    {
        while (m_Socket != null && m_Socket.Connected)
        {
            long timeNow = GetTimeStamp();
            if (timeNow - lastPingTime > m_PingInterval)
            {
                MsgPing msgPing = new MsgPing();
                SendMessage(msgPing);
                lastPingTime = GetTimeStamp();
            }

            //如果心跳包过长时间没收到，就关闭连接
            if (timeNow - lastPongTime > m_PingInterval * 4)
            {
                Close(false);
            }
        }
    }

    /// <summary>
    /// 重连方法
    /// </summary>
    public void ReConnect()
    {
        Connect();
        m_ReConnect = true;
    }

    /// <summary>
    /// 链接服务器函数
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Connect()
    {
        if (m_Socket != null && m_Socket.Connected)
        {
            Debug.LogError("链接失败，已经链接了！");
            return;
        }

        if (m_Connecting)
        {
            Debug.LogError("链接失败，正在链接中！");
            return;
        }
        // 结束之前的线程（确保不会有重复线程）
        if (m_MsgThread != null && m_MsgThread.IsAlive)
        {
            // 停止消息处理线程
            m_MsgThread.Abort(); // 或者使用更优雅的停止方式
        }
        if (m_HeartThread != null && m_HeartThread.IsAlive)
        {
            // 停止心跳线程
            m_HeartThread.Abort(); // 或者使用更优雅的停止方式
        }
        //保证当前一定处于未连接状态才会执行以下代码
        InitState();
        m_Socket.NoDelay = true;
        m_Connecting = true;
        m_Socket.BeginConnect(m_Ip, m_Port, ConnectCallback, m_Socket);
    }

    /// <summary>
    /// 初始化状态
    /// </summary>
    void InitState()
    {
        //初始化变量
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_ReadBuff = new ByteArray();
        m_WriteQueue = new Queue<ByteArray>();
        m_Connecting = false;
        m_Closing = false;
        m_MsgList = new List<MsgBase>();
        m_UnityMsgList = new List<MsgBase>();
        m_MsgCount = 0;
        lastPingTime = GetTimeStamp();
        lastPongTime = GetTimeStamp();
    }

    /// <summary>
    /// 连接到服务器之后的回调函数
    /// </summary>
    /// <param name="ar"></param>
    void ConnectCallback(IAsyncResult ar)//参数为异步操作的结果
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;//从结果获取socket对象
            socket.EndConnect(ar);//结束异步连接
            FirstEvent(NetEvent.ConnectSucc, "");//执行连接成功对应的委托
            m_MsgThread = new Thread(MsgThread);
            m_MsgThread.IsBackground = true;//将线程设置为后台线程，当主线程结束，后台线程也会结束，若是前台线程则不会结束
            m_MsgThread.Start();//开始执行该线程绑定的方法
            m_Connecting = false;
            m_HeartThread = new Thread(PingThread);
            m_HeartThread.IsBackground = true;
            m_HeartThread.Start();
            if (!m_IsConnectSuccessed)//第一次连接成功执行的逻辑
            { // 在回调中使用主线程调度
                m_IsConnectSuccessed = true;
                GameManager.Instance.ConnectSuccess();//连接成功后执行
            }
            Debug.Log("Socket Connect Success");
            m_Socket.BeginReceive(m_ReadBuff.Bytes, m_ReadBuff.WriteIdx, m_ReadBuff.Remain, 0, ReceiveCallBack, socket);//开始异步接受消息，并且在回调函数中继续接受后续数据包
        }
        catch (SocketException ex)
        {
            Debug.LogError("Socket Connect fail:" + ex.ToString());
            m_Connecting = false;
        }
    }


    /// <summary>
    /// 接受数据回调
    /// </summary>
    /// <param name="ar"></param>
    void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            if (count <= 0)
            {
                Close();
                //关闭链接
                return;
            }

            m_ReadBuff.WriteIdx += count;
            OnReceiveData();
            if (m_ReadBuff.Remain < 8)
            {
                m_ReadBuff.MoveBytes();
                m_ReadBuff.ReSize(m_ReadBuff.Length * 2);//扩容，防止一个数据包过大
            }
            socket.BeginReceive(m_ReadBuff.Bytes, m_ReadBuff.WriteIdx, m_ReadBuff.Remain, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError("Socket ReceiveCallBack fail:" + ex.ToString());
            Close();
        }
    }

    /// <summary>
    /// 对数据进行处理
    /// </summary>
    void OnReceiveData()
    {
        if (m_ReadBuff.Length <= 4 || m_ReadBuff.ReadIdx < 0)//判断是否存在至少四个字节的数据，一个int代表消息内容的长度
            return;

        int readIdx = m_ReadBuff.ReadIdx;
        byte[] bytes = m_ReadBuff.Bytes;
        int bodyLength = BitConverter.ToInt32(bytes, readIdx);//将消息内容长度的字节转换为int数字
        //判断消息长度是否小于完整消息的长度（长度字节+消息内容字节）
        if (m_ReadBuff.Length < bodyLength + 4)
        {
            return;
        }
        //消息长度正确，解析消息
        m_ReadBuff.ReadIdx += 4;//跳过长度字节
        int nameCount = 0;
        ProtocolEnum protocol = MsgBase.DecodeName(m_ReadBuff.Bytes, m_ReadBuff.ReadIdx, out nameCount);//解析协议名
        if (protocol == ProtocolEnum.None)//判断协议名是否正确
        {
            Debug.LogError("OnReceiveData MsgBase.DecodeName fail");
            Close();
            return;
        }
        m_ReadBuff.ReadIdx += nameCount;//跳过协议名
        int bodyCount = bodyLength - nameCount;//获取协议内容字节长度（总长-协议名长）
        try
        {
            MsgBase msgBase = MsgBase.Decode(protocol, m_ReadBuff.Bytes, m_ReadBuff.ReadIdx, bodyCount);//调用解析方法
            if (msgBase == null)
            {
                Debug.LogError("接受数据协议内容解析出错");
                Close();
                return;
            }
            //解析成功
            m_ReadBuff.ReadIdx += bodyCount;//跳过协议内容字节
            m_ReadBuff.CheckAndMoveBytes();//移动数据到头部
            //将协议添加到消息队列中
            lock (m_MsgList)
            {
                m_MsgList.Add(msgBase);
            }
            m_MsgCount++;
            //处理粘包，接收了多个数据包，继续处理数据
            if (m_ReadBuff.Length > 4)
            {
                OnReceiveData();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Socket OnReceiveData error:" + ex.ToString());
            Close();
        }
    }

    /// <summary>
    /// 发送数据到服务器
    /// </summary>
    /// <param name="msgBase"></param>
    public void SendMessage(MsgBase msgBase)
    {
        if (m_Socket == null || !m_Socket.Connected)
        {
            return;
        }
        if (m_Connecting)
        {
            Debug.LogError("正在链接服务器中，无法发送消息！");
            return;
        }

        if (m_Closing)
        {
            Debug.LogError("正在关闭链接中，无法发送消息!");
            return;
        }

        try
        {
            byte[] nameBytes = MsgBase.EncodeName(msgBase);
            byte[] bodyBytes = MsgBase.Encond(msgBase);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] byteHead = BitConverter.GetBytes(len);
            byte[] sendBytes = new byte[byteHead.Length + len];
            Array.Copy(byteHead, 0, sendBytes, 0, byteHead.Length);
            Array.Copy(nameBytes, 0, sendBytes, byteHead.Length, nameBytes.Length);
            Array.Copy(bodyBytes, 0, sendBytes, byteHead.Length + nameBytes.Length, bodyBytes.Length);
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;
            lock (m_WriteQueue)
            {
                m_WriteQueue.Enqueue(ba);
                count = m_WriteQueue.Count;
            }

            if (count == 1) //保证必须等待一个消息发送完毕后才能发送下一条消息
            {
                m_Socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, m_Socket);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("SendMessage error:" + ex.ToString());
            Close();
        }
    }

    /// <summary>
    /// 发送结束回调
    /// </summary>
    /// <param name="ar"></param>
    void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket == null || !socket.Connected) return;
            int count = socket.EndSend(ar);//获取完成发送时实际发送的字节数
            //判断是否发送完成
            ByteArray ba;
            lock (m_WriteQueue)
            {
                ba = m_WriteQueue.First();//获取队列中的第一条数据，也就是这次发送的数据
            }
            ba.ReadIdx += count;//更新读取索引，移动发送的字节数的位置
            //代表发送完整
            if (ba.Length == 0)
            {
                lock (m_WriteQueue)
                {
                    m_WriteQueue.Dequeue();//清除发送队列中的第一条数据
                    if (m_WriteQueue.Count > 0)//如果还是存在待发送数据则继续取出剩下的第一条发送，每次发送完成都回调检查直到发送完毕
                    {
                        ba = m_WriteQueue.First();
                    }
                    else
                    {
                        ba = null;
                    }
                }
            }

            //发送不完整或发送完整且存在第二条数据
            if (ba != null)
            {
                socket.BeginSend(ba.Bytes, ba.ReadIdx, ba.Length, 0, SendCallBack, socket);
            }
            //确保关闭链接前，先把消息发送出去
            else if (m_Closing)
            {
                RealClose();
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError("SendCallBack error:" + ex.ToString());
            Close();
        }
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    /// <param name="normal"></param>
    public void Close(bool normal = true)
    {
        if (m_Socket == null || m_Connecting)
        {
            return;
        }

        if (m_Connecting) return;

        if (m_WriteQueue.Count > 0)
        {
            m_Closing = true;
        }
        else
        {
            RealClose(normal);
        }
    }
    /// <summary>
    /// 关闭连接后的处理方法
    /// </summary>
    /// <param name="normal"></param>
    void RealClose(bool normal = true)
    {
        SecretKey = "";
        m_Socket.Close();
        FirstEvent(NetEvent.Close, normal.ToString());
        m_Diaoxian = true;
        if (m_HeartThread != null && m_HeartThread.IsAlive)
        {
            m_HeartThread.Abort();
            m_HeartThread = null;
        }
        if (m_MsgThread != null && m_MsgThread.IsAlive)
        {
            m_MsgThread.Abort();//强制终止线程
            m_MsgThread = null;
        }
        Debug.Log("Close Socket");
    }

    public void SetKey(string key)
    {
        SecretKey = key;
    }

    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}
