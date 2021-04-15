using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Done by Jun Hao and Zhi Fah
public class QuestionAdder : MonoBehaviour
{
    [SerializeField] private AudioClip cfmClickSFX;

    // The instance of the file that can be reference by other files later
    public static QuestionAdder instance;
    // The panel where users are going to type their questions and answers
    public GameObject AddQuestionPanel;
    // This panel where the user will filter the world, section and difficulty of where their new question is going to be added
    public GameObject OptionSelectionPanel;
    //The Dropdown for selecting the world
    public Dropdown WorldSelection;
    // Variable to store the world that the user has selected
    public static int World;
    //The Dropdown for selecting the difficulty
    public Dropdown DifficultySelect;
    // Variable to store the difficulty that the user has selected
    public static string Difficulty;
    //The Dropdown for selecting the section
    public Dropdown SectionSelect;
    // Variable to store the section that the user has selected
    public static int Section;
    // This method ensures there is only 1 instance of the selection panel
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
    // This methods extracts the data in the dropdown for world
    public void WorldSelect()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
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
    }
    // This methods extracts the data in the dropdown for difficulty
    public void DifficultySelection()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        Difficulty = DifficultySelect.options[DifficultySelect.value].text;
    }
    // This methods extracts the data in the dropdown for section
    public void SectionSelection()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        Section = int.Parse(SectionSelect.options[SectionSelect.value].text);
    }
    // This method brings the user to the page where he/she can type his new question and answers out
    public void PressNextButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        AddQuestionPanel.gameObject.SetActive(true);
        OptionSelectionPanel.gameObject.SetActive(false);
    }
    //This method brings the user back to the page where he/she can change the filters for world, difficulty and section
    public void PressBackButton()
    {
        AudioManager.Instance.PlaySFX(cfmClickSFX);
        AddQuestionPanel.gameObject.SetActive(false);
        OptionSelectionPanel.gameObject.SetActive(true);
    }
}
