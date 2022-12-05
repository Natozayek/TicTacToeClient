using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
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



    public int messageFromServer = -1;
    public string s_RoomName = "";
    public string displayMessageInScree = "";
    public string roomName = "";
    public string playerdata = "";

    public int buttonIndex;
    public int turnOfPlayer;
    public List<string> clipName = new List<string>();

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

            connectionID = NetworkTransport.Connect(hostID, "192.168.0.19", socketPort, 0, out error); // server is local on network
            Debug.Log(connectionID + "   -> cID.");

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
            case 0:
                messageFromServer = 0;
                
                break;
            case 1:
                messageFromServer = 1;// ERROR -  Account name already exist
                
                break;

            case 2:
                messageFromServer = 2;   //ERROR - Logging Verification - Wrong username 
                
                break;
            case 3:
                messageFromServer = 3; //ERROR - Logging Verification - WrongPassword
 
                break;
            case 4:
                messageFromServer = 4; //Account Created Successfully 
      
                break;
            case 5:
                messageFromServer = 5;//Room Created
          
                s_RoomName = dataReceived[1].ToString();
                Debug.Log(s_RoomName + "Message");
                break;
            case 6:
                messageFromServer = 6;//Join Game Room
       
                s_RoomName = dataReceived[1].ToString();
                Debug.Log(s_RoomName + "Message");
                break;
            case 7:
                roomName = dataReceived[1].ToString();
                turnOfPlayer = int.Parse(dataReceived[2]);
                messageFromServer = 7;//Start Match 
        

                break;

            case 8:
                messageFromServer = 8;//Player X Made a Move

                buttonIndex = int.Parse(dataReceived[1]);
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log("Button IndexPressed By otherPlayer.  -->" + buttonIndex.ToString());
                Debug.Log("Turn of player was -> " + turnOfPlayer.ToString());
                break;

            case 9:

                roomName = dataReceived[1].ToString();
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log(roomName + "Message");
                Debug.Log(turnOfPlayer + " its your turn ");
                messageFromServer = 9;//Restart Match 
       
                break;


            case 10:
                messageFromServer = 10;//Access Denied - UserAlreadyLogged
                break;
            case 11:
                messageFromServer = 11;//Leave Game Room;
                break;

            case 12:
                messageFromServer = 12;//Message Receive - Now Display messageFromServer in screen
                displayMessageInScree = dataReceived[1].ToString();
                break;
            case 13://Spectator mode ON
                ControllerManager.Instance.isSpectator = true;
                break;
            case 15://Get replay data
                clipName.Add(dataReceived[1].ToString());
                messageFromServer = 15;
                break;

            case 16://DataRecived for replay
                for (int i = 2; i < dataReceived.Length; i++)
                {
                        playerdata = playerdata + dataReceived[i].ToString();    
                }
                messageFromServer = 16;
                break;
            case 17://DataConfirmation
                messageFromServer = 17;
                break;
        }
        


    }
    static public void SetSystemManager(GameObject SystemManager)
    {
        sManager = SystemManager;
    }
}

static public class ClientToServerSignifiers
{
    static public int AccessVerification = 0;
    static public int CreateNewAccount = 1;
    static public int CreateORJoinGameRoom = 2;
    static public int GameisReady = 3;
    static public int playerMoved = 4;
    static public int RestartMatch = 5;
    static public int LeaveGameNotification = 6;
    static public int SendMessageToOtherPlayer = 7;
    static public int SaveReplayData = 8;
    static public int SpectateRoom = 9;
    static public int LogOut = 11;
    static public int WatchReplay = 12;
    static public int PlayReplay = 13;

    static public int LeaveGameRoomLobby = 14;
}                                                   

static public class ServerToClientSignifiers
{

    static public int AcessGranted = 0;
    static public int AccountNameAlreadyExist = 1;
    static public int WrongUsername  = 2;
    static public int WrongPassword = 3;
    static public int AccountCreatedSuccessfully = 4;
    static public int RoomCreated = 5;
    static public int JoinRoomX = 6;
    static public int StartMatch = 7;
    static public int PlayerXMadeAMove = 8;
    static public int RestartMatch = 9;
    static public int UserAlreadyLogged = 10;
    static public int LeaveGameRoom = 11;
    static public int DisplayMessageInScreen = 12;
    static public int GetReplayData =15;
    static public int ReplayModeOn = 16;
    static public int DataConfirmation = 17;
    static public int LeaveGameRoomLobby = 14;


}