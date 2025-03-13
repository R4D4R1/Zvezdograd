using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;

public class SelectionController : MonoBehaviour
{
    private SelectableBuilding _currentHoveredObject;
    private SelectableBuilding _selectedBuilding;
    private Camera _mainCamera;

    [SerializeField] private Canvas _canvas;

    [SerializeField] private Color _outlineColor;
    [Range(0f,1f), SerializeField] private float _outlineWidth;

    private GameObject _currentPopUp;

    private ControllersManager _controllersManager;
    private PopUpFactory _popUpFactory;
    private Camera mainCamera;

    [Inject]
    public void Construct(PopUpFactory popUpFactory,Camera mainCamera,ControllersManager controllersManager)
    {
        _controllersManager = controllersManager;
        _popUpFactory = popUpFactory;
        _mainCamera = mainCamera;
    }

    void Update()
    {
        HandleHover();
        HandleSelection();
    }

    private void Start()
    {
        _controllersManager.TimeController.NextTurnButton.OnClickAsObservable()
            .Subscribe(_ => Deselect())
            .AddTo(this);
    }

    void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SelectableBuilding hitObject = hit.collider.GetComponentInParent<SelectableBuilding>();

            if (hitObject && hitObject.BuildingIsSelectable) // Проверяем, активное ли здание
            {
                if (_currentHoveredObject != hitObject)
                {
                    if (_currentHoveredObject != null && _currentHoveredObject != _selectedBuilding)
                    {
                        Outline previousOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (previousOutline != null)
                        {
                            previousOutline.enabled = false;
                        }
                    }
                    _currentHoveredObject = hitObject;
                    if (_currentHoveredObject != _selectedBuilding)
                    {
                        Outline newOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (newOutline != null)
                        {
                            newOutline.enabled = true;
                        }
                        Bootstrapper.Instance?.SoundController?.PlayHoverSound();
                    }
                }
            }
            else if (_currentHoveredObject != null && _currentHoveredObject != _selectedBuilding)
            {
                Outline outline = _currentHoveredObject.GetComponentInChildren<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
                _currentHoveredObject = null;
            }
        }
        else if (_currentHoveredObject != null && _currentHoveredObject != _selectedBuilding)
        {
            Outline outline = _currentHoveredObject.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
            _currentHoveredObject = null;
        }
    }

    void HandleSelection()
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

                if (hitObject && hitObject.BuildingIsSelectable) // Проверяем, активное ли здание
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

                    if (selectedOutline != null)
                    {
                        selectedOutline.enabled = true;
                    }

                    Bootstrapper.Instance?.SoundController?.PlaySelectionSound();

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
                                popUpObject.FoodTrucksBuilding = repairableBuilding as FoodTrucksBuilding;
                                popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenFoodTrucksMenu;
                            }

                            if (repairableBuilding.Type == RepairableBuilding.BuildingType.Hospital)
                            {
                                popUpObject.HospitalBuilding = repairableBuilding as HospitalBuilding;
                                popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenHospitalMenu;
                            }
                        }
                        else if (repairableBuilding.CurrentState == RepairableBuilding.State.Damaged)
                        {
                            _currentPopUp = _popUpFactory.CreateSpecialPopUp();
                            SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "РЕМОНТ");

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

                    // Спавн попапа
                    RectTransform popUpRect = _currentPopUp.GetComponent<RectTransform>();
                    Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, hit.point);
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPosition, _canvas.worldCamera, out localPoint);
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
        // Отключаем аутлайн у всех объектов с компонентом Outline
        Outline[] allOutlines = FindObjectsByType<Outline>(FindObjectsSortMode.None);
        foreach (Outline outline in allOutlines)
        {
            outline.enabled = false;
        }

        // Сбрасываем текущий выбранный объект
        _selectedBuilding = null;

        // Прячем все попапы на сцене
        InfoPopUp[] allPopUps = FindObjectsByType<InfoPopUp>(FindObjectsSortMode.None);
        foreach (InfoPopUp popUp in allPopUps)
        {
            if (popUp.IsActive)
            {
                popUp.HidePopUp();
            }
        }

        // Сбрасываем текущий попап, если он есть
        if (_currentPopUp != null)
        {
            _currentPopUp = null;
        }
    }

}
