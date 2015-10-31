using UnityEngine;
using System.Collections;

public class Player_Controls : MonoBehaviour {

    /// <summary>
    /// Number of time player can hit an enemy without dying
    /// </summary>
    public int lives = 3;
    
    /// <summary>
    /// How fast the player can move in units/second
    /// </summary>
    private float movementSpeed;

    /// <summary>
    /// Holds half of the Windows size in Unity Units for bounds checking
    /// </summary>
    public Vector2 halfWindowSize;

    // Use this for initialization
    void Start () {
        movementSpeed = 5.0f;
        halfWindowSize.x = halfWindowSize.y * Screen.width / Screen.height;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime, Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime, 0);
        if(Mathf.Abs(transform.position.x) > halfWindowSize.x)
        {
            transform.position = new Vector2(Mathf.Sign(transform.position.x) * halfWindowSize.x, transform.position.y);
        }
        if (Mathf.Abs(transform.position.y) > halfWindowSize.y)
        {
            transform.position = new Vector2(transform.position.x, Mathf.Sign(transform.position.y) * halfWindowSize.y);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        lives--;
        Destroy(other.gameObject);
    }
}
