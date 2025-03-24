using TMPro;
using UnityEngine;

public class TowerStatsUI : MonoBehaviour
{
    [Header("UI References")]
    private TMP_Text damageText;
    private TMP_Text attackSpeedText;
    private TMP_Text attackRangeText;
    private TMP_Text extraStatText; // For Slow Factor or Explosion Damage
    private Tower currentTower;
    private Canvas canvas;

    private void OnEnable()
    {
        EventManager.OnTowerClickedEvent += OnTowerClicked;
    }

    private void Update ()
    {
        if (currentTower != null && !currentTower.isClicked)
        {
            HideStats ();
            currentTower = null;
        }
    }

    private void OnTowerClicked(Tower tower)
    {
        currentTower = tower;
        canvas = currentTower.GetComponentInChildren<Canvas> (true);
        canvas.gameObject.SetActive (true);
        SetUpReferences (canvas);
        DisplayTowerStats (tower);
    }

    private void LateUpdate()
    {
        if (canvas != null && currentTower != null && Camera.main != null)
        {
            // Calculate the horizontal direction from the tower to the camera.
            Vector3 towerToCamera = Camera.main.transform.position - currentTower.transform.position;
            towerToCamera.y = 0f; // Keep it horizontal.
            towerToCamera.Normalize();

            // Determine the horizontal offset direction.
            Vector3 horizontalDir = Vector3.Cross(Vector3.up, towerToCamera).normalized;

            // Define your offsets.
            float distanceOffset = 2.0f; // Horizontal offset amount.
            Vector3 horizontalOffset = horizontalDir * distanceOffset;

            Vector3 verticalOffset = new Vector3(0f, 2.5f, 0f); // Adjust vertical offset if needed.
            // Place the canvas relative to the tower using the tower-to-camera direction, plus horizontal and vertical offsets.
            Vector3 desiredPosition = currentTower.transform.position - towerToCamera + verticalOffset + horizontalOffset;
            canvas.transform.position = desiredPosition;

            // Rotate the canvas to face the camera.
            Quaternion lookRotation = Quaternion.LookRotation(Camera.main.transform.position - canvas.transform.position);
            // Apply an additional 180° rotation around Y to flip the text correctly.
            canvas.transform.rotation = lookRotation * Quaternion.Euler(0, 180f, 0);
        }
    }


    private void SetUpReferences(Canvas canvas)
    {
        if (canvas.transform.childCount < 4)
        {
            Debug.LogError("Canvas does not have enough children for text references.");
            return;
        }
        
        damageText = canvas.transform.GetChild(0).GetComponent<TMP_Text>();
        if (damageText == null) Debug.LogError("damageText is null!");

        attackSpeedText = canvas.transform.GetChild(1).GetComponent<TMP_Text>();
        if (attackSpeedText == null) Debug.LogError("attackSpeedText is null!");

        attackRangeText = canvas.transform.GetChild(2).GetComponent<TMP_Text>();
        if (attackRangeText == null) Debug.LogError("attackRangeText is null!");

        extraStatText = canvas.transform.GetChild(3).GetComponent<TMP_Text>();
        if (extraStatText == null) Debug.LogError("extraStatText is null!");
    }


    public void DisplayTowerStats(Tower tower)
    {
        if (tower == null) return;

        damageText.text = $"Damage: {tower.towerDamage}";
        attackSpeedText.text = $"Attack Speed: {tower.fireRate}/s";
        attackRangeText.text = $"Range: {tower.detectionRange}";

        // Check for additional stats (slowFactor for MageTower, explosionDamage for CannonTower)
        if (tower is MageTower mageTower)
        {
            extraStatText.gameObject.SetActive(true);
            extraStatText.text = $"Slow Factor: {mageTower.slowFactor}";
        }
        else if (tower is CannonTower cannonTower)
        {
            extraStatText.gameObject.SetActive(true);
            extraStatText.text = $"Explosion Damage: {cannonTower.explosionDamage}";
        }
        else
        {
            extraStatText.gameObject.SetActive(false); // Hide extra stats if not applicable
        }
    }

    public void HideStats()
    {
        damageText.text = "";
        attackSpeedText.text = "";
        attackRangeText.text = "";


        if (extraStatText != null) // If the tower has extra stats, hide them.
        {
            extraStatText.text = "";
            extraStatText.gameObject.SetActive(false);
        }

        canvas.gameObject.SetActive (false); 
    }
}
