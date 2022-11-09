using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{
    public int turnofPlayer; // 0 = X player  & 1 = O Player
    public int turnCount;
    public GameObject[] turnDisplay;//shows whos turn it is
    public Sprite[] playerIcons;
    public Button[] playerSpaces; //Playable space for game
    public int[] usedButton;

    public Text winnerText;//holds text component of winner text
    public GameObject[] winLines;//holds lines of winning display

    bool gameDone = false;

    public int Player1Score;
    public int Player2Score;

    public Text player1ScoreText;
    public Text player2ScoreText;

    public GameObject resetGameButton;

    public static ControllerManager Instance;

    private void Awake()
    {
        Instance = this;
      
    }
    void Start()
    {
        
        
    }

    public void gameSetUp()
    {
        turnofPlayer = NetworkedClient.Instance.turnOfPlayer;
        turnCount = 0;
        turnDisplay[0].SetActive(true);
        turnDisplay[1].SetActive(false);

        for (int i = 0; i < playerSpaces.Length; i++)
        {
            playerSpaces[i].interactable = true;// make all the buttons interactables
            playerSpaces[i].GetComponent<Image>().sprite = null;
        }

        for(int i = 0; i< usedButton.Length; i++)
        {
            usedButton[i] = -200;
        }

        if(turnofPlayer == 1)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(turnofPlayer == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(gameDone)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            winnerText.gameObject.SetActive(true);
            resetGameButton.gameObject.SetActive(true);
        }
        
    }

    public void onButtonClicked(int buttonIndex)
    {
        turnCount++;
        playerSpaces[buttonIndex].image.sprite = playerIcons[turnofPlayer];
        playerSpaces[buttonIndex].interactable = false;
        usedButton[buttonIndex] = turnofPlayer + 1;

        NotifyServer(buttonIndex, turnofPlayer);

        if (turnCount > 4)
        {
            CheckWinCondition(turnofPlayer);
        }
        //Player X has made a move notify button index pressed and which player made a move before 
      
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


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


        

        if (playerTurn == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            turnofPlayer = 1;
            turnDisplay[1].SetActive(true);
            turnDisplay[0].SetActive(false);

        }
        if (playerTurn == 1)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            turnofPlayer = 0;
            turnDisplay[0].SetActive(true);
            turnDisplay[1].SetActive(false);

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

                DisplayWinState(i);
                
                return;
            }
        }

    }

    void DisplayWinState(int index)
    {
        

        gameDone = true;

        if (index == 0)
        {
            winnerText.text = "Player X Wins!";

            Player1Score++;

            player1ScoreText.text = Player1Score.ToString();
        }
        else if(index == 1)
        {
            winnerText.text = "Player 0 Wins!";

            Player2Score++;
            player2ScoreText.text = Player2Score.ToString();
        }

        //for (int i = 0; i < playerSpaces.Length; i++)
        //{
        //    playerSpaces[i].interactable = false;//disable buttons
         
        //}
        winLines[index].SetActive(true);// set active line at player index

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void ResetGameVariables()
    {
        gameDone=false;
        winnerText.gameObject.SetActive(false);
        resetGameButton.gameObject.SetActive(false);
        gameSetUp();
        for (int i = 0; i < winLines.Length; i++)
        {
            winLines[i].SetActive(false);//Disable all lines
        }
        winnerText.text = "";

    }

    public void ResetGame()
    {
        Debug.Log("Reset Game");
        string reset = "5," + NetworkedClient.Instance.roomName;
        NetworkedClient.Instance.SendMessageToHost(reset);
        Debug.Log("MessageSent to Server");
    }

public void LeaveGame()
    {

        ResetGameVariables();
        Player1Score = 0;
        Player2Score = 0;
        player1ScoreText .text= "";
        player1ScoreText.text = "";
        player2ScoreText.text = "";

        SystemManager.Instance.LeaveGame();
        //How to expulse other player from game and gameroom, to lobby?

    }


    public void NotifyServer(int buttonIndex, int turnOfPlayerX)
    {
        Debug.Log("PLAYER X MOVED, Pressed button at index " + buttonIndex + " and it was turnofPlayer = " + turnOfPlayerX);
        string playerMoved = "4," + buttonIndex.ToString() + "," + turnOfPlayerX.ToString();
        NetworkedClient.Instance.SendMessageToHost(playerMoved);
        Debug.Log("MessageSent to Server");
    }
}