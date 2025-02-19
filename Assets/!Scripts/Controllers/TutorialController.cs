using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private CollectableBuilding collectableBuilding;
    [SerializeField] private RepairableBuilding intactBuilding;
    [SerializeField] private RepairableBuilding damagedBuilding;
    [SerializeField] private FactoryBuilding factoryBuilding;

    [SerializeField] private GameObject _specialPopUpPrefab;
    [SerializeField] private Transform _popUpParent;

    [Header("Текст для подсказок зданий")]
    [SerializeField] private string collectableBuildingDescription;
    [SerializeField] private string intactBuildingDescription;
    [SerializeField] private string damagedBuildingDescription;
    [SerializeField] private string factoryBuildingDescription;
    [SerializeField] private string cityHallBuildingDescription;
    [SerializeField] private string hospitalBuildingDescription;
    [SerializeField] private string foodTruckBuildingDescription;

    private GameObject _currentPopUp;
    private Queue<SelectableBuilding> tutorialBuildings = new();
    private Dictionary<SelectableBuilding, string> buildingDescriptions = new();

    private Camera _mainCamera;
    private Canvas _canvas;

    public void StartTutorial()
    {
        _mainCamera = Camera.main; // Получаем основную камеру
        _canvas = _popUpParent.GetComponentInParent<Canvas>(); // Получаем родительский Canvas

        var buildingController = ControllersManager.Instance.buildingController;

        AddBuildingToTutorial(buildingController.GetCityHallBuilding(), cityHallBuildingDescription);
        AddBuildingToTutorial(buildingController.GetHospitalBuilding(), hospitalBuildingDescription);
        AddBuildingToTutorial(buildingController.GetFoodTruckBuilding(), foodTruckBuildingDescription);
        AddBuildingToTutorial(factoryBuilding, factoryBuildingDescription);
        AddBuildingToTutorial(intactBuilding, intactBuildingDescription);
        AddBuildingToTutorial(damagedBuilding, damagedBuildingDescription);
        AddBuildingToTutorial(collectableBuilding, collectableBuildingDescription);

        ShowTutorial();
    }

    private void AddBuildingToTutorial(SelectableBuilding building, string description)
    {
        if (building != null)
        {
            tutorialBuildings.Enqueue(building);
            buildingDescriptions[building] = description;
        }
    }

    public void ShowTutorial()
    {
        ControllersManager.Instance.selectionController.Deselect();

        if (tutorialBuildings.Count == 0)
        {
            Debug.Log("Туториал завершен!");
            return;
        }

        var tutorialBuilding = tutorialBuildings.Dequeue();
        if (tutorialBuilding == null) return;

        var outline = tutorialBuilding.GetComponentInChildren<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        }

        _currentPopUp = Instantiate(_specialPopUpPrefab, _popUpParent);

        // Позиционируем поп-ап над зданием
        Vector3 buildingWorldPosition = tutorialBuilding.transform.GetChild(0).transform.localPosition;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(buildingWorldPosition);

        Vector2 localPosition = _canvas.transform.InverseTransformPoint(screenPosition);
        _currentPopUp.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0);

        Debug.Log(buildingWorldPosition);
        Debug.Log(screenPosition);

        var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
        string description = buildingDescriptions.TryGetValue(tutorialBuilding, out var desc) ? desc : "Нет описания";

        popUpObject.ShowPopUp(tutorialBuilding.BuildingNameText, description, "ПРОДОЛЖИТЬ");
        popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
    }
}
