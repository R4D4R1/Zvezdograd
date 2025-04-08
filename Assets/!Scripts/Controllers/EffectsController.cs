using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using UniRx;

public class EffectsController : MonoInit
{
    [FormerlySerializedAs("_snow")] [SerializeField] private GameObject snow;
    [FormerlySerializedAs("_ashes")] [SerializeField] private GameObject ashes;

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
            .Subscribe(_ => StartedSnowEvent())
            .AddTo(this);
        
        return UniTask.CompletedTask;
    }

    private void StartedSnowEvent()
    {
        snow.SetActive(true);
        ashes.SetActive(false);
    }
}
