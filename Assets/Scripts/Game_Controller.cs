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

    public Text waveStatsUI;

    public Text timeStatsUI;

    public GameObject gameOverCanvas;

    /////Variables

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
        gameController = this;
        lost = false;
        Random.seed = (int)System.DateTime.Now.Ticks;
        gameOverCanvas.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        halfWindowSize.y = 4.85f;
        halfWindowSize.x = halfWindowSize.y * Screen.width / Screen.height;
        increaseWave();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!lost)
        {
            playTime += Time.fixedDeltaTime;
            playTimeUI.text = "Time: " + (int)playTime;

            timeTilNextWave -= Time.fixedDeltaTime;
            if (timeTilNextWave <= 0)
            {
                increaseWave();
            }

            spawnTimer += Time.fixedDeltaTime;
            if (spawnTimer >= minSpawnDelay)
            {
                if (Random.Range(0.0f, 1.0f) > chanceToSpawn)
                {
                    spawnTimer = 0;
                    spawnEnemy();
                }
            }
        }
	}

    public void increaseWave()
    {
        //Update all of the spawn/level related variables
        timeTilNextWave = 15.0f + (wave * 1.5f);
        minSpawnDelay = 1.0f * (1.0f / (wave + 1));
        chanceToSpawn = 0.8f * (1.0f / ((0.25f * wave) + 1));
        wave++;
        waveUI.text = "Wave: " + wave;
    }

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
        enemyToSpawn.setUpEnemy(Enemy_Controller.EnemyType.Simple, halfWindowSize.x, wave);

    }

    public void lostGame()
    {
        lost = true;
        gameOverCanvas.SetActive(true);
        waveStatsUI.text = "Wave: " + wave;
        timeStatsUI.text = "Time: " + (int) (playTime * 100) / 100.0f;
    }

    public void restartLevel()
    {
        Application.LoadLevel(1);
    }

    public void mainMenu()
    {
        Application.LoadLevel(0);
    }
}
