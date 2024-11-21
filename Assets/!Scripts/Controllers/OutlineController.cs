using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private RepairableBuilding currentHoveredObject; // Текущий объект, над которым находится курсор
    private RepairableBuilding selectedObject; // Выбранный объект
    private Camera mainCamera; // Основная камера

    [SerializeField] private GameObject popUpPrefab; // Префаб панели UI
    [SerializeField] private Transform popUpParent; // Родительский объект для поп-апов
    private GameObject currentPopUp; // Текущий экземпляр панели UI

    void Start()
    {
        mainCamera = Camera.main; // Инициализация основной камеры
    }

    void Update()
    {
        HandleHover(); // Обработка наведения курсора
        HandleSelection(); // Обработка выбора объекта
    }

    void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Проверка на RepairableBuilding на родителе
            RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

            if (hitObject)
            {
                if (currentHoveredObject != hitObject)
                {
                    // Отключение обводки у предыдущего объекта
                    if (currentHoveredObject != null && currentHoveredObject != selectedObject)
                    {
                        // Отключаем обводку у дочернего объекта
                        Outline previousOutline = currentHoveredObject.GetComponentInChildren<Outline>();
                        if (previousOutline != null)
                        {
                            previousOutline.enabled = false;
                        }
                    }
                    currentHoveredObject = hitObject;
                    // Включение обводки у нового объекта
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
                // Отключение обводки, если курсор ушёл с объекта
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
            // Отключение обводки, если курсор ушёл с объекта
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
                // Проверка на RepairableBuilding на родителе
                RepairableBuilding hitObject = hit.collider.GetComponentInParent<RepairableBuilding>();

                if (hitObject)
                {
                    // Отключение обводки у предыдущего выбранного объекта и удаление текущего поп-апа
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
                    // Включение обводки у нового выбранного объекта
                    Outline selectedOutline = selectedObject.GetComponentInChildren<Outline>();
                    if (selectedOutline != null)
                    {
                        selectedOutline.enabled = true;
                    }

                    // Создание нового поп-апа и установка текста
                    currentPopUp = Instantiate(popUpPrefab, popUpParent);
                    PopUp popUpScript = currentPopUp.GetComponent<PopUp>();
                    popUpScript.LabelText.text = selectedObject.BuildingNameText;
                    popUpScript.DescriptionText.text = selectedObject.DescriptionText;

                    // Установка позиции поп-апа с учетом смещения вверх на половину высоты
                    RectTransform popUpRect = currentPopUp.GetComponent<RectTransform>();
                    Vector3 screenPosition = mainCamera.WorldToScreenPoint(hit.point);
                    float offsetY = popUpRect.rect.height * 0.5f; // Половина высоты поп-апа
                    float offsetX = popUpRect.rect.width * 0.5f; // Половина ширины поп-апа

                    // Сдвиг поп-апа вверх
                    currentPopUp.transform.position = new Vector3(screenPosition.x + offsetX, screenPosition.y + offsetY, screenPosition.z);
                }
                else
                {
                    // Отключение обводки и удаление поп-апа, если выбран пустой объект
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
                // Отключение обводки и удаление поп-апа, если выбран пустой объект
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
