// Authors: Jethro, Su Te
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

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        if(loginUI!=null)
        loginUI.SetActive(false);
        if(registerUI!=null)
        registerUI.SetActive(false);

        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }
    public void ClearScreenForTeacher() //Turn off all screens
    {
        if(loginUI!=null)
        loginUI.SetActive(true);
        if(registerUI!=null)
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }

    public void LoginScreen() //Back button
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void RegisterScreen() // Regester button
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void UserDataScreen() //Logged in
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        userDataUI.SetActive(true);
    }

    public void EnterGame() //Enter Game
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        levelLoader.LoadCharSel();
    }

    public void ScoreboardScreen() //Scoreboard button
    {

        AudioManager.Instance.PlaySFX(cfmClickSFX);
        ClearScreen();
        scoreboardUI.SetActive(true);
    }

    public void WorldSelect()
    {
        Debug.Log(WorldSelection.options[WorldSelection.value].text);
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
        Debug.Log(World);
    }

    public void SectionSelection()
    {
        Section = int.Parse(SectionSelect.options[SectionSelect.value].text);
        Debug.Log(Section);
    }

    public void WorldLdrboardSelect()
    {
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

    public void SectionLdrboardSelection()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        SectionLdrboard = int.Parse(SectionLeaderboard.options[SectionLeaderboard.value].text);
        Debug.Log(SectionLdrboard);
    }
    
}
