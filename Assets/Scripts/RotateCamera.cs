using Unity.VisualScripting;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField]private float rotationSpeed = 10f; // Speed of rotation
    [SerializeField] private Transform targetPoint;

    void Update()
    {
        // Rotate around the target point
        transform.RotateAround(targetPoint.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
