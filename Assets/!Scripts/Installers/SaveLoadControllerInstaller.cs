using UnityEngine;
using Zenject;

public class SaveLoadControllerInstaller : MonoBehaviour
{
    [Inject] private ResourceViewModel _resourceViewModel;
    [Inject] private TimeController _timeController;
    [Inject] private PeopleUnitsController _peopleUnitsController;
    [Inject] private PopUpsController _popUpsController;
    [Inject] private BuildingsController _buildingsController;
    [Inject] private MainGameUIController _mainGameUIController;

    private void Start()
    {
        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.InjectMainGameDependencies(_resourceViewModel, _timeController,
            _peopleUnitsController, _popUpsController, _buildingsController, _mainGameUIController);
    }
}

