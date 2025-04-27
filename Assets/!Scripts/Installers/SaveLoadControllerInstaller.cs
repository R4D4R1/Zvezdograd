using UnityEngine;
using Zenject;

public class SaveLoadControllerInstaller : MonoBehaviour
{
    [Inject] private ResourceViewModel _resourceViewModel;
    [Inject] private TimeController _timeController;
    [Inject] private PeopleUnitsController _peopleUnitsController;
    [Inject] private PopUpsController _popUpsController;
    [Inject] private BuildingsController _buildingsController;

    private void Start()
    {
        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.InjectDependencies(_resourceViewModel, _timeController,
            _peopleUnitsController, _popUpsController, _buildingsController);
    }
}

