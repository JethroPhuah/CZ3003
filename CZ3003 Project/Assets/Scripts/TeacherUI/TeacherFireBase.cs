using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
//Done by Jun Hao and Zhi Fah

public class TeacherFireBase : MonoBehaviour
{
    [SerializeField] private AudioClip cfmClickSFX;
    //Firebase variables
    [Header("Firebase")]
    //Variable for DependencyStatus
    public DependencyStatus dependencyStatus;
    //Variable for FirebaseAuth 
    public FirebaseAuth auth;
    //Variable for user for firebase
    public FirebaseUser User;
    //Variable for Database reference
    public DatabaseReference DBreference;

    //User Data variables
    [Header("UserData")]
    //Component to be instantiated for statistic scroll view
    public GameObject statisticElement;
    //The content of the scrollview on statistic page
    public Transform scoreboardContent;
    //Component to be instantiated for leaderboard scroll view
    public GameObject leaderElement;
    //The content of the scrollview on leaderboard page
    public Transform leaderboardContent;

    //Question and Answers variables
    [Header("QnA")]
    //The question to be added under question adding function
    public InputField QuestionInputField;
    //The answer to be added under question adding function
    public InputField AnswerInputField1;
    //The answer to be added under question adding function
    public InputField AnswerInputField2;
    //The answer to be added under question adding function
    public InputField AnswerInputField3;
    //This panel is where the user types out the question and answer that he/she wants to be added
    public GameObject AddQuestionPanel;
    //The panel where the user can filter the world, section and difficulty before adding question
    public GameObject OptionSelectionPanel;
    //The panel which the user can select the functions
    public GameObject FunctionSelectionPanel;
    //The submit button to add the question
    public Button SubmitButton;
    //The student that is being searched
    string StudentSearched;
    //Searchbar on the statistic page
    public InputField SearchBar;
    //The panel which display statistics for all students
    public GameObject OverviewPanel;
    //The panel which display statistics for a student
    public GameObject StudentPanel;
    //The points gained from OODP section 1
    public Text OODP_S1_points;
    //The points gained from OODP section 2
    public Text OODP_S2_points;
    //The points gained from OODP section 3
    public Text OODP_S3_points;
    //The points gained from SE section 1
    public Text SE_S1_points;
    //The points gained from SE section 2
    public Text SE_S2_points;
    //The points gained from SE section 3
    public Text SE_S3_points;
    //The points gained from SSAD section 1
    public Text SSAD_S1_points;
    //The points gained from SSAD section 2
    public Text SSAD_S2_points;
    //The points gained from SSAD section 3
    public Text SSAD_S3_points;
    //The stars gained from OODP section 1
    public Text OODP_S1_stars;
    //The stars gained from OODP section 2
    public Text OODP_S2_stars;
    //The stars gained from OODP section 3
    public Text OODP_S3_stars;
    //The stars gained from SE section 1
    public Text SE_S1_stars;
    //The stars gained from SE section 2
    public Text SE_S2_stars;
    //The stars gained from SE section 3
    public Text SE_S3_stars;
    //The stars gained from SSAD section 1
    public Text SSAD_S1_stars;
    //The stars gained from SSAD section 2
    public Text SSAD_S2_stars;
    //The stars gained from SSAD section 3
    public Text SSAD_S3_stars;
    //The username of the user
    public Text Name;
    //Dropdown selection for world
    public Dropdown Worldselection;
    //Dropdown selection for section
    public Dropdown SectionSelection;
    //The variable to store the world selected
    string world;
    //The variable to store the section selected
    string section;
    //The question element which will be instantiated in the scrollview for the edit question panel
    public GameObject questionElement;
    //The content of the scrollview in the editquestion panel
    public Transform questionListContent;
    //The question which is going to be edited
    string Question;
    //This is the textinput field for the user to edit the question
    public InputField EditQuestionInputField;
    //This is the textinput field for the user to edit the correct answer
    public InputField EditAnswerInputField1;
    //This is the textinput field for the user to edit the wrong answer
    public InputField EditAnswerInputField2;
    //This is the textinput field for the user to edit the wrong answer
    public InputField EditAnswerInputField3;
    //This panel is where the user can edits the question
    public GameObject EditQuestionPanel;
    //This panel is where the user will select the question he/she want to edit
    public GameObject DisplayQuestionPanel;
    //This variable sums the total number of question for the world and section that the user filtered 
    int totalquestion = 0;
    // This variable will have the index of the current question selected
    int currentindex;
    //The component which will be instantiated in the scrollview of the make an assignment panel
    public GameObject CustomquestionElement;
    //The content of the scroll view for the make an assignment panel
    public Transform CustomquestionListContent;
    //The question which is being added in the make an assignment panel
    string CustomQuestion;
    //The answer for the question added in the make an assignment panel
    string A1;
    //The answer for the question added in the make an assignment panel
    string A2;
    //The answer for the question added in the make an assignment panel
    string A3;
    //This variable store the number of questions in a room created in the make an assignment function
    int questionNo;
    //This variable store the value of whether the assignment room already exist
    bool roomexist;
    //This variable displays the error message
    public Text errormsg;
    //This variable displays the error message
    public Text errormsg1;
    //This panel contains the create an assignment and view score for an assignment
    public GameObject StartPanel;
    //This panel contains the filters for selecting the question for making an assignment
    public GameObject SelectionPanel;
    //This is the room created in the make an assignment
    string Room;
    //This is the Inputfield for the user to enter the room to make
    public InputField createRoomID;
    //This is the Inputfield for the user to enter the room to view the score of players who played
    public InputField ViewRoomID;
    //The panel which shows the scores of students who played the assignment
    public GameObject ScorePanel;
    //This component is instantiated in the scroll view of the score panel
    public GameObject studentElement;
    //This is the content of the scrollview in the score panel
    public Transform studentListContent;
    //This is the error message displayed when there is missing input
    public Text missinginput;
    //This method initializes firebase when there is an instance of the script
    void Awake()
    {
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
    //This method initializes firebase
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    //This method clears all the question and answer fields

    public void ClearQuestionAndAnswersFields()
    {
        QuestionInputField.Select();
        QuestionInputField.text = "";
        AnswerInputField1.Select();
        AnswerInputField1.text = "";
        AnswerInputField2.Select();
        AnswerInputField2.text = "";
        AnswerInputField3.Select();
        AnswerInputField3.text = "";
    }

    //Function for the register button
    public void SubmitButtonMethod()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        if (QuestionInputField.text != "" & AnswerInputField1.text != "" & AnswerInputField2.text != "" & AnswerInputField3.text != "")
        {
            missinginput.text = "";
            StartCoroutine(createQuestionsAndAnswers(QuestionInputField.text, AnswerInputField1.text, AnswerInputField2.text, AnswerInputField3.text));
        }
        else 
        {
            missinginput.text = "Missing input(s)";
        }
    }
    //This method adds the question and answers to correct branch based on filters in firebase
    private IEnumerator createQuestionsAndAnswers(string _question, string _answer1, string _answer2, string _answer3)
    {
        int worldNumber = QuestionAdder.World;
        int sectionNumber = QuestionAdder.Section;
        string difficulty = QuestionAdder.Difficulty;
        Debug.Log(worldNumber);
        Debug.Log(sectionNumber);
        Debug.Log(difficulty);

        //Set the currently logged in user mastery
        var DBTask = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        DataSnapshot snapshot = DBTask.Result;
        int length = (int)snapshot.ChildrenCount;

        var qnTask = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("Question").SetValueAsync(_question);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (qnTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {qnTask.Exception}");
        }

        var a1Task = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("A1").SetValueAsync(_answer1);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (a1Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a1Task.Exception}");
        }

