using UnityEngine;
using TMPro;

public class CoinDisplayManager : MonoBehaviour
{
    [Tooltip("Floating text prefab for displaying coins (should contain a TextMeshProUGUI and FloatingText script)")]
    public GameObject floatingTextPrefab;


    private Camera mainCamera;
    [SerializeField] private Transform container;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EventManager.EnemyDeathEvent += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        EventManager.EnemyDeathEvent -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        GameObject canvas = Instantiate (floatingTextPrefab, enemy.transform.position, Quaternion.identity);
        TMP_Text text = canvas.GetComponentInChildren<TMP_Text> ();
        text.fontSize = 5;
        text.text = enemy.carriedMoney.ToString ();
    }
}
