using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private CollectableBuilding collectableBuilding;
    [SerializeField] private RepairableBuilding intactBuilding;
    [SerializeField] private RepairableBuilding damagedBuilding;
    [SerializeField] private FactoryBuilding factoryBuilding;

    [FormerlySerializedAs("_popUpParent")] [SerializeField] private Transform popUpParent;

    [FormerlySerializedAs("OnTutorialEnd")] [SerializeField] private UnityEvent onTutorialEnd;

    private GameObject _currentPopUp;
    private readonly Queue<SelectableBuilding> _tutorialBuildings = new();
    private readonly Dictionary<SelectableBuilding, string> _buildingDescriptions = new();

    private Canvas _canvas;

    private TutorialControllerConfig _tutorialControllerConfig;
    private SelectionController _selectionController;
    private BuildingController _buildingController;
    private PopUpFactory _popUpFactory;
    private Camera _mainCamera;

    public readonly Subject<Unit> OnTutorialStarted = new();

    [Inject]
    public void Construct(TutorialControllerConfig tutorialControllerConfig, SelectionController selectionController, BuildingController buildingController,
        PopUpFactory popUpFactory, Camera camera)
    {
        _tutorialControllerConfig = tutorialControllerConfig;
        _selectionController = selectionController;
        _buildingController = buildingController;
        _popUpFactory = popUpFactory;
        _mainCamera = camera;
    }

    public async void StartTutorial()
    {
        await UniTask.Delay(5000);

        OnTutorialStarted.OnNext(Unit.Default);

        _canvas = popUpParent.GetComponentInParent<Canvas>();

        // Используем данные из конфига для добавления зданий в tutorial
        AddBuildingToTutorial(_buildingController.GetCityHallBuilding(), _tutorialControllerConfig.CityHallBuildingDescription);
        AddBuildingToTutorial(_buildingController.GetHospitalBuilding(), _tutorialControllerConfig.HospitalBuildingDescription);
        AddBuildingToTutorial(_buildingController.GetFoodTruckBuilding(), _tutorialControllerConfig.FoodTruckBuildingDescription);
        AddBuildingToTutorial(factoryBuilding, _tutorialControllerConfig.FactoryBuildingDescription);
        AddBuildingToTutorial(intactBuilding, _tutorialControllerConfig.IntactBuildingDescription);
        AddBuildingToTutorial(damagedBuilding, _tutorialControllerConfig.DamagedBuildingDescription);
        AddBuildingToTutorial(collectableBuilding, _tutorialControllerConfig.CollectableBuildingDescription);

        ShowTutorial();
    }

    private void AddBuildingToTutorial(SelectableBuilding building, string description)
    {
        if (!building) return;
        _tutorialBuildings.Enqueue(building);
        _buildingDescriptions[building] = description;
    }

    public void ShowTutorial()
    {
        _selectionController.Deselect();

        if (_tutorialBuildings.Count == 0)
        {
            onTutorialEnd.Invoke();
            return;
        }

        var tutorialBuilding = _tutorialBuildings.Dequeue();
        if (!tutorialBuilding) return;

        var outline = tutorialBuilding.GetComponentInChildren<Outline>();
        if (outline)
        {
            outline.enabled = true;
        }

        _currentPopUp = _popUpFactory.CreateSpecialPopUp();

        Vector3 buildingWorldPosition = tutorialBuilding.transform.position;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(buildingWorldPosition);

        Vector2 localPosition = _canvas.transform.InverseTransformPoint(screenPosition);
        RectTransform popUpRect = _currentPopUp.GetComponent<RectTransform>();
        _currentPopUp.transform.localPosition = new Vector3(localPosition.x + popUpRect.rect.width * 0.75f, localPosition.y + popUpRect.rect.height * 0.75f, 0);

        var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
        string description = _buildingDescriptions.GetValueOrDefault(tutorialBuilding, "NO DESCRIPTION");

        popUpObject.ShowPopUp(tutorialBuilding.BuildingNameText, description, "Продолжить");
        popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
    }
}
