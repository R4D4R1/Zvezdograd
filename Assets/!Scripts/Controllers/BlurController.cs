using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;

public class BlurController : MonoBehaviour
{
    private DepthOfField _depthOfField;
    private Volume _volumeProfile;

    protected ControllersManager _controllersManager;
    protected BlurConfig _config;

    [Inject]
    public void Construct(ControllersManager controllersManager, BlurConfig config)
    {
        _controllersManager = controllersManager;
        _config = config;
    }

    public void Init()
    {
        _volumeProfile = FindFirstObjectByType<Volume>();
        _volumeProfile.profile.TryGet(out _depthOfField);

        _controllersManager.MainGameUIController.OnUITurnOn
            .Subscribe(_ => UnBlurBackGroundSmoothly())
            .AddTo(this);

        _controllersManager.MainGameUIController.OnUITurnOff
            .Subscribe(_ => BlurBackGroundSmoothly())
            .AddTo(this);

        _controllersManager.MainGameController.OnGameStarted
            .Subscribe(_ => BlurBackGroundNow())
            .AddTo(this);

        _controllersManager.TutorialController.OnTutorialStarted
            .Subscribe(_ => UnBlurBackGroundSmoothly())
            .AddTo(this);

        Debug.Log($"{name} - Initialized successfully");
    }

    private void BlurBackGroundSmoothly()
    {
        DOTween.To(() => _depthOfField.focalLength.value,
            x => _depthOfField.focalLength.value = x,
            _config.BlurValue, _config.TimeOfDepthAppearing);
    }

    private void UnBlurBackGroundSmoothly()
    {
        DOTween.To(() => _depthOfField.focalLength.value,
            x => _depthOfField.focalLength.value = x,
            1, _config.TimeOfDepthAppearing);
    }

    private void BlurBackGroundNow()
    {
        _depthOfField.focalLength.value = _config.BlurValue;
    }

    private void UnBlurBackGroundNow()
    {
        _depthOfField.focalLength.value = 1;
    }
}
