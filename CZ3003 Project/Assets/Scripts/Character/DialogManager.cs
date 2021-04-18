// Authors: Jethro
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    // DialogBox object in unity.
    [SerializeField] GameObject dialogBox;

    // Dialogtext object in unity.
    [SerializeField] Text dialogText;

    // Speed at which letters are printed out in the dialogbox.
    [SerializeField] int lettersPerSec;

    // // Creating an instance for Dialogmanager to be called in another class.
    public static DialogManager Instance{get;private set;}

    // Instantiating an event to show dialog for other classes to subscribe to it.
    public event Action OnShowDialog;

    // Instantiating an event to signal an end to dialog for other classes to subscribe to it.
    public event Action OnCloseDialog;

    // bool variable to see if the dialogbox is showing or not.
    public bool IsShowing{get;private set;}
    
    // Reference object of dialog.
    Dialog dialog;

    int currentLine = 0;

    // bool variable to see if the dialog box is still printing.
    bool isTyping;

    // Action to show that the dialog is over.
    Action onDialogFinished;

    // Awake function to call whenever this class is called.
    private void Awake() 
    {
        Instance=this;    
    }

    // Method to Handle all updates on every frames that will be called in GameController/PVP Controller class.
    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            ++currentLine;
            if(currentLine<dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    } 

    // Method to print out the first line of dialog and to call the onShowDialog Event.
    public IEnumerator ShowDialog(Dialog dialog, Action onFinished=null)
    {
        yield return new WaitForEndOfFrame();
        IsShowing=true;
        OnShowDialog?.Invoke();
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
        Debug.Log("How many times");
    }

    // Method to print out the words one by one in the dialog box.
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true; // to ensure that the dialog box will finish displaying first line even if player enters spacebar
        dialogText.text = "";
        foreach(var letter in line.ToCharArray())
        {
            dialogText.text+=letter;
            yield return new WaitForSeconds(1f/lettersPerSec);
        }
        isTyping = false;
    }
}
