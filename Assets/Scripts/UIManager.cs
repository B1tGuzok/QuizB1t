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
    public GameObject endWindowUI;


    public GameObject chooseActionUI;
    public GameObject blockUserDataPanel;
    public GameObject closeGameLoginWindowUI;
    public GameObject blockLoginPanel;
    public GameObject closeGameRegisterWindowUI;
    public GameObject blockRegisterPanel;
    public GameObject settingsUI;
    public Text username;
    public TMP_InputField username_Input;


    public GameObject lvlButtonsUI;
    public bool lvlButtonsActive = false;
    public GameObject confirmEndGameWindow;
    public Text correctAnswers;
    public Text lives;
    public Sprite win;
    public Sprite lose;
    public Text endText;

    public List<Button> lvlButtons;


    //public Component[] lvlButtons; //for buttons component
    public Sprite unlockLvl;
    public Sprite lockLvl;
    public Button menuButton;

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

    public void ClearScreen() //Под Вопросом
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
        Clear();
        //openedLvls = FirebaseManager.instance.openedLvlsForUI;
        //UpdateLvlButtons();
        userDataUI.SetActive(true);
    }

    public void ScoreboardScreen() // Scoreboard button
    {
        scoreboardUI.SetActive(true);
    }

    public void OpenUsernamePanel()
    {
        changeUsernameUI.SetActive(true);
    }

    public void CloseUsernamePanel()
    {
        changeUsernameUI.SetActive(false);
    }

    public void UpdateLvlButtons()
    {
        foreach(Button btn in lvlButtons)
        {
            if (int.Parse(btn.GetComponentInChildren<TextMeshProUGUI>().text) > UserData.OpenedLvls_Player)
            {
                btn.interactable = false;
                btn.GetComponent<Image>().sprite = lockLvl;
            }
            else
            {
                btn.interactable = true;
                btn.GetComponent<Image>().sprite = unlockLvl;
            }
        }
    }

    public void LockLvlButtons()
    {
        foreach (Button btn in lvlButtons)
            btn.interactable = false;
    }

    public void UnlockLvlButtons() // Don't use
    {
        foreach (Button btn in lvlButtons)
            btn.interactable = true;
    }

    //NEW================================================

    public void GameScreen()
    {
        Clear();
        gameUI.SetActive(true);
    }

    public void Clear()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        gameUI.SetActive(false);
    }

    public void OpenConfirmEndGameWindow()
    {
        confirmEndGameWindow.SetActive(true);
    }

    public void CloseConfirmEndGameWindow()
    {
        confirmEndGameWindow.SetActive(false);
    }

    public void ToggleLvlButtons()
    {
        lvlButtonsActive = !lvlButtonsActive;
        lvlButtonsUI.SetActive(lvlButtonsActive);
    }

    public void UpdateGameLabels(int _correctAnswers, int _lives)
    {
        correctAnswers.text = _correctAnswers.ToString() + " / 10";
        lives.text = _lives.ToString();
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
        }
    }

    public void CloseEndWindow()
    {
        endWindowUI.SetActive(false);
    }

    public void onCloseGameButtonClick()
    {
        closeGameLoginWindowUI.SetActive(true);
        closeGameRegisterWindowUI.SetActive(true);
        blockLoginPanel.SetActive(true);
        blockRegisterPanel.SetActive(true);
    }

    public void onYesCloseGameButtonClick()
    {
        Application.Quit();
        Debug.Log("Game closed!");
    }

    public void onNoCloseGameButtonClick()
    {
        closeGameLoginWindowUI.SetActive(false);
        closeGameRegisterWindowUI.SetActive(false);
        blockLoginPanel.SetActive(false);
        blockRegisterPanel.SetActive(false);
    }
}