using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class InputManager : MonoBehaviour
{
    [SerializeField] Camera sceneCamera;
    Vector3 lastPosition;

    [SerializeField] LayerMask placementLayerMask;
    public event Action OnClicked, OnExit;
    private void Update() {
        if(Input.GetMouseButtonDown(0)){
            OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            OnExit?.Invoke();
        }
    }
    public bool isPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();
    
    public Vector3 GetSelectedMapPosition(){
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask)){
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
