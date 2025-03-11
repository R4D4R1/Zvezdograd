using UnityEngine;

[CreateAssetMenu(fileName = "SelectionConfig", menuName = "Configs/SelectionConfig")]
public class SelectionConfig : ScriptableObject
{
    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;
    [Range(0f, 1f)] public float outlineWidth = 0.5f;

    [Header("PopUp Prefabs")]
    public GameObject popUpPrefab;
    public GameObject specialPopUpPrefab;

    [Header("PopUp Parent")]
    public Transform popUpParent;
}
