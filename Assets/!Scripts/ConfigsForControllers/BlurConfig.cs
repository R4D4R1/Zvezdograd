using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BlurConfig", menuName = "Configs/BlurConfig")]
public class BlurConfig : ScriptableObject
{
    [Range(0.25f, 1f), SerializeField]
    private float timeOfDepthAppearing = 0.25f;

    [Range(1, 200f), SerializeField]
    private float blurValue = 100f;

    public float TimeOfDepthAppearing => timeOfDepthAppearing;
    public float BlurValue => blurValue;
}
