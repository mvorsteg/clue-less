using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkStart : MonoBehaviour
{
    public bool isServer;
    public bool isInitialzied= false;
    public string processName = "UNNAMED";

    public BaseNetworkInterface netIFace;
    private void OnEnable()
    {
        if (isServer)
        {
            ServerNetworkInterface server = new ServerNetworkInterface(IPAddress.Any, 50003, 6);
            server.Initialize();
            server.processName = processName;
            server.Log("Starting with \"Server\" argument");
            //string msg;

            //server.ShutDown();
            netIFace = server;
            isInitialzied = true;
        }
        else
        {
            ClientNetworkInterface client = new ClientNetworkInterface(IPAddress.Loopback, 50003);
            client.Initialize();
            client.processName = processName;
            client.Log("Starting with \"Client\" argument");

            //string msg;
            // while ((msg = Console.ReadLine()) != "quit")
            // {
            //     client.SendChatMessage(msg);
            // }
            //Console.ReadKey();
            netIFace = client;
            isInitialzied = true;
        }
    }

    private void OnDisable()
    {
        if (isInitialzied)
        {
            netIFace.ShutDown();
        }
    }
}