using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

public class BuildingSelectionController : MonoInit
{
    [FormerlySerializedAs("_canvas")] [SerializeField] private Canvas canvas;

    [FormerlySerializedAs("_outlineColor")] [SerializeField] private Color outlineColor;
    [FormerlySerializedAs("_outlineWidth")] [Range(0f,1f), SerializeField] private float outlineWidth;

    private SelectableBuilding _currentHoveredObject;
    private SelectableBuilding _selectedBuilding;
    private GameObject _currentPopUp;
    private bool _isActivated;

    // INJECT OBJECTS
    private PopUpFactory _popUpFactory;
    private Camera _mainCamera;
    private SoundController _soundController;
    private TimeController _timeController;
    private MainGameUIController _mainGameUIController;
    private MainGameController _mainGameController;

    [Inject]
    public void Construct(PopUpFactory popUpFactory,Camera mainCamera,
        SoundController soundController,TimeController timeController,
        MainGameUIController mainGameUIController,MainGameController mainGameController)
    {
        _popUpFactory = popUpFactory;
        _mainCamera = mainCamera;
        _soundController = soundController;
        _timeController = timeController;
        _mainGameUIController = mainGameUIController;
        _mainGameController = mainGameController;
    }

    public override void Init()
    {
        base.Init();
        _timeController.NextTurnButton.OnClickAsObservable()
            .Subscribe(_ => Deselect())
            .AddTo(this);

        _timeController.OnNextTurnBtnClickEnded
            .Subscribe(_ => SetSelectionControllerState(true))
            .AddTo(this);

        _mainGameUIController.OnUITurnOff
            .Subscribe(_ => Deselect())
            .AddTo(this);

        _mainGameUIController.OnUITurnOff
            .Subscribe(_ => SetSelectionControllerState(false))
            .AddTo(this);

        _mainGameUIController.OnUITurnOn
            .Subscribe(_ => SetSelectionControllerState(true))
            .AddTo(this);

        _mainGameController.OnGameStarted
            .Subscribe(_ => SetSelectionControllerState(false))
            .AddTo(this);
    }

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
    }

    private void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SelectableBuilding hitObject = hit.collider.GetComponentInParent<SelectableBuilding>();

            if (hitObject && hitObject.buildingIsSelectable)
            {
                if (_currentHoveredObject != hitObject)
                {
                    if (_currentHoveredObject && _currentHoveredObject != _selectedBuilding)
                    {
                        Outline previousOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (previousOutline)
                        {
                            previousOutline.enabled = false;
                        }
                    }
                    _currentHoveredObject = hitObject;
                    if (_currentHoveredObject != _selectedBuilding)
                    {
                        Outline newOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (newOutline)
                        {
                            newOutline.enabled = true;
                        }
                        _soundController?.PlayHoverSound();
                    }
                }
            }
            else if (_currentHoveredObject && _currentHoveredObject != _selectedBuilding)
            {
                Outline outline = _currentHoveredObject.GetComponentInChildren<Outline>();
                if (outline)
                {
                    outline.enabled = false;
                }
                _currentHoveredObject = null;
            }
        }
        else if (_currentHoveredObject && _currentHoveredObject != _selectedBuilding)
        {
            Outline outline = _currentHoveredObject.GetComponentInChildren<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }
            _currentHoveredObject = null;
        }
    }

    private void HandleSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SelectableBuilding hitObject = hit.collider.GetComponentInParent<SelectableBuilding>();

                if (hitObject && hitObject.buildingIsSelectable)
                {
                    if (hitObject == _selectedBuilding)
                    {
                        return;
                    }

                    if (_selectedBuilding != null)
                    {
                        Outline previousSelectedOutline = _selectedBuilding.GetComponentInChildren<Outline>();
                        if (previousSelectedOutline != null)
                        {
                            previousSelectedOutline.enabled = false;
                        }
                        if (_currentPopUp != null)
                        {
                            Deselect();
                        }
                    }

                    _selectedBuilding = hitObject;
                    Outline selectedOutline = _selectedBuilding.GetComponentInChildren<Outline>();

                    if (selectedOutline)
                    {
                        selectedOutline.enabled = true;
                    }

                    _soundController?.PlaySelectionSound();

                    if (_selectedBuilding is RepairableBuilding repairableBuilding)
                    {
                        if (repairableBuilding.CurrentState == RepairableBuilding.State.Intact
                          && repairableBuilding.Type == RepairableBuilding.BuildingType.LivingArea)
                        {
                            _currentPopUp =  _popUpFactory.CreateInfoPopUp();
                            InfoPopUp popUpObject = _currentPopUp.GetComponent<InfoPopUp>();
                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText);
                        }
                        else if (repairableBuilding.CurrentState == RepairableBuilding.State.Intact
                           && repairableBuilding.Type != RepairableBuilding.BuildingType.LivingArea)
                        {
                            _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                            SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "ОТКРЫТЬ");

                            if (repairableBuilding.Type == RepairableBuilding.BuildingType.CityHall)
                            {
                                popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenCityHallMenu;
                            }

                            if (repairableBuilding.Type == RepairableBuilding.BuildingType.Factory)
                            {
                                popUpObject.FactoryBuilding = repairableBuilding as FactoryBuilding;

                                popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenFactoryMenu;
                            }

                            if (repairableBuilding.Type == RepairableBuilding.BuildingType.FoodTrucks)
                            {
                                popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenFoodTrucksMenu;
                            }

                            if (repairableBuilding.Type == RepairableBuilding.BuildingType.Hospital)
                            {
                                popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenHospitalMenu;
                            }
                        }
                        else if (repairableBuilding.CurrentState == RepairableBuilding.State.Damaged)
                        {
                            _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                            SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "ПОЧИНИТЬ");

                            popUpObject.RepairableBuilding = repairableBuilding;
                            popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenRepairMenu;
                        }
                    }

                    if (_selectedBuilding is CollectableBuilding collectableBuilding)
                    {
                        _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                        SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                        popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "СОБРАТЬ");

                        popUpObject.CollectableBuilding = collectableBuilding;
                        popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenCollectMenu;
                    }

                    RectTransform popUpRect = _currentPopUp.GetComponent<RectTransform>();
                    Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, hit.point);
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out localPoint);
                    _currentPopUp.transform.localPosition = localPoint + new Vector2(popUpRect.rect.width * 0.5f, popUpRect.rect.height * 0.5f);
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

    public void Deselect()
    {
        var allOutlines = FindObjectsByType<Outline>(FindObjectsSortMode.None);
        foreach (var outline in allOutlines)
        {
            outline.enabled = false;
        }

        _selectedBuilding = null;

        InfoPopUp[] allPopUps = FindObjectsByType<InfoPopUp>(FindObjectsSortMode.None);
        foreach (InfoPopUp popUp in allPopUps)
        {
            if (popUp.IsActive)
            {
                popUp.HidePopUp();
            }
        }

        if (_currentPopUp)
        {
            _currentPopUp = null;
        }
    }

    private void TurnOfUI()
    {
        Deselect();
        SetSelectionControllerState(false);
    }
}
