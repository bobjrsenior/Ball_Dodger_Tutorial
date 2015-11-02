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

    /// <summary>
    /// Starts the game by loading the game scene
    /// </summary>
    public void play()
    {
        Application.LoadLevel(1);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /////Stat Retreival

    /// <summary>
    /// Retrieve the best wave from the PlayerPrefs
    /// </summary>
    /// <returns>Highest wave achieved</returns>
    int getHighestWave()
    {
        return PlayerPrefs.GetInt("wave", 0);
    }

    /// <summary>
    /// Retrieve the best time from the PlayerPrefs
    /// </summary>
    /// <returns>Best time achieved</returns>
    float getHighestTime()
    {
        return PlayerPrefs.GetFloat("time", 0);
    }

}
