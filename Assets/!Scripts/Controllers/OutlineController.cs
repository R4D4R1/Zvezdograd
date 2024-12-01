using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance;

    private RepairableBuilding _currentHoveredObject; // ������� ������, ��� ������� ��������� ������
    private RepairableBuilding _selectedBuilding; // ��������� ������
    private Camera _mainCamera; // �������� ������

    [SerializeField] private GameObject _popUpPrefab; // ������ ������ UI
    [SerializeField] private GameObject _specialPopUpPrefab; // Repair or Special building
    [SerializeField] private Transform _popUpParent; // ������������ ������ ��� ���-����
    private GameObject _currentPopUp; // ������� ��������� ������ UI

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
        _mainCamera = Camera.main; // ������������� �������� ������
    }

    void Update()
    {
        HandleHover(); // ��������� ��������� �������
        HandleSelection(); // ��������� ������ �������
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
            // �������� �� RepairableBuilding �� ��������
            RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

            if (hitObject)
            {
                if (_currentHoveredObject != hitObject)
                {
                    // ���������� ������� � ����������� �������
                    if (_currentHoveredObject != null && _currentHoveredObject != _selectedBuilding)
                    {
                        // ��������� ������� � ��������� �������
                        Outline previousOutline = _currentHoveredObject.GetComponentInChildren<Outline>();
                        if (previousOutline != null)
                        {
                            previousOutline.enabled = false;
                        }
                    }
                    _currentHoveredObject = hitObject;
                    // ��������� ������� � ������ �������
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
                // ���������� �������, ���� ������ ���� � �������
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
            // ���������� �������, ���� ������ ���� � �������
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
                // �������� �� RepairableBuilding �� ��������
                RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

                if (hitObject)
                {
                    // ���������� ������� � ����������� ���������� ������� � �������� �������� ���-���
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
                    // ��������� ������� � ������ ���������� �������
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


                    // ��������� ������� ���-��� � ������ �������� ����� �� �������� ������
                    RectTransform popUpRect = _currentPopUp.GetComponent<RectTransform>();
                    Vector3 screenPosition = _mainCamera.WorldToScreenPoint(hit.point);
                    float offsetY = popUpRect.rect.height * 0.5f; // �������� ������ ���-���
                    float offsetX = popUpRect.rect.width * 0.5f; // �������� ������ ���-���

                    // ����� ���-��� �����
                    _currentPopUp.transform.position = new Vector3(screenPosition.x + offsetX, screenPosition.y + offsetY, screenPosition.z);
                }
                else
                {
                    // ���������� ������� � �������� ���-���, ���� ������ ������ ������
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
                // ���������� ������� � �������� ���-���, ���� ������ ������ ������
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
        // ���������� ������� � �������� ���-���, ���� ������ ������
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
