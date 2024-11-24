using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // ��������� ���������� ��� ��������
    [SerializeField] private int provision = 0;
    [SerializeField] private int medicine = 0;
    [SerializeField] private int rawMaterials = 0;
    [SerializeField] private int buildingMaterials = 0;

    // ��������� ���������� ��� ������������
    [SerializeField] private int stability = 100;

    // �������� ��� �������� � ������������
    public Slider provisionSlider;
    public Slider medicineSlider;
    public Slider rawMaterialsSlider;
    public Slider buildingMaterialsSlider;
    public Slider stabilitySlider;

    void Start()
    {
        // ������������� ��������� � �������� ��������
        InitializeSliders();
    }

    private void OnEnable()
    {
        TimeController.Instance.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDisable()
    {
        TimeController.Instance.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        int regularBuildings = BuildingBombingController.Instance.RegularBuildings.Count;
        for (int i = 0; i < regularBuildings; i++)
        {
           // �������� ���������� �����������
        }
    }

    // ������������� ���������
    private void InitializeSliders()
    {
        // �������� ��� ��������
        provisionSlider.maxValue = 5;
        provisionSlider.minValue = 0;
        provisionSlider.value = provision;
        provisionSlider.onValueChanged.AddListener(AddOrRemoveProvision);

        medicineSlider.maxValue = 5;
        medicineSlider.minValue = 0;
        medicineSlider.value = medicine;
        medicineSlider.onValueChanged.AddListener(AddOrRemoveMedicine);

        rawMaterialsSlider.maxValue = 5;
        rawMaterialsSlider.minValue = 0;
        rawMaterialsSlider.value = rawMaterials;
        rawMaterialsSlider.onValueChanged.AddListener(AddOrRemoveRawMaterials);

        buildingMaterialsSlider.maxValue = 5;
        buildingMaterialsSlider.minValue = 0;
        buildingMaterialsSlider.value = buildingMaterials;
        buildingMaterialsSlider.onValueChanged.AddListener(AddOrRemoveBuildingMaterials);

        // ������� ��� ������������
        stabilitySlider.maxValue = 100;
        stabilitySlider.minValue = 0;
        stabilitySlider.value = stability;
        stabilitySlider.onValueChanged.AddListener(AddOrRemoveStability);
    }

    // ������ ���������� �������� ��������

    private void AddOrRemoveProvision(float value)
    {
        Mathf.Clamp(provision += Mathf.RoundToInt(value), 0, 5);
    }

    private void AddOrRemoveMedicine(float value)
    {
        Mathf.Clamp(medicine += Mathf.RoundToInt(value), 0, 5);
    }

    private void AddOrRemoveRawMaterials(float value)
    {
        Mathf.Clamp(rawMaterials += Mathf.RoundToInt(value), 0, 5);
    }

    private void AddOrRemoveBuildingMaterials(float value)
    {
        Mathf.Clamp(buildingMaterials += Mathf.RoundToInt(value), 0, 5);
    }

    private void AddOrRemoveStability(float value)
    {
        Mathf.Clamp(stability += Mathf.RoundToInt(value), 0, 100);
    }
}
