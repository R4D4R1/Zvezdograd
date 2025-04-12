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
    [SerializeField] private FactoryBuilding factoryBuilding;

    [SerializeField] private RectTransform clockUI;
    [SerializeField] private RectTransform provisionUI;
    [SerializeField] private RectTransform medsUI;
    [SerializeField] private RectTransform rawMaterialsUI;
    [SerializeField] private RectTransform readyMaterialsUI;
    [SerializeField] private RectTransform unitsUI;
    [SerializeField] private RectTransform stabilityUI;
    [SerializeField] private RectTransform nextTurnUI;

    [FormerlySerializedAs("_popUpParent")] [SerializeField] private Transform popUpParent;
    [FormerlySerializedAs("onTutorialEnd")] [SerializeField] public UnityEvent OnTutorialEnd;

    private GameObject _currentPopUp;
    private readonly Queue<SelectableBuilding> _tutorialBuildings = new();
    private readonly Queue<RectTransform> _uiTutorials = new();
    private readonly Dictionary<SelectableBuilding, string> _buildingDescriptions = new();
    private readonly Dictionary<RectTransform, string> _uiTutorialsLabels = new();
    private readonly Dictionary<RectTransform, string> _uiTutorialsDescriptions = new();

    private Canvas _canvas;
    private TutorialControllerConfig _tutorialControllerConfig;
    private BuildingSelectionController _buildingSelectionController;
    private BuildingsController _buildingsController;
    private PopUpFactory _popUpFactory;
    private Camera _mainCamera;
    private bool _isBuildingTutorialStarted;

    public readonly Subject<Unit> OnTutorialStarted = new();
    public readonly Subject<Unit> OnNewBuildingTutorialShow = new();
    public readonly Subject<Unit> OnBuildingTutorialStarted = new();

    [Inject]
    public void Construct(TutorialControllerConfig tutorialControllerConfig, BuildingsController buildingsController,
        PopUpFactory popUpFactory, Camera mainCamera)
    {
        _tutorialControllerConfig = tutorialControllerConfig;
        _buildingsController = buildingsController;
        _popUpFactory = popUpFactory;
        _mainCamera = mainCamera;
        _isBuildingTutorialStarted = false;
    }

    public async void StartTutorial()
    {
        await UniTask.Delay(500);
        OnTutorialStarted.OnNext(Unit.Default);

        _canvas = popUpParent.GetComponentInParent<Canvas>();

        // Add buildings to tutorial using config data
        AddBuildingToTutorial(_buildingsController.GetCityHallBuilding(), _tutorialControllerConfig.CityHallBuildingDescription);
        AddBuildingToTutorial(_buildingsController.GetHospitalBuilding(), _tutorialControllerConfig.HospitalBuildingDescription);
        AddBuildingToTutorial(_buildingsController.GetFoodTruckBuilding(), _tutorialControllerConfig.FoodTruckBuildingDescription);
        AddBuildingToTutorial(factoryBuilding, _tutorialControllerConfig.FactoryBuildingDescription);
        AddBuildingToTutorial(intactBuilding, _tutorialControllerConfig.IntactBuildingDescription);
        AddBuildingToTutorial(collectableBuilding, _tutorialControllerConfig.CollectableBuildingDescription);

        // Add UI elements to tutorial using config data
        AddUIToTutorial(clockUI, _tutorialControllerConfig.ClockLabel, _tutorialControllerConfig.ClockDescription);
        AddUIToTutorial(provisionUI, _tutorialControllerConfig.ProvisionLabel, _tutorialControllerConfig.ProvisionDescription);
        AddUIToTutorial(medsUI, _tutorialControllerConfig.MedsLabel, _tutorialControllerConfig.MedsDescription);
        AddUIToTutorial(rawMaterialsUI, _tutorialControllerConfig.RawMaterialsLabel, _tutorialControllerConfig.RawMaterialsDescription);
        AddUIToTutorial(readyMaterialsUI, _tutorialControllerConfig.ReadyMaterialsLabel, _tutorialControllerConfig.ReadyMaterialsDescription);
        AddUIToTutorial(unitsUI, _tutorialControllerConfig.UnitsLabel, _tutorialControllerConfig.UnitsDescription);
        AddUIToTutorial(stabilityUI, _tutorialControllerConfig.StabilityLabel, _tutorialControllerConfig.StabilityDescription);
        AddUIToTutorial(nextTurnUI, _tutorialControllerConfig.NextTurnLabel, _tutorialControllerConfig.NextTurnDescription);

        ShowTutorial();
    }

    private void AddBuildingToTutorial(SelectableBuilding building, string description)
    {
        if (!building) return;
        _tutorialBuildings.Enqueue(building);
        _buildingDescriptions[building] = description;
    }

    private void AddUIToTutorial(RectTransform rectTransform, string label, string description)
    {
        if (!rectTransform) return;
        _uiTutorials.Enqueue(rectTransform);
        _uiTutorialsLabels[rectTransform] = label;
        _uiTutorialsDescriptions[rectTransform] = description;
    }

    public void ShowTutorial()
    {
        if (_uiTutorials.Count > 0)
        {
            ShowUITutorial();
        }
        else if (_tutorialBuildings.Count > 0)
        {
            ShowBuildingTutorial();
        }
        else
        {
            OnTutorialEnd.Invoke();
        }
    }

    private void ShowUITutorial()
    {
        var UITutorial = _uiTutorials.Dequeue();
        _currentPopUp = _popUpFactory.CreateSpecialPopUp();
        _currentPopUp.transform.position = UITutorial.transform.position;

        var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
        var label = _uiTutorialsLabels.GetValueOrDefault(UITutorial, "NO LABEL!");
        var description = _uiTutorialsDescriptions.GetValueOrDefault(UITutorial, "NO DESCRIPTION!");

        popUpObject.ShowPopUp(label, description, "Продолжить");
        popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
    }

    private void ShowBuildingTutorial()
    {
        OnNewBuildingTutorialShow.OnNext(Unit.Default);

        if (!_isBuildingTutorialStarted)
        {
            OnBuildingTutorialStarted.OnNext(Unit.Default);
        }

        var tutorialBuilding = _tutorialBuildings.Dequeue();
        var outline = tutorialBuilding.GetComponentInChildren<Outline>();
        
        if (outline) outline.enabled = true;

        _currentPopUp = _popUpFactory.CreateSpecialPopUp();

        var buildingWorldPosition = tutorialBuilding.transform.position;
        var screenPosition = _mainCamera.WorldToScreenPoint(buildingWorldPosition);
        var localPosition = _canvas.transform.InverseTransformPoint(screenPosition);

        var popUpRect = _currentPopUp.GetComponent<RectTransform>();
        _currentPopUp.transform.localPosition = new Vector3(localPosition.x + popUpRect.rect.width * 0.75f,
            localPosition.y + popUpRect.rect.height * 0.75f, 0);

        var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
        var description = _buildingDescriptions.GetValueOrDefault(tutorialBuilding, "NO DESCRIPTION");

        popUpObject.ShowPopUp(tutorialBuilding.BuildingLabel, description, "Продолжить");
        popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
    }
}
