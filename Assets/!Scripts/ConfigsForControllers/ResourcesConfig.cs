using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesConfig", menuName = "Configs/ResourcesConfig")]
public class ResourcesConfig : ScriptableObject
{
    [Header("RESOURCE CONFIG")]

    [SerializeField, Range(1, 10)] private int _provision = 10;
    [SerializeField, Range(1, 10)] private int _medicine = 10;
    [SerializeField, Range(1, 10)] private int _rawMaterials = 10;
    [SerializeField, Range(1, 10)] private int _readyMaterials = 10;
    [SerializeField, Range(1, 100)] private int _stability = 100;

    public int Provision => _provision;
    public int Medicine => _medicine;
    public int RawMaterials => _rawMaterials;
    public int ReadyMaterials => _readyMaterials;
    public int Stability => _stability;
}