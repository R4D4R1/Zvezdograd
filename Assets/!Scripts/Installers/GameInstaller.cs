using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("DEPENDENICES")]
    [SerializeField] private EventPopUp eventPopUp;
    [SerializeField] private ControllersManager controllersManager;

    [Header("CONFIGS")]
    [SerializeField] private BlurConfig blurConfig;
    [SerializeField] private ResourcesConfig resourceConfig;

    [Header("PARENTS PREFABS")]
    [SerializeField] private Transform _parentPopUpPrefab;
    [SerializeField] private Transform _parentNotificationsPrefab;

    [Header("POP UPS PREFABS")]
    [SerializeField] private GameObject _infoPopUpPrefab;
    [SerializeField] private GameObject _specialPopUpPrefab;
    [SerializeField] private GameObject _notificationPrefab;

    public override void InstallBindings()
    {
        if (controllersManager == null)
        {
            throw new System.Exception("ControllersManager is not assigned in GameInstaller!");
        }
        if (Camera.main == null)
        {
            throw new System.Exception("Main Camera is missing in the scene!");
        }

        // ������������ ��� ������� ��������
        Container.Bind<BlurConfig>().FromInstance(blurConfig).AsSingle();

        // ������������ �����������
        Container.Bind<MainGameController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SelectionController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MainGameUIController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopupEventController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TimeController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PeopleUnitsController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BombBuildingController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BlurController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TutorialController>().FromComponentInHierarchy().AsSingle();

        // �������
        Container.Bind<PopUpFactory>()
            .AsSingle()
            .WithArguments(Container, _parentPopUpPrefab, _parentNotificationsPrefab, _infoPopUpPrefab, _specialPopUpPrefab, _notificationPrefab);

        // Putting dependencies inside container
        Container.Bind<ControllersManager>().FromInstance(controllersManager).AsSingle();
        Container.Bind<EventPopUp>().FromInstance(eventPopUp).AsSingle();

        // ���������� �����������
        Container.Bind<PopUpsController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();

        Container.Bind<ResourceModel>().AsSingle();
        Container.BindInstance(resourceConfig).AsSingle();
        Container.Bind<ResourceViewModel>().AsSingle();
    }

}
