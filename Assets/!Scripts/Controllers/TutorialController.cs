using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private CollectableBuilding collectableBuilding;
    [SerializeField] private RepairableBuilding intactBuilding;
    [SerializeField] private RepairableBuilding damagedBuilding;
    [SerializeField] private FactoryBuilding factoryBuilding;

    [SerializeField] private Transform _popUpParent;

    [Header("Текст для подсказок зданий")]
    [SerializeField] private string collectableBuildingDescription;
    [SerializeField] private string intactBuildingDescription;
    [SerializeField] private string damagedBuildingDescription;
    [SerializeField] private string factoryBuildingDescription;
    [SerializeField] private string cityHallBuildingDescription;
    [SerializeField] private string hospitalBuildingDescription;
    [SerializeField] private string foodTruckBuildingDescription;

    [SerializeField] private UnityEvent OnTutorialEnd;

    private GameObject _currentPopUp;
    private Queue<SelectableBuilding> tutorialBuildings = new();
    private Dictionary<SelectableBuilding, string> buildingDescriptions = new();

    private Camera _mainCamera;
    private Canvas _canvas;

    protected ControllersManager _controllersManager;
    private PopUpFactory _popUpFactory;

    [Inject]
    public void Construct(ControllersManager controllersManager, PopUpFactory popUpFactory)
    {
        _controllersManager = controllersManager;
        _popUpFactory = popUpFactory;
    }

    public async void StartTutorial()
    {
        await UniTask.Delay(500);

        _mainCamera = Camera.main;
        _canvas = _popUpParent.GetComponentInParent<Canvas>();

        var buildingController = _controllersManager.BuildingController;

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
        _controllersManager.SelectionController.Deselect();

        if (tutorialBuildings.Count == 0)
        {
            OnTutorialEnd.Invoke();

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

        _currentPopUp = _popUpFactory.CreateSpecialPopUp();

        // Позиционируем поп-ап над зданием
        Vector3 buildingWorldPosition = tutorialBuilding.transform.position;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(buildingWorldPosition);

        Vector2 localPosition = _canvas.transform.InverseTransformPoint(screenPosition);
        RectTransform popUpRect = _currentPopUp.GetComponent<RectTransform>();
        _currentPopUp.transform.localPosition = new Vector3(localPosition.x + popUpRect.rect.width * 0.75f, localPosition.y + popUpRect.rect.height * 0.75f, 0);

        //Debug.Log(buildingWorldPosition);
        //Debug.Log(screenPosition);

        var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
        string description = buildingDescriptions.TryGetValue(tutorialBuilding, out var desc) ? desc : "Нет описания";

        popUpObject.ShowPopUp(tutorialBuilding.BuildingNameText, description, "ПРОДОЛЖИТЬ");
        popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
    }
}
