using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TowerUpgradeData
{
    public GameObject prefab;
    public int upgradeCost;
    public TowerStats stats;

    [System.Serializable]
    public class TowerStats
    {
        public float damage;
        public float fireRate;
        public float range;
    }
}

public class TowerMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button upgradeButton;
    public Button tankButton;
    public Button fastestButton;
    public Button meleeButton;

    private TMP_Text upgradeCost;

    [Header("Upgrade Settings")]
    public List<TowerUpgradeData> balistaTowerUpgrades;
    public List<TowerUpgradeData> cannonTowerUpgrades;
    public List<TowerUpgradeData> mageTowerUpgrades;

    private TowerUpgradeAnimation upgradeAnim;

    void OnEnable ()
    {
        // Add listeners to buttons
        upgradeButton.onClick.AddListener(UpgradeTower);
        tankButton.onClick.AddListener(() => FocusTargeting("tank"));
        fastestButton.onClick.AddListener(() => FocusTargeting("fastest"));
        meleeButton.onClick.AddListener(() => FocusTargeting("melee"));

        upgradeCost = upgradeButton.gameObject.GetComponentInChildren <TMP_Text> ();
    }

    // private void Start()
    // {
        
    // }

    void Update ()
    {
        if (GetSelectedTower () != null)
        {
            Tower selectedTower = GetSelectedTower ();
            List<TowerUpgradeData> upgradeList = GetUpgradeList (selectedTower);
            TowerUpgradeData currentUpgradeData = GetNextUpgradeData (GetCurrentUpgradeData (selectedTower, upgradeList), upgradeList);
            UpdateButtonState (currentUpgradeData);
            if ((upgradeCost.text != currentUpgradeData.upgradeCost.ToString ()) && currentUpgradeData != null)
            {
                upgradeCost.text = currentUpgradeData.upgradeCost.ToString ();
            }
        }
    }

    public void UpgradeTower()
    {
        Debug.Log (GetSelectedTower());
        Tower selectedTower = GetSelectedTower();
        if (selectedTower == null) return;

        // Get the upgrade list based on the selected tower type
        List<TowerUpgradeData> upgradeList = GetUpgradeList(selectedTower);
        if (upgradeList == null || upgradeList.Count == 0) return;

    
        // Get the current upgrade data
        TowerUpgradeData currentUpgradeData = GetCurrentUpgradeData(selectedTower, upgradeList);
        if (currentUpgradeData == null) return;

        // Get the next upgrade data
        TowerUpgradeData nextUpgradeData = GetNextUpgradeData(currentUpgradeData, upgradeList);
        if (nextUpgradeData == null) return;


        // Check if the player has enough gold for the upgrade
        if (GameManager.instance.GoldManager.gold < nextUpgradeData.upgradeCost)
        {
            Debug.Log("Not enough gold to upgrade tower!");
            return;
        }


        // Deduct upgrade cost
        GameManager.instance.GoldManager.removeGold (nextUpgradeData.upgradeCost);

        // Perform tower upgrade
        PerformTowerUpgrade(selectedTower, nextUpgradeData);
    }

    private List<TowerUpgradeData> GetUpgradeList (Tower selectedTower)
    {
        List<TowerUpgradeData> upgradeList = null;

        // Determine which upgrade list to use based on the selected tower's tag
        if (selectedTower.CompareTag("Balista Tower"))
        {
            upgradeList = balistaTowerUpgrades;
        }
        else if (selectedTower.CompareTag("Cannon Tower"))
        {
            upgradeList = cannonTowerUpgrades;
        }
        else if (selectedTower.CompareTag("Mage Tower"))
        {
            upgradeList = mageTowerUpgrades;
        }

        return upgradeList;
    }

    private TowerUpgradeData GetCurrentUpgradeData(Tower selectedTower, List<TowerUpgradeData> upgradeList)
    {
        foreach (TowerUpgradeData upgradeData in upgradeList)
        {
            if (upgradeData.prefab.name + "(Clone)" == selectedTower.gameObject.name)
            {
                return upgradeData;
            }
        }
        return null; // No matching upgrade found
    }

    private TowerUpgradeData GetNextUpgradeData(TowerUpgradeData currentUpgradeData, List<TowerUpgradeData> upgradeList)
    {
        int currentIndex = upgradeList.IndexOf(currentUpgradeData);
        
        if (currentIndex >= 0 && currentIndex < upgradeList.Count - 1)
        {
            return upgradeList[currentIndex + 1]; // Return the next upgrade
        }
        
        return null; // No further upgrade available
    }

    private void PerformTowerUpgrade(Tower selectedTower, TowerUpgradeData nextUpgradeData)
    {
        Debug.Log ("tower ugprade");
        Transform towerTransform = selectedTower.transform;
        
        // Instantiate upgraded tower
        GameObject upgradedTower = Instantiate(nextUpgradeData.prefab, towerTransform.position, towerTransform.rotation, towerTransform.parent);
        Debug.Log (upgradedTower);
        // Apply new stats
        Tower upgradedTowerScript = upgradedTower.GetComponent<Tower>();
        if (upgradedTowerScript != null)
        {
            ApplyTowerStats(upgradedTowerScript, nextUpgradeData.stats);
        }

        // Handle upgrade animation
        if (upgradedTower.TryGetComponent(out TowerUpgradeAnimation upgradeAnim))
        {
            upgradeAnim.StartAnim();
        }

        // Destroy old tower
        selectedTower.isClicked = false;
        Destroy(selectedTower.gameObject);
    }

    private Tower GetSelectedTower ()
    {
        foreach (Tower tower in FindObjectsByType <Tower> (sortMode: FindObjectsSortMode.None))
        {
            if (tower.isClicked) return tower;
        }

        return null;
    }

    private void ApplyTowerStats(Tower tower, TowerUpgradeData.TowerStats stats)
    {
        // Apply the stats to the tower
        tower.towerDamage = stats.damage;
        tower.fireRate = stats.fireRate;
        tower.detectionRange = stats.range;
        // Add any additional stat applications as needed
    }

    private void FocusTargeting(string mode)
    {
        Tower selectedTower = GetSelectedTower();

        if (selectedTower != null)
        {
            switch (mode.ToLower())
            {
                case "tank":
                    selectedTower.TargetEnemiesByTag("Tank");
                    break;
                case "fastest":
                    selectedTower.TargetFastestEnemies();
                    break;
                case "melee":
                    selectedTower.TargetEnemiesByTag("Melee");
                    break;
            }
        }
    }

    // Method to update the upgrade button's state and appearance based on gold amount
    private void UpdateButtonState(TowerUpgradeData currentUpgradeData)
    {
        if (currentUpgradeData == null) return;

        // Check if the player has enough gold for the upgrade
        if (GameManager.instance.GoldManager.gold >= currentUpgradeData.upgradeCost)
        {
            upgradeButton.interactable = true;
            upgradeButton.enabled = true;
            // Reset color to normal (you can customize this color)
            upgradeButton.GetComponentInChildren<Image>().color = Color.white;
            upgradeCost.color = Color.white;
        }
        else
        {
            upgradeButton.interactable = false;
            upgradeButton.enabled = false;
            // Change color to indicate the button is disabled
            upgradeButton.GetComponentInChildren<Image>().color = new Color32(93, 93, 93, 255);
            upgradeCost.color = new Color32(93, 93, 93, 255);
        }
    }
}