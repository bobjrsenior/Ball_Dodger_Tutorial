using UnityEngine;
using System.Collections;

public class Player_Controls : MonoBehaviour {

    /// <summary>
    /// Number of time player can hit an enemy without dying
    /// </summary>
    private int lives = 5;
    
    /// <summary>
    /// How fast the player can move in units/second
    /// </summary>
    private float movementSpeed;

    /// <summary>
    /// Holds half of the Windows size in Unity Units for bounds checking
    /// </summary>
    private Vector2 halfWindowSize;

    // Use this for initialization
    void Start () {
        movementSpeed = 5.0f;
        //Slightly less than camera viewport (5)
        halfWindowSize.y = 4.85f;
        //y * aspect ratio
        halfWindowSize.x = halfWindowSize.y * Screen.width / Screen.height;

        //Mke sure the lives UI is displaying correctly
        Game_Controller.gameController.updateLives(lives);
    }
	
	// Update is called once per frame
	void Update () {
        if (!Game_Controller.lost)
        {
            //Move depending on input
            transform.Translate(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime, 0);

            //Keep the player withing the window
            if (Mathf.Abs(transform.position.x) > halfWindowSize.x)
            {
                transform.position = new Vector2(Mathf.Sign(transform.position.x) * halfWindowSize.x, transform.position.y);
            }
            if (Mathf.Abs(transform.position.y) > halfWindowSize.y)
            {
                transform.position = new Vector2(transform.position.x, Mathf.Sign(transform.position.y) * halfWindowSize.y);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Currently, the only thing the player can hit is the enemy
        //Otherwise we woiuld need to add checks such as giving the enemy a certain tag in the editor
        Game_Controller.gameController.updateLives(--lives);
        Destroy(other.gameObject);
        if(lives <= 0)
        {
            Game_Controller.gameController.lostGame();
            Destroy(gameObject);
        }
    }
}
