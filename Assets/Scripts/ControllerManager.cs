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


    public int Player1Score;
    public int Player2Score;

    public Text player1ScoreText;
    public Text player2ScoreText;

    // Start is called before the first frame update
    void Start()
    {
        gameSetUp();
    }

    void gameSetUp()
    {
        turnofPlayer = 0;
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
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void onButtonClicked(int buttonIndex)
    {
        playerSpaces[buttonIndex].image.sprite = playerIcons[turnofPlayer];
        playerSpaces[buttonIndex].interactable = false;

        usedButton[buttonIndex] = turnofPlayer+1;
        turnCount++;

        if(turnCount > 4)
        {
            CheckWinCondition();
        }

        if(turnofPlayer == 0)
        {
            turnofPlayer = 1; 
            turnDisplay[1].SetActive(true); 
            turnDisplay[0].SetActive(false);
           
        }
        else
        {
            turnofPlayer = 0;
            turnDisplay[0].SetActive(true);
            turnDisplay[1].SetActive(false);

        }
    }

    public void CheckWinCondition() // 8 possibles way to win //3 horizontal lines //3 vertical lines and 2 diagonal lines
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
            if (solutions[i] == 3*(turnofPlayer+1))
            {
                Debug.Log("Player " + turnofPlayer + " won!");

                DisplayWinState(i);
                return;
            }
        }

    }

    void DisplayWinState(int index)
    {
        winnerText.gameObject.SetActive(true);
        if(turnofPlayer == 0)
        {
            winnerText.text = "Player X Wins!";

            Player1Score++;

            player1ScoreText.text = Player1Score.ToString();
        }
        else if(turnofPlayer ==1)
        {
            winnerText.text = "Player 0 Wins!";

            Player2Score++;
            player2ScoreText.text = Player2Score.ToString();
        }

        

        for (int i = 0; i < playerSpaces.Length; i++)
        {
            playerSpaces[i].interactable = false;//disable buttons
         
        }
        winLines[index].SetActive(true);// set active line at player index

    }

    public void ResetGameVariables()
    {
        gameSetUp();

        for (int i = 0; i < winLines.Length; i++)
        {
            winLines[i].SetActive(false);//Disable all lines
        }
        winnerText.text = "";



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

}
