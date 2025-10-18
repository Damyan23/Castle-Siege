using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverParent;
    [SerializeField] private GameObject gameWonParent;
    [HideInInspector] public static bool isGameOver;
    void OnEnable ()
    {
        EventManager.OnCastleDeathEvent += GameOver;
        EventManager.OnGameWonEvent += GameWon;
    }

    void OnDisable ()
    {
        EventManager.OnCastleDeathEvent -= GameOver;
    }
    void GameOver ()
    {
        gameOverParent.SetActive (true);
        Time.timeScale = 0;
        PauseMenuController.isPaused = true;
        isGameOver = true;
    }

    void GameWon ()
    {
        gameWonParent.SetActive (true);
        Time.timeScale = 0;
        PauseMenuController.isPaused = true;
        isGameOver = true;
    }
}
