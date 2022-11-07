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

        if (NetworkedClient.Instance.message == 4)
        {
            messageInfo.GetComponent<Text>().text = "Account created, please login!";
            StartCoroutine(DisableMessage());
        }
        if (NetworkedClient.Instance.message == 5)
        {
            newGameRoom.gameObject.SetActive(true);
            gameRoomNameParent.gameObject.SetActive(true);
            roomName1.gameObject.SetActive(true);

            string data = NetworkedClient.Instance.stringMessage.ToString();
            jointoRoom(data);
            StartCoroutine(DisableMessage());
        }

        // USER ALREADY LOGGED
        //possible ifstatement


    }

    //FUNCTION TO VERIFY WITH SERVER THE LOGING USERNAME AND PASSWORD
    public void LoginVerification()
    {
        Debug.Log("LOGIN VERIFICATION");
        string userNameNpassword = "0," + GetUsername() + "," + GetPassWord();

        NetworkedClient.Instance.SendMessageToHost(userNameNpassword);

      

    }

    //Function for create account button
    public void CreateNewAccount()
    {
        string userNameNpassword =  "1," + GetNewUsername() + "," + GetNewPassWord();
        NetworkedClient.Instance.SendMessageToHost(userNameNpassword);

        newUser.SetActive(false);
        newUsername.text = "";
        newPassword.text = "";
    }
    public void AccountCreation()//TO POP UP THE WINDOW OF ACCOUNT CREATION
    {
        newUser.SetActive(true);
    }

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
        return(newUsername.text);
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

    

    //Helpers

    public void Disabling()
    {
       
        DisableMessage();   
    }
    public IEnumerator DisableMessage()
    {
        Debug.Log("DISABLIGN");
        
        yield return new WaitForSeconds(4.0f);

        //message.gameObject.SetActive(false);
        messageInfo.GetComponent<Text>().text = "Please fill with name\r\n and password";
        messageAGranted.SetActive(false);
        messageADenied.SetActive(false);
        messageWrongUsername.SetActive(false);
       // messageUsernameAlreadyExist1.SetActive(false);
       // messageAccountHasBeenCreated.SetActive(false);

    
       
        username.text = "";
        password.text = "";

        if(accesGranted)
        {
            Debug.Log("Disabling loging box");
            loginBox.SetActive(false);
            inputBoxForNewGameRoom.SetActive(true);
        }
    }

    public void CreateORJoinGameRoom()
    {
        string gameroomCreation = "2," + GetUsername() + "," + GetGameRoomName().ToString();
        NetworkedClient.Instance.SendMessageToHost(gameroomCreation);
        
    }

    public void jointoRoom(string roomName)
    {
    
    
        Debug.Log(roomName + "JOIN TO ROOM NAME ");
        if(roomName1.IsActive())
        roomName1.GetComponent<Text>().text = roomName;
        inputBoxForNewGameRoom.SetActive(false);
    }


    public void LeaveGameRoomLobby()
    {
        //gameRoomNameText.text = "";


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

}
