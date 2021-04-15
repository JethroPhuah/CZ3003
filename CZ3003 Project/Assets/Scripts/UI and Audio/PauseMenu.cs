// Authors: Daryl Neo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Slider AudioSlider;
    public Slider SFXSlider;
    
    [SerializeField] private AudioClip levelMusic;
    public LevelLoader battleLoader;
    public FirebaseManager firebaseManager;
    public bool GameisPaused = false;

    public GameObject pauseMenuUI;
    
    // Update is called once per frame
    void Update()
    {
        // Ensures volume will always aligned with slider values
        AudioManager.Instance.SetMusicVolume(AudioSlider.value);
        AudioManager.Instance.SetSFXVolume(SFXSlider.value);

        // When ESC is pressed
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // Pause the game if game not paused
            if(GameisPaused)
            {
                Resume();
            }
            // Resume the game if game is paused
            else
            {
                Pause();
            }
        }
    }

    // Method for Resume
    public void Resume()
    {
        // Resume activeSource music
        AudioManager.Instance.ResumeMusic();

        // Disable the Pause Menu UI
        pauseMenuUI.SetActive(false);

        // Reset timeScale to default
        Time.timeScale = 1f;

        // Set bool to false
        GameisPaused = false;
    }

    // Method for Pause
    public void Pause()
    {
        // Pause activeSource music
        AudioManager.Instance.PauseMusic();

        // Enable the Pause Menu UI
        pauseMenuUI.SetActive(true);

        // Stop time while game is paused
        Time.timeScale = 0f;

        // Set bool to true
        GameisPaused = true;
    }

    // Method for Quit
    public void Quit()
    {
        // Play fade to black animation
        battleLoader.FadetoBlack();

        // Quit the application
        Application.Quit();
    }

    // Method for Menu
    public void Menu()
    {
        // Play Menu music
        AudioManager.Instance.PlayMusic(levelMusic);

        // Disable Pause menu
        pauseMenuUI.SetActive(false);

        // Set bool to false
        GameisPaused = false;

        // Reset timescale to default
        Time.timeScale = 1f;

        // Transition Scene to Main menu while having animation
        battleLoader.LoadMain();
    }

    // Method for Save
    public void Save() {
        // Update the Stars earned to Database
        firebaseManager.UpdateStarsToDb();

        // Resume activeSource music
        AudioManager.Instance.ResumeMusic();

        // Enable Pause UI
        pauseMenuUI.SetActive(false);

        // Reset timescale to default
        Time.timeScale = 1f;

        // Resume activeSource music
        AudioManager.Instance.ResumeMusic();

        // Set bool to false
        GameisPaused = false;
    }
}
