using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class BlurController : MonoBehaviour
{
    public static BlurController Instance;

    private DepthOfField _depthOfField;

    [SerializeField] private Volume _volumeProfile;
    [SerializeField, Range(0.25f, 5f)] private float _timeOfDepthAppearing = 0.25f;
    [SerializeField, Range(1, 50f)] private float _blurValue = 20f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        _volumeProfile.profile.TryGet(out _depthOfField);
    }

    public void BlurBackGroundSmoothly()
    {
        DOTween.To(() => _depthOfField.focalLength.value, x => _depthOfField.focalLength.value = x, _blurValue, _timeOfDepthAppearing);
    }

    public void UnBlurBackGroundSmoothly()
    {
        DOTween.To(() => _depthOfField.focalLength.value, x => _depthOfField.focalLength.value = x, 1, _timeOfDepthAppearing);
    }

    public void BlurBackGroundNow()
    {
        _depthOfField.focalLength.value = _blurValue;
    }

    public void UnBlurBackGroundNow()
    {
        _depthOfField.focalLength.value = 1;
    }
}