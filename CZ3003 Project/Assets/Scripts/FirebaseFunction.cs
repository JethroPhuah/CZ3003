using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;


public class FirebaseFunction : MonoBehaviour
{
    [SerializeField] private AudioClip cfmClickSFX;
    public GameObject userDataUI;
    public GameObject modeSelectionPanel;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public static FirebaseUser User;
    public DatabaseReference DBreference;

    [Header("UserData")]
    public TMP_Text usernameTitle;
    public TMP_Text totalStars;
    public TMP_Text totalPoints;
    public TMP_Text selectedStars;
    public TMP_Text selectedPoints;
    public Transform scoreboardContent;
    public GameObject scoreElement;
    public TMP_Text ranktext;
    private void Start()
    {
        FirebaseManager.Instance.usernameTitle = usernameTitle;
        FirebaseManager.Instance.totalStars = totalStars;
        FirebaseManager.Instance.totalPoints = totalPoints;
        FirebaseManager.Instance.selectedStars = selectedStars;
        FirebaseManager.Instance.selectedPoints = selectedPoints;
        FirebaseManager.Instance.scoreboardContent = scoreboardContent;
        FirebaseManager.Instance.ranktext = ranktext;
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

    public void ScoreboardButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        FirebaseManager.Instance.ScoreboardButton();
    }

    public void SignOutButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        FirebaseManager.Instance.SignOutButton();
    }

    public void DisplayStar()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        FirebaseManager.Instance.displayStarsPoints();
    }

    public void ShowButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        FirebaseManager.Instance.displayWorldSectionData();
    }

    public void ResetButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        FirebaseManager.Instance.displayTotalPoints();
    }

    public void viewProfile()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StartCoroutine(loadMainMenu());
    }

    private IEnumerator loadMainMenu()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        //get the currently logged in user data
        FirebaseUser  User = FirebaseManager.User;
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
        modeSelectionPanel.SetActive(false);
        userDataUI.SetActive(true);
    }

    public void BacktoGameMode() 
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        userDataUI.SetActive(false);
        modeSelectionPanel.SetActive(true);
    }
}
