using UnityEngine;

[CreateAssetMenu(fileName = "BlurConfig", menuName = "Configs/BlurConfig")]
public class BlurConfig : ScriptableObject
{
    [Range(0.25f, 5f), SerializeField]
    private float _timeOfDepthAppearing = 0.25f;

    [Range(1, 200f), SerializeField]
    private float _blurValue = 100f;

    public float TimeOfDepthAppearing => _timeOfDepthAppearing;
    public float BlurValue => _blurValue;
}
