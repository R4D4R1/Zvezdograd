using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class BuildingSelectionController : MonoInit
{
    [FormerlySerializedAs("_canvas")] [SerializeField] private Canvas canvas;
    [FormerlySerializedAs("_outlineColor")] [SerializeField] private Color outlineColor;
    [FormerlySerializedAs("_outlineWidth")] [Range(0f, 1f), SerializeField] private float outlineWidth;

    private SelectableBuilding _currentHoveredObject;
    private SelectableBuilding _selectedBuilding;
    private GameObject _currentPopUp;
    private bool _isActivated;

    // Injected dependencies
    private PopUpFactory _popUpFactory;
    private Camera _mainCamera;
    private SoundController _soundController;
    private TimeController _timeController;
    private MainGameUIController _mainGameUIController;
    private MainGameController _mainGameController;
    private TutorialController _tutorialController;
    private EventController _eventController;

    [Inject]
    public void Construct(PopUpFactory popUpFactory, Camera mainCamera,
        SoundController soundController, TimeController timeController,
        MainGameUIController mainGameUIController, MainGameController mainGameController,
        TutorialController tutorialController,EventController eventController)
    {
        _popUpFactory = popUpFactory;
        _mainCamera = mainCamera;
        _soundController = soundController;
        _timeController = timeController;
        _mainGameUIController = mainGameUIController;
        _mainGameController = mainGameController;
        _tutorialController = tutorialController;
        _eventController = eventController;
    }

    public override UniTask Init()
    {
        base.Init();

        _timeController.OnNextTurnBtnClickStarted
            .Subscribe(_ => Deselect()).AddTo(this);

        _timeController.OnNextTurnBtnClickEnded
            .Subscribe(_ => SetSelectionControllerState(true)).AddTo(this);

        _mainGameUIController.OnUITurnOff
            .Subscribe(_ => { Deselect(); SetSelectionControllerState(false); }).AddTo(this);

        _mainGameUIController.OnUITurnOn
            .Subscribe(_ => SetSelectionControllerState(true)).AddTo(this);

        _mainGameController.OnGameStarted
            .Subscribe(_ => SetSelectionControllerState(false)).AddTo(this);

        _tutorialController.OnNewBuildingTutorialShow
            .Subscribe(_ => Deselect()).AddTo(this);

        _tutorialController.OnTutorialStarted
            .Subscribe(_ => { SetSelectionControllerState(false); Deselect(); }).AddTo(this);
        
        _eventController.OnGameOverStarted
            .Subscribe(_ => { SetSelectionControllerState(false); Deselect(); }).AddTo(this);

        
        return UniTask.CompletedTask;
    }

    private void OnEnable() => _tutorialController.OnTutorialEnd.AddListener(Deselect);
    private void OnDisable() => _tutorialController.OnTutorialEnd.RemoveAllListeners();

    private void Update()
    {
        if (_isActivated)
        {
            HandleHover();
            HandleSelection();
        }
    }

    private void SetSelectionControllerState(bool isActive)
    {
        _isActivated = isActive;
        if (_mainGameController.GameOverState != MainGameController.GameOverStateEnum.Playing)
        {
            _isActivated = false;
        }
    }

    private void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var hitObject = hit.collider.GetComponentInParent<SelectableBuilding>();

            if (hitObject && hitObject.buildingIsSelectable)
            {
                if (_currentHoveredObject != hitObject)
                {
                    DisableOutline(_currentHoveredObject, exclude: _selectedBuilding);
                    _currentHoveredObject = hitObject;
                    EnableOutline(_currentHoveredObject, exclude: _selectedBuilding);
                }
            }
            else
            {
                DisableOutline(_currentHoveredObject, exclude: _selectedBuilding);
                _currentHoveredObject = null;
            }
        }
        else
        {
            DisableOutline(_currentHoveredObject, exclude: _selectedBuilding);
            _currentHoveredObject = null;
        }
    }

    private void HandleSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var hitObject = hit.collider.GetComponentInParent<SelectableBuilding>();

                if (hitObject && hitObject.buildingIsSelectable)
                {
                    if (hitObject == _selectedBuilding) return;

                    Deselect();
                    _selectedBuilding = hitObject;
                    EnableOutline(_selectedBuilding);
                    _soundController?.PlaySelectionSound();

                    ShowBuildingPopUp(hitObject, hit.point);
                }
                else
                {
                    Deselect();
                }
            }
            else
            {
                Deselect();
            }
        }
    }

    private void ShowBuildingPopUp(SelectableBuilding building, Vector3 hitPoint)
    {
        if (building is RepairableBuilding repairable)
        {
            switch (repairable.CurrentState)
            {
                case RepairableBuilding.State.Intact:
                    if (repairable.Type == RepairableBuilding.BuildingType.LivingArea)
                    {
                        _currentPopUp = _popUpFactory.CreateInfoPopUp();
                        _currentPopUp.GetComponent<InfoPopUp>().ShowPopUp(building.BuildingLabel, building.BuildingDescription);
                    }
                    else
                    {
                        _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                        var popup = _currentPopUp.GetComponent<SpecialPopUp>();
                        popup.ShowPopUp(building.BuildingLabel, building.BuildingDescription, "ОТКРЫТЬ");
                        popup.CurrentFunc = repairable.Type switch
                        {
                            RepairableBuilding.BuildingType.CityHall => SpecialPopUp.PopUpFuncs.OpenCityHallMenu,
                            RepairableBuilding.BuildingType.Factory => SpecialPopUp.PopUpFuncs.OpenFactoryMenu,
                            RepairableBuilding.BuildingType.FoodTrucks => SpecialPopUp.PopUpFuncs.OpenFoodTrucksMenu,
                            RepairableBuilding.BuildingType.Hospital => SpecialPopUp.PopUpFuncs.OpenHospitalMenu,
                            _ => popup.CurrentFunc
                        };
                        if (repairable.Type == RepairableBuilding.BuildingType.Factory)
                            popup.FactoryBuilding = repairable as FactoryBuilding;
                    }
                    break;

                case RepairableBuilding.State.Damaged:
                    _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                    var damagedPopup = _currentPopUp.GetComponent<SpecialPopUp>();
                    damagedPopup.ShowPopUp(repairable.RepairableBuildingConfig.DamagedBuildingLabel,
                        repairable.RepairableBuildingConfig.DamagedBuildingDescription, "ПОЧИНИТЬ");
                    damagedPopup.RepairableBuilding = repairable;
                    damagedPopup.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenRepairMenu;
                    break;
            }
        }
        else if (building is CollectableBuilding collectable)
        {
            _currentPopUp = _popUpFactory.CreateSpecialPopUp();
            var popup = _currentPopUp.GetComponent<SpecialPopUp>();
            popup.ShowPopUp(building.BuildingLabel, building.BuildingDescription, "СОБРАТЬ");
            popup.CollectableBuilding = collectable;
            popup.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenCollectMenu;
        }

        PositionPopUp(hitPoint);
    }

    private void PositionPopUp(Vector3 worldPosition)
    {
        if (_currentPopUp == null) return;

        var rectTransform = _currentPopUp.GetComponent<RectTransform>();
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out var localPoint);
        _currentPopUp.transform.localPosition = localPoint + new Vector2(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f);
    }

    private void EnableOutline(SelectableBuilding building, SelectableBuilding exclude = null)
    {
        if (building && building != exclude)
        {
            var outline = building.GetComponentInChildren<Outline>();
            if (outline) outline.enabled = true;
            _soundController?.PlayHoverSound();
        }
    }

    private void DisableOutline(SelectableBuilding building, SelectableBuilding exclude = null)
    {
        if (building && building != exclude)
        {
            var outline = building.GetComponentInChildren<Outline>();
            if (outline) outline.enabled = false;
        }
    }

    private void Deselect()
    {
        foreach (var outline in FindObjectsByType<Outline>(FindObjectsSortMode.None))
        {
            outline.enabled = false;
        }

        foreach (var popup in FindObjectsByType<InfoPopUp>(FindObjectsSortMode.None))
        {
            if (popup.IsActive) popup.HidePopUp();
        }

        _selectedBuilding = null;
        _currentPopUp = null;
    }
}
