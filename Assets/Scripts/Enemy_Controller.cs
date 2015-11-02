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
        //Spawn Direction is which way the enemy will move on spawn if simple
        //If Spawn Direction is North, spawn below the map with a random x location
        if(dir == (int)SpawnDirection.North)
        {
            transform.position = new Vector2(Random.Range(-halfWindowSize.x - 0.25f, halfWindowSize.x - 0.25f), Random.Range(-7.0f, -6.0f));
        }//If Spawn Direction is South, spawn above the map with a random x location
        else if (dir == (int)SpawnDirection.South)
        {
            transform.position = new Vector2(Random.Range(-halfWindowSize.x + 0.25f, halfWindowSize.x - 0.25f), Random.Range(6.0f, 7.0f));
        }//If Spawn Direction is East, spawn left of the map with a random y location
        if (dir == (int)SpawnDirection.East)
        {
            transform.position = new Vector2(Random.Range(-halfWindowSize.x - 2, -halfWindowSize.x - 1), Random.Range(-halfWindowSize.y + 0.1f, halfWindowSize.y - 0.1f));
        }//If Spawn Direction is West, spawn right of the map with a random y location
        else if (dir == (int)SpawnDirection.West)
        {
            transform.position = new Vector2(Random.Range(halfWindowSize.x + 1, halfWindowSize.x + 2), Random.Range(-halfWindowSize.y + 0.1f, halfWindowSize.y - 0.1f));
        }
        //Set a random movement speed based on the current wave
        movementSpeed = Random.Range(1.0f, 4.0f + (wave * 0.5f));

        //Set enemy type specific variables and begin coroutines
        if (type == EnemyType.Simple)
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(simpleEnemyUpdate());
        }
        else if(type == EnemyType.Sin)
        {
            spriteRenderer.color = Color.green;
            //extrainfo for Sin enemy = amplitude and frequency of sinusoidal movement
            extraInfo.x = Random.Range(2.0f, 4.0f + (wave * 0.25f));
            extraInfo.y = Random.Range(1.0f, 4.0f + (wave * 0.25f));
            //timer for Sin enemy = position in sine wave
            timer = 0;
            StartCoroutine(sinEnemyUpdate());
        }
        else if(type == EnemyType.Traveler)
        {
            spriteRenderer.color = Color.yellow;
            //extraInfo for Traveler enemy = a random location on the map to move to
            extraInfo = new Vector2(Random.Range(-halfWindowSize.x, halfWindowSize.x), Random.Range(-halfWindowSize.y, halfWindowSize.y));
            //timer for Traveler enemy = how long the enemy will stay alive
            timer = Random.Range(3.0f, 5.0f + (wave * 0.5f));
            StartCoroutine(travelerEnemyUpdate());
        }
        else if (type == EnemyType.Chaser)
        {
            movementSpeed = Random.Range(0.5f, 2.5f);
            spriteRenderer.color = Color.magenta;
            //timer for Traveler enemy = how long the enemy will stay alive
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
            //Move accross the map at a constant speed of movementSpeed Units/second
            //When enemy reaches the other side, add itself to the enemy pool, set enactive, and end the coroutine
            switch (dir)
            {
                case (int)SpawnDirection.North:
                    transform.Translate(0, movementSpeed * Time.deltaTime, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.x > 1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
                case (int)SpawnDirection.South:
                    transform.Translate(0, -movementSpeed * Time.deltaTime, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.x < -1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
                case (int)SpawnDirection.East:
                    transform.Translate(movementSpeed * Time.deltaTime, 0, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.y > 1 + halfWindowSize.y)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
                case (int)SpawnDirection.West:
                    transform.Translate(-movementSpeed * Time.deltaTime, 0, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.y < -1 - halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
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
            //Move accross the map at a constant speed of movementSpeed Units/second
            //Also move in the perpendicular direction with amplitude extraInfo.x and frequency extraInfo.y
            //When enemy reaches the other side, add itself to the enemy pool, set enactive, and end the coroutine
            timer += Time.deltaTime;
            switch (dir)
            {
                case (int)SpawnDirection.North:
                    transform.Translate(extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, movementSpeed * Time.deltaTime, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.x > 1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
                case (int)SpawnDirection.South:
                    transform.Translate(extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, -movementSpeed * Time.deltaTime, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.x < -1 + halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
                case (int)SpawnDirection.East:
                    transform.Translate(movementSpeed * Time.deltaTime, extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.y > 1 + halfWindowSize.y)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
                case (int)SpawnDirection.West:
                    transform.Translate(-movementSpeed * Time.deltaTime, extraInfo.x * Mathf.Sin(timer * extraInfo.y) * Time.deltaTime, 0);

                    //Have you hit the other side of the screen?
                    if (transform.position.y < -1 - halfWindowSize.x)
                    {
                        Game_Controller.gameController.addToPool(this);
                        gameObject.SetActive(false);
                        yield break;
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
            //if timer <= 0, the enemy's life is up, so die
            if(timer <= 0)
            {
                Game_Controller.gameController.addToPool(this);
                gameObject.SetActive(false);
                yield break;
            }
            //Find direction to the target
            Vector2 dir = (extraInfo - (Vector2)transform.position);
            //If you are close to the target, find a new target
            if (dir.sqrMagnitude < 1.0f)
            {
                extraInfo = new Vector2(Random.Range(-halfWindowSize.x, halfWindowSize.x), Random.Range(-halfWindowSize.y, halfWindowSize.y));
            }
            //move towards the target at movementSpeed Units/second
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
            //if timer <= 0, the enemy's life is up, so die
            if (timer <= 0)
            {
                Game_Controller.gameController.addToPool(this);
                gameObject.SetActive(false);
                yield break;
            }
            //Avoids trying to access a deleted game object
            if (!Game_Controller.lost)
            {
                //Find direction to the target
                Vector2 dir = ((Vector2)Game_Controller.gameController.player.position - (Vector2)transform.position).normalized;
                //move towards the target at movementSpeed Units/second
                transform.Translate(dir.x * movementSpeed * Time.deltaTime, dir.y * movementSpeed * Time.deltaTime, 0);
            }
            yield return null;
        }
    }
}
