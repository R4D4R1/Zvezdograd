using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Zenject;

public class BlurController : MonoBehaviour
{
    private DepthOfField _depthOfField;
    private Volume _volumeProfile;

    protected BlurConfig _config;

    [Inject]
    public void Construct(BlurConfig config)
    {
        _config = config;
    }

    public void Init()
    {
        _volumeProfile = FindFirstObjectByType<Volume>();
        _volumeProfile.profile.TryGet(out _depthOfField);

        Debug.Log($"{name} - Initialized successfully");
    }

    public void BlurBackGroundSmoothly()
    {
        DOTween.To(() => _depthOfField.focalLength.value, x => _depthOfField.focalLength.value = x, _config.BlurValue, _config.TimeOfDepthAppearing);
    }

    public void UnBlurBackGroundSmoothly()
    {
        DOTween.To(() => _depthOfField.focalLength.value, x => _depthOfField.focalLength.value = x, 1, _config.TimeOfDepthAppearing);
    }

    public void BlurBackGroundNow()
    {
        _depthOfField.focalLength.value = _config.BlurValue;
    }

    public void UnBlurBackGroundNow()
    {
        _depthOfField.focalLength.value = 1;
    }
}
