using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemManager : MonoBehaviour
{

    public static SystemManager Instance;
    //Buttons
    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject acountCreationButton;
    [SerializeField] GameObject createNewAccButton;
    [SerializeField] GameObject joinGameRoom;
    [SerializeField] GameObject leaveGameRoom;


    //Input fields
    [SerializeField] public InputField username;
    [SerializeField] public InputField password;

    [SerializeField] public InputField newUsername;
    [SerializeField] public InputField newPassword;

    [SerializeField] public InputField gameRoomName;
    [SerializeField] public GameObject gameRoomNameParent;

    [SerializeField] public Text roomName1;

    //New user ui
    [SerializeField]GameObject newUser;

    //message
    public GameObject messageInfo, messageAGranted, messageADenied, messageWrongUsername, messageUsernameAlreadyExist1, messageAccountHasBeenCreated, loginBox, inputBoxForNewGameRoom, newGameRoom, newGame;

    private bool accesGranted;
    private bool isThePlayerReady;


    public float timer;
    protected int connectionID;

   
    public int ConnectionID { get { return connectionID; } set { connectionID = value; } }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
     // DataManager.SetSystemManager(gameObject);
      NetworkedClient.SetSystemManager(gameObject);
       
    }



    private void Update()
    {
        if(NetworkedClient.Instance.message >= 0)
        {
            ShowMessage();
            NetworkedClient.Instance.message = -1;
        }
        
     
        
    }
    //FUNCTION TO HANDLE EVENTS IN GAME
    public void ShowMessage()
    {
        if (NetworkedClient.Instance.message == 0)//ACCESS GRANTED = 0
        {
            messageInfo.SetActive(false);
            messageAGranted.SetActive(true);
            accesGranted = true;
            StartCoroutine(DisableMessage());

        }

        //ACCOUNT CREATION 1 == USERNAME ALREADY EXIST
        if (NetworkedClient.Instance.message == 1)
        {

            messageInfo.GetComponent<Text>().text = "Username already exists ";
            StartCoroutine(DisableMessage());

        }
        //ACCESS DENIED = 2 ------------WRONG USERNAME
      
        if (NetworkedClient.Instance.message == 2)
        {
            messageInfo.SetActive(false);
            messageWrongUsername.SetActive(true);
            StartCoroutine(DisableMessage());
        }
        //ACCESS DENIED = 3 .  ---------WRONG PASSWORD
        if (NetworkedClient.Instance.message == 3)
        {
            messageInfo.SetActive(false);
            messageADenied.SetActive(true);
            StartCoroutine(DisableMessage());
        }
        //ACCOUNT CREATED = 4. READY TO LOGGING
        if (NetworkedClient.Instance.message == 4)
        {
            messageInfo.GetComponent<Text>().text = "Account created, please login!";
            StartCoroutine(DisableMessage());
        }
        //CREATE ROOM - with name string data.
        if (NetworkedClient.Instance.message == 5) 
        {
            newGameRoom.gameObject.SetActive(true);
            gameRoomNameParent.gameObject.SetActive(true);
            roomName1.gameObject.SetActive(true);

            string data = NetworkedClient.Instance.stringMessage.ToString();
            jointoRoom(data);
        }
        // JOIN ROOM - with name string data
        if (NetworkedClient.Instance.message == 6)
        {
            newGameRoom.gameObject.SetActive(true);
            gameRoomNameParent.gameObject.SetActive(true);
            roomName1.gameObject.SetActive(true);

            inputBoxForNewGameRoom.SetActive(false);
            string data = NetworkedClient.Instance.stringMessage.ToString();
            jointoRoom(data);
        }


        // USER ALREADY LOGGED
        //possible ifstatement


    }


    #region FUNCTIONS TO LET KNOW SERVER ABOUT NEW ACCOUNT CREATION /LOGGING VERIFICATION/ && EVENTS IN LOGING BOX SCENE
    // FUNCTION TO CREATE ACCOUNT IN SERVER
    public void CreateNewAccount()
    {
        Debug.Log("CREATING ACCOUNT");
        string userNameNpassword =  "1," + GetNewUsername() + "," + GetNewPassWord();
        NetworkedClient.Instance.SendMessageToHost(userNameNpassword);
        Debug.Log("MessageSent to Server");

        //Clearing input boxes of account creation
        newUser.SetActive(false);
        newUsername.text = "";
        newPassword.text = "";
    }
    public void AccountCreation()//TO POP UP THE WINDOW OF ACCOUNT CREATION
    {
        newUser.SetActive(true);
    }

    //FUNCTION TO VERIFY WITH SERVER THE LOGING USERNAME AND PASSWORD
    public void LoginVerification()
    {
        Debug.Log("LOGIN VERIFICATION");
        string userNameNpassword = "0," + GetUsername() + "," + GetPassWord();
        NetworkedClient.Instance.SendMessageToHost(userNameNpassword);
        Debug.Log("MessageSent to Server");
    }

    #endregion


    #region FUNCTIONS TO LET KNOW SERVER ABOUT GAMEROOM CREATION OR JOIN/ PREPARE GAME/ LEAVE GAME/ LEAVE GAME ROOM
    public void CreateORJoinGameRoom()
    {
        Debug.Log("CREATING OR JOINING ROOM EVENTS ->> name: " +GetGameRoomName());
        string gameroomCreation = "2," + GetUsername() + "," + GetGameRoomName().ToString();
        NetworkedClient.Instance.SendMessageToHost(gameroomCreation);
        Debug.Log("MessageSent to Server");

    }

    public void jointoRoom(string roomName)
    {
        Debug.Log("JOINING TO ROOM --->   " + roomName );
        if(roomName1.IsActive())
        roomName1.GetComponent<Text>().text = roomName;
        inputBoxForNewGameRoom.SetActive(false);
        Debug.Log("Room: " + roomName + " joined");
    }


    public void LeaveGameRoomLobby()
    {

        inputBoxForNewGameRoom.SetActive(true);
        newGameRoom.SetActive(false);
       
    }    
    public void GameReady()
    {
        isThePlayerReady = true;
        newGameRoom.SetActive(false);
        newGame.SetActive(true);
    }
    public void LeaveGame()
    {
        //After reseting variables in cotroller manager, this function will be called to handle the "scene" transition;
        isThePlayerReady = false;
        newGameRoom.SetActive(false);
        newGame.SetActive(false);
        inputBoxForNewGameRoom.SetActive(true);

        //gameRoomName.text = "";
        //roomName1.text = ""; 
        
        
        //Notify the server i have left the game
        //What does the server needs to know and do?

    }
    #endregion


    #region GETTERS for Username/ Password/ NewUsername/ NewPassword/ GameRoomName
    //Getters
    public string GetUsername()
    {
        return (username.text);
    }
    public string GetPassWord()
    {
        return (password.text);
    }
    public string GetNewUsername()
    {
        return (newUsername.text);
    }
    public string GetNewPassWord()
    {
        return (newPassword.text);
    }

    public string GetGameRoomName()
    {
        return gameRoomName.text;
    }
    public int GetConnectionID()
    {
        return ConnectionID;
    }

    #endregion

    #region    HELPER FUNCTIONS TO RESET VARIABLES IN LOGING BOX
    //public void Disabling()
    //{
    //    DisableMessage();
    //}
    public IEnumerator DisableMessage()
    {
        Debug.Log("DISABLIGN MESSAGES IN LOGING BOX");
        yield return new WaitForSeconds(2.0f);
        messageInfo.GetComponent<Text>().text = "Please fill with name\r\n and password";
        messageAGranted.SetActive(false);
        messageADenied.SetActive(false);
        messageWrongUsername.SetActive(false);

        //Reseting vaiables in loging box
        username.text = "";
        password.text = "";

        if (accesGranted)// Go to lobby
        {
            Debug.Log("Disabling loging box");

            //loginBox.gameObject.SetActive(false);
            GameObject.Find("LoginBox").gameObject.active = false;

            Debug.Log("Login box disabled.");

            inputBoxForNewGameRoom.gameObject.active = true;
        }
    }
    #endregion

}
