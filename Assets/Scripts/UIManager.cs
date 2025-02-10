using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject scoreboardUI;
    public GameObject gameUI;
    public GameObject changeUsernameUI;
    public GameObject lvlButtonsUI;
    public GameObject endWindowUI;

    public Component[] lvlButtons; //for buttons component
    public Sprite unlockLvl;
    public Sprite win;
    public Sprite lose;
    bool lvlButtonsActive = false;

    public Text correctAnswers;
    public Text lives;
    public Text endText;

    int openedLvls = 1;

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
        lvlButtonsActive = false;
        lvlButtonsUI.SetActive(false);
    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        FirebaseManager.instance.ClearLoginFields();
        loginUI.SetActive(true);
    }

    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        FirebaseManager.instance.ClearRegisterFields();
        registerUI.SetActive(true);
    }

    public void UserDataScreen() // Logged In
    {
        ClearScreen();
        openedLvls = FirebaseManager.instance.openedLvlsForUI;
        UpdateLvlButtons();
        userDataUI.SetActive(true);
    }

    public void ScoreboardScreen() // Scoreboard button
    {
        ClearScreen();
        scoreboardUI.SetActive(true);
    }

    public void OpenGameScreen()
    {
        gameUI.SetActive(true);
    }

    public void CloseGameScreen()
    {
        gameUI.SetActive(false);
        UserDataScreen();
    }

    public void OpenUsernamePanel()
    {
        changeUsernameUI.SetActive(true);
    }

    public void CloseUsernamePanel()
    {
        changeUsernameUI.SetActive(false);
    }

    public void OpenEndWindow(bool _win)
    {
        if (_win)
        {
            endWindowUI.GetComponent<Image>().sprite = win;
            endText.text = "Вы выиграли!";
            endWindowUI.SetActive(true);
        }
        else
        {
            endWindowUI.GetComponent<Image>().sprite = lose;
            endText.text = "Вы проиграли!";
            endWindowUI.SetActive(true);
            Debug.Log("ТЫ ЛОХ");
        }
    }

    public void CloseEndWindow()
    {
        endWindowUI.SetActive(false);
    }

    public void PlayButton()
    {
        lvlButtonsActive = !lvlButtonsActive;
        lvlButtonsUI.SetActive(lvlButtonsActive);
    }

    private void UpdateLvlButtons()
    {
        lvlButtons = lvlButtonsUI.GetComponentsInChildren<Button>(); //get all buttons component

        for (int i = 0; i < lvlButtons.Length; i++)
        {
            if (i + 1 > openedLvls)
            {
                lvlButtons[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                lvlButtons[i].GetComponent<Button>().interactable = true;
                ChangeLvlButtonSprite(lvlButtons[i].GetComponent<Image>());
            }
        }
    }

    private void ChangeLvlButtonSprite(Component Image)
    {
        Image.GetComponent<Image>().sprite = unlockLvl;
    }

    private void OnGUI()
    {
        correctAnswers.text = GameManager.instance.correctAnswers.ToString() + " / 10";
        lives.text = GameManager.instance.lives.ToString();
    }
}