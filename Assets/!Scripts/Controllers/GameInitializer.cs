using UnityEngine;
using Zenject;

public class GameInitializer : MonoBehaviour
{
    private BuildingSelectionController _buildingSelectionController;
    private MainGameUIController _mainGameUIController;
    private MainGameController _mainGameController;
    private TimeController _timeController;
    private PeopleUnitsController _peopleUnitsController;
    private BuildingsController _buildingsController;
    private BlurController _blurController;
    private PopUpsController _popUpsController;
    private EventController _eventController;
    private EffectsController _effectsController;

    [Inject]
    public void Construct(
        BuildingSelectionController buildingSelectionController,
        MainGameUIController mainGameUIController,
        MainGameController mainGameController,
        TimeController timeController,
        PeopleUnitsController peopleUnitsController,
        BuildingsController buildingsController,
        BlurController blurController,
        PopUpsController popUpsController,
        EventController eventController,
        TutorialController tutorialController,
        EffectsController effectsController
        )
    {
        _mainGameUIController = mainGameUIController;
        _buildingSelectionController = buildingSelectionController;
        _mainGameController = mainGameController;
        _timeController = timeController;
        _peopleUnitsController = peopleUnitsController;
        _buildingsController = buildingsController;
        _blurController = blurController;
        _popUpsController = popUpsController;
        _eventController = eventController;
        _effectsController = effectsController;
    }

    // ReSharper disable once AsyncVoidMethod
    private void Start()
    {
        _blurController.Init();
        _mainGameUIController.Init();
        _buildingSelectionController.Init();
        _buildingsController.Init();
        
        _popUpsController.Init();

        _mainGameController.Init();
        _timeController.Init();
        _peopleUnitsController.Init();
        _eventController.Init();
        _effectsController.Init();
    }
}
