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
    public ClientConsole console; // temp, this is really bad coupling

    public BaseNetworkInterface(IPAddress ipAddress, int portNum)
    {   
        this.ipAddress = ipAddress;
        this.portNum = portNum;
    }

    public virtual void Initialize()
    {
        // do nothing in base class
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
        Debug.Log(String.Format("[{0}] {1}", processName, message));
        if (console != null)
        {
            console.QueueMessageForDisplay(message);
        }
        //Thread.Sleep(500);
        // BaseNetworkInterface.Log(message);
    }
}