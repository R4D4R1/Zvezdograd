using UnityEngine;
using Zenject;

public class EffectsController : MonoBehaviour
{
    [SerializeField] private GameObject _snow;
    [SerializeField] private GameObject _ashes;
    protected ControllersManager _controllersManager;

    [Inject]
    public void Construct(ControllersManager controllersManager)
    {
        _controllersManager = controllersManager;
    }

    private void OnEnable()
    {
        _controllersManager.PopupEventController.OnSnowStartedEvent += StartedSnowEvent;
    }

    private void OnDisable()
    {
        _controllersManager.PopupEventController.OnSnowStartedEvent -= StartedSnowEvent;
    }

    public void StartedSnowEvent()
    {
        _snow.SetActive(true);
        _ashes.SetActive(false);
    }
}
