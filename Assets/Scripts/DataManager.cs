using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class DataManager : MonoBehaviour
{
    static  GameObject sManager, Network;
    public static int indexOf = 0;


    static public void SetSystemManager(GameObject SystemManager)
    {
        sManager = SystemManager;
    }

  

    static public string GetNameForLogin()
    {
        return sManager.GetComponent<SystemManager>().GetUsername();
    }
    static public string GetPassWordForLogin()
    {
        return sManager.GetComponent<SystemManager>().GetPassWord();
    }
    static public string GetNewNameFromInput()
    {
        return sManager.GetComponent<SystemManager>().GetNewUsername();
    }

    static public string GetNewPassWordFromInput()
    {
        return sManager.GetComponent<SystemManager>().GetNewPassWord();
    }

    static public void SaveData()
    {
        using (StreamWriter sw = File.AppendText( GetNewNameFromInput() + ".txt"))
        {
            sw.WriteLine(GetNewPassWordFromInput());
        }

    }

    static public void VerifyData()
    {
        if (File.Exists(GetNameForLogin() + ".txt"))
        {
            using (StreamReader sr = new StreamReader(GetNameForLogin() + ".txt"))
            {
                string line = sr.ReadLine();

                Debug.Log(line);
                Debug.Log(GetPassWordForLogin());

                if ((line == GetPassWordForLogin()))
                {
                    Debug.Log(line);
                    Debug.Log("True");
                    indexOf = 1;
                    Debug.Log(GetLoginIndex());
                    sManager.GetComponent<SystemManager>().ShowMessage();
                    //NetworkedClient.Instance.Connect();
                    NetworkedClient.Instance.SendMessageToHost(GetNameForLogin());
                   // NetworkedClient.Instance.UpdateNetworkConnection();
                    //AddConnectionID(NetworkedClient.Instance.GetConnectionID());
                    

                }
                else
                {
                    Debug.Log(line);
                    Debug.Log("WrongPassword");
                    indexOf = 0;
                    Debug.Log(GetLoginIndex());
                    sManager.GetComponent<SystemManager>().ShowMessage();
                }
                sr.Close();
            }
            
        }
        else
        {
            indexOf = 3;
            sManager.GetComponent<SystemManager>().ShowMessage();
        }

        
    }

    static public void AddConnectionID(int connetionID)
    {
        Debug.Log(GetNameForLogin() + ".txt");

        if (File.Exists(GetNameForLogin() + ".txt"))
        {
           
            using (StreamWriter sw = File.AppendText(GetNameForLogin() + ".txt"))
            {
                Debug.Log("INSIDE");
                //sw.WriteLine(GetPassWordForLogin().ToString());
               
                sw.WriteLine(connetionID);

               sw.Close();
            }
        }
    }
    static public int GetLoginIndex()
    {
        return indexOf;
    }

    static public int SetLoginIndex(int data)
    {
        return data;
    }
}
