using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using UniRx;

public class SnowEffects : MonoInit
{
    [SerializeField] private GameObject snow;
    [SerializeField] private GameObject ashes;

    private EventController _eventController;

    [Inject]
    public void Construct(EventController eventController)
    {
        _eventController = eventController;
    }

    public override UniTask Init()
    {
        base.Init();
        _eventController.OnSnowStarted
            .Subscribe(_ => SetSnowState(true))
            .AddTo(this);

        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.OnSnowChangeState
            .Subscribe(SetSnowState)
            .AddTo(this);

        return UniTask.CompletedTask;
    }

    private void SetSnowState(bool state)
    {
        snow.SetActive(state);
        ashes.SetActive(!state);
    }
}
