using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{
    public static NetworkedClient Instance;
    
    static GameObject sManager;
    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 3333;
    byte error;
    bool isConnected = false;
    bool stablishedNetwork;
    int ourClientID;

    public int message = -1;
    public string stringMessage = "";
    public string displayMessageInScree = "";
    public string roomName = "";

    public int buttonIndex;
    public int turnOfPlayer;

    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Connect();
    }
    // Update is called once per frame
    void Update()
    {


        if (isConnected)
        {

            if(Input.GetKey(KeyCode.S))
            {
               // SendMessageToHost();
            }
            UpdateNetworkConnection();
        }
       
    }

    public void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }


    }

    public void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.0.13", socketPort, 0, out error); // server is local on network
           
            Debug.Log(connectionID + "   -> cID.");
           
            //DataManager.AddConnectionID(connectionID);

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID + " SUCCESS");

            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }
 
    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("Message ==> " + msg + ".    ID:" + id);

        string[] dataReceived = msg.Split(',');
        switch (int.Parse(dataReceived[0]))
        {
            case 0://Acess granted
                message = 0;
                break;
            case 1:
                message = 1;// ERROR-  Account name already exist
                break;

            case 2:
                message = 2;   //ERROR Loging Verification - Wrong username 
                break;
            case 3:
                message = 3; //ERROR Loging Verification - Wrong Password
                break;
            case 4:
                message = 4; //Account created successfully 
                Debug.Log(dataReceived[1]);
                break;
            case 5:

                message = 5;
                stringMessage = dataReceived[1].ToString();
                Debug.Log(stringMessage + "Message");
                break;
            case 6:
                message = 6;//Join game room
                stringMessage = dataReceived[1].ToString();
                Debug.Log(stringMessage + "Message");
        
                break;

            case 7:

                roomName = dataReceived[1].ToString();
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log(roomName + "Message");
                Debug.Log(turnOfPlayer + " its your turn ");
                message = 7;//Start Match 

                break;

            case 8:
                message = 8;//PlayerXMadeaMove
                buttonIndex = int.Parse(dataReceived[1]);
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log("Button IndexPressedBy otherPlayer.  -->" + buttonIndex.ToString());
                Debug.Log("Turn of player was -> " + turnOfPlayer.ToString());
                break;

            case 9:

                roomName = dataReceived[1].ToString();
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log(roomName + "Message");
                Debug.Log(turnOfPlayer + " its your turn ");
                message = 9;//Restart Match 
                break;


            case 10:
                message = 10;//Acces Denied
                Debug.Log(stringMessage);
                break;


            case 11:
                message = 11;//Acces Denied
                Debug.Log(stringMessage);
                break;

            case 12:
                message = 12;//Message Receive - Now Display message in screen
                displayMessageInScree = dataReceived[1].ToString();
                break;
            case 13:
                ControllerManager.Instance.isSpectator = true;
                break;
        }
        


    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public int  GetConnectionID()
    {
        return connectionID;
    }

    static public void SetSystemManager(GameObject SystemManager)
    {
        sManager = SystemManager;
    }
}