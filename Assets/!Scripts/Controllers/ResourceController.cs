using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // ��������� ���������� ��� ��������
    [Range(0f, 5f)]
    [SerializeField] private int provision = 0;

    [Range(0f, 5f)]
    [SerializeField] private int medicine = 0;

    [Range(0f, 5f)]
    [SerializeField] private int rawMaterials = 0;

    [Range(0f, 5f)]
    [SerializeField] private int buildingMaterials = 0;

    // ��������� ���������� ��� ������������
    [Range(0f, 100f)]
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
        TimeController.Instance.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDestroy()
    {
        TimeController.Instance.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        // �� ������ ������� ��������� ������ ������������ ������ �� 2 � ������ �����
        foreach (var building in BuildingBombingController.Instance.RegularBuildings)
        {
            if (building.CurrentState == RepairableBuilding.State.Damaged)
            {
                AddOrRemoveStability(-2);
            }
        }
    }

    // ������������� ���������
    private void InitializeSliders()
    {
        // �������� ��� ��������
        provisionSlider.maxValue = 5;
        provisionSlider.minValue = 0;
        provisionSlider.value = provision;
        provisionSlider.interactable = false;

        medicineSlider.maxValue = 5;
        medicineSlider.minValue = 0;
        medicineSlider.value = medicine;
        medicineSlider.interactable = false;

        rawMaterialsSlider.maxValue = 5;
        rawMaterialsSlider.minValue = 0;
        rawMaterialsSlider.value = rawMaterials;
        rawMaterialsSlider.interactable = false;

        buildingMaterialsSlider.maxValue = 5;
        buildingMaterialsSlider.minValue = 0;
        buildingMaterialsSlider.value = buildingMaterials;
        buildingMaterialsSlider.interactable = false;

        // ������� ��� ������������
        stabilitySlider.maxValue = 100;
        stabilitySlider.minValue = 0;
        stabilitySlider.value = stability;
        stabilitySlider.interactable = false;
    }

    // ������ ���������� �������� ��������

    private void AddOrRemoveProvision(int value)
    {
        provision = Mathf.Clamp(provision + value, 0, 5);
        UpdateProvisionSlider();
    }

    private void AddOrRemoveMedicine(int value)
    {
        medicine = Mathf.Clamp(medicine + value, 0, 5);
        UpdateMedicineSlider();
    }

    private void AddOrRemoveRawMaterials(int value)
    {
        rawMaterials = Mathf.Clamp(rawMaterials + value, 0, 5);
        UpdateRawMaterialsSlider();
    }

    private void AddOrRemoveBuildingMaterials(int value)
    {
        buildingMaterials = Mathf.Clamp(buildingMaterials + value, 0, 5);
        UpdateBuildingMaterialsSlider();
    }

    private void AddOrRemoveStability(int value)
    {
        stability = Mathf.Clamp(stability + value, 0, 100);
        UpdateStabilitySlider();
    }

    // ������ ���������� �������� ���������

    private void UpdateProvisionSlider()
    {
        provisionSlider.value = provision;
    }

    private void UpdateMedicineSlider()
    {
        medicineSlider.value = medicine;
    }

    private void UpdateRawMaterialsSlider()
    {
        rawMaterialsSlider.value = rawMaterials;
    }

    private void UpdateBuildingMaterialsSlider()
    {
        buildingMaterialsSlider.value = buildingMaterials;
    }

    private void UpdateStabilitySlider()
    {
        stabilitySlider.value = stability;
    }
}