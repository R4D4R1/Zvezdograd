using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionController : MonoBehaviour
{
    private SelectableBuilding _currentHoveredObject;
    private SelectableBuilding _selectedBuilding;
    private Camera _mainCamera;

    [SerializeField] private GameObject _popUpPrefab;
    [SerializeField] private GameObject _specialPopUpPrefab;
    [SerializeField] private Transform _popUpParent;

    [SerializeField] private RepairPopUp _repairPopUp;
    [SerializeField] private CollectPopUp _collectPopUp;
    [SerializeField] private FactoryPopUp _factoryPopUp;

    private GameObject _currentPopUp;
    private Canvas _canvas;


    public RepairPopUp GetRepairPopUp()
    {
        return _repairPopUp;
    }
    public CollectPopUp GetCollectPopUp()
    {
        return _collectPopUp;
    }
    public FactoryPopUp GetFactoryPopUp()
    {
        return _factoryPopUp;
    }

    void Start()
    {
        _mainCamera = Camera.main;
        _canvas = _popUpParent.GetComponentInParent<Canvas>();
    }

    void Update()
    {
        HandleHover();
        HandleSelection();
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

            if (hitObject && hitObject.BuildingIsActive) // Проверяем, активное ли здание
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
                        ControllersManager.Instance.SoundController.PlayHoverSound();
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

                if (hitObject && hitObject.BuildingIsActive) // Проверяем, активное ли здание
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
                            _currentPopUp.GetComponent<InfoPopUp>().HidePopUp();
                        }
                    }

                    _selectedBuilding = hitObject;
                    Outline selectedOutline = _selectedBuilding.GetComponentInChildren<Outline>();

                    if (selectedOutline != null)
                    {
                        selectedOutline.enabled = true;
                    }

                    ControllersManager.Instance.SoundController.PlaySelectionSound();

                    if (_selectedBuilding is RepairableBuilding repairableBuilding)
                    {
                        if (repairableBuilding.CurrentState == RepairableBuilding.State.Intact
                          && repairableBuilding.Type == RepairableBuilding.BuildingType.LivingArea)
                        {
                            _currentPopUp = Instantiate(_popUpPrefab, _popUpParent);
                            InfoPopUp popUpObject = _currentPopUp.GetComponent<InfoPopUp>();
                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText);
                        }
                        else if (repairableBuilding.CurrentState == RepairableBuilding.State.Intact
                           && repairableBuilding.Type == RepairableBuilding.BuildingType.Factory)
                        {
                            _currentPopUp = Instantiate(_specialPopUpPrefab, _popUpParent);
                            SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "ОТКРЫТЬ");

                            popUpObject.FactoryBuilding = repairableBuilding as FactoryBuilding;
                            popUpObject.SetToOpenSpecialMenu();
                        }
                        else if (repairableBuilding.CurrentState == RepairableBuilding.State.Damaged)
                        {
                            _currentPopUp = Instantiate(_specialPopUpPrefab, _popUpParent);
                            SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                            popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "РЕМОНТ");

                            popUpObject.RepairableBuilding = repairableBuilding;
                            popUpObject.SetToRepair();
                        }
                    }

                    if (_selectedBuilding is CollectableBuilding collectableBuilding)
                    {
                        _currentPopUp = Instantiate(_specialPopUpPrefab, _popUpParent);
                        SpecialPopUp popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();

                        popUpObject.ShowPopUp(_selectedBuilding.BuildingNameText, _selectedBuilding.DescriptionText, "СОБРАТЬ");

                        popUpObject.CollectableBuilding = collectableBuilding;
                        popUpObject.SetToCollect();
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
        if (_selectedBuilding != null)
        {
            Outline outline = _selectedBuilding.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
            _selectedBuilding = null;
            if (_currentPopUp != null)
            {
                _currentPopUp.GetComponent<InfoPopUp>().HidePopUp();
            }
        }
    }
}
