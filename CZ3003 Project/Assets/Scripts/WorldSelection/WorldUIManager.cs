// Authors: Jethro, Jun Hao
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
//Done by Jun Hao and Jethro and Darryl
public class WorldUIManager : MonoBehaviour
{
    [Header("Sounds")]
    //This is the music for the game
    [SerializeField] private AudioClip cfmClickSFX;
    //This is an instance of the world UI manager
    public static WorldUIManager instance;
    //This is the map selection panel
    public GameObject mapSelectionPanel;
    //This is the level selection panels
    public GameObject[] levelSelectionPanels;
    //This is the mode selection panel
    public GameObject modeSelectionPanel;
    //This is the PVP selection panel
    public GameObject PVPSelectionPanel;
    //This is the custom mode selection panel
    public GameObject CustomSelection;
    //This is the stars gained by the user
    public int stars;
    //This displays the total stars a user has
    public Text startText;
    //This are the worlds that can be selected
    public WorldSelection[] WorldSelections;
    //This is the text containing the total stars
    public Text[] questStarsText;
    //This is the text containing the stars for each world
    public Text[] lockedStarsText;
    //This is the text containing the stars for each world
    public Text[] unlockedStarsText;
    //This method ensures there is only 1 instance of the object
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        //DontDestroyOnLoad(gameObject);

    }
    //This method allows the user to select their character
    private void Start()
    {

        int temp = PlayerPrefs.GetInt("CharacterSelected");
        //PlayerPrefs.DeleteAll();   //delete all player key and values from player prefs
        PlayerPrefs.SetInt("CharacterSelected",temp);
    }
    //This method update the stars the user has
    private void Update()
    {
        UpdateUnlockedStar();
        UpdateStars();
    }
    //This method update the stars the user has for the world
    private void UpdateUnlockedStar()
    {
        int totalStars1 = PlayerPrefs.GetInt("Lv" + 1) + PlayerPrefs.GetInt("Lv" + 2) + PlayerPrefs.GetInt("Lv" + 3);
        int totalStars2 = PlayerPrefs.GetInt("Lv" + 4) + PlayerPrefs.GetInt("Lv" + 5) + PlayerPrefs.GetInt("Lv" + 6);
        int totalStars3 = PlayerPrefs.GetInt("Lv" + 7) + PlayerPrefs.GetInt("Lv" + 6) + PlayerPrefs.GetInt("Lv" + 9);
        unlockedStarsText[0].text = totalStars1.ToString() + "/ 12";
        unlockedStarsText[1].text = totalStars2.ToString() + "/ 12";
        unlockedStarsText[2].text = totalStars3.ToString() + "/ 12";
    }
    //This method update the total stars the user has
    private void UpdateStars()
    {
        stars = PlayerPrefs.GetInt("Lv" + 1) + PlayerPrefs.GetInt("Lv" + 2) + PlayerPrefs.GetInt("Lv" + 3) + PlayerPrefs.GetInt("Lv" + 4) + PlayerPrefs.GetInt("Lv" + 5) + PlayerPrefs.GetInt("Lv" + 6) + PlayerPrefs.GetInt("Lv" + 7) + PlayerPrefs.GetInt("Lv" + 8) + PlayerPrefs.GetInt("Lv" + 9) + PlayerPrefs.GetInt("Lv" + 10) + PlayerPrefs.GetInt("Lv" + 11) + PlayerPrefs.GetInt("Lv" + 12) + PlayerPrefs.GetInt("Lv" + 13) + PlayerPrefs.GetInt("Lv" + 14) + PlayerPrefs.GetInt("Lv" + 15) + PlayerPrefs.GetInt("Lv" + 16);
        startText.text = stars.ToString();
    }
    //This method is called when the user click on a map
    public void PressMapButton(int _mapIndex)
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        levelSelectionPanels[_mapIndex].gameObject.SetActive(true);
        mapSelectionPanel.gameObject.SetActive(false);
    }
    //This method is called when the user wants to return to the map selection panel
    public void PressSectionBackButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        mapSelectionPanel.gameObject.SetActive(true);
        for (int i=0; i< levelSelectionPanels.Length;i++)
            levelSelectionPanels[i].gameObject.SetActive(false);
    }
    //This method allow the gameplay to start
    //_sceneName: The scene to transition to
    public void SceneTransition(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }
    //This method is called when the user clicks on quest
    public void PressQuestModeButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        mapSelectionPanel.gameObject.SetActive(true);
        modeSelectionPanel.gameObject.SetActive(false);
    }
    //This method returns user back to mode selection
    public void PressQuestBackButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        mapSelectionPanel.gameObject.SetActive(false);
        modeSelectionPanel.gameObject.SetActive(true);
    }
    //This button is called when the user clicks on custom
    public void PressCustomModeButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        PVPSelectionPanel.gameObject.SetActive(false);
        modeSelectionPanel.gameObject.SetActive(false);
        CustomSelection.gameObject.SetActive(true);
    }
    //This method is called when the user clicks on the PVP button
    public void PressPVPModeButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        PVPSelectionPanel.gameObject.SetActive(true);
        modeSelectionPanel.gameObject.SetActive(false);
    }
    //This method returns user back to mode selection
    public void PressPVPBackButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        PVPSelectionPanel.gameObject.SetActive(false);
        modeSelectionPanel.gameObject.SetActive(true);
    }
    //This method returns the user to the level selection
    public void BackLevelSelection()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        SceneManager.LoadScene("Map Selection");
    }
    //This method is call when the gameplay starts
    public void gameStart()
    {
        for (int i=0; i< levelSelectionPanels.Length;i++)
            levelSelectionPanels[i].gameObject.SetActive(false);
    }
    //This method clear all the panels on the screen
    public void setAllToFalse() {
        mapSelectionPanel.gameObject.SetActive(false);
        for (int i=0; i< levelSelectionPanels.Length;i++)
            levelSelectionPanels[i].gameObject.SetActive(false);
        modeSelectionPanel.gameObject.SetActive(false);
        PVPSelectionPanel.gameObject.SetActive(false);
    }
    //This method opens the forum
    public void Openforum()
    {
        Application.OpenURL("https://softwaremon.quora.com/");
    }
    //This method is similar to logout
    public void pressExitButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX); 
        Application.Quit();
    }
    //This method is called when the user is done making their custom room
    public void PressCustomBackButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        modeSelectionPanel.gameObject.SetActive(true);
        CustomSelection.gameObject.SetActive(false);
    }

}
