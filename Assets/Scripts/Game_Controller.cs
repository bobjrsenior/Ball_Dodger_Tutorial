using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour {

    ////Game Object and Componenet References

    /// <summary>
    /// References the UI object that will display how long you have been alive
    /// </summary>
    public Text playTime_UI;

    /// <summary>
    /// Referenced UI object that will display what wave you are on
    /// </summary>
    public Text wave_UI;

    /////Variables

    /// <summary>
    /// Holds how long you have been alive
    /// </summary>
    private float playTime = 0.0f;

    /// <summary>
    /// Holds what wave you are currently on
    /// </summary>
    private int wave = 1;

    /////Spawning Related Variables

    /// <summary>
    /// The minimum time to wait between ball spawns
    /// </summary>
    private float minSpawnDelay = 1.0f;

    /// <summary>
    /// The odds of spawning a ball in a current from (excluding minSpawnDelay)
    /// </summary>
    private float chanceToSpawn = 0.8f;

    /// <summary>
    /// Time Left until the next wave starts
    /// </summary>
    private float timeTilNextWave = 15.0f;

	// Use this for initialization
	void Start () {
        wave_UI.text = "Wave: " + wave;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        playTime += Time.fixedDeltaTime;
        playTime_UI.text = "Time: " + (int) playTime;
	}
}
