using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabase database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PreviewSystem preview;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private SoundManager soundManager;
    IBuildingState buildingState;
    private GridData floorData, furnitureData;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private void Start() {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }
    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           objectPlacer,
                                           soundManager);

        inputManager.OnClicked += PlaceStracture;
        inputManager.OnExit += StopPlacement;
    }
    public void StartRemoving(){
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid,preview,floorData,furnitureData,objectPlacer,soundManager);
        inputManager.OnClicked += PlaceStracture;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStracture()
    {
        if(inputManager.isPointerOverUI()){ 
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        buildingState.OnAction(gridPosition);
    }

    /*private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(
            gridPosition, 
            database.objectsData[selectedObjectIndex].Size
        );
    }*/

    private void StopPlacement()
    {
        if(buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStracture;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update() {
        if(buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition){
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }    
}
