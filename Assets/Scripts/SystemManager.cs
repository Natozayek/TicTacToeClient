using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SystemManager : MonoBehaviour
{

    public static SystemManager Instance;
    [Header("Buttons")]
    
    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject acountCreationButton;
    [SerializeField] GameObject createNewAccButton;
    [SerializeField] GameObject joinGameRoom;
    [SerializeField] GameObject leaveGameRoom;

    [Header("InputFields")]
    [SerializeField] public InputField username;
    [SerializeField] public InputField password;
    [SerializeField] public InputField newUsername;
    [SerializeField] public InputField newPassword;
    [SerializeField] public InputField gameRoomName;

    [Header("RoomName")]
    [SerializeField] public Text roomName1;

    [Header("UserCreation")]
    [SerializeField]GameObject newUser;

    [Header("UIMessages")]
    public GameObject messageInfo, messageAGranted, messageADenied, messageWrongUsername, messageUsernameAlreadyExist1, messageAccountHasBeenCreated, loginBox, inputBoxForNewGameRoom, newGameRoom, newGame;
 
    [Header("UIButtons&Dropwdown")]
    [SerializeField] public GameObject playButton;
    [SerializeField]public GameObject dropdown;
    public Text labeText;
    
    private bool GameIsReady;
    private bool isDataAlreadyRecived;
    string roomNameString;
    
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        NetworkedClient.SetSystemManager(gameObject);
    }

    private void Update()
    {
     
        if(NetworkedClient.Instance.messageFromServer >= 0)
        {
            ShowMessage();
            NetworkedClient.Instance.messageFromServer = -1;
        }
        
    }
    //FUNCTION TO HANDLE EVENTS IN GAME
    public void ShowMessage()
    {
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.AcessGranted)
        {
            messageInfo.SetActive(false);
            messageAGranted.SetActive(true);
            StartCoroutine(DisableMessage());
            StartCoroutine(AccessGranted());
           
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.AccountNameAlreadyExist)
        {
            messageInfo.GetComponent<Text>().text = "Username already exists ";
            StartCoroutine(DisableMessage());
        }
        if (NetworkedClient.Instance.messageFromServer ==  ServerToClientSignifiers.WrongUsername)
        {
            messageInfo.SetActive(false);
            messageWrongUsername.SetActive(true);
            StartCoroutine(DisableMessage());
           
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.WrongPassword)
        {
            messageInfo.SetActive(false);
            messageADenied.SetActive(true);
            StartCoroutine(DisableMessage());
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.AccountCreatedSuccessfully)
        {
            messageInfo.GetComponent<Text>().text = "Account created, please login!";
            StartCoroutine(DisableMessage());
         
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.RoomCreated) 
        {
            newGameRoom.gameObject.SetActive(true);
            string roomName = NetworkedClient.Instance.s_RoomName.ToString();
            jointoRoom(roomName);
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.JoinRoomX)
        {
            newGameRoom.gameObject.SetActive(true);
            inputBoxForNewGameRoom.SetActive(false);
            string roomName = NetworkedClient.Instance.s_RoomName.ToString();
            GameIsReady = true;
            jointoRoom(roomName);

            if(ControllerManager.Instance.isSpectator)
            {
                GameReady();
                ControllerManager.Instance.gameSetUp();
            }
        
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.StartMatch)
        {
            GameReady();
            ControllerManager.Instance.gameSetUp();
        }

        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.PlayerXMadeAMove)
        {
            ControllerManager.Instance.reciveButtonClicked(NetworkedClient.Instance.buttonIndex, NetworkedClient.Instance.turnOfPlayer);
          
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.RestartMatch)
        {
            ControllerManager.Instance.ResetGameVariables();
        }

        if(NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.UserAlreadyLogged)
        {

            messageInfo.GetComponent<Text>().text = "Username already logged in";
            StartCoroutine(DisableMessage());
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.LeaveGameRoom)
        {
            ControllerManager.Instance.LeaveGame();
            deactivate();
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.DisplayMessageInScreen)
        {
            ControllerManager.Instance.messagetoPlayer.text = NetworkedClient.Instance.displayMessageInScree;
            StartCoroutine(ControllerManager.Instance.DisableMessage2());
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.GetReplayData)
        {
           if(!isDataAlreadyRecived)
            {

                foreach (var item in NetworkedClient.Instance.clipName)
                {
                    dropdown.transform.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = item });
                }

                NetworkedClient.Instance.clipName.Clear();

                dropdown.transform.GetComponent<Dropdown>().onValueChanged.AddListener(
                       delegate
                       {
                           var drop = dropdown.transform.GetComponent<Dropdown>();
                           itemSelected(drop);
                       });
            }
          
           

        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.ReplayModeOn)
        {
            playButton.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(false);
            inputBoxForNewGameRoom.SetActive(false);
            newGame.gameObject.SetActive(true);
            ControllerManager.Instance.isReplayMode = true;
            ControllerManager.Instance.gameSetUp();
        }
        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.DataConfirmation)
        {
            isDataAlreadyRecived = true;
        }

        if (NetworkedClient.Instance.messageFromServer == ServerToClientSignifiers.LeaveGameRoomLobby)
        {
            LeaveGameRoomLobby();
        }

     }


    #region FUNCTIONS TO LET KNOW SERVER ABOUT NEW ACCOUNT CREATION /LOGGING VERIFICATION/ && EVENTS IN LOGING BOX SCENE
    // FUNCTION TO CREATE ACCOUNT IN SERVER
    public void CreateNewAccount()
    {
        string userNameNpassword =  ClientToServerSignifiers.CreateNewAccount + "," + GetNewUsername() + "," + GetNewPassWord();
        NetworkedClient.Instance.SendMessageToHost(userNameNpassword);
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
        string userNameNpassword =  ClientToServerSignifiers.AccessVerification + "," + GetUsername() + "," + GetPassWord();
        NetworkedClient.Instance.SendMessageToHost(userNameNpassword);
    }

    #endregion

    #region FUNCTIONS TO LET KNOW SERVER ABOUT GAMEROOM CREATION OR JOIN/ PREPARE GAME/ LEAVE GAME/ LEAVE GAME ROOM/REPLAY GAMES/

    public void itemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        labeText.text = dropdown.options[index].text;
}
    public void CreateORJoinGameRoom()
    {
        string gameroomCreation = ClientToServerSignifiers.CreateORJoinGameRoom + "," + GetUsername() + "," + GetGameRoomName().ToString();
        NetworkedClient.Instance.SendMessageToHost(gameroomCreation);
        
    }
    public void SpectateGameRoom()
    {
        string SpectateRoom = ClientToServerSignifiers.SpectateRoom + "," + GetUsername() + "," + GetGameRoomName().ToString();
        NetworkedClient.Instance.SendMessageToHost(SpectateRoom);
    }
    public void jointoRoom(string roomName)
    {
      
        if(roomName1.IsActive())
        roomName1.GetComponent<Text>().text = roomName;
        inputBoxForNewGameRoom.SetActive(false);
        roomNameString = roomName;
        if(GameIsReady && !ControllerManager.Instance.isSpectator)
        {
            string startGame = ClientToServerSignifiers.GameisReady + "," + GetGameRoomName().ToString();
            NetworkedClient.Instance.SendMessageToHost(startGame);
        }
    }
    public void LeaveGameRoomLobby()
    {
        inputBoxForNewGameRoom.SetActive(true);
        newGameRoom.SetActive(false);
        roomName1.GetComponent<Text>().text = "";
       string LeaveGameRoomLobby = ClientToServerSignifiers.LeaveGameRoomLobby + "," + roomNameString;
        NetworkedClient.Instance.SendMessageToHost(LeaveGameRoomLobby);

        roomNameString = "";
    }    
    public void GameReady()
    {
        newGameRoom.SetActive(false);
        newGame.SetActive(true);
    }
    public void LeaveGame()
    {
        //After reseting variables in cotroller manager, this function will be called to handle the "scene" transition;
         GameIsReady = false;
         newGameRoom.SetActive(false);
        newGame.SetActive(false);
        inputBoxForNewGameRoom.SetActive(true);
    }
    public void WatchReplay()
    {
        dropdown.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        string dropdown2 =  ClientToServerSignifiers.WatchReplay + ",";
        NetworkedClient.Instance.SendMessageToHost(dropdown2);
    }

    public void PlayReplay()
    {
        string replayName = labeText.text;
        string PlayReplayName =  ClientToServerSignifiers.PlayReplay + "," + replayName;
        NetworkedClient.Instance.SendMessageToHost(PlayReplayName);
    }

    public void LogOut()
    {
        string playerLogOut = ClientToServerSignifiers.LogOut + ",";
        NetworkedClient.Instance.SendMessageToHost(playerLogOut);
        inputBoxForNewGameRoom.SetActive(false);
        loginBox.SetActive(true);
        messageInfo.SetActive(true);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Exiting application");
        string playerLogOut = ClientToServerSignifiers.LogOut + ",";
        NetworkedClient.Instance.SendMessageToHost(playerLogOut);
    }

    private void deactivate()
    {
            roomName1.GetComponent<Text>().text = "";
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


    #endregion

    #region    HELPER FUNCTIONS TO RESET VARIABLES IN LOGING BOX
    public IEnumerator DisableMessage()
    {
        yield return new WaitForSeconds(2.0f);
        messageInfo.GetComponent<Text>().text = "Please fill with name\r\n and password";
        messageAGranted.SetActive(false);
        messageADenied.SetActive(false);
        messageWrongUsername.SetActive(false);

        //Reseting vaiables in loging box
        username.text = "";
        password.text = "";
    }

    public IEnumerator AccessGranted()
    {
        yield return new WaitForSeconds(2.1f);
        GameObject.Find("LoginBox").gameObject.active = false;
        inputBoxForNewGameRoom.gameObject.active = true;
    }
    #endregion

}
