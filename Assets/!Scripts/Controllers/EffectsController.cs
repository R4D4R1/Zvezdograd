using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using UniRx;

public class EffectsController : MonoInit
{
    [FormerlySerializedAs("_snow")] [SerializeField] private GameObject snow;
    [FormerlySerializedAs("_ashes")] [SerializeField] private GameObject ashes;

    private PopupEventController _popupEventController;

    [Inject]
    public void Construct(PopupEventController popupEventController)
    {
        _popupEventController = popupEventController;
    }

    public override void Init()
    {
        base.Init();
        _popupEventController.OnSnowStarted
            .Subscribe(_ => StartedSnowEvent())
            .AddTo(this);
    }

    private void StartedSnowEvent()
    {
        snow.SetActive(true);
        ashes.SetActive(false);
    }
}
