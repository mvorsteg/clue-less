using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public abstract class BaseNetworkInterface
{
    protected IPAddress ipAddress;
    protected int portNum;
    protected NetworkStream networkStream; 
    protected bool isConnected;
    public string processName;
    public ConsoleLogger logger;

    public BaseNetworkInterface(IPAddress ipAddress, int portNum)
    {   
        this.ipAddress = ipAddress;
        this.portNum = portNum;
    }

    public virtual void Initialize(BaseEngine engine, ConsoleLogger logger, string processName)
    {
        this.logger = logger;
        this.processName = processName;
    }

    public virtual void ShutDown()
    {
        networkStream.Dispose();
        isConnected = false;
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