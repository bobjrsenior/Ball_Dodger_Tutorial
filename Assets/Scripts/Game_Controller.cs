﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour {

    /// <summary>
    /// Static reference to this script for easy access by other scripts
    /// </summary>
    public static Game_Controller gameController;

    ////Game Object and Componenet References

    /// <summary>
    /// Links to the player's transform component
    /// </summary>
    public Transform player;

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
    /// References the UI object that will display your current lives
    /// </summary>
    public Text livesUI;

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
        waveUI.text = "Wave: " + ++wave;
    }

    /// <summary>
    /// Adds an Enemy to the Enemy pool
    /// </summary>
    /// <param name="enemy">The Enemy object to add to the Enemy pool</param>
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
        //Retrieve enemy to spawn
        Enemy_Controller enemyToSpawn;
        //If the Enemy pool isn't empty, get it there
        if(head != null)
        {
            head.gameObject.SetActive(true);
            enemyToSpawn = head;
            head = head.next;
            enemyToSpawn.next = null;
        }//Otherwise, make a new enemy
        else
        {
            enemyToSpawn = (Instantiate(EnemyPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Enemy_Controller>();
        }
        Enemy_Controller.EnemyType type = Enemy_Controller.EnemyType.Simple;
        //Determine the Enemy type based on wave number and a bit of randomness
        if(wave == 1)
        {
            type = Enemy_Controller.EnemyType.Simple;
        }
        else if(wave < 4)
        {
            int temp = (int)Mathf.Round(Random.Range(0.0f, 0.6f + (0.2f * (wave - 2))));
            if(temp == 1)
            {
                type = Enemy_Controller.EnemyType.Sin;
            }
        }
        else if(wave < 7)
        {
            int temp = (int)Random.Range(0.0f, 2.25f + (0.2f * (wave - 2)));
            if (temp == 1)
            {
                type = Enemy_Controller.EnemyType.Sin;
            }
           else if (temp == 2)
            {
                type = Enemy_Controller.EnemyType.Traveler;
            }
        }
        else
        {
            int temp = (int)Random.Range(0.0f, 3.25f + (0.2f * (wave - 7)));
            if(temp > 3)
            {
                temp = (wave * temp) % 4; 
            }
            if (temp == 1)
            {
                type = Enemy_Controller.EnemyType.Sin;
            }
            else if (temp == 2)
            {
                type = Enemy_Controller.EnemyType.Traveler;
            }
            else if (temp == 3)
            {
                type = Enemy_Controller.EnemyType.Chaser;
            }
        }
        //Setup the enemy and have it start
        enemyToSpawn.setUpEnemy(type, wave, halfWindowSize);

    }

    /// <summary>
    /// Called when the game has been lost
    /// Checks for highscores and sets up the game over UI
    /// </summary>
    public void lostGame()
    {
        lost = true;
        //Show the game over screen
        gameOverCanvas.SetActive(true);
        //Display what wave you were on and how long you survived
        waveStatsUI.text = "Wave: " + wave;
        timeStatsUI.text = "Time: " + (int) (playTime * 100) / 100.0f;

        //If it was your highest wave
        if (wave > curBestWave)
        {
            //Show that it was a new best and save the new record
            waveStatsUI.text += "\nBest: " + wave + "\nNew Best";
            curBestWave = wave;
            storeHighestWave();
        }
        else
        {
            //Show the current record
            waveStatsUI.text += "\nBest: " + curBestWave;
        }
        //If it was your best time
        if (playTime > curBestTime)
        {
            //Show that it was a new best and save the new record
            timeStatsUI.text += "\nBest: " + ((int)(playTime * 100) / 100.0f) + "\nNew Best";
            curBestTime = playTime;
            storeHighestTime();
            //Write PlayerPrefs to disk
            //Not done if you get a best wave because you will always also get a best time
            PlayerPrefs.Save();
        }
        else
        {
            //Show the current record
            timeStatsUI.text += "\nBest: " + (int)(curBestTime * 100) / 100.0f; ;
        }
    }

    /// <summary>
    /// Updates the Lives_UI_Text
    /// </summary>
    public void updateLives(int lives)
    {
        livesUI.text = "Lives: " + lives;
    }

    /////UI Button Functions

    /// <summary>
    /// Starts the game over by reloading the scene
    /// </summary>
    public void restartLevel()
    {
        Application.LoadLevel(1);
    }

    /// <summary>
    /// Goes to the main menu by loading the main menu scene
    /// </summary>
    public void mainMenu()
    {
        Application.LoadLevel(0);
    }


    /////Stat Storage and Retreival

    /// <summary>
    /// Stores the new highest wave
    /// </summary>
    void storeHighestWave()
    {
        PlayerPrefs.SetInt("wave", wave);
    }

    /// <summary>
    /// Stores the new highest/best time
    /// </summary>
    void storeHighestTime()
    {
        PlayerPrefs.SetFloat("time", playTime);
    }

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
