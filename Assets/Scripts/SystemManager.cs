using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemManager : MonoBehaviour
{
    //Buttons
    [SerializeField] GameObject loginButton;
    [SerializeField] GameObject acountCreationButton;
    [SerializeField] GameObject createNewAccButton;

    //Input fields
    [SerializeField] public InputField username;
    [SerializeField] public InputField password;

    [SerializeField] public InputField newUsername;
    [SerializeField] public InputField newPassword;

    //New user ui
    [SerializeField]GameObject newUser;

    //message
    public GameObject messageInfo, messageAGranted, messageADenied, messageWrongUsername;


    public float timer;
    protected int connectionID;

    public int ConnectionID { get { return connectionID; } set { connectionID = value; } }

    // Start is called before the first frame update
    void Start()
    {
      DataManager.SetSystemManager(gameObject);
      NetworkedClient.SetSystemManager(gameObject);
    }
 

    public void ShowMessage()
    {
        if (DataManager.GetLoginIndex() == 1)
        {
            messageInfo.SetActive(false);
            messageAGranted.SetActive(true);
            StartCoroutine(DisableMessage());


        }
        if (DataManager.GetLoginIndex() == 0)
        {
            messageInfo.SetActive(false);
            messageADenied.SetActive(true);
            StartCoroutine(DisableMessage());

        }
        if(DataManager.GetLoginIndex() == 3)
        {
            messageInfo.SetActive(false);
            messageWrongUsername.SetActive(true);
            StartCoroutine(DisableMessage());
        }
            
 
    }
    public void LoginVerification()
    {
        DataManager.VerifyData();
    }
    public void AccountCreation()
    {
        newUser.SetActive(true);
    }

   
    //Function for create account button
    public void CreateNewAccount()
    {
        DataManager.SaveData();
        newUser.SetActive(false);
        newUsername.text = "";
        newPassword.text = "";

      
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

        // message.gameObject.SetActive(false);
        messageAGranted.SetActive(false);
        messageADenied.SetActive(false);
        messageWrongUsername.SetActive(false);
        messageInfo.SetActive(true);
       
        username.text = "";
        password.text = "";
    }

}