        var a2Task = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("A2").SetValueAsync(_answer2);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (a2Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a2Task.Exception}");
        }

        var a3Task = DBreference.Child("Qns").Child($"{worldNumber}").Child($"{sectionNumber}").Child(difficulty).Child($"{length + 1}").Child("A3").SetValueAsync(_answer3);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (a3Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a3Task.Exception}");
        }

        ClearQuestionAndAnswersFields();

        AddQuestionPanel.gameObject.SetActive(false);
        OptionSelectionPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);// shift this to button
    }
    //Method is called when searchbar is filled
    public void SearchStudent()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StudentSearched = SearchBar.text;
        Debug.Log(StudentSearched);
        SearchBar.text = "";
        StartCoroutine(ShowInformation(StudentSearched));
    }
    //This method is called to display the individual student statistic from searchbar
    private IEnumerator ShowInformation(string _StudentID)
    {

        //Get Student
        var DBTask = DBreference.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        Debug.Log("something");

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("Yo");
            OODP_S1_points.text = "0";
            OODP_S2_points.text = "0";
            OODP_S3_points.text = "0";
            SE_S1_points.text = "0";
            SE_S2_points.text = "0";
            SE_S3_points.text = "0";
            SSAD_S1_points.text = "0";
            SSAD_S2_points.text = "0";
            SSAD_S3_points.text = "0";

            OODP_S1_stars.text = "0";
            OODP_S2_stars.text = "0";
            OODP_S3_stars.text = "0";
            SE_S1_stars.text = "0";
            SE_S2_stars.text = "0";
            SE_S3_stars.text = "0";
            SSAD_S1_stars.text = "0";
            SSAD_S2_stars.text = "0";
            SSAD_S3_stars.text = "0";
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                
                string username = childSnapshot.Child("username").Value.ToString();
                Debug.Log(username);
                int oodpS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{1}").Child("Points").Value.ToString());
                int oodpS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{2}").Child("Points").Value.ToString());
                int oodpS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{3}").Child("Points").Value.ToString());

                int seS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{1}").Child("Points").Value.ToString());
                int seS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{2}").Child("Points").Value.ToString());
                int seS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{3}").Child("Points").Value.ToString());

                int ssadS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{1}").Child("Points").Value.ToString());
                int ssadS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{2}").Child("Points").Value.ToString());
                int ssadS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{3}").Child("Points").Value.ToString());

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

                if (username == _StudentID)
                {
                    Debug.Log("match");
                    OODP_S1_points.text = oodpS1pts.ToString(); // need change wonky
                    OODP_S2_points.text = oodpS2pts.ToString();
                    OODP_S3_points.text = oodpS3pts.ToString();

                    SE_S1_points.text = seS1pts.ToString();
                    SE_S2_points.text = seS2pts.ToString();
                    SE_S3_points.text = seS3pts.ToString();

                    SSAD_S1_points.text = ssadS1pts.ToString();
                    SSAD_S2_points.text = ssadS2pts.ToString();
                    SSAD_S3_points.text = ssadS3pts.ToString();

                    OODP_S1_stars.text = oodpS1stars.ToString();
                    OODP_S2_stars.text = oodpS2stars.ToString();
                    OODP_S3_stars.text = oodpS3stars.ToString();

                    SE_S1_stars.text = seS1stars.ToString();
                    SE_S2_stars.text = seS2stars.ToString();
                    SE_S3_stars.text = seS3stars.ToString();

                    SSAD_S1_stars.text = ssadS1stars.ToString();
                    SSAD_S2_stars.text = ssadS2stars.ToString();
                    SSAD_S3_stars.text = ssadS3stars.ToString();

                    Name.text = username;
                    break;
                }
                OODP_S1_points.text = "0"; //invalid user
                OODP_S2_points.text = "0";
                OODP_S3_points.text = "0";
                SE_S1_points.text = "0";
                SE_S2_points.text = "0";
                SE_S3_points.text = "0";
                SSAD_S1_points.text = "0";
                SSAD_S2_points.text = "0";
                SSAD_S3_points.text = "0";

                OODP_S1_stars.text = "0";
                OODP_S2_stars.text = "0";
                OODP_S3_stars.text = "0";
                SE_S1_stars.text = "0";
                SE_S2_stars.text = "0";
                SE_S3_stars.text = "0";
                SSAD_S1_stars.text = "0";
                SSAD_S2_stars.text = "0";
                SSAD_S3_stars.text = "0";
                Name.text = "Invalid User";
            }
        }
    }
    //Method is called when the filter button is clicked
    public void filter()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        world = Worldselection.options[Worldselection.value].text;
        section = SectionSelection.options[SectionSelection.value].text;
        StartCoroutine(LoadOverallStatistics(world, section));
    }
    //This method creates the entries in the scrollview depending on the filters selected
    private IEnumerator LoadOverallStatistics(string world, string section)
    {
        var DBTask = DBreference.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                int stars = 0;
                int points = 0;
                string username = childSnapshot.Child("username").Value.ToString();
                int oodpS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{1}").Child("Points").Value.ToString());
                int oodpS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{2}").Child("Points").Value.ToString());
                int oodpS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{1}").Child($"{3}").Child("Points").Value.ToString());

                int seS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{1}").Child("Points").Value.ToString());
                int seS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{2}").Child("Points").Value.ToString());
                int seS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{2}").Child($"{3}").Child("Points").Value.ToString());

                int ssadS1pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{1}").Child("Points").Value.ToString());
                int ssadS2pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{2}").Child("Points").Value.ToString());
                int ssadS3pts = int.Parse(childSnapshot.Child("BattleStats").Child($"{3}").Child($"{3}").Child("Points").Value.ToString());

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
                if (world == "OODP")
                {
                    switch (section)
                    {
                        case "1":
                            {
                                points = oodpS1pts;
                                stars = oodpS1stars;
                                break;
                            }
                        case "2":
                            {
                                points = oodpS2pts;
                                stars = oodpS2stars;
                                break;
                            }
                        case "3":
                            {
                                points = oodpS3pts;
                                stars = oodpS3stars;
                                break;
                            }
                        case "TOTAL":
                            {
                                points = oodpS1pts + oodpS2pts + oodpS3pts;
                                stars = oodpS1stars + oodpS2stars + oodpS3stars;
                                break;
                            }
                    }

                }
                else if (world == "SE")
                {
                    switch (section)
                    {
                        case "1":
                            {
                                points = seS1pts;
                                stars = seS1stars;
                                break;
                            }
                        case "2":
                            {
                                points = seS2pts;
                                stars = seS2stars;
                                break;
                            }
                        case "3":
                            {
                                points = seS3pts;
                                stars = seS3stars;
                                break;
                            }
                        case "TOTAL":
                            {
                                points = seS1pts + seS2pts + seS3pts;
                                stars = seS1stars + seS2stars + seS3stars;
                                break;
                            }
                    }
                }
                else if (world == "SSAD")
                {
                    switch (section)
                    {
                        case "1":
                            {
                                points = ssadS1pts;
                                stars = ssadS1stars;
                                break;
                            }
                        case "2":
                            {
                                points = ssadS2pts;
                                stars = ssadS2stars;
                                break;
                            }
                        case "3":
                            {
                                points = ssadS3pts;
                                stars = ssadS3stars;
                                break;
                            }
                        case "TOTAL":
                            {
                                points = ssadS1pts + ssadS2pts + ssadS3pts;
                                stars = ssadS1stars + ssadS2stars + ssadS3stars;
                                break;
                            }
                    }
                }
                else if (world == "TOTAL")
                {
                    switch (section)
                    {
                        case "1":
                            {
                                points = ssadS1pts + oodpS1pts + seS1pts;
                                stars = ssadS1stars + oodpS1stars + seS1stars;
                                break;
                            }
                        case "2":
                            {
                                points = ssadS2pts + oodpS2pts + seS2pts;
                                stars = ssadS2stars + oodpS2stars + seS2stars;
                                break;
                            }
                        case "3":
                            {
                                points = ssadS3pts + oodpS3pts + seS3pts;
                                stars = ssadS3stars + oodpS3stars + seS3stars;
                                break;
                            }
                        case "TOTAL":
                            {
                                points = ssadS1pts + oodpS1pts + seS1pts + ssadS2pts + oodpS2pts + seS2pts + ssadS3pts + oodpS3pts + seS3pts;
                                stars = ssadS1stars + oodpS1stars + seS1stars + ssadS2stars + oodpS2stars + seS2stars + ssadS3stars + oodpS3stars + seS3stars;
                                break;
                            }
                    }
                }
                GameObject scoreboardElement = Instantiate(statisticElement, scoreboardContent);
                scoreboardElement.GetComponent<StatisticElement>().NewScoreElement(username,stars, points);
            }
        }
    }
    //Button to call the display of all the question which can be edited based on the filters world, section and difficulty
    public void DisplayAllQuestionButtonMethod()
    {
        StartCoroutine(LoadAllQuestionsToDisplay());
    }
    //This method pulls the relevant data from firebase based on the filters
    private IEnumerator LoadAllQuestionsToDisplay()
    {
        int world = EditQuestion.World;
        int section = EditQuestion.Section;
        string difficulty = EditQuestion.Difficulty;

        var DBTask = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (Transform child in questionListContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string question = childSnapshot.Child("Question").Value.ToString();
                Debug.Log(question);
                GameObject questionListElement = Instantiate(questionElement, questionListContent);
                questionListElement.GetComponent<QuestionItem>().NewQuestionItem(question, world, section);
            }
        }
    }
    //This method is called when the user wants to edit a question
    //_question: This is the question to be edited
    public void EditData(string _question)
    {
        Question = _question;
        EditQuestionInputField.text = Question;
        StartCoroutine(GetQuestionData());
        EditQuestionPanel.gameObject.SetActive(true);
        DisplayQuestionPanel.gameObject.SetActive(false);
    }
    //This method pulls the question from the database and its answers before populating the questions and answers fields to be edited
    private IEnumerator GetQuestionData() { 
        int world = EditQuestion.World;
        int section = EditQuestion.Section;
        string difficulty = EditQuestion.Difficulty;

        var DBTask = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string question = childSnapshot.Child("Question").Value.ToString();
                totalquestion = totalquestion + 1;
                if (question == Question)
                {
                    string Answer1 = childSnapshot.Child("A1").Value.ToString();
                    string Answer2 = childSnapshot.Child("A2").Value.ToString();
                    string Answer3 = childSnapshot.Child("A3").Value.ToString();
                    EditAnswerInputField1.text = Answer1;
                    EditAnswerInputField2.text = Answer2;
                    EditAnswerInputField3.text = Answer3;
                    currentindex = totalquestion;
                }
             }
        }
    }
    //This method reads the new edited questions and answers
    public void savenewquestion()
    {
        string revisedquestion = EditQuestionInputField.text;
        string revisedA1 = EditAnswerInputField1.text;
        string revisedA2 = EditAnswerInputField2.text;
        string revisedA3 = EditAnswerInputField3.text;
        StartCoroutine(setrevisedquestion(revisedquestion,revisedA1,revisedA2,revisedA3));
    }
    //This method save the edited questions and answers to the correct place in firebase
    private IEnumerator setrevisedquestion(string _question, string _answer1, string _answer2, string _answer3)
    {
        //need integrate with jh one!
        int index = totalquestion - currentindex + 1;
        int world = EditQuestion.World;
        int section = EditQuestion.Section;
        string difficulty = EditQuestion.Difficulty;


        var qnTask = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).Child($"{index}").Child("Question").SetValueAsync(_question);

        yield return new WaitUntil(predicate: () => qnTask.IsCompleted);

        if (qnTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {qnTask.Exception}");
        }

        var a1Task = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).Child($"{index}").Child("A1").SetValueAsync(_answer1);

        yield return new WaitUntil(predicate: () => a1Task.IsCompleted);

        if (a1Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a1Task.Exception}");
        }

        var a2Task = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).Child($"{index}").Child("A2").SetValueAsync(_answer2);

        yield return new WaitUntil(predicate: () => a2Task.IsCompleted);

        if (a2Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a2Task.Exception}");
        }

        var a3Task = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).Child($"{index}").Child("A3").SetValueAsync(_answer3);

        yield return new WaitUntil(predicate: () => a3Task.IsCompleted);

        if (a3Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a3Task.Exception}");
        }

        EditQuestionPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);// shift this to button
    }
    //This method check whether the room exist when making an assignment
    public void checkRoomExist()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StartCoroutine(CheckExistingRoom());
    }
    //This method compares the room to the existing room in firebase
    private IEnumerator CheckExistingRoom(){
        Room = createRoomID.text;
        if (Room == "")
        {
            errormsg.text = "Please input something.";
        }
        else{
            var DBTask = DBreference.Child("custom").GetValueAsync();
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
            roomexist = false;
            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else{
                DataSnapshot snapshot = DBTask.Result;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    string roomid = childSnapshot.Key.ToString();
                    if (roomid == Room) {
                        roomexist = true;
                        errormsg.text = "Room already exist!";
                    }
                }
                if (roomexist != true)
                {
                SelectionPanel.gameObject.SetActive(true);
                StartPanel.gameObject.SetActive(false);
                errormsg.text = "";
                }
            }
        }
        
    }
    //This is the method to display all the questions to be selected when the user wants to make an assignment
    public void displayallquestions(){
        StartCoroutine(filterquestions());
    }
    //This method pull the relevant question and create the component to be displayed in the scrollview item
    public IEnumerator filterquestions(){
        int world = FilterAssignment.World;
        int section = FilterAssignment.Section;
        string difficulty = FilterAssignment.Difficulty;

         var DBTask = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (Transform child in CustomquestionListContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string question = childSnapshot.Child("Question").Value.ToString();
                GameObject CustomquestionListElement = Instantiate(CustomquestionElement, CustomquestionListContent);
                CustomquestionListElement.GetComponent<QuestionListItem>().NewQuestionItem(question);
            }
        }
    }
    //This sets the question count to 0 for every new room
    public void newRoom()
    {
        questionNo = 0;
    }
    //This method will add the question into an assignment
    public void Addquestion(string _question){
        CustomQuestion = _question;
        StartCoroutine(LoadQuestionAndAnswer());
        StartCoroutine(PushtoDB());
    }
    //This method will pull the Answers to the question selected
    public IEnumerator LoadQuestionAndAnswer(){
        int world = FilterAssignment.World;
        int section = FilterAssignment.Section;
        string difficulty = FilterAssignment.Difficulty;

         var DBTask = DBreference.Child("Qns").Child($"{world}").Child($"{section}").Child(difficulty).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
           
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string question = childSnapshot.Child("Question").Value.ToString();
                if (question == CustomQuestion)
                {
                    A1 = childSnapshot.Child("A1").Value.ToString();
                    A2 = childSnapshot.Child("A2").Value.ToString();
                    A3 = childSnapshot.Child("A3").Value.ToString();
                    Debug.Log(A1);
                }
            }
        }
    }
    //The method pushed the question and answers selected into the room in the firebase backend
    public IEnumerator PushtoDB(){
        int world = FilterAssignment.World;
        int section = FilterAssignment.Section;
        string difficulty = FilterAssignment.Difficulty;
        questionNo = questionNo + 1;

        var qnTask = DBreference.Child("custom").Child(Room).Child($"{questionNo}").Child("Question").SetValueAsync(CustomQuestion);

        yield return new WaitUntil(predicate: () => qnTask.IsCompleted);

        if (qnTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {qnTask.Exception}");
        }

        var a1Task = DBreference.Child("custom").Child(Room).Child($"{questionNo}").Child("A1").SetValueAsync(A1);

        yield return new WaitUntil(predicate: () => a1Task.IsCompleted);

        if (a1Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a1Task.Exception}");
        }

        var a2Task = DBreference.Child("custom").Child(Room).Child($"{questionNo}").Child("A2").SetValueAsync(A2);

        yield return new WaitUntil(predicate: () => a2Task.IsCompleted);

        if (a2Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a2Task.Exception}");
        }

        var a3Task = DBreference.Child("custom").Child(Room).Child($"{questionNo}").Child("A3").SetValueAsync(A3);

        yield return new WaitUntil(predicate: () => a3Task.IsCompleted);

        if (a3Task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {a3Task.Exception}");
        }

        FirebaseUser User;
        User = FirebaseManager.User;
        string username = "";
        var aTask = DBreference.Child("teachers").Child(User.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => aTask.IsCompleted);
        Debug.Log("reached here at users");
        if (aTask.Exception != null)
        {
            Debug.Log("hello");
            Debug.LogWarning(message: $"Failed to register task with {aTask.Exception}");
        }
        else if (aTask.Result.Value == null)
        {
            Debug.Log("what");
        }
        else
        {
            DataSnapshot snapshot = aTask.Result;
            username = snapshot.Child("username").Value.ToString();
        }
        var UCTask = DBreference.Child("custom").Child(Room).Child("usercreated").SetValueAsync(username);

        yield return new WaitUntil(predicate: () => UCTask.IsCompleted);

        if (UCTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {UCTask.Exception}");
        }
        var UserTask = DBreference.Child("custom").Child(Room).Child("users").SetValueAsync(null);

        yield return new WaitUntil(predicate: () => UserTask.IsCompleted);

        if (UserTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {UserTask.Exception}");
        }
    }
    //This methods checks if the custom room exist to view scores
    public void checkRoomExistForViewScore()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StartCoroutine(CheckExistingRoomView());
    }
    //This method will check the custom room with the pre-existing rooms in the firebase backend
    private IEnumerator CheckExistingRoomView(){
        Room = ViewRoomID.text;
        //Debug.Log(Room);
        var DBTask = DBreference.Child("custom").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        roomexist = false;
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else{
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string roomid = childSnapshot.Key.ToString();
                if (roomid == Room) {
                    ScorePanel.gameObject.SetActive(true);
                    StartPanel.gameObject.SetActive(false);
                    roomexist = true;
                    errormsg1.text = "";
                }
            }
            if (roomexist != true)
            {
                errormsg1.text = "Room does not exist!";
            }
        }
    }
    //This method displays all the students and their score for the assignment
    public void displayStudentScores(){
        StartCoroutine(StudentScore());
    }
    //This method pulls the student data and score from the firebase
    public IEnumerator StudentScore(){
        Room = ViewRoomID.text;

         var DBTask = DBreference.Child("custom").Child(Room).Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.Log("Yea");
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (Transform child in studentListContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string score = childSnapshot.Value.ToString();
                string user = childSnapshot.Key.ToString();
                GameObject studentListElement = Instantiate(studentElement, studentListContent);
                studentListElement.GetComponent<StudentScoreItem>().CreateItem(user, score);
            }
        }
    }

    //Function for the scoreboard button
    public void ScoreboardButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StartCoroutine(LoadScoreboardData());
    }
    //This method pulls the data from firebase to be displayed
    private IEnumerator LoadScoreboardData()
    {
        int rank = 0;
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

            //Destroy any existing scoreboard elements
            foreach (Transform child in leaderboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                rank += 1;
                string username = childSnapshot.Child("username").Value.ToString();
                int points = int.Parse(childSnapshot.Child("TotalPoints").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(leaderElement, leaderboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement($"{rank}", username, worldsection, points);
            }

            //Go to scoreboard screen
            //UIcontroller.instance.PressLeaderboardButton();
        }
    }
    //This method is called when the leaderboard is filtered
    public void displayWorldSectionData()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        StartCoroutine(LoadWorldSectionData());
    }
    //This method pulls the relevant data to be displayed
    private IEnumerator LoadWorldSectionData()
    {
        int rank = 0;
        int worldNumber = UIcontroller.WorldLdrboard;
        int sectionNumber = UIcontroller.SectionLdrboard;
        Debug.Log(worldNumber);
        Debug.Log(sectionNumber);
        //Get all the users data ordered by points
        var DBTask = DBreference.Child("users").OrderByChild("BattleStats/" + $"{worldNumber}" + "/" + $"{sectionNumber}" + "/Points").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in leaderboardContent.transform)
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

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(leaderElement, leaderboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement($"{rank}", username, $"{worldNumber}" + "." + $"{sectionNumber}", points);
            }
        }
    }
}
    
