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
                    Debug.Log("Connection established.");
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("Client ID : " + recConnectionID + " disconnected.");
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

            connectionID = NetworkTransport.Connect(hostID, "192.168.0.17", socketPort, 0, out error); // server is local on network
            Debug.Log("Client ID: " + connectionID + " trying to connect to server.");

            if (error == 0)
            {
                isConnected = true;
                Debug.Log("Client ID: " + connectionID + " is connected to the server.");
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
        string[] dataReceived = msg.Split(',');
        int messageType = int.Parse(dataReceived[0]);
        switch (messageType)
        {
            case ServerToClientSignifiers.AcessGranted:
                messageFromServer = 0;
                Debug.Log("Message from server => AcessGranted");
                break;
            case ServerToClientSignifiers.AccountNameAlreadyExist:
                messageFromServer = 1;
                Debug.Log("Message from server => ERROR -  Account name already exist");

                break;

            case ServerToClientSignifiers.WrongUsername:
                messageFromServer = 2;   
                Debug.Log("Message from server => ERROR - Logging Verification - Wrong username");
                break;
            case ServerToClientSignifiers.WrongPassword:
                messageFromServer = 3; 
                Debug.Log("Message from server =>ERROR - Logging Verification - WrongPassword");
                break;
            case ServerToClientSignifiers.AccountCreatedSuccessfully:
                messageFromServer = 4; 
                Debug.Log("Message from server => Account Created Successfully");
                break;
            case ServerToClientSignifiers.RoomCreated:
                messageFromServer = 5;
          
                s_RoomName = dataReceived[1].ToString();
                Debug.Log("Message from server => Room: " + s_RoomName +" created");
                break;
            case ServerToClientSignifiers.JoinRoomX:
                messageFromServer = 6;//Join Game Room
       
                s_RoomName = dataReceived[1].ToString();
                Debug.Log("Message from server =>//Joinining Game Room " + s_RoomName);
                break;
            case ServerToClientSignifiers.StartMatch:
                roomName = dataReceived[1].ToString();
                turnOfPlayer = int.Parse(dataReceived[2]);
                messageFromServer = 7;
                Debug.Log("Message from server => Starting match");

                break;

            case ServerToClientSignifiers.PlayerXMadeAMove:
                messageFromServer = 8;

                buttonIndex = int.Parse(dataReceived[1]);
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log("Button IndexPressed By otherPlayer.  -->" + buttonIndex.ToString());
                Debug.Log("Turn of player was -> " + turnOfPlayer.ToString());
                break;

            case ServerToClientSignifiers.RestartMatch:

                roomName = dataReceived[1].ToString();
                turnOfPlayer = int.Parse(dataReceived[2]);
                Debug.Log(turnOfPlayer + " its your turn ");
                Debug.Log("Match restarted");
                messageFromServer = 9;
       
                break;


            case ServerToClientSignifiers.UserAlreadyLogged:
                messageFromServer = 10;
                Debug.Log("Message from server => ERROR - Access Denied - User already logged");
                break;
            case ServerToClientSignifiers.LeaveGameRoom:
                messageFromServer = 11;
                Debug.Log("Message from server => ERROR - Leaving Game Room");
                break;

            case ServerToClientSignifiers.DisplayMessageInScreen:
                messageFromServer = 12;
                displayMessageInScree = dataReceived[1].ToString();
                Debug.Log("Message from server => Message Received - Now Displaying message from server in screen");
                break;
            case ServerToClientSignifiers.SpectatorModeIsON:
                ControllerManager.Instance.isSpectator = true;
                Debug.Log("Message from server => Spectator mode is ON");
                break;
            case ServerToClientSignifiers.GetReplayData:
                clipName.Add(dataReceived[1].ToString());
                messageFromServer = 15;
                Debug.Log("Message from server => Getting replay data");
                break;

            case ServerToClientSignifiers.ReplayModeOn:
                for (int i = 2; i < dataReceived.Length; i++)
                {
                        playerdata = playerdata + dataReceived[i].ToString();    
                }
                messageFromServer = 16;
                Debug.Log("Message from server => Data recived for replay");
                break;
            case ServerToClientSignifiers.DataConfirmation:
                messageFromServer = 17;
                Debug.Log("Message from server => Data confirmed");
                break;

            case ServerToClientSignifiers.InvalidAccountInformation:
                messageFromServer = 20;
                Debug.Log("Message from server => Invalid username or password");
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

     public const int AcessGranted = 0;
     public const int AccountNameAlreadyExist = 1;
     public const int WrongUsername  = 2;
     public const int WrongPassword = 3;
     public const int AccountCreatedSuccessfully = 4;
     public const int RoomCreated = 5;
     public const int JoinRoomX = 6;
     public const int StartMatch = 7;
     public const int PlayerXMadeAMove = 8;
     public const int RestartMatch = 9;
     public const int UserAlreadyLogged = 10;
     public const int LeaveGameRoom = 11;
     public const int DisplayMessageInScreen = 12;
     public const int SpectatorModeIsON = 13;
     public const int GetReplayData =15;
     public const int ReplayModeOn = 16;
     public const int DataConfirmation = 17;
     public const int LeaveGameRoomLobby = 14;
     public const int InvalidAccountInformation = 20;

}