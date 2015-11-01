using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main_Menu_Controller : MonoBehaviour {

    /// <summary>
    /// References the UI object that will display the players best wave on the main menu ui
    /// </summary>
    public Text bestWaveUI;

    /// <summary>
    /// References the UI object that will display the players best play time on the main menu ui
    /// </summary>
    public Text bestTimeUI;

    void Start()
    {
        //Set up the highscores in the ui
        bestWaveUI.text = "Best Wave: " + getHighestWave();
        bestTimeUI.text = "Best Time: " + (int) (getHighestTime() * 100) / 100.0f;
    }

    /////UI Button Functions

    public void play()
    {
        Application.LoadLevel(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    /////Stat Storage and Retreival

    int getHighestWave()
    {
        return PlayerPrefs.GetInt("wave", 0);
    }

    float getHighestTime()
    {
        return PlayerPrefs.GetFloat("time", 0);
    }

}
