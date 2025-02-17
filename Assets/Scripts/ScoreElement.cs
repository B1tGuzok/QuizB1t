using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text openedLvlsText;
    public TMP_Text correctAnswersText;
    public TMP_Text mistakesText;

    public void NewScoreElement(string _username, int _openedLvls, int _correctAnswers, int _mistakes)
    {
        usernameText.text = _username;
        openedLvlsText.text = _openedLvls.ToString();
        correctAnswersText.text = _correctAnswers.ToString();
        mistakesText.text = _mistakes.ToString();
    }
}
