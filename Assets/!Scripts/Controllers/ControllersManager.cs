using UnityEngine;

public class ControllersManager : MonoBehaviour
{
    public static ControllersManager Instance;

    public SelectionController selectionController { get; private set; }
    public MainGameUIController mainGameUIController { get; private set; }
    public MainGameController mainGameController { get; private set; }
    public TimeController timeController { get; private set; }
    public ResourceController resourceController { get; private set; }
    public PeopleUnitsController peopleUnitsController { get; private set; }
    public BuildingController buildingController { get; private set; }
    public BlurController blurController { get; private set; }
    public PopUpsController popUpsController { get; private set; }
    public PopupEventController popupEventController { get; private set; }
    public TutorialController tutorialController { get; private set; }
    public Camera MainCamera { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        peopleUnitsController = GetComponentInChildren<PeopleUnitsController>();
        selectionController = GetComponentInChildren<SelectionController>();
        mainGameUIController = GetComponentInChildren<MainGameUIController>();
        mainGameController = GetComponentInChildren<MainGameController>();
        timeController = GetComponentInChildren<TimeController>();
        resourceController = GetComponentInChildren<ResourceController>();
        buildingController = GetComponentInChildren<BuildingController>();
        blurController = GetComponentInChildren<BlurController>();
        popUpsController = GetComponentInChildren<PopUpsController>();
        tutorialController = GetComponentInChildren<TutorialController>();
        popupEventController = GetComponentInChildren<PopupEventController>();

        MainCamera = Camera.main;
    }
}
