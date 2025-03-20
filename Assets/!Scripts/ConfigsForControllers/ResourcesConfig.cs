using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ResourcesConfig", menuName = "Configs/ResourcesConfig")]
public class ResourcesConfig : ScriptableObject
{
    [Header("RESOURCE CONFIG")]

    [SerializeField, Range(1, 10)] private int _provision = 10;
    [SerializeField, Range(1, 10)] private int _medicine = 10;
    [SerializeField, Range(1, 10)] private int _rawMaterials = 10;
    [SerializeField, Range(1, 10)] private int _readyMaterials = 10;
    [SerializeField, Range(1, 100)] private int _stability = 100;

    [Header("MAX VALUES")]
    [SerializeField, Range(1, 10)] private int _maxProvision = 10;
    [SerializeField, Range(1, 10)] private int _maxMedicine = 10;
    [SerializeField, Range(1, 10)] private int _maxRawMaterials = 10;
    [SerializeField, Range(1, 10)] private int _maxReadyMaterials = 10;
    [SerializeField, Range(1, 100)] private int _maxStability = 100;

    [Header("STABILITY COLORS")]
    [SerializeField] private Color _lowStabilityColor;
    [SerializeField] private Color _mediumStabilityColor;
    [SerializeField] private Color _highStabilityColor;

    public int Provision => _provision;
    public int Medicine => _medicine;
    public int RawMaterials => _rawMaterials;
    public int ReadyMaterials => _readyMaterials;
    public int Stability => _stability;

    public int MaxProvision => _maxProvision;
    public int MaxMedicine => _maxMedicine;
    public int MaxRawMaterials => _maxRawMaterials;
    public int MaxReadyMaterials => _maxReadyMaterials;
    public int MaxStability => _maxStability;

    public Color GetStabilityColor(int stability)
    {
        if (stability <= 25) return _lowStabilityColor;
        if (stability <= 75) return _mediumStabilityColor;
        return _highStabilityColor;
    }
}
