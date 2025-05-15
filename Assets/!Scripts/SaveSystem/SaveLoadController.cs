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

#pragma warning disable UDR0001
[RequireComponent(typeof(CanvasGroup))]
public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private List<GameObject> saveSlots;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button backButton;
    [SerializeField, Range(0, 4)] private int autoSaveSlot;

    private List<TextMeshProUGUI> _slotTexts = new();
    private static int? currentSlot;
    private static bool IsStartedFromMainMenu { get; set; } = false;

    protected static ResourceViewModel _resourceViewModel;
    private LoadLevelController _loadLevelController;
    private static TimeController _timeController;
    private static PeopleUnitsController _peopleUnitsController;
    private static PopUpsController _popUpsController;
    private static BuildingsController _buildingsController;
    private static EventController _eventController;
    private static MainGameController _mainGameController;
    private static MainGameUIController _mainGameUIController;
    private CanvasGroup _canvasGroup;

    public readonly Subject<Unit> OnCloseSaveMenuBtnClicked = new();
    public readonly Subject<Unit> OnSaveLoaded = new();
    public readonly Subject<bool> OnSnowChangeState = new();

    private static readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new IgnoreUnityPropertiesResolver(),
        Error = (sender, args) =>
        {
            // Обработка ошибок сериализации
            Debug.LogWarning($"Error during serialization: {args.ErrorContext.Error.Message}");
            args.ErrorContext.Handled = true;
        }
    };

    [Inject]
    public void Construct(LoadLevelController loadLevelController)
    {
        _loadLevelController = loadLevelController;
    }

    public void InjectMainGameDependencies(ResourceViewModel resourceViewModel,
    TimeController timeController, PeopleUnitsController peopleUnitsController,
    PopUpsController popUpsController, BuildingsController buildingsController,
    MainGameUIController mainGameUIController, EventController eventController,
    MainGameController mainGameController)
    {
        _resourceViewModel = resourceViewModel;
        _timeController = timeController;
        _peopleUnitsController = peopleUnitsController;
        _popUpsController = popUpsController;
        _buildingsController = buildingsController;
        _eventController = eventController;
        _mainGameController = mainGameController;
        _mainGameUIController = mainGameUIController;

        _mainGameUIController.OnOpenSaveMenuBtnClicked
            .Subscribe(_ => OnOpenkBtnClicked())
            .AddTo(this);

        _timeController.OnNextDayEvent
            .Subscribe(_ => AutoSave())
            .AddTo(this);

        _mainGameController.OnNewGameStarted
            .Subscribe(_ => StartNewGame())
            .AddTo(this);
    }

    public void StartNewGame()
    {
        OnSnowChangeState.OnNext(false);
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

        if (SceneManager.GetActiveScene().name == Scenes.MAIN_MENU)
        {
            saveButton.gameObject.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name == Scenes.GAME_SCENE)
        {
            saveButton.gameObject.SetActive(true);
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
        saveButton.gameObject.SetActive(false);

        foreach (var slot in saveSlots)
        {
            _slotTexts.Add(slot.GetComponentInChildren<TextMeshProUGUI>());
        }

        UpdateSlotTexts();

        // Привязка событий к кнопкам
        for (int i = 0; i < saveSlots.Count; i++)
        {
            int index = i;
            var button = saveSlots[i].GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectSlot(index));
            }
            else
            {
                Debug.LogWarning($"Save slot at index {i} does not have a Button component.");
            }
        }


        loadButton.onClick.AddListener(LoadGame);
        deleteButton.onClick.AddListener(DeleteSave);
        backButton.onClick.AddListener(OnCloseBtnClicked);
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
        string filePathToAutoSaveSlot = GetFilePath(autoSaveSlot);
        if (!File.Exists(filePathToAutoSaveSlot) && slotIndex == autoSaveSlot)
        {
            Debug.LogWarning($"Слот автосохранения пуст. Выбор невозможен.");
            return;
        }

        currentSlot = slotIndex;
        int slotNum = 0;

        foreach (var slot in saveSlots)
        {
            var unselectedText = slot.transform.GetComponentInChildren<UnselectedText>(true);
            var selectedText = slot.transform.GetComponentInChildren<SelectedText>(true);

            bool isCurrent = currentSlot == slotNum;

            if (unselectedText != null) unselectedText.gameObject.SetActive(!isCurrent);
            if (selectedText != null) selectedText.gameObject.SetActive(isCurrent);

            slotNum++;
        }
    }

    public void SaveGame()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected.");
            return;
        }

        if (currentSlot == autoSaveSlot)
        {
            Debug.LogWarning("Нельзя сохранять вручную в слот автосохранения.");
            return;
        }

        SaveToSlot(currentSlot.Value, isAuto: false);
    }

    private void SaveToSlot(int slotIndex, bool isAuto)
    {
        string filePath = GetFilePath(slotIndex);

        GameData gameData = new GameData
        {
            periodOfDay = _timeController.CurrentPeriod,
            delayedActions = _timeController.DelayedActions,
            allUnitsData = new List<PeopleUnitData>(),
            localIncreaseMaxAPValue = _timeController.LocalIncreaseMaxAPValue,
            localIncreaseAddAPValue = _timeController.LocalIncreaseAddAPValue,
            currentActionPoints = _timeController.CurrentActionPoints,
            isSnowing = _eventController.IsSnowing,

            provision = _resourceViewModel.Provision.Value,
            medicine = _resourceViewModel.Medicine.Value,
            rawMaterials = _resourceViewModel.RawMaterials.Value,
            readyMaterials = _resourceViewModel.ReadyMaterials.Value,
            stability = _resourceViewModel.Stability.Value
        };
        gameData.SetDate(_timeController.CurrentDate);

        foreach (var unit in _peopleUnitsController._allUnits)
        {
            gameData.allUnitsData.Add(new PeopleUnitData
            {
                currentState = unit.GetCurrentState(),
                xPosition = unit.transform.position.x,
                busyTime = unit.BusyTurns,
                restingTime = unit.RestingTurns
            });
        }

        var popUps = _popUpsController.AllPopUps.OfType<ISaveablePopUp>();
        var buildings = _buildingsController.AllBuildings.OfType<ISaveableBuilding>();

        CombinedGameData combined = new CombinedGameData
        {
            mainGameData = gameData,
            popUpSaveData = popUps.Select(p => p.SaveData()).ToList(),
            buildingSaveData = buildings.Select(b => b.SaveData()).ToList()
        };

        string json = JsonConvert.SerializeObject(combined, settings);
        File.WriteAllText(filePath, json);

        if (!isAuto) ClearCurrentSaveSlot();
        UpdateSingleSlotText(slotIndex);
    }

    private async void LoadGame()
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
            ClearCurrentSaveSlot();
        }
    }

    private void LoadDataFromCurrentSlot()
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
            _eventController.IsSnowing = gameData.isSnowing;

            // Load units
            List<PeopleUnit> units = _peopleUnitsController._allUnits;
            for (int i = 0; i < gameData.allUnitsData.Count && i < units.Count; i++)
            {
                var unit = units[i];
                var unitData = gameData.allUnitsData[i];
                unit.transform.position = new Vector3(unitData.xPosition, unit.transform.position.y, 0);
                unit.SetState(unitData.currentState, unitData.busyTime, unitData.restingTime);
            }

            // Load resources
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Provision, gameData.provision - _resourceViewModel.Provision.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Medicine, gameData.medicine - _resourceViewModel.Medicine.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.RawMaterials, gameData.rawMaterials - _resourceViewModel.RawMaterials.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, gameData.readyMaterials - _resourceViewModel.ReadyMaterials.Value));
            _resourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.Stability, gameData.stability - _resourceViewModel.Stability.Value));

            OnSaveLoaded.OnNext(Unit.Default);

            // Load popups - ensure all popups have proper IDs assigned
            foreach (var data in combined.popUpSaveData)
            {
                var popup = _popUpsController.AllPopUps.OfType<ISaveablePopUp>()
                    .FirstOrDefault(p => p.PopUpID == data.popUpID);

                if (popup != null)
                {
                    popup.LoadData(data);
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
            OnSnowChangeState.OnNext(_eventController.IsSnowing);
        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} not found.");
        }
    }

    private void DeleteSave()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("No save slot selected. Cannot delete save.");
            return;
        }

        if (currentSlot == autoSaveSlot)
        {
            Debug.LogWarning("Cannot delete autosave.");
            ClearCurrentSaveSlot();
            return;
        }

        string filePath = GetFilePath(currentSlot.Value);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            UpdateSlotTexts();
            ClearCurrentSaveSlot();
        }
        else
        {
            Debug.LogWarning($"Save slot {currentSlot.Value} does not exist.");
            ClearCurrentSaveSlot();
        }
    }

    public async void LoadLastSave()
    {
        int? latestSlot = null;
        DateTime latestTime = DateTime.MinValue;

        for (int i = 0; i < saveSlots.Count; i++)
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
        foreach (var slot in saveSlots)
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
            _slotTexts[i].text = GetSlotText(i);
        }
    }

    private void UpdateSingleSlotText(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _slotTexts.Count)
        {
            _slotTexts[slotIndex].text = GetSlotText(slotIndex);
        }
    }

    private void AutoSave()
    {
        if(_mainGameController.GameOverState == MainGameController.GameOverStateEnum.Playing)
            SaveToSlot(autoSaveSlot, isAuto: true);
    }

    private static string GetFilePath(int slotIndex)
    {
        return $"{Application.persistentDataPath}/save_slot_{slotIndex + 1}.json";
    }

    private string GetSlotText(int slotIndex)
    {
        string filePath = GetFilePath(slotIndex);

        if (!File.Exists(filePath))
        {
            return (slotIndex == autoSaveSlot) ? "МЕСТО ДЛЯ АВТОСОХРАНЕНИЯ" : "МЕСТО ДЛЯ СОХРАНЕНИЯ";
        }

        try
        {
            string json = File.ReadAllText(filePath);
            CombinedGameData combined = JsonConvert.DeserializeObject<CombinedGameData>(json, settings);
            DateTime lastWriteTime = File.GetLastWriteTime(filePath);
            var date = combined.mainGameData.GetDate();
            var periodOfDay = combined.mainGameData.periodOfDay;
            var label = (slotIndex == autoSaveSlot) ? "АВТОСОХРАНЕНИЕ" : $"СОХРАНЕНИЕ";

            return $"{label} {date:dd.MM.yyyy} {periodOfDay} {lastWriteTime}";
        }
        catch
        {
            return "Ошибка чтения";
        }
    }

}
