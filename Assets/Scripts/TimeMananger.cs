using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMananger : MonoBehaviour
{
    [Tooltip ("The default time scale is 1, decreasing it slows down the game, increasing it speeds it up")]
    [SerializeField, Range (0, 3)] private float timeScale = 1f;

    // Update is called once per frame
    void Update()
    {
        ChangeTimeScale ();
    }

    void ChangeTimeScale ()
    {
        if (Time.timeScale != timeScale && !PauseMenuController.isPaused) Time.timeScale = timeScale;
    }
}
