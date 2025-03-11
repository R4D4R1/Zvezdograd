using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesConfig", menuName = "Configs/ResourcesConfig")]
public class ResourcesConfig : ScriptableObject
{
    public int InitialProvision = 10;
    public int InitialMedicine = 10;
    public int InitialRawMaterials = 10;
    public int InitialReadyMaterials = 10;
    public int InitialStability = 100;
}