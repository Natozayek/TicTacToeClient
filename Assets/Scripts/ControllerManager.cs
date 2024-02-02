using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{
    private ReplayManager replayManager;
    public int turnoOfPlayer; // 0 = X player  & 1 = O Player
    public int turnCount;
    public GameObject[] turnDisplay;//shows whos turn it is
    public Sprite[] playerIcons;
    public Button[] playerSpaces; //Playable space for game
    public int[] usedButton;

    
    public Text winnerText;//holds text component of winner textss
    public GameObject[] winLines;//holds lines of winning display

    
    public int Player1Score;
    public int Player2Score;

    public Text player1ScoreText;
    public Text player2ScoreText;

    public InputField replayName;
    public GameObject saveReplayButton;

    public GameObject resetGameButton;
    public GameObject leaveGameButton;
    public Text messagetoPlayer;
    public static ControllerManager Instance;

    public bool isSpectator;
    public bool isInGameRoom;
    public bool isReplayMode;
    bool theGameIsDone = false;


    private void Awake()
    {
        Instance = this;
        replayManager = gameObject.AddComponent<ReplayManager>();
        replayManager.Initialize(this);
    }
 
    public void gameSetUp()
    {
       if(!isSpectator && !isReplayMode)
        {
            isInGameRoom = true;
            turnoOfPlayer = NetworkedClient.Instance.turnOfPlayer;
        }

        Debug.Log("GAME SETUP");
        Debug.Log(NetworkedClient.Instance.turnOfPlayer.ToString() + " <=  NetworkClient turnofPlayer X");
        turnCount = NetworkedClient.Instance.turnOfPlayer;
        if (turnCount == 0)
        {
            turnDisplay[0].SetActive(true);
            turnDisplay[1].SetActive(false);
        }
        else
        {
            turnDisplay[1].SetActive(true);
            turnDisplay[0].SetActive(false);
        }
     

        for (int i = 0; i < playerSpaces.Length; i++)
        {
            playerSpaces[i].interactable = true;// make all the buttons interactables
            playerSpaces[i].GetComponent<Image>().sprite = null;
        }

        for(int i = 0; i< usedButton.Length; i++)
        {
            usedButton[i] = -200;
        }

        if(turnoOfPlayer == 1 && !isSpectator)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(turnoOfPlayer == 0 && !isSpectator)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        {
            leaveGameButton.gameObject.SetActive(true);
        }

      

        if (isReplayMode)
        {
            for (int i = 0; i < playerSpaces.Length; i++)
            {
                playerSpaces[i].interactable = false;// make all the buttons interactables
                playerSpaces[i].GetComponent<Image>().sprite = null;
            }
            StartCoroutine(replayManager.ReplayMoves());
        }
    }
    
    void Update()
    {
        UpdateCursorVisibility();
        if (isSpectator)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            for (int i = 0; i < playerSpaces.Length; i++)
            {
                playerSpaces[i].interactable = false;// make all the buttons interactables
            }
        }
    }

    public void onButtonClicked(int buttonIndex)
    {
        Debug.Log("Button Pressed at index =>" + buttonIndex);
        turnCount++;
        playerSpaces[buttonIndex].image.sprite = playerIcons[turnoOfPlayer];
        playerSpaces[buttonIndex].interactable = false;
        usedButton[buttonIndex] = turnoOfPlayer + 1;

        NotifyServer(buttonIndex, turnoOfPlayer);

        if (turnCount > 4)
        {
            CheckWinCondition(turnoOfPlayer);
        }

        if (turnCount >= 9)
        {
            theGameIsDone = true;
        }
        //Player X has made a move notify button index pressed and which player made a move before 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (turnoOfPlayer == 0)
        {
            turnDisplay[1].SetActive(true);
            turnDisplay[0].SetActive(false);
        }
        if (turnoOfPlayer == 1)
        {
          
            turnDisplay[0].SetActive(true);
            turnDisplay[1].SetActive(false);

        }
    }
    public void onButtonClickedForReplay(int buttonIndex, int turnofReplay)
    {
        Debug.Log("Button Pressed at index =>" + buttonIndex  + " by turn of player ==>" + turnofReplay);
        turnCount++;
     
        playerSpaces[buttonIndex].image.sprite = playerIcons[turnofReplay];
        playerSpaces[buttonIndex].interactable = false;
        usedButton[buttonIndex] = turnofReplay + 1;

        if (turnCount > 4)
        {
            CheckWinCondition(turnofReplay);
        }

        if (turnCount >= 9)
        {
            theGameIsDone = true;
        }
    }
    public void reciveButtonClicked(int buttonIndex, int playerTurn)
    {
        turnCount++;

        playerSpaces[buttonIndex].image.sprite = playerIcons[playerTurn];
        playerSpaces[buttonIndex].interactable = false;

        usedButton[buttonIndex] = playerTurn + 1;

        if (turnCount > 4)
        {
            CheckWinCondition(playerTurn);
        }

        if(!theGameIsDone)
        {
            if (playerTurn == 0)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                turnoOfPlayer = 1;
                turnDisplay[1].SetActive(true);
                turnDisplay[0].SetActive(false);

            }
           else if (playerTurn == 1)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                turnoOfPlayer = 0;
                turnDisplay[0].SetActive(true);
                turnDisplay[1].SetActive(false);

            }
        }
    }
    public void CheckWinCondition(int turnOfPlayer) // 8 possibles way to win //3 horizontal lines //3 vertical lines and 2 diagonal lines
    {
        int possibleHorizontalSol1 = usedButton[0] + usedButton[1] + usedButton[2];
        int possibleHorizontalSol2 = usedButton[3] + usedButton[4] + usedButton[5];
        int possibleHorizontalSol3 = usedButton[6] + usedButton[7] + usedButton[8];

        int possibleVerticalSol1 = usedButton[0] + usedButton[3] + usedButton[6];
        int possibleVerticalSol2 = usedButton[1] + usedButton[4] + usedButton[7];
        int possibleVerticalSol3 = usedButton[2] + usedButton[5] + usedButton[8];

        int possibleDiagonalSol1 = usedButton[0] + usedButton[4] + usedButton[8];
        int possibleDiagonalSol2 = usedButton[2] + usedButton[4] + usedButton[6];

        var solutions = new int[] { possibleHorizontalSol1, possibleHorizontalSol2, possibleHorizontalSol3,
                                              possibleVerticalSol1, possibleVerticalSol2, possibleVerticalSol3,
                                              possibleDiagonalSol1, possibleDiagonalSol2};

        for(int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == 3*(turnOfPlayer + 1))
            {
                Debug.Log("Player " + turnOfPlayer + " won!");

                DisplayWinState(i, turnOfPlayer);
                
                return;
            }
        }

    }

    void DisplayWinState(int index, int turnofPlayer)
    {
        theGameIsDone = true;
        if (turnofPlayer == 0)
        {
            winnerText.text = "Player " + index + " Wins!";
            Player1Score++;
            player1ScoreText.text = Player1Score.ToString();
        }
        else if(turnofPlayer == 1)
        {
            winnerText.text = "Player " + index + " Wins!";
            Player2Score++;
            player2ScoreText.text = Player2Score.ToString();
        }
        if(!isReplayMode)
        {
            for (int i = 0; i < playerSpaces.Length; i++)
            {
                playerSpaces[i].interactable = false;//disable buttons
            }
        }
       
        winLines[index].SetActive(true);// set active line at player index

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void ResetGameVariables()
    {
        theGameIsDone=false;
        saveReplayButton.gameObject.GetComponent<Button>().interactable = true;
        resetGameButton.gameObject.SetActive(false);
        saveReplayButton.gameObject.SetActive(false);
        replayName.gameObject.SetActive(false);
  
        for (int i = 0; i < winLines.Length; i++)
        {
            winLines[i].SetActive(false);//Disable all lines
        }
        winnerText.text = " Playing";
        player1ScoreText.text = Player1Score.ToString();
        player2ScoreText.text = Player2Score.ToString();

        gameSetUp();
    }

    public void ResetGame()
    {
        Debug.Log("Reset Game");
        string reset = ClientToServerSignifiers.RestartMatch + "," + NetworkedClient.Instance.roomName;
        NetworkedClient.Instance.SendMessageToHost(reset);
        Debug.Log("MessageSent to Server");
    }

public void LeaveGame()
    {
        replayManager.SetTurnOfReplay(0);
        turnCount = 0;
        turnoOfPlayer = 0;
        Player1Score = 0;
        Player2Score = 0;
        player1ScoreText .text= "";
        player1ScoreText.text = "";
        player2ScoreText.text = "";
        theGameIsDone = false;
        isInGameRoom = false;
        isSpectator = false;
        isReplayMode = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        resetGameButton.gameObject.SetActive(false);
        leaveGameButton.gameObject.SetActive(false);
        saveReplayButton.gameObject.SetActive(false);
        replayName.gameObject.SetActive(false);

        for (int i = 0; i < winLines.Length; i++)
        {
            winLines[i].SetActive(false);//Disable all lines
        }

        SystemManager.Instance.LeaveGame();
        NetworkedClient.Instance.playerdata = "";
        Debug.Log(NetworkedClient.Instance.playerdata);
    }

    public void NotifyServer(int buttonIndex, int turnOfPlayerX)
    {
        string playerMoved = ClientToServerSignifiers.playerMoved + "," + buttonIndex.ToString() + "," + turnOfPlayerX.ToString();
        NetworkedClient.Instance.SendMessageToHost(playerMoved);
    }

    public void LeaveGameNotification()
    {
        if(!isSpectator)
        {
            string playerLeftRoomX =  ClientToServerSignifiers.LeaveGameNotification + "," + NetworkedClient.Instance.roomName;
            NetworkedClient.Instance.SendMessageToHost(playerLeftRoomX);
        }
        else
        {
            LeaveGame();
        }
    }


   public  void HelloClicked()
    {
        string msg = ClientToServerSignifiers.SendMessageToOtherPlayer + "," + "Hello!" ;
        NetworkedClient.Instance.SendMessageToHost(msg);
    }
    public void complimentClicked()
    {
        string msg = ClientToServerSignifiers.SendMessageToOtherPlayer + "," + "Hey! Nice move!";
        NetworkedClient.Instance.SendMessageToHost(msg);
    }
    public void rematchClicked()
    {
        string msg = ClientToServerSignifiers.SendMessageToOtherPlayer + "," + "Do you want to play again?";
        NetworkedClient.Instance.SendMessageToHost(msg);
    }
    public void noClicked()
    {
        string msg = ClientToServerSignifiers.SendMessageToOtherPlayer + "," + "NO!";
        NetworkedClient.Instance.SendMessageToHost(msg);
    }
    public void yesClicked()
    {
        string msg = ClientToServerSignifiers.SendMessageToOtherPlayer + "," + "YES!";
        NetworkedClient.Instance.SendMessageToHost(msg);
    }
    public IEnumerator DisableMessage2()
    {
        yield return new WaitForSeconds(2.0f);
        messagetoPlayer.GetComponent<Text>().text = ""; 
    }
    private void UpdateCursorVisibility()
    {
        if (theGameIsDone && (!isSpectator && !isReplayMode))
        {
            SetCursorVisibility(true);
            saveReplayButton.gameObject.SetActive(true);
            replayName.gameObject.SetActive(true);
            resetGameButton.gameObject.SetActive(true);
        }
   
    }

    public void SetCursorVisibility(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }




}


