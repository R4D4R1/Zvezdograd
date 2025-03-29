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
    
    [SerializeField] private RectTransform clockUI;
    [SerializeField] private RectTransform provisionUI;
    [SerializeField] private RectTransform medsUI;
    [SerializeField] private RectTransform rawMaterialsUI;
    [SerializeField] private RectTransform readyMaterialsUI;
    [SerializeField] private RectTransform unitsUI;
    [SerializeField] private RectTransform stabilityUI;
    [SerializeField] private RectTransform nextTurnUI;

    [FormerlySerializedAs("_popUpParent")] [SerializeField] private Transform popUpParent;

    [FormerlySerializedAs("OnTutorialEnd")] [SerializeField] private UnityEvent onTutorialEnd;

    private GameObject _currentPopUp;
    private readonly Queue<SelectableBuilding> _tutorialBuildings = new();
    private readonly Queue<RectTransform> _uiTutorials = new();
    private readonly Dictionary<SelectableBuilding, string> _buildingDescriptions = new();
    private readonly Dictionary<RectTransform, string> _uiTutorialsLabels = new();
    private readonly Dictionary<RectTransform, string> _uiTutorialsDescriptions = new();

    private Canvas _canvas;

    private TutorialControllerConfig _tutorialControllerConfig;
    private BuildingSelectionController _buildingSelectionController;
    private BuildingController _buildingController;
    private PopUpFactory _popUpFactory;
    private Camera _mainCamera;

    public readonly Subject<Unit> OnTutorialStarted = new();

    [Inject]
    public void Construct(TutorialControllerConfig tutorialControllerConfig, BuildingSelectionController buildingSelectionController, BuildingController buildingController,
        PopUpFactory popUpFactory, Camera camera)
    {
        _tutorialControllerConfig = tutorialControllerConfig;
        _buildingSelectionController = buildingSelectionController;
        _buildingController = buildingController;
        _popUpFactory = popUpFactory;
        _mainCamera = camera;
    }

    public async void StartTutorial()
    {
        await UniTask.Delay(500);

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

        AddUIToTutorial(clockUI,_tutorialControllerConfig.ClockLabel,_tutorialControllerConfig.ClockDescription);
        AddUIToTutorial(provisionUI,_tutorialControllerConfig.ProvisionLabel,_tutorialControllerConfig.ProvisionDescription);
        AddUIToTutorial(medsUI,_tutorialControllerConfig.MedsLabel,_tutorialControllerConfig.MedsDescription);
        AddUIToTutorial(rawMaterialsUI,_tutorialControllerConfig.RawMaterialsLabel,_tutorialControllerConfig.RawMaterialsDescription);
        AddUIToTutorial(readyMaterialsUI,_tutorialControllerConfig.ReadyMaterialsLabel,_tutorialControllerConfig.ReadyMaterialsDescription);
        AddUIToTutorial(unitsUI,_tutorialControllerConfig.UnitsLabel,_tutorialControllerConfig.UnitsDescription);
        AddUIToTutorial(stabilityUI,_tutorialControllerConfig.StabilityLabel,_tutorialControllerConfig.StabilityDescription);
        AddUIToTutorial(nextTurnUI,_tutorialControllerConfig.NextTurnLabel,_tutorialControllerConfig.NextTurnDescription);
        
        ShowTutorial();
    }

    private void AddBuildingToTutorial(SelectableBuilding building, string description)
    {
        if (!building) return;
        _tutorialBuildings.Enqueue(building);
        _buildingDescriptions[building] = description;
    }
    
    private void AddUIToTutorial(RectTransform transform, string label, string description)
    {
        if (!transform) return;
        _uiTutorials.Enqueue(transform);
        _uiTutorialsLabels[transform] = label;
        _uiTutorialsDescriptions[transform] = description;
    }

    public void ShowTutorial()
    {
        _buildingSelectionController.Deselect();
        
        if (_uiTutorials.Count > 0)
        {
            var UITutorial = _uiTutorials.Dequeue();
                
            _currentPopUp = _popUpFactory.CreateSpecialPopUp();
            //_currentPopUp.transform.SetParent(UITutorial);
            _currentPopUp.transform.position = UITutorial.transform.position;
            
            var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
            var label = _uiTutorialsLabels.GetValueOrDefault(UITutorial, "NO LABEL!");
            var description = _uiTutorialsDescriptions.GetValueOrDefault(UITutorial, "NO DESCRIPTION!");
            
            popUpObject.ShowPopUp(label, description, "Продолжить");
            popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
        }
        else if (_uiTutorials.Count == 0 && _tutorialBuildings.Count > 0) 
        {
            var tutorialBuilding = _tutorialBuildings.Dequeue();

            var outline = tutorialBuilding.GetComponentInChildren<Outline>();
            
            if (outline)
            {
                outline.enabled = true;
            }

            _currentPopUp = _popUpFactory.CreateSpecialPopUp();

            var buildingWorldPosition = tutorialBuilding.transform.position;
            var screenPosition = _mainCamera.WorldToScreenPoint(buildingWorldPosition);

            var localPosition = _canvas.transform.InverseTransformPoint(screenPosition);
            var popUpRect = _currentPopUp.GetComponent<RectTransform>();
            _currentPopUp.transform.localPosition = new Vector3(localPosition.x + popUpRect.rect.width * 0.75f,
                localPosition.y + popUpRect.rect.height * 0.75f, 0);

            var popUpObject = _currentPopUp.GetComponent<SpecialPopUp>();
            var description = _buildingDescriptions.GetValueOrDefault(tutorialBuilding, "NO DESCRIPTION");

            popUpObject.ShowPopUp(tutorialBuilding.BuildingNameText, description, "Продолжить");
            popUpObject.CurrentFunc = SpecialPopUp.PopUpFuncs.OpenNextTutorialPopUp;
        }
        else if (_tutorialBuildings.Count == 0 && _uiTutorials.Count == 0) 
        {
            onTutorialEnd.Invoke();
        }
    }
}
