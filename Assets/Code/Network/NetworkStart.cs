using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class NetworkStart : MonoBehaviour
{
    public bool isInitialzied= false;
    public string processName = "UNNAMED";

    public InputField ipField;
    public InputField portField;
    public ConsoleLogger clientLogger, serverLogger;
    public ClientConsole clientConsole, serverConsole;
    public GuestEngine guestEngine;
    public HostEngine hostEngine;

    public BaseNetworkInterface netIFace;

    public void CreateServer()
    {
        int port;
        if (!Int32.TryParse(portField.text, out port))
        {
            Debug.Log(String.Format("failed to parse port {0}", portField.text));
        }
        Initialize(true, IPAddress.Any, port);
    }

    public void JoinServer()
    {
        IPAddress ip;
        int port;
        if (!IPAddress.TryParse(ipField.text, out ip))
        {
            Debug.Log(String.Format("failed to parse IP {0}", ipField.text));
        } 
        if (!Int32.TryParse(portField.text, out port))
        {
            Debug.Log(String.Format("failed to parse port {0}", portField.text));
        }
        Initialize(false, ip, port);
    }

    public void Initialize(bool isServer, IPAddress address, int port)
    {
        if (isServer)
        {
            ServerNetworkInterface server = new ServerNetworkInterface(address, port, 6);
            server.Initialize(hostEngine, serverLogger);
            clientConsole.netInterface = server;
            server.processName = "Host Server";
            server.Log("Starting with \"Server\" argument");

            netIFace = server;
            isInitialzied = true;
            
            // also create client that will listen on loopback
            ClientNetworkInterface client = new ClientNetworkInterface(IPAddress.Loopback, port);
            //client.Initialize();
            client.logger = clientLogger;
            client.processName = "Host Client";

            clientConsole.netInterface = client;
            clientConsole.engine = guestEngine;

            guestEngine.logger = clientLogger;

            netIFace = client;
            isInitialzied = true;
        }
        else
        {
            ClientNetworkInterface client = new ClientNetworkInterface(address, port);
            //client.Initialize();
            client.logger = clientLogger;
            client.processName = processName;
            client.Log("Starting with \"Client\" argument");

            clientConsole.netInterface = client;
            clientConsole.engine = guestEngine;

            guestEngine.logger = clientLogger;

            netIFace = client;
            isInitialzied = true;
        }
    }

    // private void OnEnable()
    // {
    //     if (isServer)
    //     {
    //         Initialize(null);
    //     }
    // }
    
    // private void OnDisable()
    // {
    //     if (isInitialzied)
    //     {
    //         netIFace.ShutDown();
    //     }
    // }
}