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
    public BuildingBombingController buildingBombingController { get; private set; }
    public BlurController blurController { get; private set; }
    public SoundController SoundController { get; private set; }
    public Camera MainCamera { get; private set; } // Добавлено поле для кэширования основной камеры

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
        buildingBombingController = GetComponentInChildren<BuildingBombingController>();
        blurController = GetComponentInChildren<BlurController>();
        SoundController = GetComponentInChildren<SoundController>();

        MainCamera = Camera.main;
    }
}
