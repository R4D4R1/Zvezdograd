using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Zenject;
using UniRx;

public class BlurController : MonoInit
{
    private DepthOfField _depthOfField;
    private Volume _volumeProfile;

    private MainGameUIController _mainGameUIController;
    private MainGameController _mainGameController;
    private TutorialController _tutorialController;
    private BlurConfig _config;

    [Inject]
    public void Construct(MainGameUIController mainGameUIController,
        MainGameController mainGameController,TutorialController tutorialController, BlurConfig config)
    {
        _mainGameUIController = mainGameUIController;
        _mainGameController = mainGameController;
        _tutorialController = tutorialController;
        _config = config;
    }

    public override void Init()
    {
        base.Init();
        _volumeProfile = FindFirstObjectByType<Volume>();
        _volumeProfile.profile.TryGet(out _depthOfField);

        _mainGameUIController.OnUITurnOn
            .Subscribe(_ => UnBlurBackGroundSmoothly())
            .AddTo(this);

        _mainGameUIController.OnUITurnOff
            .Subscribe(_ => BlurBackGroundSmoothly())
            .AddTo(this);

        _mainGameController.OnGameStarted
            .Subscribe(_ => BlurBackGroundNow())
            .AddTo(this);

        _tutorialController.OnTutorialStarted
            .Subscribe(_ => UnBlurBackGroundSmoothly())
            .AddTo(this);
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
