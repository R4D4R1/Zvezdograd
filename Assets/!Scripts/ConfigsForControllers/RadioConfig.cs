using UnityEngine;

[CreateAssetMenu(fileName = "RadioConfig", menuName = "Configs/RadioConfig")]
public class RadioConfig : ScriptableObject
{
    [SerializeField] private float _volume = 1f;
    [SerializeField] private bool _isLooping = false;

    public float Volume => _volume;
    public bool IsLooping => _isLooping;
}
