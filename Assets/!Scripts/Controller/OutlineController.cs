using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private SelectableObject currentHoveredObject; // ������� ������, ��� ������� ��������� ������
    private SelectableObject selectedObject; // ��������� ������
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
            SelectableObject hitObject = hit.collider.GetComponent<SelectableObject>();
            if (hitObject)
            {
                if (currentHoveredObject != hitObject)
                {
                    // ���������� ������� � ����������� �������
                    if (currentHoveredObject != null && currentHoveredObject != selectedObject)
                    {
                        currentHoveredObject.GetComponent<Outline>().enabled = false;
                    }
                    currentHoveredObject = hitObject;
                    // ��������� ������� � ������ �������
                    if (currentHoveredObject != selectedObject)
                    {
                        currentHoveredObject.GetComponent<Outline>().enabled = true;
                    }
                }
            }
            else if (currentHoveredObject != null && currentHoveredObject != selectedObject)
            {
                // ���������� �������, ���� ������ ���� � �������
                currentHoveredObject.GetComponent<Outline>().enabled = false;
                currentHoveredObject = null;
            }
        }
        else if (currentHoveredObject != null && currentHoveredObject != selectedObject)
        {
            // ���������� �������, ���� ������ ���� � �������
            currentHoveredObject.GetComponent<Outline>().enabled = false;
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
                SelectableObject hitObject = hit.collider.GetComponent<SelectableObject>();
                if (hitObject)
                {
                    // ���������� ������� � ����������� ���������� ������� � �������� �������� ���-���
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<Outline>().enabled = false;
                        Destroy(currentPopUp);
                    }
                    selectedObject = hitObject;
                    selectedObject.GetComponent<Outline>().enabled = true;

                    // �������� ������ ���-��� � ��������� ������
                    currentPopUp = Instantiate(popUpPrefab, popUpParent);
                    PopUpBuilding popUpScript = currentPopUp.GetComponent<PopUpBuilding>();
                    popUpScript.TextComponent.text = selectedObject.BuildingName;

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
                        selectedObject.GetComponent<Outline>().enabled = false;
                        selectedObject = null;
                        Destroy(currentPopUp);
                    }
                }
            }
            else
            {
                // ���������� ������� � �������� ���-���, ���� ������ ������ ������
                if (selectedObject != null)
                {
                    selectedObject.GetComponent<Outline>().enabled = false;
                    selectedObject = null;
                    Destroy(currentPopUp);
                }
            }
        }
    }
}
