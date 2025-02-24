using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string GetFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"saveSlot{slotIndex}.json");
    }

    public void SaveGame(int slotIndex)
    {
        var unitsController = ControllersManager.Instance.peopleUnitsController;
        var timeController = ControllersManager.Instance.timeController;

        string filePath = GetFilePath(slotIndex);

        GameData gameData = new GameData
        {
            periodOfDay = timeController.CurrentPeriod,
            allUnitsData = new List<PeopleUnitData>(),
            allBuildingsData = new List<BuildingData>()
        };

        gameData.SetDate(timeController.CurrentDate);

        foreach (var unit in unitsController.GetAllUnits())
        {
            PeopleUnitData unitData = new PeopleUnitData
            {
                currentState = unit.GetCurrentState(),
                position = unit.transform.position,
                busyTime = unit.BusyTime,
                restingTime = unit.RestingTime
            };
            gameData.allUnitsData.Add(unitData);
        }

        // Сохраняем данные зданий
        var buildingController = ControllersManager.Instance.buildingController;
        // Перед сохранением данных зданий
        foreach (var building in buildingController.RegularBuildings.Concat(buildingController.SpecialBuildings))
        {
            Debug.Log($"Saving building: {building.name}, ID: {building.BuildingId}, State: {building.CurrentState}");
            BuildingData buildingData = new()
            {
                BuildingId = building.BuildingId,
                currentState = building.CurrentState,
                turnsToRepair = building.TurnsToRepair,
            };
            gameData.allBuildingsData.Add(buildingData);
        }

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Game saved in slot {slotIndex}.");
    }

    public void LoadGame(int slotIndex)
    {
        string filePath = GetFilePath(slotIndex);
        var unitsController = ControllersManager.Instance.peopleUnitsController;
        var timeController = ControllersManager.Instance.timeController;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            Debug.Log(gameData.periodOfDay);
            Debug.Log(gameData.GetDate());

            timeController.SetDateAndPeriod(gameData.GetDate(), gameData.periodOfDay);

            List<PeopleUnit> units = unitsController.GetAllUnits();

            for (int i = 0; i < gameData.allUnitsData.Count && i < units.Count; i++)
            {
                var unit = units[i];
                var unitData = gameData.allUnitsData[i];

                unit.transform.position = unitData.position;
                unit.SetState(unitData.currentState, unitData.busyTime, unitData.restingTime);
            }

            // Загружаем данные зданий
            var buildingController = ControllersManager.Instance.buildingController;
            foreach (var buildingData in gameData.allBuildingsData)
            {
                // Логируем все здания, чтобы убедиться в их наличии
                Debug.Log($"Looking for BuildingId: {buildingData.BuildingId}");

                // Ищем здание по ID
                var building = buildingController.RegularBuildings.Concat(buildingController.SpecialBuildings)
                    .FirstOrDefault(b => b.BuildingId == buildingData.BuildingId);

                if (building != null)
                {
                    building.CurrentState = buildingData.currentState;
                    building.TurnsToRepair = buildingData.turnsToRepair;

                    Debug.Log($"Loaded building: {building.name}, State: {building.CurrentState}");
                }
                else
                {
                    Debug.LogWarning($"Building with ID {buildingData.BuildingId} not found.");
                }
            }

            unitsController.UpdateReadyUnits();
            Debug.Log($"Game loaded from slot {slotIndex}.");
        }
        else
        {
            Debug.LogWarning($"Save slot {slotIndex} not found.");
        }
    }




    public void DeleteSave(int slotIndex)
    {
        string filePath = GetFilePath(slotIndex);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save slot {slotIndex} deleted.");
        }
        else
        {
            Debug.LogWarning($"Save slot {slotIndex} does not exist.");
        }
    }

    public bool SaveExists(int slotIndex)
    {
        return File.Exists(GetFilePath(slotIndex));
    }
}
