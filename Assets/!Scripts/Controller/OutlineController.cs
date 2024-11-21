using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private RepairableBuilding currentHoveredObject; // ������� ������, ��� ������� ��������� ������
    private RepairableBuilding selectedObject; // ��������� ������
    private Camera mainCamera; // �������� ������

    [SerializeField] private GameObject popUpPrefab; // ������ ������ UI
    [SerializeField] private Transform popUpParent; // ������������ ������ ��� ���-����
    private GameObject currentPopUp; // ������� ��������� ������ UI

    void Start()
    {
        mainCamera = Camera.main; // ������������� �������� ������
    }

    void Update()
    {
        HandleHover(); // ��������� ��������� �������
        HandleSelection(); // ��������� ������ �������
    }

    void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // �������� �� RepairableBuilding �� ��������
            RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

            if (hitObject)
            {
                if (currentHoveredObject != hitObject)
                {
                    // ���������� ������� � ����������� �������
                    if (currentHoveredObject != null && currentHoveredObject != selectedObject)
                    {
                        // ��������� ������� � ��������� �������
                        Outline previousOutline = currentHoveredObject.GetComponentInChildren<Outline>();
                        if (previousOutline != null)
                        {
                            previousOutline.enabled = false;
                        }
                    }
                    currentHoveredObject = hitObject;
                    // ��������� ������� � ������ �������
                    if (currentHoveredObject != selectedObject)
                    {
                        Outline newOutline = currentHoveredObject.GetComponentInChildren<Outline>();
                        if (newOutline != null)
                        {
                            newOutline.enabled = true;
                        }
                    }
                }
            }
            else if (currentHoveredObject != null && currentHoveredObject != selectedObject)
            {
                // ���������� �������, ���� ������ ���� � �������
                Outline outline = currentHoveredObject.GetComponentInChildren<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
                currentHoveredObject = null;
            }
        }
        else if (currentHoveredObject != null && currentHoveredObject != selectedObject)
        {
            // ���������� �������, ���� ������ ���� � �������
            Outline outline = currentHoveredObject.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
            currentHoveredObject = null;
        }
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // �������� �� RepairableBuilding �� ��������
                RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

                if (hitObject)
                {
                    // ���������� ������� � ����������� ���������� ������� � �������� �������� ���-���
                    if (selectedObject != null)
                    {
                        Outline previousSelectedOutline = selectedObject.GetComponentInChildren<Outline>();
                        if (previousSelectedOutline != null)
                        {
                            previousSelectedOutline.enabled = false;
                        }
                        if (currentPopUp != null)
                        {
                            currentPopUp.GetComponent<PopUp>().HidePopUp();
                        }
                    }
                    selectedObject = hitObject;
                    // ��������� ������� � ������ ���������� �������
                    Outline selectedOutline = selectedObject.GetComponentInChildren<Outline>();
                    if (selectedOutline != null)
                    {
                        selectedOutline.enabled = true;
                    }

                    // �������� ������ ���-��� � ��������� ������
                    currentPopUp = Instantiate(popUpPrefab, popUpParent);
                    PopUp popUpScript = currentPopUp.GetComponent<PopUp>();
                    popUpScript.LabelText.text = selectedObject.BuildingNameText;
                    popUpScript.DescriptionText.text = selectedObject.DescriptionText;

                    // ��������� ������� ���-��� � ������ �������� ����� �� �������� ������
                    RectTransform popUpRect = currentPopUp.GetComponent<RectTransform>();
                    Vector3 screenPosition = mainCamera.WorldToScreenPoint(hit.point);
                    float offsetY = popUpRect.rect.height * 0.5f; // �������� ������ ���-���
                    float offsetX = popUpRect.rect.width * 0.5f; // �������� ������ ���-���

                    // ����� ���-��� �����
                    currentPopUp.transform.position = new Vector3(screenPosition.x + offsetX, screenPosition.y + offsetY, screenPosition.z);
                }
                else
                {
                    // ���������� ������� � �������� ���-���, ���� ������ ������ ������
                    if (selectedObject != null)
                    {
                        Outline outline = selectedObject.GetComponentInChildren<Outline>();
                        if (outline != null)
                        {
                            outline.enabled = false;
                        }
                        selectedObject = null;
                        if (currentPopUp != null)
                        {
                            currentPopUp.GetComponent<PopUp>().HidePopUp();
                        }
                    }
                }
            }
            else
            {
                // ���������� ������� � �������� ���-���, ���� ������ ������ ������
                if (selectedObject != null)
                {
                    Outline outline = selectedObject.GetComponentInChildren<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                    selectedObject = null;
                    if (currentPopUp != null)
                    {
                        currentPopUp.GetComponent<PopUp>().HidePopUp();
                    }
                }
            }
        }
    }
}
