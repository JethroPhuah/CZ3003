using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//Done by Jun Hao and Zhi Fah

public class UIcontroller : MonoBehaviour
{
    [SerializeField] private AudioClip cfmClickSFX;

    // This is an instance of UIcontroller
    public static UIcontroller instance;
    //This is the panel to add question
    public GameObject AddQuestionPanel;
    //This is the leaderboard panel
    public GameObject LeaderBoardPanel;
    //This is the view statistic panel
    public GameObject ViewStatisticPanel;
    //This is the make an assignment panel
    public GameObject MakeAssignmentPanel;
    //This is the function selection panel
    public GameObject FunctionSelectionPanel;
    //This is the edit question panel
    public GameObject EditQuestionPanel;
    //This is the score panel 
    public GameObject ScorePanel;
    //This is the dropdown for world in the leaderboard panel
    public Dropdown WorldLeaderboard;
    //This is the variable for the selected world
    public static int WorldLdrboard;
    //This is the dropdown for section in the leaderboard panel
    public Dropdown SectionLeaderboard;
    //This is the variable for the selected section
    public static int SectionLdrboard;
    //This method create an instance of the script
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
    //This method is called when the user selects to add a question
    public void PressAddQuestionButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        AddQuestionPanel.gameObject.SetActive(true);
        FunctionSelectionPanel.gameObject.SetActive(false);
    }
    //This method is called when the user selects the leaderboard
    public void PressLeaderboardButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        LeaderBoardPanel.gameObject.SetActive(true);
        FunctionSelectionPanel.gameObject.SetActive(false);
    }
    //This method is called when the user wants to view statistics
    public void PressViewStatisticButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ViewStatisticPanel.gameObject.SetActive(true);
        FunctionSelectionPanel.gameObject.SetActive(false);
    }
    //This method is called when the user wants to make an assignment
    public void PressMakeAssignmentButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        MakeAssignmentPanel.gameObject.SetActive(true);
        FunctionSelectionPanel.gameObject.SetActive(false);
    }
    //This method is called when the user wants to edit a question
    public void PressEditQuestionButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        EditQuestionPanel.gameObject.SetActive(true);
        FunctionSelectionPanel.gameObject.SetActive(false);
    }
    //This method is called when the user wants to exit the add question panel
    public void PressBackAddQuestionButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        AddQuestionPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);
    }
    //This method is called when the user wants to exit the leaderboard panel
    public void PressBackLeaderboardButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        LeaderBoardPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);
    }
    //This method is called when the user wants to exit the view statistic panel
    public void PressBackViewStatisticButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ViewStatisticPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);
    }
    //This method is called when the user wants to exit the make an assignment panel
    public void PressBackMakeAssignmentButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        MakeAssignmentPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);
    }
    //This method is called when the user wants to exit the edit question panel
    public void PressBackEditQuestionButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        EditQuestionPanel.gameObject.SetActive(false);
        FunctionSelectionPanel.gameObject.SetActive(true);
    }
    //This method is called when the user wants to exit the teacher UI
    public void Presslogoutbutton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        SceneManager.LoadScene("LoginScene");
    }
    //This method is called when the user wants to exit the score panel
    public void PressBackScorePanel()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ScorePanel.gameObject.SetActive(false);
        MakeAssignmentPanel.gameObject.SetActive(true);
    }
    //This method obtains the world selected by the user
    public void WorldLdrboardSelect()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        Debug.Log(WorldLeaderboard.options[WorldLeaderboard.value].text);
        switch (WorldLeaderboard.options[WorldLeaderboard.value].text)
        {
            case "OODP":
                WorldLdrboard = 1;
                break;
            case "SE":
                WorldLdrboard = 2;
                break;
            case "SSAD":
                WorldLdrboard = 3;
                break;
            default:
                Debug.Log("Error Occured");
                break;
        }
        Debug.Log(WorldLdrboard);
    }
    //This method obtains the section selected by the user
    public void SectionLdrboardSelection()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        SectionLdrboard = int.Parse(SectionLeaderboard.options[SectionLeaderboard.value].text);
        Debug.Log(SectionLdrboard);
    }
}
