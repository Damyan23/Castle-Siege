using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused;
    [SerializeField] private GameObject buttonsContainer;
    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape) && !GameOverMenuController.isGameOver)
        {
            if (!isPaused) Pause ();
            else Unpause ();
        }
    }

    void Pause ()
    {
        Time.timeScale = 0;
        isPaused = true;
        buttonsContainer.SetActive (true);
    }

    public void Unpause ()
    {
        Time.timeScale = 1;
        isPaused = false;
        buttonsContainer.SetActive (false);
    }

    public void Restart ()
    {
        Unpause ();
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void MainMenu ()
    {
        Unpause ();
        SceneManager.LoadScene (0);
    }

    public void Quit ()
    {
        Application.Quit ();
    }
}
