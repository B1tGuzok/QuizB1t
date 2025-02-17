using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public struct Question
{
    public string question;
    public string answer1;
    public string answer2;
    public string answer3;
    public string answer4;
    public string rightAnswer;

    public Question(string que, string ans1, string ans2, string ans3, string ans4, string rans)
    {
        question = que;
        answer1 = ans1;
        answer2 = ans2;
        answer3 = ans3;
        answer4 = ans4;
        rightAnswer = rans;
    }
}

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBReference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //UserData variables
    [Header("UserData")]
    public GameObject scoreElement;
    public Transform scoreBoardContent;

    //Lvls variables
    [Header("Lvls")]
    public GameObject firstBtn, secondBtn, thirdBtn;
    //public List<Question> questions = new List<Question>(); //questions for ONE lvl
    public int openedLvlsForUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        DBReference = FirebaseDatabase.DefaultInstance.RootReference;
        //DBReference.KeepSynced(true); //DELETE WHEN I FINISH DEVELOPMENT. NO NEED TO SYNCHRONIZE DATA
    }

    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
        warningLoginText.text = "";
    }

    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        warningRegisterText.text = "";
    }

    public void LoginButton()
    {
        UIManager.instance.blockLoginPanel.SetActive(true);
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void SignOut()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        UserData.ClearData();
        ClearRegisterFields();
        ClearLoginFields();
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Ошибка!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Введите почту!";
                    break;
                case AuthError.MissingPassword:
                    message = "Введите пароль";
                    break;
                case AuthError.WrongPassword:
                    message = "Неправильный пароль";
                    break;
                case AuthError.UserNotFound:
                    message = "Аккаунта с такой почтой не существует!";
                    break;
            }
            warningLoginText.text = message;
            UIManager.instance.blockLoginPanel.SetActive(false);
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Вы вошли!";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            UIManager.instance.username.text = UserData.Username_Player;
            UIManager.instance.UpdateLvlButtons();
            UIManager.instance.UserDataScreen();
            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Введите никнейм!";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Пароли не совпадают!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Ошибка!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Введите почту!";
                        break;
                    case AuthError.MissingPassword:
                        message = "Введите пароль!";
                        break;
                    case AuthError.WeakPassword:
                        message = "Пароли не совпадают!";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Почта уже используется!";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        StartCoroutine(UpdateUsernameDatabase(_username));
                        yield return new WaitForSeconds(1);
                        StartCoroutine(UpdateOpenedLvls(1));
                        StartCoroutine(UpdateCorrectAnswers(0));
                        StartCoroutine(UpdateMistakes(0));
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
            }
        }
    }

    public IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }


    public IEnumerator UpdateUsernameDatabase(string _username) // Also create user in database
    {
        var DBTask = DBReference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    public IEnumerator UpdateCorrectAnswers(int _correctAnswers)
    {
        var DBTask = DBReference.Child("users").Child(User.UserId).Child("correctAnswers").SetValueAsync(_correctAnswers);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("CorrectAnswers updated!");
        }
    }

    public IEnumerator UpdateMistakes(int _mistakes)
    {
        var DBTask = DBReference.Child("users").Child(User.UserId).Child("mistakes").SetValueAsync(_mistakes);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("Mistakes updated!");
        }
    }

    public IEnumerator UpdateOpenedLvls(int _openedLvls)
    {
        var DBTask = DBReference.Child("users").Child(User.UserId).Child("openedLvls").SetValueAsync(_openedLvls);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //OpenedLvls are now updated
            Debug.Log("OpenedLvls updated!");
        }
    }

    private IEnumerator LoadUserData()
    {
        var DBTask = DBReference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exist
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            UserData.Username_Player = snapshot.Child("username").Value.ToString();
            UserData.CorrectAnswers_Player = int.Parse(snapshot.Child("correctAnswers").Value.ToString());
            UserData.OpenedLvls_Player = int.Parse(snapshot.Child("openedLvls").Value.ToString());
            Debug.Log(UserData.OpenedLvls_Player);
            UserData.Mistakes_Player = int.Parse(snapshot.Child("mistakes").Value.ToString());
        }
    }

    public IEnumerator LoadScoreboardData()
    {
        var DBTask = DBReference.Child("users").OrderByChild("openedLvls").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            // Clear Scoreboard
            foreach (Transform child in scoreBoardContent.transform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int openedLvls = int.Parse(childSnapshot.Child("openedLvls").Value.ToString());
                int mistakes = int.Parse(childSnapshot.Child("mistakes").Value.ToString());
                int correctAnswers = int.Parse(childSnapshot.Child("correctAnswers").Value.ToString());

                // Create userData string
                GameObject scoreboardElement = Instantiate(scoreElement, scoreBoardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, openedLvls, correctAnswers, mistakes);
            }

            UIManager.instance.ScoreboardScreen();
        }
    }

    //=======================================FOR QUESTIONS=======================================

    public IEnumerator LoadQuestions(string _selectedLvl, List<Question> _questions)
    {
        var DBTask = DBReference.Child("lvls").Child(_selectedLvl).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exist
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            Debug.Log(snapshot.ChildrenCount);

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string question = childSnapshot.Child("question").Value.ToString();
                Debug.Log(question);
                string answer1 = childSnapshot.Child("answer1").Value.ToString();
                string answer2 = childSnapshot.Child("answer2").Value.ToString();
                string answer3 = childSnapshot.Child("answer3").Value.ToString();
                string answer4 = childSnapshot.Child("answer4").Value.ToString();
                string rightAnswer = childSnapshot.Child("rightAnswer").Value.ToString();

                _questions.Add(new Question(question, answer1, answer2, answer3, answer4, rightAnswer));
            }
            GameManager.instance.ShowQuestion();
            UIManager.instance.GameScreen();

            Debug.Log($"Count questions in FirebaseManager - {_questions.Count}");
        }
    }

    //============================================FOR DEBUG=================================DELETE AFTER END OF DEVELOPMENT

    public void EnterData()
    {
        emailLoginField.text = "b1t@gmail.com";
        passwordLoginField.text = "b1t12345";
    }
}