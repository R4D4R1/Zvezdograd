using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // ��������� ���������� ��� ��������
    [Range(0f, 10f)]
    [SerializeField] private int provision = 0;

    [Range(0f, 10f)]
    [SerializeField] private int medicine = 0;

    [Range(0f, 10f)]
    [SerializeField] private int rawMaterials = 0;

    [Range(0f, 10f)]
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
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDestroy()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        // �� ������ ������� ��������� ������ ������������ ������ �� 2 � ������ �����
        foreach (var building in ControllersManager.Instance.buildingBombingController.RegularBuildings)
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
        provisionSlider.maxValue = 10;
        provisionSlider.minValue = 0;
        provisionSlider.value = provision;
        provisionSlider.interactable = false;

        medicineSlider.maxValue = 10;
        medicineSlider.minValue = 0;
        medicineSlider.value = medicine;
        medicineSlider.interactable = false;

        rawMaterialsSlider.maxValue = 10;
        rawMaterialsSlider.minValue = 0;
        rawMaterialsSlider.value = rawMaterials;
        rawMaterialsSlider.interactable = false;

        buildingMaterialsSlider.maxValue = 10;
        buildingMaterialsSlider.minValue = 0;
        buildingMaterialsSlider.value = buildingMaterials;
        buildingMaterialsSlider.interactable = false;

        // ������� ��� ������������
        stabilitySlider.maxValue = 100;
        stabilitySlider.minValue = 0;
        stabilitySlider.value = stability;
        stabilitySlider.interactable = false;
    }

    public int GetProvision()
    {
        return provision;
    }
    public int GetMedicine()
    {
        return medicine;
    }
    public int GetRawMaterials()
    {
        return rawMaterials;
    }
    public int GetBuildingMaterials()
    {
        return buildingMaterials;
    }
    public int GetStability()
    {
        return stability;
    }

    // ������ ���������� �������� ��������

    public void AddOrRemoveProvision(int value)
    {
        provision = Mathf.Clamp(provision + value, 0, 10);
        UpdateProvisionSlider();
    }

    public void AddOrRemoveMedicine(int value)
    {
        medicine = Mathf.Clamp(medicine + value, 0, 10);
        UpdateMedicineSlider();
    }

    public void AddOrRemoveRawMaterials(int value)
    {
        rawMaterials = Mathf.Clamp(rawMaterials + value, 0, 10);
        UpdateRawMaterialsSlider();
    }

    public void AddOrRemoveBuildingMaterials(int value)
    {
        buildingMaterials = Mathf.Clamp(buildingMaterials + value, 0, 10);
        UpdateBuildingMaterialsSlider();
    }

    public void AddOrRemoveStability(int value)
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