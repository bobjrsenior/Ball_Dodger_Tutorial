using UnityEngine;
using System.Collections;

public class Main_Menu_Controller : MonoBehaviour {

    public void play()
    {
        Application.LoadLevel(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
