using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TowerPlacer : MonoBehaviour
{
    [Header("Tower Settings")]
    public GameObject ballistaTowerPrefab;
    public GameObject cannonTowerPrefab;
    public GameObject lightningTowerPrefab;
    public float minimumTowerDistance = 0.5f;
    public LayerMask groundLayer;
    public LayerMask towerLayer;
    public LayerMask gridLayer;
    public GridSystem grid;

    [Header("UI")]
    [SerializeField] GameObject ballistaButtonHolder;
    [SerializeField] GameObject cannonButtonHolder;
    [SerializeField] GameObject lightningButtonHolder;
    private Button ballistaButton;
    private Button cannonButton;
    private Button lightningButton;
    private TMP_Text ballistaCostText;
    private TMP_Text cannonCostText;
    private TMP_Text lightningCostText;
    [Header ("References")]
    [SerializeField] private GameObject gridManager;
    [HideInInspector] public GameObject currentTower;
    [SerializeField] private AudioClip placeTowerSound; // Assign sound in Unity Inspector
    private AudioSource audioSource;

    private SkinnedMeshRenderer towerRenderer;
    private Material previewMaterial;
    private Tower towerScript;
    private bool canPlace = true;
    private GameObject selectedTowerPrefab;
    private TowerPlacementAnimation towerAnim;
    [SerializeField] private Transform towerContainer;
    private GoldManager goldManager;
    [SerializeField] private TowerMenu towerMenu;

    private void Start()
    {
        InitializeButtons ();
 
        gridManager.SetActive (false);
        goldManager = GameManager.instance.GoldManager;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        InitializeButtonCostText();
    }

    private void InitializeButtons ()
    {
        ballistaButton = ballistaButtonHolder.GetComponent <Button> ();
        cannonButton = cannonButtonHolder.GetComponent <Button> ();
        lightningButton = lightningButtonHolder.GetComponent <Button> ();

        AssignButtonListener(ballistaButton, "Ballista");
        AssignButtonListener(cannonButton, "Cannon");
        AssignButtonListener(lightningButton, "Lightning");
    }

    private void AssignButtonListener(Button button, string towerType)
    {
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                switch (towerType)
                {
                    case "Ballista":
                        StartTowerPlacement(ballistaTowerPrefab);
                        break;
                    case "Cannon":
                        StartTowerPlacement(cannonTowerPrefab);
                        break;
                    case "Lightning":
                        StartTowerPlacement(lightningTowerPrefab);
                        break;
                    default:
                        Debug.LogWarning($"Unknown tower type: {towerType}");
                        break;
                }
            });
        }
    }

    private void InitializeButtonCostText ()
    {
        ballistaCostText = ballistaButtonHolder.GetComponentInChildren<TMP_Text> ();
        cannonCostText = cannonButtonHolder.GetComponentInChildren<TMP_Text> ();
        lightningCostText = lightningButtonHolder.GetComponentInChildren<TMP_Text> ();


        ballistaCostText.text = ballistaTowerPrefab.GetComponent<Tower>().cost.ToString();
        cannonCostText.text = cannonTowerPrefab.GetComponent<Tower>().cost.ToString();
        lightningCostText.text = lightningTowerPrefab.GetComponent<Tower>().cost.ToString();
    }

   private bool IsValidPlacement(Vector3 position)
    {
        // Define the box size based on the grid cell size
        Vector3 boxSize = new Vector3(grid.cellSize.x, 1f, grid.cellSize.z) / 2;

        // Perform a boxcast to check for collisions
        Collider[] hitColliders = Physics.OverlapBox(position, boxSize, Quaternion.identity, towerLayer | groundLayer | gridLayer);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == currentTower || gameObject.GetComponent<LineRenderer>() != null) continue;
            if (collider.gameObject.CompareTag("Path") || collider.gameObject.layer == 7)
            {
                return false; // Invalid placement
            }
        }

        return true; // Valid placement
    }

    private void UpdateTowerPosition()
    {
        // Snap the position to grid
        Vector3 snappedPosition = grid.cellIndicator.transform.position + new Vector3(grid.cellSize.x / 2, 0, grid.cellSize.z / 2);

        // Set the tower's position
        currentTower.transform.position = snappedPosition;

        // Check if placement is valid
        canPlace = IsValidPlacement(snappedPosition);

        // Update preview material color
        if (previewMaterial != null)
        {
            previewMaterial.color = canPlace ? Color.white : Color.red;
            grid.UpdateCellIndicatorColor(canPlace);
        }
    }

    public void StartTowerPlacement(GameObject towerPrefab)
    {
        gridManager.SetActive(true);
        selectedTowerPrefab = towerPrefab;


        if (currentTower == null)
        {
            CreateNewTower();
        }
        else
        {
            CancelPlacement();
            CreateNewTower();
        }
    }

    private void CreateNewTower()
    {
        currentTower = Instantiate(selectedTowerPrefab, towerContainer);

        EventManager.OnTowerBuy (currentTower.GetComponent<Tower>());

        towerRenderer = currentTower.GetComponentInChildren<SkinnedMeshRenderer>();
        towerAnim = currentTower.GetComponent<TowerPlacementAnimation>();

        if (towerRenderer != null)
        {
            previewMaterial = new Material(towerRenderer.sharedMaterial);
            towerRenderer.material = previewMaterial;
            previewMaterial.color = Color.white;
        }

        towerScript = currentTower.GetComponent<Tower>();

        if (towerScript != null)
        {
            towerScript.enabled = false;
        }
    }


    private void Update()
    {
        UpdateButtonUI ();

        if (currentTower == null) return;

        UpdateTowerPosition();

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceTower();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void UpdateButtonUI()
    {
        UpdateButtonState(ballistaButton, ballistaTowerPrefab);
        UpdateButtonState(cannonButton, cannonTowerPrefab);
        UpdateButtonState(lightningButton, lightningTowerPrefab);
    }

    private void UpdateButtonState(Button button, GameObject towerPrefab)
    {
        Tower tower = towerPrefab.GetComponent<Tower>();
        RawImage buttonImage = button.GetComponentInChildren<RawImage>();
        TMP_Text costText = button.gameObject.GetComponentInChildren<TMP_Text>();

        if (goldManager.gold >= tower.cost) // Check gold condition directly
        {
            button.interactable = true;
            button.enabled = true;
            buttonImage.color = Color.white;
            costText.color = Color.white;
        }
        else 
        {
            button.interactable = false;
            button.enabled = false;
            buttonImage.color = new Color32(93, 93, 93, 255);
            costText.color = new Color32(93, 93, 93, 255);
        }
    }


    private void PlaceTower()
    {
        if (canPlace)
        {
            if (towerScript != null)
            {
                towerScript.enabled = true;
                ApplyInitialStats(towerScript);
            }

            if (towerRenderer != null)
            {
                towerRenderer.material = new Material(selectedTowerPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial);
            }

            if (towerAnim != null)
            {
                towerAnim.SetOriginalPosition(currentTower.transform.position);
                towerAnim.StartAnim();
            }

            if (placeTowerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(placeTowerSound);
            }

            currentTower = null;
            towerRenderer = null;
            previewMaterial = null;
            towerScript = null;
            gridManager.SetActive(false);
        }
    }

    private void CancelPlacement()
    {
        if (currentTower != null)
        {
            EventManager.OnTowerReturn (currentTower.GetComponent<Tower>());
            Destroy(currentTower);
            currentTower = null;
            towerRenderer = null;
            previewMaterial = null;
            towerScript = null;
            gridManager.SetActive(false);
        }
    }

    private void ApplyInitialStats(Tower tower)
    {
        // Get reference to TowerMenu to access the upgrade data
        if (towerMenu == null)
        {
            Debug.LogError("TowerMenu reference is missing!");
            return;
        }

        List<TowerUpgradeData> upgradeList = null;

        // Determine which upgrade list to use based on the tower's tag
        if (tower.CompareTag("Balista Tower"))
        {
            upgradeList = towerMenu.balistaTowerUpgrades;
        }
        else if (tower.CompareTag("Cannon Tower"))
        {
            upgradeList = towerMenu.cannonTowerUpgrades;
        }
        else if (tower.CompareTag("Mage Tower"))
        {
            upgradeList = towerMenu.mageTowerUpgrades;
        }

        // Apply stats from the first element in the upgrade list
        if (upgradeList != null && upgradeList.Count > 0)
        {
            TowerUpgradeData.TowerStats initialStats = upgradeList[0].stats;
            
            // Apply the stats to the tower
            tower.towerDamage = initialStats.damage;
            tower.fireRate = initialStats.fireRate;
            tower.detectionRange = initialStats.range;

            // Apply type-specific stats
            if (tower is CannonTower cannonTower)
            {
                cannonTower.explosionDamage = initialStats.explosionDamage;
            }
            else if (tower is MageTower mageTower)
            {
                mageTower.slowFactor = initialStats.slowFactor;
            }
            // Add any additional stat type applications as needed
        }
    }

}
