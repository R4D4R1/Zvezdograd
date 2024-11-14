using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private SelectableObject currentHoveredObject; // Текущий объект, над которым находится курсор
    private SelectableObject selectedObject; // Выбранный объект
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
            SelectableObject hitObject = hit.collider.GetComponent<SelectableObject>();
            if (hitObject)
            {
                if (currentHoveredObject != hitObject)
                {
                    // Отключение обводки у предыдущего объекта
                    if (currentHoveredObject != null && currentHoveredObject != selectedObject)
                    {
                        currentHoveredObject.GetComponent<Outline>().enabled = false;
                    }
                    currentHoveredObject = hitObject;
                    // Включение обводки у нового объекта
                    if (currentHoveredObject != selectedObject)
                    {
                        currentHoveredObject.GetComponent<Outline>().enabled = true;
                    }
                }
            }
            else if (currentHoveredObject != null && currentHoveredObject != selectedObject)
            {
                // Отключение обводки, если курсор ушёл с объекта
                currentHoveredObject.GetComponent<Outline>().enabled = false;
                currentHoveredObject = null;
            }
        }
        else if (currentHoveredObject != null && currentHoveredObject != selectedObject)
        {
            // Отключение обводки, если курсор ушёл с объекта
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
                    // Отключение обводки у предыдущего выбранного объекта и удаление текущего поп-апа
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<Outline>().enabled = false;
                        Destroy(currentPopUp);
                    }
                    selectedObject = hitObject;
                    selectedObject.GetComponent<Outline>().enabled = true;

                    // Создание нового поп-апа и установка текста
                    currentPopUp = Instantiate(popUpPrefab, popUpParent);
                    PopUpBuilding popUpScript = currentPopUp.GetComponent<PopUpBuilding>();
                    popUpScript.TextComponent.text = selectedObject.BuildingName;

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
                        selectedObject.GetComponent<Outline>().enabled = false;
                        selectedObject = null;
                        Destroy(currentPopUp);
                    }
                }
            }
            else
            {
                // Отключение обводки и удаление поп-апа, если выбран пустой объект
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
