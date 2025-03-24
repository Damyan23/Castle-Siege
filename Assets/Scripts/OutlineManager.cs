using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineManager : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask selectableLayer; // Layer 7 in Unity
    [SerializeField] private Color outlineHoverColor = Color.green;
    [SerializeField] private Color outlineSelectedColor = Color.blue;
    [SerializeField] private GameObject towerMenu;
    [SerializeField] private GameObject placerMenu;
    private TowerPlacer towerPlacer;

    private Outline currentHoveredOutline; // The currently hovered object
    [HideInInspector] public Outline selectedOutline; // The currently selected object

    private void Start()
    {
        towerPlacer = GameManager.instance.TowerPlacer;
        UpdateMenuState(); // Ensure the correct menu is shown at start
    }

    private void Update()
    {
        if (towerPlacer.currentTower != null) return;
        
        HandleHover();
        HandleSelection();
    }

    private void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayer))
        {
            Tower tower = hit.collider.GetComponent<Tower>();
            if (tower == null || !tower.isActiveAndEnabled) return;

            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null && outline != currentHoveredOutline)
            {
                // Reset the previously hovered outline if it's not the selected one
                if (currentHoveredOutline != null && currentHoveredOutline != selectedOutline)
                {
                    currentHoveredOutline.OutlineColor = Color.clear;
                }

                currentHoveredOutline = outline;
                if (currentHoveredOutline != selectedOutline)
                {
                    currentHoveredOutline.OutlineColor = outlineHoverColor;
                }
            }
        }
        else if (currentHoveredOutline != null && currentHoveredOutline != selectedOutline)
        {
            // Clear the hover effect when the mouse is not over a selectable object
            currentHoveredOutline.OutlineColor = Color.clear;
            currentHoveredOutline = null;
        }
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            // **Prevent deselecting if clicking on UI elements**
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Stop execution if clicking on UI
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayer))
            {
                Tower tower = hit.collider.GetComponent<Tower>();
                Outline outline = hit.collider.GetComponent<Outline>();

                if (tower != null && outline != null) // Clicked on a tower
                {
                    // If clicking a different tower, deselect the previous one
                    if (selectedOutline != null && selectedOutline != outline)
                    {
                        DeselectCurrent();
                    }

                    // Select the new tower
                    selectedOutline = outline;
                    selectedOutline.OutlineColor = outlineSelectedColor;
                    EventManager.OnTowerClicked (tower);
                    tower.isClicked = true;
                }
                else
                {
                    // Clicked on something else -> deselect the current tower
                    DeselectCurrent();
                }
            }
            else
            {
                // Clicked on empty space -> deselect
                DeselectCurrent();
            }

            // **Update the menu state after selection/deselection**
            UpdateMenuState();
        }
    }

    private void DeselectCurrent()
    {
        if (selectedOutline != null)
        {
            Tower selectedTower = selectedOutline.GetComponent<Tower>();
            if (selectedTower != null)
            {
                selectedTower.isClicked = false;
            }

            selectedOutline.OutlineColor = Color.clear; // Remove selection outline
            selectedOutline = null;
        }
    }

    public void UpdateMenuState()
    {
        // If a tower is selected, show tower menu; otherwise, show placer menu
        if (selectedOutline != null)
        {
            towerMenu?.SetActive(true);
            placerMenu?.SetActive(false);
        }
        else
        {
            towerMenu?.SetActive(false);
            placerMenu?.SetActive(true);
        }
    }
}
