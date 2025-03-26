using UnityEngine;
using Zenject;

public class GameInitializer : MonoBehaviour
{
    private SelectionController _selectionController;
    private MainGameUIController _mainGameUIController;
    private MainGameController _mainGameController;
    private TimeController _timeController;
    private PeopleUnitsController _peopleUnitsController;
    private BuildingController _buildingController;
    private BlurController _blurController;
    private PopUpsController _popUpsController;
    private EventController _eventController;
    private EffectsController _effectsController;

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
        EventController eventController,
        TutorialController tutorialController,
        EffectsController effectsController
        )
    {
        _mainGameUIController = mainGameUIController;
        _selectionController = selectionController;
        _mainGameController = mainGameController;
        _timeController = timeController;
        _peopleUnitsController = peopleUnitsController;
        _buildingController = buildingController;
        _blurController = blurController;
        _popUpsController = popUpsController;
        _eventController = eventController;
        _effectsController = effectsController;
    }

    private void Start()
    {
        _blurController.Init();
        _mainGameUIController.Init();
        _selectionController.Init();
        _buildingController.Init();
        _mainGameController.Init();
        _timeController.Init();
        _peopleUnitsController.Init();
        _popUpsController.Init();
        _eventController.Init();
        _effectsController.Init();
    }
}
