using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class BaseNetworkInterface : MonoBehaviour
{
    protected IPAddress ipAddress;
    protected int portNum;
    protected NetworkStream networkStream; 
    protected bool isConnected;
    public string processName;
    public ConsoleLogger logger;
    protected Queue<Tuple<int, MessageIDs, byte[]>> messageQueue;

    protected virtual void Awake()
    {
        messageQueue = new Queue<Tuple<int, MessageIDs, byte[]>>();
    }

    protected virtual void Update()
    {
        if (messageQueue.TryDequeue(out Tuple<int, MessageIDs, byte[]> messageData))
        {
            ProcessMessage(messageData.Item1, messageData.Item2, messageData.Item3);
        }
    }

    public virtual void Initialize(IPAddress ipAddress, int portNum, BaseEngine engine, ConsoleLogger logger, string processName)
    {
        this.ipAddress = ipAddress;
        this.portNum = portNum;
        this.logger = logger;
        this.processName = processName;
    }

    public virtual void ShutDown()
    {
        if (networkStream != null)
        {
            networkStream.Dispose();
        }
        isConnected = false;
    }

    public virtual void DisconnectPlayer(int playerID)
    {
        // do nothing in base class
    }

    public void EnqueueMessage(int playerID, MessageIDs messageID, byte[] buffer)
    {
        messageQueue.Enqueue(new Tuple<int, MessageIDs, byte[]>(playerID, messageID, buffer));
    }

    public virtual void ProcessMessage(int clientID, MessageIDs messageID, byte[] buffer)
    {
        // do nothing in base class
    }

    // utility for Debug.Log so I don't have to append [Client] or [Server] to every message to see where it came from
    public void Log(string message)
    {
        //Debug.Log(String.Format("[{0}] {1}", processName, message));
        if (logger != null)
        {
            logger.Log(message, SubsystemType.Network);
        }
        //Thread.Sleep(500);
        // BaseNetworkInterface.Log(message);
    }
}