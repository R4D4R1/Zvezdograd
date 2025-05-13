using UnityEngine;
using Zenject;

public class SaveLoadControllerInstaller : MonoBehaviour
{
    private ResourceViewModel _resourceViewModel;
    private TimeController _timeController;
    private PeopleUnitsController _peopleUnitsController;
    private PopUpsController _popUpsController;
    private BuildingsController _buildingsController;
    private MainGameUIController _mainGameUIController;
    private EventController _eventController;
    private MainGameController _mainGameController;


    [Inject]
    public void Construct(
        ResourceViewModel resourceViewModel, TimeController timeController,
        PeopleUnitsController peopleUnitsController, PopUpsController popUpsController,
        BuildingsController buildingsController, MainGameUIController mainGameUIController,
        EventController eventController, MainGameController mainGameController)
    {
        _resourceViewModel = resourceViewModel;
        _timeController = timeController;
        _peopleUnitsController = peopleUnitsController;
        _popUpsController = popUpsController;
        _buildingsController = buildingsController;
        _mainGameUIController = mainGameUIController;
        _eventController = eventController;
        _mainGameController = mainGameController;
    }

    private void Start()
    {
        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.InjectMainGameDependencies(_resourceViewModel, _timeController,
            _peopleUnitsController, _popUpsController, _buildingsController, _mainGameUIController,
            _eventController, _mainGameController);
    }
}

