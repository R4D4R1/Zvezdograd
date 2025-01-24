using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // ��������� ���������� ��� ��������

    [SerializeField] private TextMeshProUGUI ProvisionText;
    [SerializeField] private TextMeshProUGUI MedicineText;
    [SerializeField] private TextMeshProUGUI RawmaterialsText;
    [SerializeField] private TextMeshProUGUI ReadyMaterialsText;

    [Range(0f, 10f)]
    [SerializeField] private int provision = 0;

    [Range(0f, 10f)]
    [SerializeField] private int medicine = 0;

    [Range(0f, 10f)]
    [SerializeField] private int rawMaterials = 0;

    [Range(0f, 10f)]
    [SerializeField] private int readyMaterials = 0;

    // ��������� ���������� ��� ������������
    [Range(0f, 100f)]
    [SerializeField] private int stability = 100;

    // ����� ��� ������������ � ������������� ��������
    [SerializeField] private Color minColor = Color.red;    // ������� ����
    [SerializeField] private Color maxColor = Color.blue;   // ����� ����

    // ������ �� Image ������� �������� ������������
    [SerializeField] private Image stabilityFillImage;

    // �������� ��� �������� � ������������
    public Slider provisionSlider;
    public Slider medicineSlider;
    public Slider rawMaterialsSlider;
    public Slider ReadyMaterialsSlider;
    public Slider stabilitySlider;

    private int _maxProvision;
    private int _maxMedicine;
    private int _maxRawMaterials;
    private int _maxReadyMaterials;
    private int _maxStability;

    public bool IsStabilityZero { get;private set; }

    void Start()
    {
        // ������������� ��������� � �������� ��������
        InitializeSliders();

        _maxProvision = 10;
        _maxMedicine = 10;
        _maxReadyMaterials = 10;
        _maxRawMaterials = 10;
        _maxStability = 100;

        IsStabilityZero = false;

        UpdateMedicineSlider();
        UpdateProvisionSlider();
        UpdateRawMaterialsSlider();
        UpdateReadyMaterialsSlider();   
        UpdateStabilitySlider();

        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDestroy()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        // �� ������ ������� ��������� ������ ������������ ������ �� 2 � ������ �����
        foreach (var building in ControllersManager.Instance.buildingController.RegularBuildings)
        {
            if (building.CurrentState == RepairableBuilding.State.Damaged && !IsStabilityZero)
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

        ReadyMaterialsSlider.maxValue = 10;
        ReadyMaterialsSlider.minValue = 0;
        ReadyMaterialsSlider.value = readyMaterials;
        ReadyMaterialsSlider.interactable = false;

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
    public int GetReadyMaterials()
    {
        return readyMaterials;
    }
    public int GetStability()
    {
        return stability;
    }

    public int GetMaxProvision()
    {
        return _maxProvision;
    }
    public int GetMaxMedicine()
    {
        return _maxMedicine;
    }
    public int GetMaxRawMaterials()
    {
        return _maxRawMaterials;
    }
    public int GetMaxReadyMaterials()
    {
        return _maxReadyMaterials;
    }
    public int GetMaxStability()
    {
        return _maxStability;
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

    public void AddOrRemoveReadyMaterials(int value)
    {
        readyMaterials = Mathf.Clamp(readyMaterials + value, 0, 10);
        UpdateReadyMaterialsSlider();
    }

    public void AddOrRemoveStability(int value)
    {
        stability = Mathf.Clamp(stability + value, 0, 100);
        UpdateStabilitySlider();

        if(stability == 0)
        {
            IsStabilityZero = true;

            // GAME OVER
        }
    }

    // ������ ���������� �������� ���������

    private void UpdateProvisionSlider()
    {
        provisionSlider.value = provision;
        ProvisionText.text = "�������� " + provision;
    }

    private void UpdateMedicineSlider()
    {
        medicineSlider.value = medicine;
        MedicineText.text = "����������� " + medicine;
    }

    private void UpdateRawMaterialsSlider()
    {
        rawMaterialsSlider.value = rawMaterials;
        RawmaterialsText.text = "����� " + rawMaterials;
    }

    private void UpdateReadyMaterialsSlider()
    {
        ReadyMaterialsSlider.value = readyMaterials;
        ReadyMaterialsText.text = "�������������� " + readyMaterials;
    }

    private void UpdateStabilitySlider()
    {
        stabilitySlider.value = stability;
        UpdateStabilityFillColor(); // ���������� ����� �������
    }

    private void UpdateStabilityFillColor()
    {
        if (stabilityFillImage != null)
        {
            // ������������ ����� � ����������� �� �������� �������� ������������
            float t = stability / 100f; // ������������ �������� (�� 0 �� 1)
            stabilityFillImage.color = Color.Lerp(minColor, maxColor, t);
        }
    }
}