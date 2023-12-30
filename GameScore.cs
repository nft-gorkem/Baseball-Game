using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameScore : MonoBehaviour
{

    // Start is called before the first frame update
    public TextMeshProUGUI bluescoreText, redscoreText, roundText;

    void Start()
    {
        //scoreText.text = scoreText.ToString();
        UpdateTheText();
        //BlueTeamScores();
        //RedTeamScores();
    }
    void Update()
    {
        UpdateTheText();
    }
    public void UpdateTheText()
    {
        bluescoreText.GetComponent<TMPro.TextMeshProUGUI>().text = PlayerPrefs.GetInt("BlueScore", 0).ToString();
        redscoreText.GetComponent<TMPro.TextMeshProUGUI>().text = PlayerPrefs.GetInt("RedScore", 0).ToString();
        roundText.GetComponent<TMPro.TextMeshProUGUI>().text = PlayerPrefs.GetInt("roundCount", 0).ToString();
    }

    public void BlueTeamScores()
    {
        int x = PlayerPrefs.GetInt("BlueScore", 0);
        x += 1;
        PlayerPrefs.SetInt("BlueScore", x);
        bluescoreText.text = ("Blue Team " + PlayerPrefs.GetInt("BlueScore", 0)).ToString();
    }

    public void RedTeamScores()
    {
        int x = PlayerPrefs.GetInt("RedScore", 0);
        x += 1;
        PlayerPrefs.SetInt("RedScore", x);
        redscoreText.text = ("Red Team " + PlayerPrefs.GetInt("RedScore", 0)).ToString();
    }

    void NextLevel()//Increase the level
    {
        PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel", 1) + 1);
    }
}
