using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour {

    /// <summary>
    /// A link to the next enemy
    /// Used for the Enemy Spawn Pool (null if not in pool/is active)
    /// </summary>
    public Enemy_Controller next = null;

    /// <summary>
    /// Links to the Enemy's SpriteRenderer component in order to change colors
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Holds the different types of enemies
    /// </summary>
    public enum EnemyType { Simple, Sin, Traveler, Chaser};

    /// <summary>
    /// Directions the enemy could face on spawn
    /// </summary>
    enum SpawnDirection { North = 0, South = 1, East = 2, West = 3};

    /// <summary>
    /// Which direction the enemy is currently going
    /// </summary>
    private int dir;

    /// <summary>
    /// How fast the enemy moves
    /// </summary>
    private float movementSpeed;

    /// <summary>
    /// Hold extra information the enemy could use
    /// </summary>
    private Vector2 extraInfo;

    /// <summary>
    /// Used for anything timer related
    /// </summary>
    private float timer;

    /// <summary>
    /// Holds half of the Windows size in Unity Units for bounds checking
    /// </summary>
    public Vector2 halfWindowSize;

    void Awake()
    {
        //Find the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called in order to get an Enemy ready for battle
    /// </summary>
    /// <param name="type">What type of enemy will this be (Stored in EnemyType enum)</param>
    /// <param name="winSize">Size of the window to determine spawn locations</param>
    /// <param name="wave">What wave is the game currently on?</param>
    public void setUpEnemy(EnemyType type, int wave, Vector2 winSize)
    {
        halfWindowSize = winSize;
        dir = Random.Range(0, 4);
        if(dir == (int)SpawnDirection.North)
        {
            transform.position = new Vector2(Random.Range(-halfWindowSize.x - 0.25f, halfWindowSize.x - 0.25f), Random.Range(-7.0f, -6.0f));
        }
        else if (dir == (int)SpawnDirection.South)
        {
            transform.position = new Vector2(Random.Range(-halfWindowSize.x + 0.25f, halfWindowSize.x - 0.25f), Random.Range(6.0f, 7.0f));
        }
        if (dir == (int)SpawnDirection.East)
        {
            transform.position = new Vector2(Random.Range(-halfWindowSize.x - 2, -halfWindowSize.x - 1), Random.Range(-halfWindowSize.y + 0.1f, halfWindowSize.y - 0.1f));
        }
        else if (dir == (int)SpawnDirection.West)
        {
            transform.position = new Vector2(Random.Range(halfWindowSize.x + 1, halfWindowSize.x + 2), Random.Range(-halfWindowSize.y + 0.1f, halfWindowSize.y - 0.1f));
        }

        movementSpeed = Random.Range(1.0f, 4.0f + (wave * 0.5f));
        if (type == EnemyType.Simple)
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(simpleEnemyUpdate());
        }
        else if(type == EnemyType.Sin)
        {
            spriteRenderer.color = Color.green;
            extraInfo.x = Random.Range(2.0f, 4.0f + (wave * 0.25f));
            extraInfo.y = Random.Range(1.0f, 4.0f + (wave * 0.25f));
            timer = 0;
            StartCoroutine(sinEnemyUpdate());
        }
        else if(type == EnemyType.Traveler)
        {
            spriteRenderer.color = Color.yellow;
            extraInfo = new Vector2(Random.Range(-halfWindowSize.x, halfWindowSize.x), Random.Range(-halfWindowSize.y, halfWindowSize.y));
            timer = Random.Range(3.0f, 5.0f + (wave * 0.5f));
            StartCoroutine(travelerEnemyUpdate());
        }
        else if (type == EnemyType.Chaser)
        {
            movementSpeed = Random.Range(0.5f, 2.5f);
            spriteRenderer.color = Color.magenta;
            timer = Random.Range(5.0f, 8.0f + (wave * 0.4f));
            StartCoroutine(chaserEnemyUpdate());
        }
    }

    /// <summary>
    /// The SimpleEnemy just moves stright in one directiont (North, South, East, West)
    /// Once at the other end of the screen, it adds itself to the Enemy pool and deactivates
    /// </summary>
    /// <returns></returns>
    IEnumerator simpleEnemyUpdate()
    {
        while (true) {
            switch (dir)
            {
                case (int)SpawnDirection.North:
                    transform.Translate(0, movementSpeed * Time.deltaTime, 0);
                    if(transform.position.x > 1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
                case (int)SpawnDirection.South:
                    transform.Translate(0, -movementSpeed * Time.deltaTime, 0);
                    if (transform.position.x < -1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
                case (int)SpawnDirection.East:
                    transform.Translate(movementSpeed * Time.deltaTime, 0, 0);
                    if (transform.position.y > 1 + halfWindowSize.y)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
                case (int)SpawnDirection.West:
                    transform.Translate(-movementSpeed * Time.deltaTime, 0, 0);
                    if (transform.position.y < -1 - halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// The SinEnemy moves stright in one directiont (North, South, East, West)
    /// and in the perpendicular directions in a way that forms sin waves
    /// Once at the other end of the screen, it adds itself to the Enemy pool and deactivates
    /// </summary>
    /// <returns></returns>
    IEnumerator sinEnemyUpdate()
    {
        while (true)
        {
            timer += Time.deltaTime;
            switch (dir)
            {
                case (int)SpawnDirection.North:
                    transform.Translate(extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, movementSpeed * Time.deltaTime, 0);
                    if (transform.position.x > 1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
                case (int)SpawnDirection.South:
                    transform.Translate(extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, -movementSpeed * Time.deltaTime, 0);
                    if (transform.position.x < -1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
                case (int)SpawnDirection.East:
                    transform.Translate(movementSpeed * Time.deltaTime, extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, 0);
                    if (transform.position.y > 1 + halfWindowSize.y)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
                case (int)SpawnDirection.West:
                    transform.Translate(-movementSpeed * Time.deltaTime, extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, 0);
                    if (transform.position.y < -1 - halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                    }
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// The TravelerEnemy moves to random locations
    /// This Enemy dies after a set lifetime
    /// </summary>
    /// <returns></returns>
    IEnumerator travelerEnemyUpdate()
    {
        while (true)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                Game_Controller.gameController.addToPool(this);
                gameObject.SetActive(false);
            }
            Vector2 dir = (extraInfo - (Vector2)transform.position);
            if (dir.sqrMagnitude < 1.0f)
            {
                extraInfo = new Vector2(Random.Range(-halfWindowSize.x, halfWindowSize.x), Random.Range(-halfWindowSize.y, halfWindowSize.y));
            }
            dir.Normalize();
            transform.Translate(dir.x * movementSpeed * Time.deltaTime, dir.y * movementSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }

    /// <summary>
    /// The ChaserEnemy towards the player
    /// This Enemy dies after a set lifetime
    /// </summary>
    /// <returns></returns>
    IEnumerator chaserEnemyUpdate()
    {
        while (true)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Game_Controller.gameController.addToPool(this);
                gameObject.SetActive(false);
            }
            //Avoids trying to access a deleted game object
            if (!Game_Controller.lost)
            {
                Vector2 dir = ((Vector2)Game_Controller.gameController.player.position - (Vector2)transform.position).normalized;
                transform.Translate(dir.x * movementSpeed * Time.deltaTime, dir.y * movementSpeed * Time.deltaTime, 0);
            }
            yield return null;
        }
    }
}
