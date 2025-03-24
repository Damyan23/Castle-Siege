using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
   
    private Camera cam;

    private Vector3 lastPosition;

  
    [SerializeField] private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    private void Start ()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePos); 
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayermask))
        {
            lastPosition = hit.point;
        }
        
        return lastPosition;
    }

}