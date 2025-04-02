using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class GameInstaller : MonoInstaller
{
    // [Header("DEPENDENCIES")]
    // [SerializeField] private EventPopUp eventPopUp;

    [Header("CONFIGS")]
    [SerializeField] private BlurConfig blurConfig;
    [SerializeField] private ResourcesConfig resourcesConfig;
    [SerializeField] private TimeControllerConfig timeControllerConfig;
    [SerializeField] private TutorialControllerConfig tutorialControllerConfig;
    [SerializeField] private BuildingControllerConfig buildingControllerConfig;
    [SerializeField] private PeopleUnitsControllerConfig peopleUnitsControllerConfig;

    [Header("PARENTS PREFABS")]
    [SerializeField] private Transform parentPopUpPrefab;
    [SerializeField] private Transform parentNotificationsPrefab;

    [Header("POP UPS PREFABS")]
    [SerializeField] private GameObject infoPopUpPrefab;
    [SerializeField] private GameObject specialPopUpPrefab;
    [SerializeField] private GameObject notificationPrefab;

    public override void InstallBindings()
    {
        if (!Camera.main)
        {
            throw new System.Exception("Main Camera is missing in the scene!");
        }

        Container.Bind<BlurConfig>().FromInstance(blurConfig).AsSingle();
        Container.Bind<ResourcesConfig>().FromInstance(resourcesConfig).AsSingle();
        Container.Bind<TimeControllerConfig>().FromInstance(timeControllerConfig).AsSingle();
        Container.Bind<TutorialControllerConfig>().FromInstance(tutorialControllerConfig).AsSingle();
        Container.Bind<BuildingControllerConfig>().FromInstance(buildingControllerConfig).AsSingle();
        Container.Bind<PeopleUnitsControllerConfig>().FromInstance(peopleUnitsControllerConfig).AsSingle();

        Container.Bind<PopUpsController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MainGameController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BuildingSelectionController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MainGameUIController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EventController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TimeController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PeopleUnitsController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BuildingController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BlurController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TutorialController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EffectsController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<PopUpFactory>()
            .AsSingle()
            .WithArguments(Container, parentPopUpPrefab, parentNotificationsPrefab, infoPopUpPrefab, specialPopUpPrefab, notificationPrefab);


        Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();

        Container.Bind<ResourceModel>().AsSingle();
        Container.Bind<ResourceViewModel>().AsSingle();
    }
}
