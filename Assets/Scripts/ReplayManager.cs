using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    private int buttonindexReplay;
    private int turnOfReplay = 0;
    private ControllerManager controllerManager;
    public void Initialize(ControllerManager controller)
    {
        controllerManager = controller;
    }
    public void SetTurnOfReplay(int value)
    {
        turnOfReplay = value;
    }
    public IEnumerator ReplayMoves()
    {
        for (int i = 0; i < NetworkedClient.Instance.playerdata.Length; i++)
        {
            yield return new WaitForSeconds(1.5f);
            buttonindexReplay = int.Parse(NetworkedClient.Instance.playerdata[i].ToString());
            controllerManager.onButtonClickedForReplay(buttonindexReplay, turnOfReplay);

            if (turnOfReplay == 0)
            {

                turnOfReplay = 1;
                controllerManager.turnDisplay[1].SetActive(true);
                controllerManager.turnDisplay[0].SetActive(false);
            }
            else if (turnOfReplay == 1)
            {
                turnOfReplay = 0;
                controllerManager.turnDisplay[0].SetActive(true);
                controllerManager.turnDisplay[1].SetActive(false);

            }
        }
        yield return new WaitForSeconds(3.0f);

        controllerManager.LeaveGame();
    }
    public void SaveReplay()
    {
        string integerofplayer = ClientToServerSignifiers.SaveReplayData + "," + controllerManager.turnoOfPlayer.ToString() + ","
            + controllerManager.replayName.text.ToString();
        NetworkedClient.Instance.SendMessageToHost(integerofplayer);
        controllerManager.messagetoPlayer.GetComponent<Text>().text = "Match replay saved!";
        controllerManager.saveReplayButton.gameObject.GetComponent<Button>().interactable = false;
        controllerManager.replayName.text = "";
        StartCoroutine(controllerManager.DisableMessage2());
    }

}
