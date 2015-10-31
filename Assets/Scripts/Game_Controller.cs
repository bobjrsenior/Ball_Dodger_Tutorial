using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour {

    /// <summary>
    /// Static reference to this script for easy access by other scripts
    /// </summary>
    public static Game_Controller gameController;

    ////Game Object and Componenet References

    /// <summary>
    /// Links to an Enemy prefab to spawn
    /// </summary>
    public GameObject EnemyPrefab;
    
    /// <summary>
    /// References the UI object that will display how long you have been alive
    /// </summary>
    public Text playTimeUI;

    /// <summary>
    /// Referenced UI object that will display what wave you are on
    /// </summary>
    public Text waveUI;

    /// <summary>
    /// Shows your game/overall best wave during a run (part of game over screen)
    /// </summary>
    public Text waveStatsUI;

    /// <summary>
    /// Shows your game/overall best play time during a run (part of game over screen)
    /// </summary>
    public Text timeStatsUI;

    /// <summary>
    /// The canvas containing the game over screen (de/activated when needed)
    /// </summary>
    public GameObject gameOverCanvas;

    /////Variables

    /// <summary>
    /// Stores the current best wave to avoid unecasarily accessing playerprefs
    /// </summary>
    int curBestWave;

    /// <summary>
    /// Stores the current best play time to avoid unecasaarily accessing playerprefs
    /// </summary>
    float curBestTime;

    /// <summary>
    /// Has the player lost the game?
    /// </summary>
    public static bool lost;

    /// <summary>
    /// Holds how long you have been alive
    /// </summary>
    private float playTime = 0.0f;

    /// <summary>
    /// Holds what wave you are currently on
    /// </summary>
    private int wave = 0;

    /////Spawning Related Variables

    /// <summary>
    /// The minimum time to wait between ball spawns
    /// </summary>
    private float minSpawnDelay = 1.0f;

    /// <summary>
    /// How long has it been since a spawn
    /// </summary>
    private float spawnTimer = 0.0f;

    /// <summary>
    /// The odds of spawning a ball in a current from (excluding minSpawnDelay)
    /// </summary>
    private float chanceToSpawn = 0.8f;

    /// <summary>
    /// Time Left until the next wave starts
    /// </summary>
    private float timeTilNextWave = 15.0f;

    /// <summary>
    /// The head of a Linked List used for object pooling
    /// </summary>
    private Enemy_Controller head = null;

    /// <summary>
    /// The tail of a Linked List used for object pooling
    /// </summary>
    private Enemy_Controller tail = null;

    /// <summary>
    /// Holds half of the Windows size in Unity Units for bounds checking
    /// </summary>
    private Vector2 halfWindowSize;

    void Awake()
    {
        //Seed the random
        Random.seed = (int)System.DateTime.Now.Ticks;

        //Set the static reference to this object
        gameController = this;

        //Since lost is static, it needs its value reset every game
        lost = false;

        //Make sure the game over ui isn't showing
        gameOverCanvas.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        //Store the highscore so you only need to access them when you get a new highscore
        //Instead of every game
        curBestWave = getHighestWave();
        curBestTime = getHighestTime();

        //Get the bounds for the game
        halfWindowSize.y = 4.85f;
        halfWindowSize.x = halfWindowSize.y * Screen.width / Screen.height;
        increaseWave();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //If we haven't lost
        if (!lost)
        {
            //Update playTimeUI
            playTime += Time.fixedDeltaTime;
            playTimeUI.text = "Time: " + (int)playTime;

            //Is it time to go to the next wave?
            timeTilNextWave -= Time.fixedDeltaTime;
            if (timeTilNextWave <= 0)
            {
                increaseWave();
            }

            //Can we try to spawn?
            spawnTimer += Time.fixedDeltaTime;
            if (spawnTimer >= minSpawnDelay)
            {
                //Add a bit of random to when enemies spawn
                if (Random.Range(0.0f, 1.0f) > chanceToSpawn)
                {
                    spawnTimer = 0;
                    spawnEnemy();
                }
            }
        }
	}

    /// <summary>
    /// Go to the next wave and change variables where necasary
    /// </summary>
    public void increaseWave()
    {
        //Update all of the spawn/level related variables
        timeTilNextWave = 15.0f + (wave * 1.5f);
        minSpawnDelay = 1.0f * (1.0f / (wave + 1));
        chanceToSpawn = 0.8f * (1.0f / ((0.25f * wave) + 1));
        wave++;
        waveUI.text = "Wave: " + wave;
    }

    /// <summary>
    /// Adds an Enemy to the Enemy pool
    /// </summary>
    /// <param name="enemy"></param>
    public void addToPool(Enemy_Controller enemy)
    {
        if(head == null)
        {
            head = enemy;
            tail = enemy;
        }
        else
        {
            tail.next = enemy;
            tail = enemy;
        }
    }

    /// <summary>
    /// Spawns an enemy
    /// Uses the Enemy Pool, if it isn't empty (head != null)
    /// </summary>
    private void spawnEnemy()
    {
        Enemy_Controller enemyToSpawn;
        if(head != null)
        {
            head.gameObject.SetActive(true);
            enemyToSpawn = head;
            head = head.next;
            enemyToSpawn.next = null;
        }
        else
        {
            enemyToSpawn = (Instantiate(EnemyPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Enemy_Controller>();
        }
        enemyToSpawn.setUpEnemy(Enemy_Controller.EnemyType.Simple, wave, halfWindowSize);

    }

    /// <summary>
    /// Called when the game has been lost
    /// Checks for highscores and sets up the game over UI
    /// </summary>
    public void lostGame()
    {
        lost = true;
        gameOverCanvas.SetActive(true);
        waveStatsUI.text = "Wave: " + wave;
        timeStatsUI.text = "Time: " + (int) (playTime * 100) / 100.0f;
        if (wave > curBestWave)
        {
            waveStatsUI.text += "\nBest: " + wave + "\nNew Best";
            curBestWave = wave;
            storeHighestWave();
        }
        else
        {
            waveStatsUI.text += "\nBest: " + curBestWave;
        }
        if (playTime > curBestTime)
        {
            timeStatsUI.text += "\nBest: " + ((int)(playTime * 100) / 100.0f) + "\nNew Best";
            curBestTime = playTime;
            storeHighestTime();
            PlayerPrefs.Save();
        }
        else
        {
            timeStatsUI.text += "\nBest: " + (int)(curBestTime * 100) / 100.0f; ;
        }
    }

    public void restartLevel()
    {
        Application.LoadLevel(1);
    }

    public void mainMenu()
    {
        Application.LoadLevel(0);
    }


    void storeHighestWave()
    {
        PlayerPrefs.SetInt("wave", wave);
    }

    void storeHighestTime()
    {
        PlayerPrefs.SetFloat("time", playTime);
    }

    int getHighestWave()
    {
        return PlayerPrefs.GetInt("wave", 0);
    }

    float getHighestTime()
    {
        return PlayerPrefs.GetFloat("time", 0);
    }
}
