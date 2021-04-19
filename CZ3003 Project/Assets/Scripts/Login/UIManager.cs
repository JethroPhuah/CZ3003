// Authors: Jethro, Su Te and Shaun
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public LevelLoader levelLoader;
    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject scoreboardUI;
    [SerializeField] private AudioClip cfmClickSFX;

    public Dropdown WorldSelection;
    public static int World;
    public Dropdown SectionSelect;
    public static int Section;

    public Dropdown WorldLeaderboard;
    public static int WorldLdrboard;
    public Dropdown SectionLeaderboard;
    public static int SectionLdrboard;

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

    // Function to turn off all screens from the main menu UI
    public void ClearScreen() 
    {
        if(loginUI!=null)
        loginUI.SetActive(false);
        if(registerUI!=null)
        registerUI.SetActive(false);

        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }

    // Function to turn off all screens from the teacher UI
    public void ClearScreenForTeacher() 
    {
        if(loginUI!=null)
        loginUI.SetActive(true);
        if(registerUI!=null)
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }

    // Function to switch to Login UI
    public void LoginScreen() 
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        loginUI.SetActive(true);
    }

    // Function to switch to Register UI
    public void RegisterScreen() 
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        registerUI.SetActive(true);
    }

    // Function to switch to user data UI
    public void UserDataScreen()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        userDataUI.SetActive(true);
    }

    // Function to switch to the game UI
    public void EnterGame()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        levelLoader.LoadCharSel();
    }

    // Function to display scoreboard UI
    public void ScoreboardScreen() 
    {

        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        scoreboardUI.SetActive(true);
    }

    // Function for selecting world
    public void WorldSelect()
    {
        switch (WorldSelection.options[WorldSelection.value].text)
        {
            case "OODP":
                World = 1;
                break;
            case "SE":
                World = 2;
                break;
            case "SSAD":
                World = 3;
                break;
            default:
                Debug.Log("Error Occured");
                break;
        }
    }

    // Function for selecting section
    public void SectionSelection()
    {
        Section = int.Parse(SectionSelect.options[SectionSelect.value].text);
    }

    // Function to display world leaderboard for the specific world
    public void WorldLdrboardSelect()
    {
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
    }

    // Function to store section leaderboard selection 
    public void SectionLdrboardSelection()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        SectionLdrboard = int.Parse(SectionLeaderboard.options[SectionLeaderboard.value].text);
    }
    
}
