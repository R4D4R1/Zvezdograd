using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _saveSlots;
    [SerializeField] private List<Button> _slotButtons;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _backButton;

    private List<TextMeshProUGUI> _slotTexts = new();
    private static int? currentSlot;
    private static bool IsStartedFromMainMenu { get; set; } = false;

    protected static ResourceViewModel _resourceViewModel;
    private LoadLevelController _loadLevelController;
    private static TimeController _timeController;
    private static PeopleUnitsController _peopleUnitsController;
    private static PopUpsController _popUpsController;
    private static BuildingsController _buildingsController;
    private static MainGameUIController _mainGameUIController;
    private CanvasGroup _canvasGroup;

    public readonly Subject<Unit> OnCloseSaveMenuBtnClicked = new();

    private static readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    [Inject]
    public void Construct(LoadLevelController loadLevelController)
    {
        _loadLevelController = loadLevelController;
    }

    public void InjectMainGameDependencies(ResourceViewModel resourceViewModel,
    TimeController timeController, PeopleUnitsController peopleUnitsController,
    PopUpsController popUpsController, BuildingsController buildingsController,
    MainGameUIController mainGameUIController)
    {
        _resourceViewModel = resourceViewModel;
        _timeController = timeController;
        _peopleUnitsController = peopleUnitsController;
        _popUpsController = popUpsController;
        _buildingsController = buildingsController;

        mainGameUIController.OnOpenSaveMenuBtnClicked
            .Subscribe(_ => OnOpenkBtnClicked())
            .AddTo(this);
    }

    public void SubscribeUIControllerInMainMenu(UIController uIController)
    {
        uIController.OnOpenSaveMenuBtnClicked
            .Subscribe(_ => OnOpenkBtnClicked())
            .AddTo(this);
    }

    public void Activate()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        if(SceneManager.GetActiveScene().name == Scenes.MAIN_MENU)
        {
            _saveButton.gameObject.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name == Scenes.GAME_SCENE)
        {
            _saveButton.gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _saveButton.gameObject.SetActive(false);

        foreach (var slot in _saveSlots)
        {
            _slotTexts.Add(slot.GetComponentInChildren<TextMeshProUGUI>());
        }

        UpdateSlotTexts();

        // Привязка событий к кнопкам
        for (int i = 0; i < _slotButtons.Count; i++)
        {
            int index = i;  // Локальная копия индекса для использования в лямбде
            _slotButtons[i].onClick.AddListener(() => SelectSlot(index));
        }

        _loadButton.onClick.AddListener(OnLoadGameBtnClicked);
        _deleteButton.onClick.AddListener(OnDeleteSaveBtnClicked);
        _backButton.onClick.AddListener(OnCloseBtnClicked);
    }

    private void OnOpenkBtnClicked()
    {
        Activate();
    }

    private void OnCloseBtnClicked()
    {
        Deactivate();
        ClearCurrentSaveSlot();
        OnCloseSaveMenuBtnClicked.OnNext(Unit.Default);
    }

    public void SelectSlot(int slotIndex)
    {
        currentSlot = slotIndex;
        Debug.Log($"Selected save slot: {currentSlot}");
        int slotNum = 0;

        foreach (var slot in _saveSlots)
        {
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

        // Сбор основной информации
        GameData gameData = new GameData
        {
            periodOfDay = _timeController.CurrentPeriod,
            delayedActions = _timeController.DelayedActions,
            allUnitsData = new List<PeopleUnitData>(),
            localIncreaseMaxAPValue = _timeController.LocalIncreaseMaxAPValue,
            localIncreaseAddAPValue = _timeController.LocalIncreaseAddAPValue,
            currentActionPoints = _timeController.CurrentActionPoints,

            provision = _resourceViewModel.Provision.Value,
            medicine = _resourceViewModel.Medicine.Value,
            rawMaterials = _resourceViewModel.RawMaterials.Value,
            readyMaterials = _resourceViewModel.ReadyMaterials.Value,
            stability = _resourceViewModel.Stability.Value
        };
        gameData.SetDate(_timeController.CurrentDate);

        foreach (var unit in _peopleUnitsController._allUnits)
        {
            PeopleUnitData unitData = new PeopleUnitData
            {
                currentState = unit.GetCurrentState(),
                position = unit.transform.position,
                busyTime = unit.BusyTurns,
                restingTime = unit.RestingTurns
            };
            gameData.allUnitsData.Add(unitData);
        }

        // Сбор попапов и зданий по интерфейсам
        var allPopups = _popUpsController.AllPopUps.OfType<ISaveablePopUp>();
        var allBuildings = _buildingsController.AllBuildings.OfType<ISaveableBuilding>();

        List<PopUpSaveData> popUpSaves = allPopups.Select(p => p.SaveData()).ToList();
        List<BuildingSaveData> buildingSaves = allBuildings.Select(b => b.SaveData()).ToList();

        foreach (var p in popUpSaves)
        {
            Debug.Log(p);
        }
        foreach (var p in buildingSaves)
        {
            Debug.Log(p);
        }

        Debug.Log(popUpSaves);
        Debug.Log(buildingSaves);

        // Комбинирование данных
        CombinedGameData combined = new CombinedGameData
        {
            mainGameData = gameData,
            popUpSaveData = popUpSaves,
            buildingSaveData = buildingSaves
        };

        string json = JsonConvert.SerializeObject(combined, settings);
        File.WriteAllText(filePath, json);

        Debug.Log(json);

        Debug.Log($"Game saved to slot {currentSlot.Value}. Path: {filePath}");

        ClearCurrentSaveSlot();
        UpdateSlotTexts();
    }

    private async void OnLoadGameBtnClicked()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected. Cannot load the game.");
            return;
        }

        string filePath = GetFilePath(currentSlot.Value);
        if (File.Exists(filePath))
        {
            if (SceneManager.GetActiveScene().name == Scenes.MAIN_MENU)
            {
                Deactivate();
                await _loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);
            }

            LoadDataFromCurrentSlot();

            if (!IsStartedFromMainMenu)
            {
                ClearCurrentSaveSlot();
            }

        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} does not exist.");
        }
    }

    private static void LoadDataFromCurrentSlot()
    {
        string filePath = GetFilePath(currentSlot.Value);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CombinedGameData combined = JsonConvert.DeserializeObject<CombinedGameData>(json, settings);
            GameData gameData = combined.mainGameData;

            // Load main game data
            _timeController.SetDateAndPeriod(gameData.GetDate(), gameData.periodOfDay);
            _timeController.SetActionPoints(gameData.currentActionPoints);
            _timeController.DelayedActions = gameData.delayedActions;
            _timeController.LocalIncreaseMaxAPValue = gameData.localIncreaseMaxAPValue;
            _timeController.LocalIncreaseAddAPValue = gameData.localIncreaseAddAPValue;

            // Load units
            List<PeopleUnit> units = _peopleUnitsController._allUnits;
            for (int i = 0; i < gameData.allUnitsData.Count && i < units.Count; i++)
            {
                var unit = units[i];
                var unitData = gameData.allUnitsData[i];
                unit.transform.position = unitData.position;
                unit.SetState(unitData.currentState, unitData.busyTime, unitData.restingTime);
            }

            // Load resources
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, gameData.provision - _resourceViewModel.Provision.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, gameData.medicine - _resourceViewModel.Medicine.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.RawMaterials, gameData.rawMaterials - _resourceViewModel.RawMaterials.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, gameData.readyMaterials - _resourceViewModel.ReadyMaterials.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, gameData.stability - _resourceViewModel.Stability.Value));

            _peopleUnitsController.UpdateReadyUnits();

            // Load popups - ensure all popups have proper IDs assigned
            foreach (var data in combined.popUpSaveData)
            {
                var popup = _popUpsController.AllPopUps.OfType<ISaveablePopUp>()
                    .FirstOrDefault(p => p.PopUpID == data.popUpID);

                if (popup != null)
                {
                    popup.LoadData(data);
                    Debug.Log($"Loaded popup with ID: {data.popUpID}");
                }
                else
                {
                    Debug.LogWarning($"Popup with ID {data.popUpID} not found!");
                }
            }

            // Load buildings - ensure all buildings have proper IDs assigned
            foreach (var data in combined.buildingSaveData)
            {
                var building = _buildingsController.AllBuildings.OfType<ISaveableBuilding>()
                    .FirstOrDefault(b => b.BuildingID == data.buildingID);

                if (building != null)
                {
                    building.LoadData(data);
                    //Debug.Log($"Loaded building with ID: {data.buildingID}");
                }
                else
                {
                    Debug.LogWarning($"Building with ID {data.buildingID} not found!");
                }
            }

            Debug.Log($"Game loaded from slot {currentSlot.Value}.");
        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} not found.");
        }
    }


    private void OnDeleteSaveBtnClicked()
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
            ClearCurrentSaveSlot();
        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} does not exist.");
        }
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

            await _loadLevelController.LoadSceneAsync(Scenes.GAME_SCENE);

            OnLoadGameBtnClicked();
        }
        else
        {
            Debug.LogWarning("No saves found to load.");
        }
    }

    public void ClearCurrentSaveSlot()
    {
        Debug.Log("ClearSlot");
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

    private void UpdateSlotTexts()
    {
        for (int i = 0; i < _slotTexts.Count; i++)
        {
            string filePath = GetFilePath(i);
            if (File.Exists(filePath))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                string json = File.ReadAllText(filePath);
                CombinedGameData gameData = JsonConvert.DeserializeObject<CombinedGameData>(json, settings);

                _slotTexts[i].text = $"СОХРАНЕНИЕ - {gameData.mainGameData.GetDate().ToString("yyyy-MM-dd")}" +
                                     $" {gameData.mainGameData.periodOfDay} {lastWriteTime}";
            }
            else
            {
                _slotTexts[i].text = $"МЕСТО ДЛЯ СОХРАНЕНИЯ";
            }
        }
    }

    private static string GetFilePath(int slotIndex)
    {
        return $"{Application.persistentDataPath}/save_slot_{slotIndex + 1}.json";
    }

    public void SetStartedNewGame()
    {
        IsStartedFromMainMenu = true;
    }
}
