using UnityEngine;
using Zenject;

public class ControllersManager : MonoBehaviour
{
    public SelectionController SelectionController { get; private set; }
    public MainGameUIController MainGameUIController { get; private set; }
    public MainGameController MainGameController { get; private set; }
    public TimeController TimeController { get; private set; }
    public PeopleUnitsController PeopleUnitsController { get; private set; }
    public BombBuildingController BombBuildingController { get; private set; }
    private BlurController BlurController { get; set; }
    public PopUpsController PopUpsController { get; private set; }
    public PopupEventController PopupEventController { get; private set; }
    public TutorialController TutorialController { get; private set; }

    [Inject]
    public void Construct(
        SelectionController selectionController,
        MainGameUIController mainGameUIController,
        MainGameController mainGameController,
        TimeController timeController,
        PeopleUnitsController peopleUnitsController,
        BombBuildingController bombBuildingController,
        BlurController blurController,
        PopUpsController popUpsController,
        PopupEventController popupEventController,
        TutorialController tutorialController
        )
    {
        MainGameUIController = mainGameUIController;
        SelectionController = selectionController;
        MainGameController = mainGameController;
        TimeController = timeController;
        PeopleUnitsController = peopleUnitsController;
        BombBuildingController = bombBuildingController;
        BlurController = blurController;
        PopUpsController = popUpsController;
        PopupEventController = popupEventController;
        TutorialController = tutorialController;
    }

    private void Awake()
    {
        BlurController.Init();
        MainGameUIController.Init();
        SelectionController.Init();
        BombBuildingController.Init();
        MainGameController.Init();
        TimeController.Init();
        PeopleUnitsController.Init();
        PopUpsController.Init();
        PopupEventController.Init();
    }
}
