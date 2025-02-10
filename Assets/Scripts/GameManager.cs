using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Player variables
    [Header("Player")]
    public int lives;
    public int mistakes;
    public int correctAnswers;

    //Question Data variables
    [Header("Question Data")]
    public Text question;
    public Button answer1;
    public Button answer2;
    public Button answer3;
    public Button answer4;
    public Button pressedButton;
    public Button rightButton;
    public Button menuButton;
    public string rightAnswer;
    public string userAnswer;
    public List<Question> questions;
    int questionID;

    //Sprites Data variables
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

    public void ShowQuestion()
    {
        menuButton.interactable = true;

        questionID = Random.Range(0, questions.Count);

        Debug.Log($"QuestionID - {questionID}");
        Debug.Log($"Count questions in GameManager - {questions.Count}");
        List<string> answers = new List<string>()
        {
            questions[questionID].answer1,
            questions[questionID].answer2,
            questions[questionID].answer3,
            questions[questionID].answer4
        };
        List<int> rnd = new List<int> { 10, 10, 10, 10 }; //can use any number except 0, 1, 2, 3
        for (int i = 0; i < rnd.Count; i++) //for random answer positions
        {
            int k;
            do
            {
                k = Random.Range(0, 4);
            }
            while (rnd.Contains(k));
            rnd[i] = k;
        }

        question.text = questions[questionID].question;
        answer1.GetComponentInChildren<TextMeshProUGUI>().text = answers[rnd[0]];
        answer2.GetComponentInChildren<TextMeshProUGUI>().text = answers[rnd[1]];
        answer3.GetComponentInChildren<TextMeshProUGUI>().text = answers[rnd[2]];
        answer4.GetComponentInChildren<TextMeshProUGUI>().text = answers[rnd[3]];
        rightAnswer = questions[questionID].rightAnswer;

        List<Button> searchRightButton = new List<Button>() { answer1, answer2, answer3, answer4 };
        for (int i = 0; i < searchRightButton.Count; i++)
        {
            if (searchRightButton[i].GetComponentInChildren<TextMeshProUGUI>().text == rightAnswer)
                rightButton = searchRightButton[i];
            else
                Debug.Log("Error! Right Button didn't found!");
        }

        questions.RemoveAt(questionID);
    }

    public void CheckAnswer(Button pressedBtn)
    {
        pressedButton = pressedBtn;
        answer1.GetComponent<Button>().interactable = false;
        answer2.GetComponent<Button>().interactable = false;
        answer3.GetComponent<Button>().interactable = false;
        answer4.GetComponent<Button>().interactable = false;

        userAnswer = pressedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        if (userAnswer == rightAnswer) 
        {
            pressedButton.GetComponent<Image>().sprite = rightSprite;
            correctAnswers++;
        }
        else
        {
            pressedButton.GetComponent<Image>().sprite = wrongSprite;
            rightButton.GetComponent<Image>().sprite = rightSprite;
            lives--;
            mistakes++;
        }

        if (mistakes == 3)
        {
            menuButton.interactable = false;
            UIManager.instance.OpenEndWindow(false);
            Invoke("EndGame", 1.5f);
        }
        else if (correctAnswers == 5)
        {
            menuButton.interactable = false;
            UIManager.instance.OpenEndWindow(true);
            if (UserData.currentLvl == UserData.OpenedLvls_Player) //open new lvl
            {
                UserData.OpenedLvls_Player += 1;
                StartCoroutine(FirebaseManager.instance.UpdateOpenedLvls(UserData.OpenedLvls_Player));
                //EndGame() and UIManager.instance.CloseEndWindow() I transferred to UpdateOpenedLvls Coroutine in FirebaseManager.cs
            }
        }
        else
        {
            Invoke("AfterCheckAnswer", 1.5f);
        }
    }

    private void AfterCheckAnswer()
    {
        pressedButton.GetComponent<Image>().sprite = defaultSprite;
        rightButton.GetComponent<Image>().sprite = defaultSprite;
        ShowQuestion();
        answer1.GetComponent<Button>().interactable = true;
        answer2.GetComponent<Button>().interactable = true;
        answer3.GetComponent<Button>().interactable = true;
        answer4.GetComponent<Button>().interactable = true;
    }

    public void EndGame()
    {
        pressedButton.GetComponent<Image>().sprite = defaultSprite;
        rightButton.GetComponent<Image>().sprite = defaultSprite;
        answer1.GetComponent<Button>().interactable = true;
        answer2.GetComponent<Button>().interactable = true;
        answer3.GetComponent<Button>().interactable = true;
        answer4.GetComponent<Button>().interactable = true;
        UserData.CorrectAnswers_Player += correctAnswers;
        UserData.Mistakes_Player += mistakes;
        StartCoroutine(FirebaseManager.instance.UpdateCorrectAnswers(UserData.CorrectAnswers_Player));
        StartCoroutine(FirebaseManager.instance.UpdateMistakes(UserData.Mistakes_Player));
        questions.Clear();
        UIManager.instance.CloseEndWindow();
        UIManager.instance.CloseGameScreen();
    }
}
