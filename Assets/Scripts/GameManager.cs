using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Player variables
    [Header("Player Data")]
    public int lives;
    public int mistakes;
    public int correctAnswers;

    //Question variables
    [Header("Question Data")]
    public Text questionTitle;
    public List<Question> questions = new List<Question>();
    public List<Button> answerButtons;
    public string rightAnswer;
    public string userAnswer;
    public Button pressedYesButton; // To avoid double click
    public Button pressedNoButton; // To avoid unnecessary closure of confirmWindow
    public Button rightButton; // To set right sprite
    public Button exitButton; // To hide exit button

    //Sprites variables
    [Header("Sprites Data")]
    public Sprite rightSprite;
    public Sprite wrongSprite;
    public Sprite defaultSprite;



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

    public void onPlayButtonClick()
    {
        UIManager.instance.ToggleLvlButtons();
    }

    public void onLvlButtonClick(string _selectedLvl)
    {
        UserData.currentLvl = int.Parse(_selectedLvl); // Save player's choice for opening new lvls
        lives = 3;
        mistakes = 0;
        correctAnswers = 0;
        UIManager.instance.LockLvlButtons();
        UIManager.instance.UpdateGameLabels(correctAnswers, lives);
        StartCoroutine(FirebaseManager.instance.LoadQuestions(_selectedLvl, questions));
    }

    public void onMenuButtonClick()
    {
        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Button>().interactable = false;
        }
        UIManager.instance.OpenConfirmEndGameWindow();
    }

    public void onYesConfirmEndGameButtonClick()
    {
        pressedYesButton.interactable = false;
        pressedNoButton.interactable = false;
        StartCoroutine(EndGame());
    }

    public void onNoConfirmEndGameButtonClick()
    {
        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Button>().interactable = true;
        }
        UIManager.instance.CloseConfirmEndGameWindow();
    }

    public void onExitButtonClick()
    {
        UIManager.instance.chooseActionUI.SetActive(true);
        UIManager.instance.blockUserDataPanel.SetActive(true);
        exitButton.gameObject.SetActive(false);
    }

    public void onChangeAccountButtonClick()
    {
        UIManager.instance.chooseActionUI.SetActive(false);
        UIManager.instance.blockUserDataPanel.SetActive(false);
        UIManager.instance.blockLoginPanel.SetActive(false);
        UIManager.instance.blockRegisterPanel.SetActive(false);
        exitButton.gameObject.SetActive(true);
        FirebaseManager.instance.SignOut();
    }

    public void onCloseGameButtonClick()
    {
        Application.Quit();
        Debug.Log("Game closed!");
    }

    public void onCloseChooseActionUIButtonClick()
    {
        exitButton.gameObject.SetActive(true);
        UIManager.instance.chooseActionUI.SetActive(false);
        UIManager.instance.blockUserDataPanel.SetActive(false);
    }

    public void onSettingsButtonClick()
    {
        UIManager.instance.username_Input.text = UserData.Username_Player;
        UIManager.instance.settingsUI.SetActive(true);
        UIManager.instance.blockUserDataPanel.SetActive(true);
    }

    public void onCloseSettingsButtonClick()
    {
        UIManager.instance.settingsUI.SetActive(false);
        UIManager.instance.blockUserDataPanel.SetActive(false);
    }

    public void onSaveSettingsButtonClick()
    {
        AudioManager.instance.Save();
        string newUsername = UIManager.instance.username_Input.text;
        if (newUsername != UserData.Username_Player)
        {
            UIManager.instance.username.text = newUsername;
            UserData.Username_Player = newUsername;
            StartCoroutine(FirebaseManager.instance.UpdateUsernameAuth(newUsername));
            StartCoroutine(FirebaseManager.instance.UpdateUsernameDatabase(newUsername));
        }
        onCloseSettingsButtonClick();
    }

    public void onScoreboardButtonClick()
    {
        UIManager.instance.blockUserDataPanel.SetActive(true);
        StartCoroutine(FirebaseManager.instance.LoadScoreboardData());
    }

    public void onCloseScoreboardButtonClick()
    {
        UIManager.instance.blockUserDataPanel.SetActive(false);
        UIManager.instance.scoreboardUI.SetActive(false);
    }

    public void ShowQuestion()
    {
        int questionID = Random.Range(0, questions.Count);

        questionTitle.text = questions[questionID].question;

        List<string> answers = new List<string>()
        {
            questions[questionID].answer1,
            questions[questionID].answer2,
            questions[questionID].answer3,
            questions[questionID].answer4
        };

        List<int> rnd = new List<int> { 10, 10, 10, 10 }; //can use any number except 0, 1, 2, 3
        for (int i = 0; i < rnd.Count; i++) // Suffle rnd array
        {
            int k;
            do
            {
                k = Random.Range(0, 4);
            }
            while (rnd.Contains(k));
            rnd[i] = k;
        }

        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[rnd[i]];
        }

        rightAnswer = questions[questionID].rightAnswer;

        questions.RemoveAt(questionID);

        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Button>().interactable = true;
            btn.GetComponent<Image>().sprite = defaultSprite;
        }
    }

    public void CheckAnswer(Button _pressedBtn)
    {
        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Button>().interactable = false; // Except double click
            if (btn.GetComponentInChildren<TextMeshProUGUI>().text == rightAnswer) // Search right button for wrong sprite
                rightButton = btn;
        }

        userAnswer = _pressedBtn.GetComponentInChildren<TextMeshProUGUI>().text;

        if (userAnswer == rightAnswer)
        {
            _pressedBtn.GetComponent<Image>().sprite = rightSprite;
            correctAnswers++;
        }
        else
        {
            _pressedBtn.GetComponent<Image>().sprite = wrongSprite;
            rightButton.GetComponent<Image>().sprite = rightSprite;
            lives--;
            mistakes++;
        }

        UIManager.instance.UpdateGameLabels(correctAnswers, lives); //Instead of OnGUI()

        if (mistakes == 3)
        {
            UIManager.instance.menuButton.interactable = false;
            UIManager.instance.OpenEndWindow(false);
            StartCoroutine(EndGame());
        }
        else if (correctAnswers == 10)
        {
            UIManager.instance.menuButton.interactable = false;
            UIManager.instance.OpenEndWindow(true);
            if (UserData.currentLvl == UserData.OpenedLvls_Player && UserData.currentLvl < 5) // Open new lvl
            {
                UserData.OpenedLvls_Player = UserData.currentLvl + 1;
            }
            StartCoroutine(EndGame());
        }
        else
        {
            Invoke("ShowQuestion", 1.5f);
        }
    }

    public IEnumerator EndGame()
    {
        UserData.CorrectAnswers_Player += correctAnswers;
        UserData.Mistakes_Player += mistakes;
        StartCoroutine(FirebaseManager.instance.UpdateOpenedLvls(UserData.OpenedLvls_Player));
        StartCoroutine(FirebaseManager.instance.UpdateCorrectAnswers(UserData.CorrectAnswers_Player));
        StartCoroutine(FirebaseManager.instance.UpdateMistakes(UserData.Mistakes_Player));

        yield return new WaitForSeconds(2);

        Debug.Log("Closing game!");
        questions.Clear();
        UIManager.instance.UpdateLvlButtons();
        UIManager.instance.lvlButtonsActive = false;
        UIManager.instance.lvlButtonsUI.SetActive(false); // Don't use ToggleLvlButtons because user can click on playButton twice
        UIManager.instance.CloseConfirmEndGameWindow();
        UIManager.instance.CloseEndWindow();
        UIManager.instance.UserDataScreen();
        UIManager.instance.menuButton.interactable = true;

        pressedYesButton.interactable = true;
        pressedNoButton.interactable = true;
    }
}
