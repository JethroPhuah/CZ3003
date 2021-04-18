// Authors: Shaun, Su Te
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{

    public TMP_Text rankText;
    public TMP_Text usernameText;
    public TMP_Text worldsectionText;
    public TMP_Text pointsText;

    public void NewScoreElement (string _rank, string _username, string _worldsection, int _points)
    {
        rankText.text = _rank;
        usernameText.text = _username;
        worldsectionText.text = _worldsection;
        pointsText.text = _points.ToString();
    }

}