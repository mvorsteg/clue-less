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
    public ClientNetworkInterface clientNet;
    public ServerNetworkInterface serverNet;
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
            serverConsole.netInterface = serverNet;
            
            hostEngine.logger = serverLogger;

            serverNet.Initialize(address, port, hostEngine, serverLogger, "Server");
            serverNet.Log("Starting with \"Server\" argument");

            netIFace = serverNet;
            isInitialzied = true;
            
            if (isClient)
            {
                // also create client that will listen on loopback
                //client.Initialize();
                clientNet.logger = clientLogger;

                clientConsole.netInterface = clientNet;
                clientConsole.engine = guestEngine;

                guestEngine.logger = clientLogger;

                clientNet.Initialize(IPAddress.Loopback, port, guestEngine, clientLogger, playerName);

                netIFace = clientNet;
            }
            isInitialzied = true;
        }
        else
        {
            //client.Initialize();
            clientNet.logger = clientLogger;
            clientNet.Log("Starting with \"Client\" argument");

            clientConsole.netInterface = clientNet;
            clientConsole.engine = guestEngine;

            guestEngine.logger = clientLogger;

            clientNet.Initialize(address, port, guestEngine, clientLogger, playerName);

            netIFace = clientNet;
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