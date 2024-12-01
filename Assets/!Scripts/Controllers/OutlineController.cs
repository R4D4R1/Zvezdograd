using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance;

    private RepairableBuilding _currentHoveredObject; // Текущий объект, над которым находится курсор
    private RepairableBuilding _selectedBuilding; // Выбранный объект
    private Camera _mainCamera; // Основная камера

    [SerializeField] private GameObject _popUpPrefab; // Префаб панели UI
    [SerializeField] private GameObject _specialPopUpPrefab; // Repair or Special building
    [SerializeField] private Transform _popUpParent; // Родительский объект для поп-апов
    private GameObject _currentPopUp; // Текущий экземпляр панели UI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _mainCamera = Camera.main; // Инициализация основной камеры
    }

    void Update()
    {
        HandleHover(); // Обработка наведения курсора
        HandleSelection(); // Обработка выбора объекта
    }

    void HandleHover()
    {
        // Check if the pointer is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Don't handle hover if the pointer is over a UI element
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Проверка на RepairableBuilding на родителе
            RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

            if (hitObject)
            {
                if (_currentHoveredObject != hitObject)
                {
                    // Отключение обводки у предыдущего объекта
                    if (_currentHoveredObject != null && _currentHoveredObject != _selectedBuilding)
                    {
                        // Отключаем обводку у дочернего объекта
                        Outline previousOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (previousOutline != null)
                        {
                            previousOutline.enabled = false;
                        }
                    }
                    _currentHoveredObject = hitObject;
                    // Включение обводки у нового объекта
                    if (_currentHoveredObject != _selectedBuilding)
                    {
                        Outline newOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (newOutline != null)
                        {
                            newOutline.enabled = true;
                        }
                    }
                }
            }
            else if (_currentHoveredObject != null && _currentHoveredObject != _selectedBuilding)
            {
                // Отключение обводки, если курсор ушёл с объекта
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
            // Отключение обводки, если курсор ушёл с объекта
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
        // Check if the pointer is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Don't handle selection if the pointer is over a UI element
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Проверка на RepairableBuilding на родителе
                RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

                if (hitObject)
                {
                    // Отключение обводки у предыдущего выбранного объекта и удаление текущего поп-апа
                    if (_selectedBuilding != null)
                    {
                        Outline previousSelectedOutline = _selectedBuilding.GetComponentInChildren<Outline>();
                        if (previousSelectedOutline != null)
                        {
                            previousSelectedOutline.enabled = false;
                        }
                        if (_currentPopUp != null)
                        {
                            _currentPopUp.GetComponent<PopUp>().HidePopUp();
                        }
                    }
                    _selectedBuilding = hitObject;
                    // Включение обводки у нового выбранного объекта
                    Outline selectedOutline = _selectedBuilding.GetComponentInChildren<Outline>();

                    if (selectedOutline != null)
                    {
                        selectedOutline.enabled = true;
                    }

                    if (_selectedBuilding.CurrentState == RepairableBuilding.State.Intact
                        && _selectedBuilding.Type == RepairableBuilding.BuildingType.LivingArea)
                    {
                        _currentPopUp = Instantiate(_popUpPrefab, _popUpParent);
                        PopUp popUpScript = _currentPopUp.GetComponent<PopUp>();

                        popUpScript.LabelText.text = _selectedBuilding.BuildingNameText;
                        popUpScript.DescriptionText.text = _selectedBuilding.DescriptionText;
                    }

                    else if (_selectedBuilding.CurrentState == RepairableBuilding.State.Intact
                        && _selectedBuilding.Type != RepairableBuilding.BuildingType.LivingArea)
                    {
                        _currentPopUp = Instantiate(_specialPopUpPrefab, _popUpParent);
                        PopUp popUpScript = _currentPopUp.GetComponent<PopUp>();

                        popUpScript.LabelText.text = _selectedBuilding.BuildingNameText;
                        popUpScript.DescriptionText.text = _selectedBuilding.DescriptionText;

                        popUpScript.BuildingToUse = _selectedBuilding;
                        popUpScript.SetToOpenSpecialMenu();
                    }

                    else if (_selectedBuilding.CurrentState == RepairableBuilding.State.Damaged)
                    {
                        _currentPopUp = Instantiate(_specialPopUpPrefab, _popUpParent);
                        PopUp popUpScript = _currentPopUp.GetComponent<PopUp>();

                        popUpScript.LabelText.text = _selectedBuilding.DamagedBuildingNameText;
                        popUpScript.DescriptionText.text = _selectedBuilding.DamagedDescriptionText;

                        popUpScript.BuildingToUse = _selectedBuilding;
                        popUpScript.SetToRepair();
                    }


                    // Установка позиции поп-апа с учетом смещения вверх на половину высоты
                    RectTransform popUpRect = _currentPopUp.GetComponent<RectTransform>();
                    Vector3 screenPosition = _mainCamera.WorldToScreenPoint(hit.point);
                    float offsetY = popUpRect.rect.height * 0.5f; // Половина высоты поп-апа
                    float offsetX = popUpRect.rect.width * 0.5f; // Половина ширины поп-апа

                    // Сдвиг поп-апа вверх
                    _currentPopUp.transform.position = new Vector3(screenPosition.x + offsetX, screenPosition.y + offsetY, screenPosition.z);
                }
                else
                {
                    // Отключение обводки и удаление поп-апа, если выбран пустой объект
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
                            _currentPopUp.GetComponent<PopUp>().HidePopUp();
                        }
                    }
                }
            }
            else
            {
                // Отключение обводки и удаление поп-апа, если выбран пустой объект
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
                        _currentPopUp.GetComponent<PopUp>().HidePopUp();
                    }
                }
            }
        }
    }

    public void Deselect()
    {
        // Отключение обводки и удаление поп-апа, если выбран объект
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
                _currentPopUp.GetComponent<PopUp>().HidePopUp();
                _currentPopUp = null;
            }
        }
    }

    //private void ShowPopUp(RepairableBuilding selectedObject)
    //{
    //    // Determine the prefab based on the state and type
    //    GameObject popUpPrefab = selectedObject.CurrentState == RepairableBuilding.State.Intact
    //                             && selectedObject.Type == RepairableBuilding.BuildingType.LivingArea
    //                             ? _popUpPrefab
    //                             : _specialPopUpPrefab;

    //    // Instantiate the popup and get the PopUp script
    //    _currentPopUp = Instantiate(popUpPrefab, _popUpParent);
    //    PopUp popUpScript = _currentPopUp.GetComponent<PopUp>();

    //    // Determine the text based on the state
    //    if (selectedObject.CurrentState == RepairableBuilding.State.Intact)
    //    {
    //        popUpScript.LabelText.text = selectedObject.BuildingNameText;
    //        popUpScript.DescriptionText.text = selectedObject.DescriptionText;
    //    }
    //    else if (selectedObject.CurrentState == RepairableBuilding.State.Damaged)
    //    {
    //        popUpScript.LabelText.text = selectedObject.DamagedBuildingNameText;
    //        popUpScript.DescriptionText.text = selectedObject.DamagedDescriptionText;
    //    }
    //}
}
