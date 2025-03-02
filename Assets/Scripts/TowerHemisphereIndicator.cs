using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerHemisphereIndicator : MonoBehaviour
{
    [Header("Hemisphere Settings")]
    [Tooltip("The range (radius) of the hemisphere (and tower).")]
    private float range = 5f;
    [Tooltip("Number of segments per circle (higher means smoother circles).")]
    [SerializeField] private int circleSegments = 40;
    [Tooltip("Number of horizontal rings to draw (from base to top).")]
    [SerializeField] private int ringCount = 4;
    [Tooltip("Width of the drawn lines.")]
    [SerializeField] float lineWidth = 0.1f;
    [Tooltip("Material for the line renderers.")]
    // A container GameObject for all the generated lines.
    private GameObject hemisphereHolder;

    [SerializeField] private Tower tower;

    private void Awake()
    {
        // Create a holder for the hemisphere lines as a child of this object.
        hemisphereHolder = new GameObject("HemisphereHolder");
        hemisphereHolder.transform.SetParent(transform, false);
        hemisphereHolder.SetActive(false);
        tower = GetComponent <Tower>();
        range = tower.detectionRange;
    }

    void Update ()
    {
        if (tower.isClicked) ShowIndicator ();
        else HideIndicator ();
    }

    /// <summary>
    /// Call this to show the hemisphere indicator.
    /// </summary>
    public void ShowIndicator()
    {
        if (hemisphereHolder.activeSelf == false)
        {
            DrawHemisphere();
            hemisphereHolder.SetActive(true);
        }
    }

    /// <summary>
    /// Call this to hide the hemisphere indicator.
    /// </summary>
    public void HideIndicator()
    {
        if (hemisphereHolder.activeSelf == true)
        hemisphereHolder.SetActive(false);

        foreach (Transform child in hemisphereHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Draws the hemisphere using horizontal rings and vertical arcs.
    /// </summary>
    void DrawHemisphere()
    {
        // Clean up any existing lines.
        foreach (Transform child in hemisphereHolder.transform)
        {
            Destroy(child.gameObject);
        }

        // Draw horizontal rings from y = 0 (base) to y = range (top).
        for (int i = 0; i <= ringCount; i++)
        {
            // Calculate the current height.
            float y = (range / ringCount) * i;
            // The circle’s radius at height y is determined by the sphere’s equation.
            float ringRadius = Mathf.Sqrt(range * range - y * y);
            DrawCircle(new Vector3(0, y, 0), ringRadius);
        }

        // Draw vertical arcs along four cardinal directions.
        DrawVerticalArc(0);
        DrawVerticalArc(90);
        DrawVerticalArc(180);
        DrawVerticalArc(270);
    }

    /// <summary>
    /// Draws a horizontal circle (ring) centered at 'center' with the specified 'radius'.
    /// </summary>
    /// <param name="center">Local center position of the circle.</param>
    /// <param name="radius">Radius of the circle.</param>
    void DrawCircle(Vector3 center, float radius)
    {
        GameObject circleObj = new GameObject("Circle");
        circleObj.transform.SetParent(hemisphereHolder.transform, false);
        LineRenderer lr = circleObj.AddComponent<LineRenderer>();
        lr.transform.gameObject.layer = 5; // Change the layer of the gameobject that holds the lr because its on the same layer as the tower and the tower placement system detects a tower
        lr.widthMultiplier = lineWidth;
        lr.positionCount = circleSegments + 1;
        lr.loop = true;
        lr.useWorldSpace = false;
        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / circleSegments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, center + new Vector3(x, 0, z));
        }
    }

    /// <summary>
    /// Draws a vertical arc in a vertical plane defined by the provided angle (in degrees) around the Y axis.
    /// </summary>
    /// <param name="angleDegrees">Angle around Y to fix the plane (0° is along +X, 90° along +Z, etc.).</param>
    void DrawVerticalArc(float angleDegrees)
    {
        GameObject arcObj = new GameObject("VerticalArc");
        arcObj.transform.SetParent(hemisphereHolder.transform, false);
        LineRenderer lr = arcObj.AddComponent<LineRenderer>();
        lr.transform.gameObject.layer = 5; // Change the layer of the gameobject that holds the lr because its on the same layer as the tower and the tower placement system detects a tower
        lr.widthMultiplier = lineWidth;
        int arcSegments = circleSegments / 2; // We need only half a circle.
        lr.positionCount = arcSegments + 1;
        lr.useWorldSpace = false;
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        // Determine the fixed X and Z directions based on the given angle.
        float dirX = Mathf.Cos(angleRad);
        float dirZ = Mathf.Sin(angleRad);
        // We will vary the polar angle from 0 (at base) to π/2 (at the top).
        for (int i = 0; i <= arcSegments; i++)
        {
            float t = (float)i / arcSegments;
            float polarAngle = t * (Mathf.PI / 2f);
            // In a sphere, horizontal distance is range * cos(polarAngle) and vertical height is range * sin(polarAngle).
            float currentY = range * Mathf.Sin(polarAngle);
            float horizontalRadius = range * Mathf.Cos(polarAngle);
            float x = horizontalRadius * dirX;
            float z = horizontalRadius * dirZ;
            lr.SetPosition(i, new Vector3(x, currentY, z));
        }
    }
}
