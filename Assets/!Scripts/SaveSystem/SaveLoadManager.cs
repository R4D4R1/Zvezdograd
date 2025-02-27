using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> slotTexts; // Тексты кнопок для отображения статуса слота
    private int? currentSlot = null;

    private void Start()
    {
        UpdateSlotTexts(); // Обновляем текст кнопок при запуске
    }

    private string GetFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"saveSlot{slotIndex}.json");
    }

    public void SelectSlot(int slotIndex)
    {
        currentSlot = slotIndex;
        Debug.Log($"Selected save slot: {slotIndex}");
    }

    public void SaveGame()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected. Cannot save the game.");
            return;
        }

        string filePath = GetFilePath(currentSlot.Value);

        var unitsController = ControllersManager.Instance.peopleUnitsController;
        var timeController = ControllersManager.Instance.timeController;
        var resourceController = ControllersManager.Instance.resourceController;

        GameData gameData = new GameData
        {
            periodOfDay = timeController.CurrentPeriod,
            allUnitsData = new List<PeopleUnitData>(),
            allBuildingsData = new List<BuildingData>(),

            provision = resourceController.GetProvision(),
            medicine = resourceController.GetMedicine(),
            rawMaterials = resourceController.GetRawMaterials(),
            readyMaterials = resourceController.GetReadyMaterials(),
            stability = resourceController.GetStability()
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

        foreach (var building in ControllersManager.Instance.buildingController.RegularBuildings)
        {
            BuildingData buildingData = new BuildingData
            {
                BuildingId = building.BuildingId,
                currentState = building.CurrentState,
                turnsToRepair = building.TurnsToRepair
            };
            gameData.allBuildingsData.Add(buildingData);
        }

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);


        Debug.Log($"Game saved to slot {currentSlot.Value}.");

        currentSlot = null;

        UpdateSlotTexts(); // Обновляем текст на кнопке после сохранения
    }

    public void UpdateSlotTexts()
    {
        for (int i = 0; i < slotTexts.Count; i++)
        {
            string filePath = GetFilePath(i);
            if (File.Exists(filePath))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                slotTexts[i].text = $"СОХРАНЕНИЕ {lastWriteTime:dd.MM.yyyy HH:mm}";
            }
            else
            {
                slotTexts[i].text = "СВОБОДНО";
            }
        }
    }

    public void LoadGame()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected. Cannot load the game.");
            return;
        }

        string filePath = GetFilePath(currentSlot.Value);
        var unitsController = ControllersManager.Instance.peopleUnitsController;
        var timeController = ControllersManager.Instance.timeController;
        var resourceController = ControllersManager.Instance.resourceController;

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

            var buildingController = ControllersManager.Instance.buildingController;
            foreach (var buildingData in gameData.allBuildingsData)
            {
                Debug.Log($"Looking for BuildingId: {buildingData.BuildingId}");

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

            resourceController.AddOrRemoveProvision(gameData.provision - resourceController.GetProvision());
            resourceController.AddOrRemoveMedicine(gameData.medicine - resourceController.GetMedicine());
            resourceController.AddOrRemoveRawMaterials(gameData.rawMaterials - resourceController.GetRawMaterials());
            resourceController.AddOrRemoveReadyMaterials(gameData.readyMaterials - resourceController.GetReadyMaterials());
            resourceController.AddOrRemoveStability(gameData.stability - resourceController.GetStability());

            unitsController.UpdateReadyUnits();
            Debug.Log($"Game loaded from slot {currentSlot.Value}.");
            currentSlot = null;

        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} not found.");
        }
    }

    public void DeleteSave()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected. Cannot delete save.");
            return;
        }

        string filePath = GetFilePath(currentSlot.Value);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save slot {currentSlot.Value} deleted.");
            UpdateSlotTexts(); // Обновляем текст на кнопке после удаления
        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} does not exist.");
        }
    }

    public bool SaveExists()
    {
        return currentSlot != null && File.Exists(GetFilePath(currentSlot.Value));
    }
}
