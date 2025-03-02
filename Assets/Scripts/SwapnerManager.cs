using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform enemyParent;
    [SerializeField] private TMP_Text waveNumber;
    [SerializeField] private TMP_Text waveTextInfo;

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField, Tooltip("Time before the first wave starts")] private float firstWaveStartDelay = 5f;
    [SerializeField, Tooltip("Time between waves")] private float waveInterval = 10f;
    private Transform spawnPoint;

    private int currentWaveIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void OnEnable()
    {
        EventManager.EnemyDeathEvent += (enemy) => RemoveEnemy(enemy.gameObject);        
    }

    void OnDisable()
    {
        EventManager.EnemyDeathEvent -= (enemy) => RemoveEnemy(enemy.gameObject);
    }

    void Start()
    {
        spawnPoint = this.transform;
        if (waveTextInfo != null)
            waveTextInfo.text = "Press F to start the first wave";
    }

    void Update()
    {
        if (currentWaveIndex == 0 && Input.GetKeyDown(KeyCode.F) && !PauseMenuController.isPaused)
        {
            StartCoroutine(StartWave(waves[currentWaveIndex]));
        }

        // Debug command to kill all enemies
        if (Input.GetKeyDown(KeyCode.K))
        {
            KillAllEnemies();
        }
    }

    private IEnumerator StartWave(Wave wave)
    {
        if (waveTextInfo != null)
            waveTextInfo.text = "Wave " + (currentWaveIndex + 1) + " starting...";

        float remainingTime = (currentWaveIndex == 0) ? firstWaveStartDelay : waveInterval;

        while (remainingTime > 0)
        {
            if (waveTextInfo != null)
                waveTextInfo.text = "Wave " + (currentWaveIndex + 1) + " starting in: " + Mathf.Ceil(remainingTime) + " seconds";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        Debug.Log($"Wave {currentWaveIndex + 1} started!");

        int remainingEnemies = GetTotalEnemies(wave);

        UpdateWaveText(remainingEnemies);  

        while (remainingEnemies > 0)
        {
            int spawnCount = Mathf.Min(wave.groupSize, remainingEnemies);

            for (int i = 0; i < spawnCount; i++)
            {
                EnemyCount randomEnemy = wave.enemies[Random.Range(0, wave.enemies.Count)];

                if (randomEnemy.count > 0)
                {
                    GameObject spawnedEnemy = Instantiate(randomEnemy.enemy, spawnPoint.position, Quaternion.identity, enemyParent);
                    activeEnemies.Add(spawnedEnemy);
                    
                    randomEnemy.count--;
                    remainingEnemies--;
                }                                    
            }
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        yield return new WaitUntil(() => activeEnemies.Count == 0);

        Debug.Log($"Wave {currentWaveIndex + 1} complete!");
        currentWaveIndex++;

        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(StartWave(waves[currentWaveIndex]));
        }
        else
        {
            if (waveTextInfo != null)
                waveTextInfo.text = "All waves completed! You win!";
            
            EventManager.OnGameWon(); // Trigger game won event
        }
    }


    private int GetTotalEnemies(Wave wave)
    {
        int total = 0;
        foreach (EnemyCount enemyCount in wave.enemies)
        {
            total += enemyCount.count;
        }
        return total;
    }

    private void UpdateWaveText(int currentActiveEnemies)
    {
        if (waveTextInfo != null)
            waveTextInfo.text = "Wave " + (currentWaveIndex + 1) + " - Remaining Enemies: " + currentActiveEnemies;
    }

    private void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        UpdateWaveText(activeEnemies.Count);
        Debug.Log ("asd");
    }

    private void KillAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        activeEnemies.Clear();
        Debug.Log("All enemies have been killed.");
    }
}

[System.Serializable]
public class EnemyCount
{
    public GameObject enemy;
    public int count;
}

[System.Serializable]
public class Wave
{
    public List<EnemyCount> enemies;
    public float spawnInterval = 1f;
    public int groupSize = 3;
}
