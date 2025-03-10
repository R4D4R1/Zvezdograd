using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _saveSlots;
    private List<TextMeshProUGUI> _slotTexts = new();
    private static int? currentSlot = null;
    public static bool IsStartedFromMainMenu { get; private set; } = false;

    private void Start()
    {
        foreach (var slot in _saveSlots)
        {
            _slotTexts.Add(slot.GetComponentInChildren<TextMeshProUGUI>());
        }

        UpdateSlotTexts();
    }
    public void SelectSlot(int slotIndex)
    {
        currentSlot = slotIndex;
        Debug.Log($"Selected save slot: {currentSlot}");
        int slotNum = 0;

        foreach (var slot in _saveSlots)
        {
            // 0 - unselected
            // 1 - selected

            var unselectedText = slot.transform.GetComponentInChildren<UnselectedText>(true);
            var selectedText = slot.transform.GetComponentInChildren<SelectedText>(true);

            if (unselectedText != null) unselectedText.gameObject.SetActive(false);
            if (selectedText != null) selectedText.gameObject.SetActive(true);

            if (currentSlot != slotNum)
            {
                if (unselectedText != null) unselectedText.gameObject.SetActive(true);
                if (selectedText != null) selectedText.gameObject.SetActive(false);
            }

            slotNum++;
        }
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

        foreach (var building in ControllersManager.Instance.buildingController.AllBuildings)
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

        ClearCurrentSaveSlot();

        UpdateSlotTexts();
    }
    public async void LoadGame()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected. Cannot load the game.");
            return;
        }

        if (SceneManager.GetActiveScene().name == Scenes.MAIN_MENU)
        {
            await Bootstrapper.Instance.loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);
        }

        //LoadDataFromSlot(currentSlot);

        if (!IsStartedFromMainMenu)
        {
            ClearCurrentSaveSlot();
        }
    }

    public static void LoadDataFromCurrentSlot()
    {
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

            Debug.Log(buildingController.gameObject);

            foreach (var buildingData in gameData.allBuildingsData)
            {
                Debug.Log($"Looking for BuildingId: {buildingData.BuildingId}");

                var building = buildingController.AllBuildings
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

            resourceController.ModifyResource(ResourceController.ResourceType.Provision, gameData.provision - resourceController.GetProvision());
            resourceController.ModifyResource(ResourceController.ResourceType.Medicine, gameData.medicine - resourceController.GetMedicine());
            resourceController.ModifyResource(ResourceController.ResourceType.RawMaterials, gameData.rawMaterials - resourceController.GetRawMaterials());
            resourceController.ModifyResource(ResourceController.ResourceType.ReadyMaterials, gameData.readyMaterials - resourceController.GetReadyMaterials());
            resourceController.ModifyResource(ResourceController.ResourceType.Stability, gameData.stability - resourceController.GetStability());

            unitsController.UpdateReadyUnits();
            Debug.Log($"Game loaded from slot {currentSlot.Value}.");
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
            UpdateSlotTexts();
        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} does not exist.");
        }

        ClearCurrentSaveSlot();
    }
    public async void LoadLastSave()
    {
        int? latestSlot = null;
        DateTime latestTime = DateTime.MinValue;

        for (int i = 0; i < _saveSlots.Count; i++)
        {
            string filePath = GetFilePath(i);
            if (File.Exists(filePath))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                if (lastWriteTime > latestTime)
                {
                    latestTime = lastWriteTime;
                    latestSlot = i;
                }
            }
        }

        if (latestSlot != null)
        {
            SelectSlot(latestSlot.Value);

            await Bootstrapper.Instance.loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);

            LoadGame();
        }
        else
        {
            Debug.LogWarning("No saves found to load.");
        }
    }
    public void ClearCurrentSaveSlot()
    {
            int slotNum = 0;
            foreach (var slot in _saveSlots)
            {
                var unselectedText = slot.transform.GetComponentInChildren<UnselectedText>(true);
                var selectedText = slot.transform.GetComponentInChildren<SelectedText>(true);

                if (currentSlot == slotNum)
                {
                    if (unselectedText != null) unselectedText.gameObject.SetActive(true);
                    if (selectedText != null) selectedText.gameObject.SetActive(false);
                }

                slotNum++;
            }
        currentSlot = null;
    }
    public void UpdateSlotTexts()
    {
        for (int i = 0; i < _slotTexts.Count; i++)
        {

            string filePath = GetFilePath(i);
            if (File.Exists(filePath))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                string json = File.ReadAllText(filePath);
                GameData gameData = JsonUtility.FromJson<GameData>(json);

                _slotTexts[i].text = $"—Œ’–¿Õ≈Õ»≈ {lastWriteTime:dd.MM.yyyy HH:mm} {gameData.GetDate().ToString("yyyy-MM-dd")}";
            }
            else
            {
                _slotTexts[i].text = "—¬Œ¡ŒƒÕŒ";
            }
        }
    }
    public bool SaveExists()
    {
        return currentSlot != null && File.Exists(GetFilePath(currentSlot.Value));
    }
    private static string GetFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"saveSlot{slotIndex}.json");
    }

    public void SetLoadedFromMainMenu()
    {
        IsStartedFromMainMenu = true;
    }
    public static void SetStartedNewGame()
    {
        IsStartedFromMainMenu = false;
    }
}
