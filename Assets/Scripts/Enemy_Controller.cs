using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour {

    /// <summary>
    /// A link to the next enemy
    /// Used for the Enemy Spawn Pool (null if not in pool/is active)
    /// </summary>
    public Enemy_Controller next = null;

    /// <summary>
    /// Holds the different types of enemies
    /// </summary>
    public enum EnemyType { Simple};

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
    /// Holds half of the Windows size in Unity Units for bounds checking
    /// </summary>
    public Vector2 halfWindowSize;

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

        movementSpeed = Random.Range(1.0f, 4.0f + (wave * .5f));
        if (type == EnemyType.Simple)
        {
            StartCoroutine(simpleEnemyUpdate());
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
}
