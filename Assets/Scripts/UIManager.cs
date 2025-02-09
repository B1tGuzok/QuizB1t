using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject scoreboardUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ClearScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }

    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void UserDataScreen() // Logged In
    {
        ClearScreen();
        userDataUI.SetActive(true);
    }

    public void ScoreboardScreen() // Scoreboard button
    {
        ClearScreen();
        scoreboardUI.SetActive(true);
    }
}