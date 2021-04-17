// Authors: Jethro, Su Te, Daryl, Zhi Fah
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public GameObject loginUI;
    public GameObject userDataUI;

    [SerializeField] private AudioClip cfmClickSFX;
    public LevelLoader levelLoader;
    //[SerializeField] QuestionManager questionManager;
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public static FirebaseUser User;
    public DatabaseReference DBreference;

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
    public TMP_Text confirmRegisterText;
    public InputField UniquePin;

  //User Data variables
    [Header("UserData")]
    public TMP_Text usernameTitle;
    public TMP_Text totalStars;
    public TMP_Text totalPoints;
    public TMP_Text selectedStars;
    public TMP_Text selectedPoints;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    public TMP_Text ranktext;

    public static string username;

    public Toggle register_toggle;

    private static FirebaseManager instance;

    public static FirebaseManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<FirebaseManager>();
                if(instance == null)
                {
                    instance = new GameObject("Spawned FirebaseManager", typeof(FirebaseManager)).GetComponent<FirebaseManager>();
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    void Awake()
    {
        // Set gameobject as  DontDestroyOnLoad
        DontDestroyOnLoad(this.gameObject);
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
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
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        UniquePin.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        //questionManager.Awake();
        //Call the login coroutine passing the email and password
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        if (CheckInput(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text) == true)
        {
            if (register_toggle.isOn & UniquePin.text == "PIN1")
            {
                StartCoroutine(TeacherRegister(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text)); // Unique pin can get from database
            }
            //Call the register coroutine passing the email, password, and username
            else if (register_toggle.isOn & UniquePin.text != "PIN1"){
                warningRegisterText.text = "Invalid Pin!";
            }
            else
            {
                StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
            }
      }
    }
    //Function for the sign out button
    public void SignOutButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFeilds();
        ClearLoginFeilds();
    }

    public void EnterGameButton() {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearRegisterFeilds();
        ClearLoginFeilds();
        //levelLoader.LoadCharSel();
    }

    //Function for the scoreboard button
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    //Function for the save button
    // public void SaveDataButton()
    // {   
    //     AudioManager.Instance.PlaySFX(cfmClickSFX);
    //     StartCoroutine(UpdateStars(1,1,1,0));
    //     StartCoroutine(UpdateUsernameAuth(usernameField.text));
    //     StartCoroutine(UpdateUsernameDatabase(usernameField.text));
    //     StartCoroutine(UpdateKills(int.Parse(killsField.text)));
    // }

    //Function for the scoreboard button
    // public void ScoreboardButton()
    // {        
    //     StartCoroutine(LoadScoreboardData());
    // }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            username = User.DisplayName;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(CheckTeacher(_email));
            //StartCoroutine(loadMainMenu());

            yield return new WaitForSeconds(1);

            //usernameField.text = User.DisplayName;
            //UIManager.instance.UserDataScreen(); // Change to user data UI
            confirmLoginText.text = "";
            initialisePlayerPrefStars();
            ClearLoginFeilds();
            ClearRegisterFeilds();
            // SceneManager.LoadScene("Character Selection");
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        var DBTask = DBreference.Child("users").GetValueAsync(); // add
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted); // add
        bool exist = false; // add
        // if (_username == "")
        // {
        //     //If the username field is blank show a warning
        //     warningRegisterText.text = "Missing Username";
        // }
        // else if(_email == ""){
        //     warningRegisterText.text = "Missing Email";
        // }
        // else if(CheckPassword(_password) == false){
        //     warningRegisterText.text = "Password Requires >=6 Characters With Lowercase Letter, Uppercase Letter And Digit!";
        // }
        // else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        // {
        //     //If the password does not match show a warning
        //     warningRegisterText.text = "Password Does Not Match!";
        // }
        // if (CheckInput(_email, _password, _username) == true) 
        // {
             //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;// add
            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children) // add to
            {
                string existing_user = childSnapshot.Child("username").Value.ToString(); 
                if (existing_user == _username){
                    warningRegisterText.text = "Username Taken! Please Choose Another Username!";
                    exist = true;
                }
            }
            if(exist == false) // here
            {
                //Call the Firebase auth signin function passing the email and password
                var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                //Wait until the task completes
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    //If there are errors handle them
                    Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    string message = "Register Failed!";
                    switch (errorCode)
                    {
                        // case AuthError.MissingEmail:
                        //     message = "Missing Email";
                        //     break;
                        // case AuthError.MissingPassword:
                        //     message = "Missing Password";
                        //     break;
                        // case AuthError.WeakPassword:
                        //     message = "Weak Password";
                        //     break;
                        case AuthError.EmailAlreadyInUse:
                            message = "Email Already In Use";
                            break;
                    }
                    warningRegisterText.text = message;
                }
                else
                {
                    //User has now been created
                    //Now get the result
                    User = RegisterTask.Result;

                    if (User != null)
                    {
                        //Create a user profile and set the username
                        UserProfile profile = new UserProfile{DisplayName = _username};

                        //Call the Firebase auth update user profile function passing the profile with the username
                        var ProfileTask = User.UpdateUserProfileAsync(profile);
                        //Wait until the task completes
                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                        // StartCoroutine(UpdateInnerStars(0));
                        InitialisePlayerProfile();
                        StartCoroutine(UpdateUsernameDatabase(_username));
                        if (ProfileTask.Exception != null)
                        {
                            //If there are errors handle them
                            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                            warningRegisterText.text = "Username Set Failed!";
                        }
                        else
                        {
                            //Username is now set
                            //Now return to login screen
                            warningRegisterText.text = "";
                            confirmRegisterText.text = "Account created successfully";
                            yield return new WaitForSeconds(1f);
                            confirmRegisterText.text = "";
                            UIManager.instance.LoginScreen();                        
                            ClearRegisterFeilds();
                            ClearLoginFeilds();
                        }
                    }
                }
            // }
        }
    }

    private IEnumerator TeacherRegister(string _email, string _password, string _username)
    {
        // if (_username == "")
        // {
        //     //If the username field is blank show a warning
        //     warningRegisterText.text = "Missing Username";
        // }
        // else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        // {
        //     //If the password does not match show a warning
        //     warningRegisterText.text = "Password Does Not Match!";
        // }
        
        // if (CheckInput(_email, _password, _username) == true) 
        // {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    // case AuthError.MissingEmail:
                    //     message = "Missing Email";
                    //     break;
                    // case AuthError.MissingPassword:
                    //     message = "Missing Password";
                    //     break;
                    // case AuthError.WeakPassword:
                    //     message = "Weak Password";
                    //     break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _email };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                    // StartCoroutine(UpdateInnerStars(0));
                    StartCoroutine(UpdateTeacherUsernameDatabase(_email));
                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        warningRegisterText.text = "";
                        confirmRegisterText.text = "Teacher Account Created Successfully";
                        yield return new WaitForSeconds(1f);
                        confirmRegisterText.text = "";
                        UIManager.instance.LoginScreen();
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            // }
        }
    }

    public bool CheckPassword(string password){
        if (password.Any(char.IsLetter) && password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Length>=6){
            return true;
            Debug.Log("pass");
        }
        else{
            // warningRegisterText.text = "Password requires at least 1 uppercase letter, lowercase letter and a digit!";
            return false;
        }
    }

    public bool CheckInput(string _email, string _password, string _username){
        bool pass = false;
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if(_email == ""){
            warningRegisterText.text = "Missing Email";
        }

        else if (_password.Length<6){
            warningRegisterText.text = "Password Must Contain At Least 6 Characters";
        }
        else if(CheckPassword(_password)==false){
            warningRegisterText.text = "Password Must Contain A Lowercase Letter, Uppercase Letter And Number!";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else{
            pass = true;
        }
        return pass;
    }

    public void ToggleTeacher()
    {
        if (register_toggle.isOn)
        {
            UniquePin.gameObject.SetActive(true);
        }
        else
        {
            UniquePin.gameObject.SetActive(false);

        }
    }

    private IEnumerator UpdateStars(int starsworld, int starssection, int starslevel, int input)
    {   
        string starworld = starsworld.ToString();
        string starsection = starssection.ToString();
        string starlevel = starslevel.ToString();
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("stars").Child(starworld).Child(starsection).Child(starlevel).SetValueAsync(input);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }

    public void UniversalUpdateStars(int starsworld, int starssection, int starlevel, int input){
        StartCoroutine(UpdateStars(starsworld,starssection, starlevel, input));
    }

    private IEnumerator UpdateBattleStats(int starsworld,int section, int input)
    {   
        string starsection = section.ToString();
        string starworld = starsworld.ToString();
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("BattleStats").Child(starworld).Child(starsection).Child("Points").SetValueAsync(input);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }

    public void initialisePlayerPrefStars() 
    {
        StartCoroutine(loadPlayerPrefStars());
    }

    private IEnumerator loadPlayerPrefStars() 
    {
        Debug.Log("reached here at load before");
        Debug.Log($"{User.UserId}");
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("stars").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        Debug.Log("reached here at load");

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            int index = 1;
            for (int world = 1; world < 4; world++) {
                for (int section = 1; section < 4; section++) {
                    int totalStarsSection = 0;
                    for (int level = 1; level < 5;  level++) {
                        if (world == 1) {
                            if (section == 1) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level}", value);
                            }
                            else if (section == 2) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+4}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+4}", value);
                            }
                            else if (section == 3) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+8}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+8}", value);
                            }
                            
                        }
                        if (world == 2) {
                            if (section == 1) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+12}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+12}", value);
                                
                            }
                            else if (section == 2) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+16}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+16}", value);
                            }
                            else if (section == 3) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+20}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+20}", value);
                            }
                            //PlayerPrefs.SetInt($"{world}.{section}", totalStarsSection);
                        }
                        if (world == 3) {
                            if (section == 1) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+24}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+24}", value);
                            }
                            else if (section == 2) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+28}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+28}", value);
                            }
                            else if (section == 3) {
                                int value = int.Parse(snapshot.Child($"{world}").Child($"{section}").Child($"{level+32}").Value.ToString());
                                Debug.Log($"{value}");
                                totalStarsSection += value;
                                PlayerPrefs.SetInt($"{level+32}", value);
                            }
                            
                        }
                    } 
                    string ind = index.ToString();
                    PlayerPrefs.SetInt("Lv" + ind, totalStarsSection);
                    Debug.Log($"{totalStarsSection}");
                    Debug.Log("Lv" + ind);
                    index++;
                }
            }
        }
        //levelLoader.LoadCharSel();
    }
    // for (int i = 0; i<snapshot.ChildrenCount; i++) {
    //             int value = int.Parse(snapshot.Child($"{i+1}").Value.ToString());
    //             Debug.Log($"{value}");
    //         }

    private void InitialisePlayerProfile()
    {   
        StartCoroutine(UpdateTotalPoints());
        StartCoroutine(UpdateBattleStats(1,1,0));
        StartCoroutine(UpdateBattleStats(1,2,0));
        StartCoroutine(UpdateBattleStats(1,3,0));
        StartCoroutine(UpdateBattleStats(2,1,0));
        StartCoroutine(UpdateBattleStats(2,2,0));
        StartCoroutine(UpdateBattleStats(2,3,0));
        StartCoroutine(UpdateBattleStats(3,1,0));
        StartCoroutine(UpdateBattleStats(3,2,0));
        StartCoroutine(UpdateBattleStats(3,3,0));

        //register stars
        StartCoroutine(UpdateStars(1,1,1,0));
        StartCoroutine(UpdateStars(1,1,2,0));
        StartCoroutine(UpdateStars(1,1,3,0));
        StartCoroutine(UpdateStars(1,1,4,0));

        StartCoroutine(UpdateStars(1,2,5,0));
        StartCoroutine(UpdateStars(1,2,6,0));
        StartCoroutine(UpdateStars(1,2,7,0));
        StartCoroutine(UpdateStars(1,2,8,0));

        StartCoroutine(UpdateStars(1,3,9,0));
        StartCoroutine(UpdateStars(1,3,10,0));
        StartCoroutine(UpdateStars(1,3,11,0));
        StartCoroutine(UpdateStars(1,3,12,0));

        StartCoroutine(UpdateStars(2,1,13,0));
        StartCoroutine(UpdateStars(2,1,14,0));
        StartCoroutine(UpdateStars(2,1,15,0));
        StartCoroutine(UpdateStars(2,1,16,0));

        StartCoroutine(UpdateStars(2,2,17,0));
        StartCoroutine(UpdateStars(2,2,18,0));
        StartCoroutine(UpdateStars(2,2,19,0));
        StartCoroutine(UpdateStars(2,2,20,0));

        StartCoroutine(UpdateStars(2,3,21,0));
        StartCoroutine(UpdateStars(2,3,22,0));
        StartCoroutine(UpdateStars(2,3,23,0));
        StartCoroutine(UpdateStars(2,3,24,0));

        StartCoroutine(UpdateStars(3,1,25,0));
        StartCoroutine(UpdateStars(3,1,26,0));
        StartCoroutine(UpdateStars(3,1,27,0));
        StartCoroutine(UpdateStars(3,1,28,0));

        StartCoroutine(UpdateStars(3,2,29,0));
        StartCoroutine(UpdateStars(3,2,30,0));
        StartCoroutine(UpdateStars(3,2,31,0));
        StartCoroutine(UpdateStars(3,2,32,0));

        StartCoroutine(UpdateStars(3,3,33,0));
        StartCoroutine(UpdateStars(3,3,34,0));
        StartCoroutine(UpdateStars(3,3,35,0));
        StartCoroutine(UpdateStars(3,3,36,0));
    }

    private IEnumerator UpdateUsernameAuth(string _username)
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

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

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
    
    public void UpdateStarsToDb() 
    {
        StartCoroutine(UpdateStars(1,1,1,PlayerPrefs.GetInt("1")));
        StartCoroutine(UpdateStars(1,1,2,PlayerPrefs.GetInt("2")));
        StartCoroutine(UpdateStars(1,1,3,PlayerPrefs.GetInt("3")));
        StartCoroutine(UpdateStars(1,1,4,PlayerPrefs.GetInt("4")));

        StartCoroutine(UpdateStars(1,2,5,PlayerPrefs.GetInt("5")));
        StartCoroutine(UpdateStars(1,2,6,PlayerPrefs.GetInt("6")));
        StartCoroutine(UpdateStars(1,2,7,PlayerPrefs.GetInt("7")));
        StartCoroutine(UpdateStars(1,2,8,PlayerPrefs.GetInt("8")));

        StartCoroutine(UpdateStars(1,3,9,PlayerPrefs.GetInt("9")));
        StartCoroutine(UpdateStars(1,3,10,PlayerPrefs.GetInt("10")));
        StartCoroutine(UpdateStars(1,3,11,PlayerPrefs.GetInt("11")));
        StartCoroutine(UpdateStars(1,3,12,PlayerPrefs.GetInt("12")));

        StartCoroutine(UpdateStars(2,1,13,PlayerPrefs.GetInt("13")));
        StartCoroutine(UpdateStars(2,1,14,PlayerPrefs.GetInt("14")));
        StartCoroutine(UpdateStars(2,1,15,PlayerPrefs.GetInt("15")));
        StartCoroutine(UpdateStars(2,1,16,PlayerPrefs.GetInt("16")));

        StartCoroutine(UpdateStars(2,2,17,PlayerPrefs.GetInt("17")));
        StartCoroutine(UpdateStars(2,2,18,PlayerPrefs.GetInt("18")));
        StartCoroutine(UpdateStars(2,2,19,PlayerPrefs.GetInt("19")));
        StartCoroutine(UpdateStars(2,2,20,PlayerPrefs.GetInt("20")));

        StartCoroutine(UpdateStars(2,3,21,PlayerPrefs.GetInt("21")));
        StartCoroutine(UpdateStars(2,3,22,PlayerPrefs.GetInt("22")));
        StartCoroutine(UpdateStars(2,3,23,PlayerPrefs.GetInt("23")));
        StartCoroutine(UpdateStars(2,3,24,PlayerPrefs.GetInt("24")));

        StartCoroutine(UpdateStars(3,1,25,PlayerPrefs.GetInt("25")));
        StartCoroutine(UpdateStars(3,1,26,PlayerPrefs.GetInt("26")));
        StartCoroutine(UpdateStars(3,1,27,PlayerPrefs.GetInt("27")));
        StartCoroutine(UpdateStars(3,1,28,PlayerPrefs.GetInt("28")));

        StartCoroutine(UpdateStars(3,2,29,PlayerPrefs.GetInt("29")));
        StartCoroutine(UpdateStars(3,2,30,PlayerPrefs.GetInt("30")));
        StartCoroutine(UpdateStars(3,2,31,PlayerPrefs.GetInt("31")));
        StartCoroutine(UpdateStars(3,2,32,PlayerPrefs.GetInt("32")));

        StartCoroutine(UpdateStars(3,3,33,PlayerPrefs.GetInt("33")));
        StartCoroutine(UpdateStars(3,3,34,PlayerPrefs.GetInt("34")));
        StartCoroutine(UpdateStars(3,3,35,PlayerPrefs.GetInt("35")));
        StartCoroutine(UpdateStars(3,3,36,PlayerPrefs.GetInt("36")));
    }


    private IEnumerator UpdateKills(int _kills)
    {
        //Set the currently logged in user kills
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("kills").SetValueAsync(_kills);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Kills are now updated
        }
    }

    private IEnumerator UpdateTotalPoints()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("TotalPoints").SetValueAsync(0);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else 
        {

        }
    }
    
    private IEnumerator LoadWorldSectionData()
    {
        int rank = 0;
        string LoggedinUser = "";
        int worldNumber = UIManager.WorldLdrboard;
        int sectionNumber = UIManager.SectionLdrboard;
        Debug.Log(worldNumber);
        Debug.Log(sectionNumber);
        //Get all the users data ordered by points
        var DBTask = DBreference.Child("users").OrderByChild("BattleStats/"+ $"{worldNumber}"+ "/"+$"{sectionNumber}"+"/Points").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Get the currently logged in user data
            var DBTask1 = DBreference.Child("users").Child(User.UserId).GetValueAsync();

            yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask1.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask1.Exception}");
            }
            else if (DBTask1.Result.Value == null)
            {
                //No data exists yet
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot1 = DBTask1.Result;

                LoggedinUser = snapshot1.Child("username").Value.ToString();
            }

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                rank += 1;
                string username = childSnapshot.Child("username").Value.ToString();
                Debug.Log(username);
                int points = int.Parse(childSnapshot.Child("BattleStats").Child($"{worldNumber}").Child($"{sectionNumber}").Child("Points").Value.ToString());

                if (LoggedinUser.Equals(username))
                {
                    ranktext.text = "You are ranked #" + $"{rank}";
                }

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement($"{rank}", username, $"{worldNumber}"+"."+$"{sectionNumber}", points);
            }
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        int rank = 0;
        string LoggedinUser = "";
        string worldsection = "All";

        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild("TotalPoints").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Get the currently logged in user data
            var DBTask1 = DBreference.Child("users").Child(User.UserId).GetValueAsync();

            yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask1.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask1.Exception}");
            }
            else if (DBTask1.Result.Value == null)
            {
                //No data exists yet
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot1 = DBTask1.Result;

                LoggedinUser = snapshot1.Child("username").Value.ToString();
            }
       

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                rank += 1;
                string username = childSnapshot.Child("username").Value.ToString();
                int points = int.Parse(childSnapshot.Child("TotalPoints").Value.ToString());

                if (LoggedinUser.Equals(username))
                {
                    ranktext.text = "You are ranked #" + $"{rank}";
                }

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement($"{rank}", username, worldsection, points);
            }

            //Go to scoareboard screen
            UIManager.instance.ScoreboardScreen();
        }
    }

    public void displayStarsPoints()
    {
        StartCoroutine(loadSelectedStarsPoints(usernameTitle.text));
    }

    public void viewProfile()
    {
        StartCoroutine(loadMainMenu());
    }

    private IEnumerator loadMainMenu()
    {
        //get the currently logged in user data
        var DBtask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBtask.IsCompleted);

        if (DBtask.Exception != null)
        {
            Debug.LogWarning(message: $"failed to register task with {DBtask.Exception}");
        }
        else if (DBtask.Result.Value == null)
        {
            //no data exists yet
            totalStars.text = "0";
            totalPoints.text = "0";
            usernameTitle.text = "Invalid username";
        }
        else
        {
            DataSnapshot snapshot = DBtask.Result;
            string username = snapshot.Child("username").Value.ToString();
            usernameTitle.text = username;
            int oodpS1starsT1 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{1}").Value.ToString());
            int oodpS1starsT2 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{2}").Value.ToString());
            int oodpS1starsT3 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{3}").Value.ToString());
            int oodpS1starsT4 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{4}").Value.ToString());
            int oodpS1stars = oodpS1starsT1 + oodpS1starsT2 + oodpS1starsT3 + oodpS1starsT4;
            Debug.Log(oodpS1stars);

            int oodpS2starsT1 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{5}").Value.ToString());
            int oodpS2starsT2 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{6}").Value.ToString());
            int oodpS2starsT3 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{7}").Value.ToString());
            int oodpS2starsT4 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{8}").Value.ToString());
            int oodpS2stars = oodpS2starsT1 + oodpS2starsT2 + oodpS2starsT3 + oodpS2starsT4;
            Debug.Log(oodpS2stars);

            int oodpS3starsT1 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{9}").Value.ToString());
            int oodpS3starsT2 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{10}").Value.ToString());
            int oodpS3starsT3 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{11}").Value.ToString());
            int oodpS3starsT4 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{12}").Value.ToString());
            int oodpS3stars = oodpS3starsT1 + oodpS3starsT2 + oodpS3starsT3 + oodpS3starsT4;
            Debug.Log(oodpS3stars);

            int seS1starsT1 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{13}").Value.ToString());
            int seS1starsT2 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{14}").Value.ToString());
            int seS1starsT3 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{15}").Value.ToString());
            int seS1starsT4 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{16}").Value.ToString());
            int seS1stars = seS1starsT1 + seS1starsT2 + seS1starsT3 + seS1starsT4;
            Debug.Log(seS1stars);

            int seS2starsT1 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{17}").Value.ToString());
            int seS2starsT2 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{18}").Value.ToString());
            int seS2starsT3 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{19}").Value.ToString());
            int seS2starsT4 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{20}").Value.ToString());
            int seS2stars = seS2starsT1 + seS2starsT2 + seS2starsT3 + seS2starsT4;
            Debug.Log(seS2stars);

            int seS3starsT1 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{21}").Value.ToString());
            int seS3starsT2 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{22}").Value.ToString());
            int seS3starsT3 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{23}").Value.ToString());
            int seS3starsT4 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{24}").Value.ToString());
            int seS3stars = seS3starsT1 + seS3starsT2 + seS3starsT3 + seS3starsT4;
            Debug.Log(seS3stars);

            int ssadS1starsT1 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{25}").Value.ToString());
            int ssadS1starsT2 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{26}").Value.ToString());
            int ssadS1starsT3 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{27}").Value.ToString());
            int ssadS1starsT4 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{28}").Value.ToString());
            int ssadS1stars = ssadS1starsT1 + ssadS1starsT2 + ssadS1starsT3 + ssadS1starsT4;
            Debug.Log(ssadS1stars);

            int ssadS2starsT1 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{29}").Value.ToString());
            int ssadS2starsT2 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{30}").Value.ToString());
            int ssadS2starsT3 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{31}").Value.ToString());
            int ssadS2starsT4 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{32}").Value.ToString());
            int ssadS2stars = ssadS2starsT1 + ssadS2starsT2 + ssadS2starsT3 + ssadS2starsT4;
            Debug.Log(ssadS2stars);

            int ssadS3starsT1 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{33}").Value.ToString());
            int ssadS3starsT2 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{34}").Value.ToString());
            int ssadS3starsT3 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{35}").Value.ToString());
            int ssadS3starsT4 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{36}").Value.ToString());
            int ssadS3stars = ssadS3starsT1 + ssadS3starsT2 + ssadS3starsT3 + ssadS3starsT4;
            Debug.Log(ssadS3stars);

            int totalStarsObtained = oodpS1stars + oodpS2stars + oodpS3stars + seS1stars + seS2stars + seS3stars + ssadS1stars + ssadS2stars + ssadS3stars;
            Debug.Log(totalStarsObtained);

            int totalPointsObtained = int.Parse(snapshot.Child("TotalPoints").Value.ToString());

            totalStars.text = totalStarsObtained.ToString();
            totalPoints.text = totalPointsObtained.ToString();

        }
        //loginUI.SetActive(false);
        userDataUI.SetActive(true);
    }

    private IEnumerator loadSelectedStarsPoints(string _username)
    {
        //get the currently logged in user data
        var DBtask = DBreference.Child("users").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBtask.IsCompleted);

        if (DBtask.Exception != null)
        {
            Debug.LogWarning(message: $"failed to register task with {DBtask.Exception}");
        }
        else if (DBtask.Result.Value == null)
        {
            //no data exists yet
            selectedStars.text = "0";
            selectedPoints.text = "0";
        }
        else
        {
            //data has been retrieved
            int worldNumber = UIManager.World;
            int sectionNumber = UIManager.Section;
            Debug.Log(worldNumber);
            Debug.Log(sectionNumber);

            DataSnapshot snapshot = DBtask.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int oodpS1starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{1}").Value.ToString());
                int oodpS1starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{2}").Value.ToString());
                int oodpS1starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{3}").Value.ToString());
                int oodpS1starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{4}").Value.ToString());
                int oodpS1stars = oodpS1starsT1 + oodpS1starsT2 + oodpS1starsT3 + oodpS1starsT4;

                int oodpS2starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{5}").Value.ToString());
                int oodpS2starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{6}").Value.ToString());
                int oodpS2starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{7}").Value.ToString());
                int oodpS2starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{8}").Value.ToString());
                int oodpS2stars = oodpS2starsT1 + oodpS2starsT2 + oodpS2starsT3 + oodpS2starsT4;

                int oodpS3starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{9}").Value.ToString());
                int oodpS3starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{10}").Value.ToString());
                int oodpS3starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{11}").Value.ToString());
                int oodpS3starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{12}").Value.ToString());
                int oodpS3stars = oodpS3starsT1 + oodpS3starsT2 + oodpS3starsT3 + oodpS3starsT4;

                int seS1starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{13}").Value.ToString());
                int seS1starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{14}").Value.ToString());
                int seS1starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{15}").Value.ToString());
                int seS1starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{16}").Value.ToString());
                int seS1stars = seS1starsT1 + seS1starsT2 + seS1starsT3 + seS1starsT4;

                int seS2starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{17}").Value.ToString());
                int seS2starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{18}").Value.ToString());
                int seS2starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{19}").Value.ToString());
                int seS2starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{20}").Value.ToString());
                int seS2stars = seS2starsT1 + seS2starsT2 + seS2starsT3 + seS2starsT4;

                int seS3starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{21}").Value.ToString());
                int seS3starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{22}").Value.ToString());
                int seS3starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{23}").Value.ToString());
                int seS3starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{24}").Value.ToString());
                int seS3stars = seS3starsT1 + seS3starsT2 + seS3starsT3 + seS3starsT4;

                int ssadS1starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{25}").Value.ToString());
                int ssadS1starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{26}").Value.ToString());
                int ssadS1starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{27}").Value.ToString());
                int ssadS1starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{28}").Value.ToString());
                int ssadS1stars = ssadS1starsT1 + ssadS1starsT2 + ssadS1starsT3 + ssadS1starsT4;

                int ssadS2starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{29}").Value.ToString());
                int ssadS2starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{30}").Value.ToString());
                int ssadS2starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{31}").Value.ToString());
                int ssadS2starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{32}").Value.ToString());
                int ssadS2stars = ssadS2starsT1 + ssadS2starsT2 + ssadS2starsT3 + ssadS2starsT4;

                int ssadS3starsT1 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{33}").Value.ToString());
                int ssadS3starsT2 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{34}").Value.ToString());
                int ssadS3starsT3 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{35}").Value.ToString());
                int ssadS3starsT4 = int.Parse(childSnapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{36}").Value.ToString());
                int ssadS3stars = ssadS3starsT1 + ssadS3starsT2 + ssadS3starsT3 + ssadS3starsT4;

                int totalStars = oodpS1stars + oodpS2stars + oodpS3stars + seS1stars + seS2stars + seS3stars + ssadS1stars + ssadS2stars + ssadS3stars;

                int oodpS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{1}").Child("Points").Value.ToString());
                int oodpS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{2}").Child("Points").Value.ToString());
                int oodpS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{3}").Child("Points").Value.ToString());

                int seS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{1}").Child("Points").Value.ToString());
                int seS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{2}").Child("Points").Value.ToString());
                int seS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{3}").Child("Points").Value.ToString());

                int ssadS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{1}").Child("Points").Value.ToString());
                int ssadS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{2}").Child("Points").Value.ToString());
                int ssadS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{3}").Child("Points").Value.ToString());

                if (username == _username)
                {
                    if (worldNumber == 1 && sectionNumber == 1)
                    {
                        selectedStars.text = oodpS1stars.ToString();
                        selectedPoints.text = oodpS1pts.ToString();
                    }
                    else if (worldNumber == 1 && sectionNumber == 2)
                    {
                        selectedStars.text = oodpS2stars.ToString();
                        selectedPoints.text = oodpS2pts.ToString();
                    }
                    else if (worldNumber == 1 && sectionNumber == 3)
                    {
                        selectedStars.text = oodpS3stars.ToString();
                        selectedPoints.text = oodpS3pts.ToString();
                    }
                    else if (worldNumber == 2 && sectionNumber == 1)
                    {
                        selectedStars.text = seS1stars.ToString();
                        selectedPoints.text = seS1pts.ToString();
                    }
                    else if (worldNumber == 2 && sectionNumber == 2)
                    {
                        selectedStars.text = seS2stars.ToString();
                        selectedPoints.text = seS2pts.ToString();
                    }
                    else if (worldNumber == 2 && sectionNumber == 3)
                    {
                        selectedStars.text = seS3stars.ToString();
                        selectedPoints.text = seS3pts.ToString();
                    }
                    else if (worldNumber == 3 && sectionNumber == 1)
                    {
                        selectedStars.text = ssadS1stars.ToString();
                        selectedPoints.text = ssadS1pts.ToString();
                    }
                    else if (worldNumber == 3 && sectionNumber == 2)
                    {
                        selectedStars.text = ssadS2stars.ToString();
                        selectedPoints.text = ssadS2pts.ToString();
                    }
                    else if (worldNumber == 3 && sectionNumber == 3)
                    {
                        selectedStars.text = ssadS3stars.ToString();
                        selectedPoints.text = ssadS3pts.ToString();
                    }
                }
            }
        }
    }

    public void displayWorldSectionData()
    {
        StartCoroutine(LoadWorldSectionData());
    }

     public void displayTotalPoints()
    {
        StartCoroutine(LoadScoreboardData());
    }

    private IEnumerator UpdateTeacherUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("teachers").Child(User.UserId).Child("username").SetValueAsync(_username);

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


    private IEnumerator CheckTeacher(string _username)
    {
        //Get all the users data ordered by kills amount
        var DBTask1 = DBreference.Child("teachers").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

        if (DBTask1.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask1.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot1 = DBTask1.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }
            bool teacher = false;
            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot1.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                if (username == _username)
                {
                    teacher = true;
                    UIManager.instance.ClearScreenForTeacher();
                    SceneManager.LoadScene("TeacherUI");
                    break;
                }
            }

            //Go to data screen
            if (teacher == false)
            {
                //get the currently logged in user data
                var DBtask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

                yield return new WaitUntil(predicate: () => DBtask.IsCompleted);

                if (DBtask.Exception != null)
                {
                    Debug.LogWarning(message: $"failed to register task with {DBtask.Exception}");
                }
                else if (DBtask.Result.Value == null)
                {
                    //no data exists yet
                    totalStars.text = "0";
                    totalPoints.text = "0";
                    usernameTitle.text = "Invalid username";
                }
                else
                {
                    DataSnapshot snapshot = DBtask.Result;
                    string username = snapshot.Child("username").Value.ToString();
                    usernameTitle.text = username;
                    int oodpS1starsT1 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{1}").Value.ToString());
                    int oodpS1starsT2 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{2}").Value.ToString());
                    int oodpS1starsT3 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{3}").Value.ToString());
                    int oodpS1starsT4 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{1}").Child($"{4}").Value.ToString());
                    int oodpS1stars = oodpS1starsT1 + oodpS1starsT2 + oodpS1starsT3 + oodpS1starsT4;
                    Debug.Log(oodpS1stars);

                    int oodpS2starsT1 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{5}").Value.ToString());
                    int oodpS2starsT2 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{6}").Value.ToString());
                    int oodpS2starsT3 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{7}").Value.ToString());
                    int oodpS2starsT4 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{2}").Child($"{8}").Value.ToString());
                    int oodpS2stars = oodpS2starsT1 + oodpS2starsT2 + oodpS2starsT3 + oodpS2starsT4;
                    Debug.Log(oodpS2stars);

                    int oodpS3starsT1 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{9}").Value.ToString());
                    int oodpS3starsT2 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{10}").Value.ToString());
                    int oodpS3starsT3 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{11}").Value.ToString());
                    int oodpS3starsT4 = int.Parse(snapshot.Child("stars").Child($"{1}").Child($"{3}").Child($"{12}").Value.ToString());
                    int oodpS3stars = oodpS3starsT1 + oodpS3starsT2 + oodpS3starsT3 + oodpS3starsT4;
                    Debug.Log(oodpS3stars);

                    int seS1starsT1 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{13}").Value.ToString());
                    int seS1starsT2 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{14}").Value.ToString());
                    int seS1starsT3 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{15}").Value.ToString());
                    int seS1starsT4 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{1}").Child($"{16}").Value.ToString());
                    int seS1stars = seS1starsT1 + seS1starsT2 + seS1starsT3 + seS1starsT4;
                    Debug.Log(seS1stars);

                    int seS2starsT1 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{17}").Value.ToString());
                    int seS2starsT2 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{18}").Value.ToString());
                    int seS2starsT3 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{19}").Value.ToString());
                    int seS2starsT4 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{2}").Child($"{20}").Value.ToString());
                    int seS2stars = seS2starsT1 + seS2starsT2 + seS2starsT3 + seS2starsT4;
                    Debug.Log(seS2stars);

                    int seS3starsT1 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{21}").Value.ToString());
                    int seS3starsT2 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{22}").Value.ToString());
                    int seS3starsT3 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{23}").Value.ToString());
                    int seS3starsT4 = int.Parse(snapshot.Child("stars").Child($"{2}").Child($"{3}").Child($"{24}").Value.ToString());
                    int seS3stars = seS3starsT1 + seS3starsT2 + seS3starsT3 + seS3starsT4;
                    Debug.Log(seS3stars);

                    int ssadS1starsT1 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{25}").Value.ToString());
                    int ssadS1starsT2 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{26}").Value.ToString());
                    int ssadS1starsT3 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{27}").Value.ToString());
                    int ssadS1starsT4 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{1}").Child($"{28}").Value.ToString());
                    int ssadS1stars = ssadS1starsT1 + ssadS1starsT2 + ssadS1starsT3 + ssadS1starsT4;
                    Debug.Log(ssadS1stars);

                    int ssadS2starsT1 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{29}").Value.ToString());
                    int ssadS2starsT2 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{30}").Value.ToString());
                    int ssadS2starsT3 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{31}").Value.ToString());
                    int ssadS2starsT4 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{2}").Child($"{32}").Value.ToString());
                    int ssadS2stars = ssadS2starsT1 + ssadS2starsT2 + ssadS2starsT3 + ssadS2starsT4;
                    Debug.Log(ssadS2stars);

                    int ssadS3starsT1 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{33}").Value.ToString());
                    int ssadS3starsT2 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{34}").Value.ToString());
                    int ssadS3starsT3 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{35}").Value.ToString());
                    int ssadS3starsT4 = int.Parse(snapshot.Child("stars").Child($"{3}").Child($"{3}").Child($"{36}").Value.ToString());
                    int ssadS3stars = ssadS3starsT1 + ssadS3starsT2 + ssadS3starsT3 + ssadS3starsT4;
                    Debug.Log(ssadS3stars);

                    int totalStarsObtained = oodpS1stars + oodpS2stars + oodpS3stars + seS1stars + seS2stars + seS3stars + ssadS1stars + ssadS2stars + ssadS3stars;
                    Debug.Log(totalStarsObtained);

                    int totalPointsObtained = int.Parse(snapshot.Child("TotalPoints").Value.ToString());

                    totalStars.text = totalStarsObtained.ToString();
                    totalPoints.text = totalPointsObtained.ToString();

                }
                loginUI.SetActive(false);
                userDataUI.SetActive(true);
            }
        }
    }
}