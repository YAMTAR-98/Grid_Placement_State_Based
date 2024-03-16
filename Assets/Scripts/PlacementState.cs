using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    SoundManager soundManager;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabase database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          SoundManager soundManager)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundManager = soundManager;

        selectedObjectIndex = database.objectsData.FindIndex(database => database.ID == ID);
        if(selectedObjectIndex > -1){
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }else 
            throw new SystemException($"No Object With ID {ID}");
        
    }
    public void EndState(){
        previewSystem.StopShowingPreview();
    }
    public void OnAction(Vector3Int gridPosition){
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        
        if(!placementValidity){
            soundManager.PlaySound(SoundManager.SoundType.Wrong);
            return;
        }
            
        soundManager.PlaySound(SoundManager.SoundType.Place);
        int index = objectPlacer.AddObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition, 
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index
        );
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }
    public void UpdateState(Vector3Int gridPosition){
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(
            gridPosition, 
            database.objectsData[selectedObjectIndex].Size
        );
    }
}
