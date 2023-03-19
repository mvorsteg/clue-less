using System;
using System.Net;
using System.Net.Sockets;

public class NetworkTest
{
    /*  this entire class only exists so that I could test out networking from a terminal without unity it won't
        really run now since I've added unity namespaces to some files and can't compile that outside unity */
    public static void Main(string[] args)
    {
        bool isServer = false;
        //determine if client or server
        for(int i = 0; i < args.Length; i++)
        {
            //BaseNetworkInterface.Log(String.Format("{0} : {1}", i, args[i]));
            if (args[i] == "-s")
            {
                isServer = true;
            }
        }
        if (isServer)
        {
            ServerNetworkInterface server = new ServerNetworkInterface(IPAddress.Any, 50003, 6);
            server.Initialize();
            server.Log("Starting with \"Server\" argument parsed from command line");
            
            while ((Console.ReadLine()) != "quit")
            {
            }
            Console.ReadKey();
            server.ShutDown();
        }
        else
        {
            ClientNetworkInterface client = new ClientNetworkInterface(IPAddress.Loopback, 50003);
            client.Initialize();
            client.Log("Starting with \"Client\" argument parsed from command line");

            string msg;
            while ((msg = Console.ReadLine()) != "quit")
            {
                //client.SendMessage(msg);
            }
            Console.ReadKey();
        }
        
    }
}