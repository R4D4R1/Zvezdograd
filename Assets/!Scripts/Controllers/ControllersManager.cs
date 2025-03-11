using UnityEngine;
using Zenject;

public class ControllersManager : MonoBehaviour
{
    public SelectionController SelectionController { get; private set; }
    public MainGameUIController MainGameUIController { get; private set; }
    public MainGameController MainGameController { get; private set; }
    public TimeController TimeController { get; private set; }
    public PeopleUnitsController PeopleUnitsController { get; private set; }
    public BuildingController BuildingController { get; private set; }
    public BlurController BlurController { get; private set; }
    public PopUpsController PopUpsController { get; private set; }
    public PopupEventController PopupEventController { get; private set; }
    public TutorialController TutorialController { get; private set; }
    public Camera MainCamera { get; private set; }

    [Inject]
    public void Construct(
        SelectionController selectionController,
        MainGameUIController mainGameUIController,
        MainGameController mainGameController,
        TimeController timeController,
        PeopleUnitsController peopleUnitsController,
        BuildingController buildingController,
        BlurController blurController,
        PopUpsController popUpsController,
        PopupEventController popupEventController,
        TutorialController tutorialController,
        Camera mainCamera)
    {
        SelectionController = selectionController;
        MainGameUIController = mainGameUIController;
        MainGameController = mainGameController;
        TimeController = timeController;
        PeopleUnitsController = peopleUnitsController;
        BuildingController = buildingController;
        BlurController = blurController;
        PopUpsController = popUpsController;
        PopupEventController = popupEventController;
        TutorialController = tutorialController;
        MainCamera = mainCamera;
    }
}
