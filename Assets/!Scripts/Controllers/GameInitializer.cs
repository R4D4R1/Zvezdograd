using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

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
    private SnowEffects _effectsController;

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
        SnowEffects effectsController
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

    private void Start()
    {
        StartAsync().Forget();
    }

    private async UniTaskVoid StartAsync()
    {
        await _blurController.Init();
        await _mainGameUIController.Init();
        await _buildingsController.Init();
        await _popUpsController.Init();
        await _buildingSelectionController.Init();

        await _timeController.Init();
        await _peopleUnitsController.Init();
        await _eventController.Init();
        await _effectsController.Init();

        await _mainGameController.Init();
    }

    //private async UniTaskVoid Start()
    //{
    //    _blurController.Init();
    //    _mainGameUIController.Init();
    //    _buildingsController.Init();
    //    _popUpsController.Init();
    //    _buildingSelectionController.Init();
    //    _mainGameController.Init();
    //    _timeController.Init();
    //    _peopleUnitsController.Init();
    //    _eventController.Init();
    //    _effectsController.Init();
    //}
}
