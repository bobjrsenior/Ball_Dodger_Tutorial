using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game_Controller : MonoBehaviour {

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

    /////Variables

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

	// Use this for initialization
	void Start () {
        increaseWave();
        Enemy_Controller test = (Instantiate(EnemyPrefab, new Vector3(3, 3, 0), Quaternion.identity) as GameObject).GetComponent<Enemy_Controller>();
        test.gameObject.SetActive(false);
        addToPool(test);
        test = (Instantiate(EnemyPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Enemy_Controller>();
        test.gameObject.SetActive(false);
        addToPool(test);

        spawnEnemy();
        spawnEnemy();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        playTime += Time.fixedDeltaTime;
        playTimeUI.text = "Time: " + (int) playTime;

        timeTilNextWave -= Time.fixedDeltaTime;
        if(timeTilNextWave <= 0)
        {
            increaseWave();
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
            print("Found in pool");
            head.gameObject.SetActive(true);
            enemyToSpawn = head;
            head = head.next;
            enemyToSpawn.next = null;
        }
        else
        {
            enemyToSpawn = (Instantiate(EnemyPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Enemy_Controller>();
        }


    }
}
