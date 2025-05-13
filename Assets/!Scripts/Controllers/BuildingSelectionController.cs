using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class BuildingSelectionController : MonoInit
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Color defaultOutlineColor;
    [SerializeField] private Color highlightOutlineColor;
    [Range(0f, 1f), SerializeField] private float outlineWidth;

    private SelectableBuilding _currentHoveredObject;
    private SelectableBuilding _selectedBuilding;
    private SelectableBuilding _highlightBuilding;
    private InfoPopUp _currentPopUp;
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
    private PopUpsController _popUpsController;

    [Inject]
    public void Construct(PopUpFactory popUpFactory, Camera mainCamera,
        SoundController soundController, TimeController timeController,
        MainGameUIController mainGameUIController, MainGameController mainGameController,
        TutorialController tutorialController, EventController eventController,
        PopUpsController popUpsController)
    {
        _popUpFactory = popUpFactory;
        _mainCamera = mainCamera;
        _soundController = soundController;
        _timeController = timeController;
        _mainGameUIController = mainGameUIController;
        _mainGameController = mainGameController;
        _tutorialController = tutorialController;
        _eventController = eventController;
        _popUpsController = popUpsController;
    }

    public override UniTask Init()
    {
        base.Init();

        foreach (var building in FindObjectsByType<SelectableBuilding>(FindObjectsSortMode.None))
        {
            building.OnPointerEnterSubject
                .Where(_ => _isActivated)
                .Subscribe(OnBuildingHovered)
                .AddTo(this);

            building.OnPointerExitSubject
                .Where(_ => _isActivated)
                .Subscribe(_ => OnBuildingHoverExit())
                .AddTo(this);

            building.OnPointerClickSubject
                .Where(_ => _isActivated)
                .Subscribe(OnBuildingClicked)
                .AddTo(this);
        }

        foreach (var gameObject in FindObjectsByType<BackgroundClickCatcher>(FindObjectsSortMode.None))
        {
            gameObject.OnBackgroundClicked
                .Where(_ => _isActivated)
                .Subscribe(_ => Deselect())
                .AddTo(this);
        }

        _timeController.OnNextTurnBtnClickBetween
            .Subscribe(_ => OnHighilightedBuildingDisable()).AddTo(this);

        _timeController.OnNextTurnBtnClickStarted
            .Subscribe(_ => Deselect()).AddTo(this);

        _timeController.OnNextTurnBtnClickEnded
            .Subscribe(_ => SetSelectionControllerState(true)).AddTo(this);

        _mainGameUIController.OnUITurnOff
            .Subscribe(_ => { Deselect(); SetSelectionControllerState(false); }).AddTo(this);

        _mainGameUIController.OnUITurnOn
            .Subscribe(_ => SetSelectionControllerState(true)).AddTo(this);

        _mainGameController.OnNewGameStarted
            .Subscribe(_ => SetSelectionControllerState(false)).AddTo(this);

        _tutorialController.OnNewBuildingTutorialShow
            .Subscribe(_ => Deselect()).AddTo(this);

        _tutorialController.OnTutorialStarted
            .Subscribe(_ => { SetSelectionControllerState(false); Deselect(); }).AddTo(this);

        _eventController.OnGameOverStarted
            .Subscribe(_ => { SetSelectionControllerState(false); Deselect(); }).AddTo(this);

        _popUpsController.HospitalPopUp.OnBuildingHighlighted
            .Subscribe(HighlightBuilding)
            .AddTo(this);

        _popUpsController.CityHallPopUp.OnBuildingHighlighted
            .Subscribe(HighlightBuilding)
            .AddTo(this);

        _popUpsController.FoodTrucksPopUp.OnBuildingHighlighted
            .Subscribe(HighlightBuilding)
        .AddTo(this);

        return UniTask.CompletedTask;
    }

    private void OnEnable() => _tutorialController.OnTutorialEnd.AddListener(Deselect);
    private void OnDisable() => _tutorialController.OnTutorialEnd.RemoveAllListeners();


    private void SetSelectionControllerState(bool isActive)
    {
        _isActivated = isActive;
        if (_mainGameController.GameOverState != MainGameController.GameOverStateEnum.Playing)
        {
            _isActivated = false;
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
                        //_currentPopUp.OnPopUpHide
                        //    .Take(1)
                        //    .Subscribe(_ =>
                        //    {

                        //    })
                        //    .AddTo(_currentPopUp);

                        _currentPopUp.ShowPopUp(building.BuildingLabel, building.BuildingDescription);
                    }
                    else
                    {
                        _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                        var popup = _currentPopUp as SpecialPopUp;
                        //popup.OnPopUpHide
                        //    .Take(1)
                        //    .Subscribe(_ =>
                        //    {

                        //    })
                        //    .AddTo(popup);
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

    private void EnableOutline(SelectableBuilding building, Color color, SelectableBuilding exclude = null)
    {
        if (building && building != exclude)
        {
            var outline = building.GetComponentInChildren<Outline>();
            if (outline)
            {
                outline.OutlineColor = color;
                outline.OutlineWidth = outlineWidth;
                outline.enabled = true;
            }
        }
    }

    private void DisableOutline(SelectableBuilding building, SelectableBuilding exclude = null)
    {
        if (building && building != exclude)
        {
            var outline = building.GetComponentInChildren<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }
        }
    }

    private async void HighlightBuilding(SelectableBuilding building)
    {
        await UniTask.Yield();

        _highlightBuilding = building;
        EnableOutline(building, highlightOutlineColor);
    }

    private void Deselect()
    {
        foreach (var outline in FindObjectsByType<Outline>(FindObjectsSortMode.None))
        {
            if (outline.gameObject != _highlightBuilding?.gameObject && outline.isActiveAndEnabled)
            {
                outline.enabled = false;
            }
        }

        foreach (var popup in FindObjectsByType<InfoPopUp>(FindObjectsSortMode.None))
        {
            if (popup.IsActive) popup.HidePopUp();
        }

        _selectedBuilding = null;
        _currentPopUp = null;
    }

    private void OnBuildingHovered(SelectableBuilding building)
    {
        if (!building.BuildingIsSelectable) return;
        if (building == _selectedBuilding || building == _highlightBuilding) return;
        DisableOutline(_currentHoveredObject, exclude: _selectedBuilding);
        _currentHoveredObject = building;
        EnableOutline(building,defaultOutlineColor, exclude: _selectedBuilding);
        _soundController?.PlayHoverSound();
    }

    private void OnBuildingHoverExit()
    {
        if (!_currentHoveredObject) return;
        DisableOutline(_currentHoveredObject, exclude: _selectedBuilding);
        _currentHoveredObject = null;
    }

    private void OnBuildingClicked(SelectableBuilding building)
    {
        if (!building.BuildingIsSelectable) return;
        if (building == _selectedBuilding) return;
        if (building == _highlightBuilding)
        {
            _highlightBuilding = null;
        }

        Deselect();
        _selectedBuilding = building;
        EnableOutline(building, defaultOutlineColor);
        _soundController?.PlaySelectionSound();

        Vector3 centerPoint = building.GetComponent<Collider>().bounds.center;
        ShowBuildingPopUp(building, centerPoint);
    }

    private void OnHighilightedBuildingDisable()
    {
        _highlightBuilding = null;
    }
}
