using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private ControllersManager controllersManager;
    [SerializeField] private BlurConfig blurConfig;
    [SerializeField] private ResourcesConfig resourceConfig;

    //[SerializeField] private MainGameConfig mainGameConfig;
    //[SerializeField] private SelectionConfig selectionConfig;
    //[SerializeField] private MainGameUIConfig mainGameUIConfig;
    //[SerializeField] private TimeConfig timeConfig;
    //[SerializeField] private TutorialConfig tutorialConfig;
    //[SerializeField] private RadioConfig radioConfig;
    //[SerializeField] private BuildingConfig buildingConfig;
    //[SerializeField] private PeopleUnitsConfig peopleUnitsConfig;

    [SerializeField] private Transform _parentPopUpPrefab;
    [SerializeField] private Transform _parentNotificationsPrefab;
    [SerializeField] private GameObject _infoPopUpPrefab;
    [SerializeField] private GameObject _specialPopUpPrefab;
    [SerializeField] private GameObject _notificationPrefab;

    public override void InstallBindings()
    {
        if (controllersManager == null)
        {
            Debug.LogError("ControllersManager is not assigned in GameInstaller!");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("Main Camera is missing in the scene!");
            return;
        }

        // Регистрируем все конфиги отдельно
        Container.Bind<BlurConfig>().FromInstance(blurConfig).AsSingle();

        //Container.Bind<MainGameConfig>().FromInstance(mainGameConfig).AsSingle();
        //Container.Bind<SelectionConfig>().FromInstance(selectionConfig).AsSingle();
        //Container.Bind<MainGameUIConfig>().FromInstance(mainGameUIConfig).AsSingle();
        //Container.Bind<TimeConfig>().FromInstance(timeConfig).AsSingle();
        //Container.Bind<PeopleUnitsConfig>().FromInstance(peopleUnitsConfig).AsSingle();
        //Container.Bind<BuildingConfig>().FromInstance(buildingConfig).AsSingle();
        //Container.Bind<TutorialConfig>().FromInstance(tutorialConfig).AsSingle();

        // Регистрируем контроллеры
        Container.Bind<MainGameController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SelectionController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MainGameUIController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupEventController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TimeController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PeopleUnitsController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BuildingController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BlurController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TutorialController>().FromComponentInHierarchy().AsSingle();

        // Фабрики
        Container.Bind<PopUpFactory>()
            .AsSingle()
            .WithArguments(Container, _parentPopUpPrefab, _parentNotificationsPrefab, _infoPopUpPrefab, _specialPopUpPrefab, _notificationPrefab);

        // Менеджер контроллеров
        Container.Bind<ControllersManager>().FromInstance(controllersManager).AsSingle();

        // Оставшиеся зависимости
        Container.Bind<PopUpsController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();

        var resourceModel = new ResourceModel(
        resourceConfig.InitialProvision,
        resourceConfig.InitialMedicine,
        resourceConfig.InitialRawMaterials,
        resourceConfig.InitialReadyMaterials,
        resourceConfig.InitialStability
        );

        Container.Bind<ResourceModel>().FromInstance(resourceModel).AsSingle();
        Container.Bind<ResourceViewModel>().AsSingle();

        Debug.Log("GameInstaller: Bindings completed successfully.");
    }

}
