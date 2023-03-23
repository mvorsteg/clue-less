using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class NetworkStart : MonoBehaviour
{
    public bool isInitialzied= false;
    public string processName = "UNNAMED";

    public InputField nameField, ipField, portField;
    public ConsoleLogger clientLogger, serverLogger;
    public ClientConsole clientConsole, serverConsole;
    public GuestEngine guestEngine;
    public HostEngine hostEngine;

    public BaseNetworkInterface netIFace;

    public void CreateServerAndLocalClient()
    {
        int port;
        if (nameField.text == "")
        {
            Debug.Log(String.Format("failed to name player"));
            return;
        }
        if (!Int32.TryParse(portField.text, out port))
        {
            Debug.Log(String.Format("failed to parse port {0}", portField.text));
            return;
        }
        Initialize(true, true, IPAddress.Any, port, nameField.text);
    }

    public void CreateServer()
    {
        int port;
        if (!Int32.TryParse(portField.text, out port))
        {
            Debug.Log(String.Format("failed to parse port {0}", portField.text));
            return;
        }
        Initialize(true, false, IPAddress.Any, port, "");
    }

    public void JoinServer()
    {
        IPAddress ip;
        int port;

        if (nameField.text == "")
        {
            Debug.Log(String.Format("failed to name player"));
            return;
        }
        if (!IPAddress.TryParse(ipField.text, out ip))
        {
            Debug.Log(String.Format("failed to parse IP {0}", ipField.text));
            return;
        } 
        if (!Int32.TryParse(portField.text, out port))
        {
            Debug.Log(String.Format("failed to parse port {0}", portField.text));
            return;
        }
        Initialize(false, true, ip, port, nameField.text);
    }

    public void Initialize(bool isServer, bool isClient, IPAddress address, int port, string playerName)
    {
        if (isServer)
        {
            ServerNetworkInterface server = new ServerNetworkInterface(address, port, 6);
            
            server.logger = serverLogger;
            serverConsole.netInterface = server;
            
            hostEngine.logger = serverLogger;

            server.Initialize(hostEngine, serverLogger, "Server");
            server.Log("Starting with \"Server\" argument");

            netIFace = server;
            isInitialzied = true;
            
            if (isClient)
            {
                // also create client that will listen on loopback
                ClientNetworkInterface client = new ClientNetworkInterface(IPAddress.Loopback, port);
                //client.Initialize();
                client.logger = clientLogger;

                clientConsole.netInterface = client;
                clientConsole.engine = guestEngine;

                guestEngine.logger = clientLogger;

                client.Initialize(guestEngine, clientLogger, playerName);

                netIFace = client;
            }
            isInitialzied = true;
        }
        else
        {
            ClientNetworkInterface client = new ClientNetworkInterface(address, port);
            //client.Initialize();
            client.logger = clientLogger;
            client.Log("Starting with \"Client\" argument");

            clientConsole.netInterface = client;
            clientConsole.engine = guestEngine;

            guestEngine.logger = clientLogger;

            client.Initialize(guestEngine, clientLogger, playerName);

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