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

    // ������������� ���������
    private void InitializeSliders()
    {
        // �������� ��� ��������
        provisionSlider.maxValue = 5;
        provisionSlider.minValue = 0;
        provisionSlider.value = provision;
        provisionSlider.onValueChanged.AddListener(UpdateProvision);

        medicineSlider.maxValue = 5;
        medicineSlider.minValue = 0;
        medicineSlider.value = medicine;
        medicineSlider.onValueChanged.AddListener(UpdateMedicine);

        rawMaterialsSlider.maxValue = 5;
        rawMaterialsSlider.minValue = 0;
        rawMaterialsSlider.value = rawMaterials;
        rawMaterialsSlider.onValueChanged.AddListener(UpdateRawMaterials);

        buildingMaterialsSlider.maxValue = 5;
        buildingMaterialsSlider.minValue = 0;
        buildingMaterialsSlider.value = buildingMaterials;
        buildingMaterialsSlider.onValueChanged.AddListener(UpdateBuildingMaterials);

        // ������� ��� ������������
        stabilitySlider.maxValue = 100;
        stabilitySlider.minValue = 0;
        stabilitySlider.value = stability;
        stabilitySlider.onValueChanged.AddListener(UpdateStability);
    }

    // ������ ���������� �������� ��������
    private void UpdateProvision(float value)
    {
        provision = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // ������������ �������� �� 0 �� 5
    }

    private void UpdateMedicine(float value)
    {
        medicine = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // ������������ �������� �� 0 �� 5
    }

    private void UpdateRawMaterials(float value)
    {
        rawMaterials = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // ������������ �������� �� 0 �� 5
    }

    private void UpdateBuildingMaterials(float value)
    {
        buildingMaterials = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // ������������ �������� �� 0 �� 5
    }

    // ����� ���������� ������������
    private void UpdateStability(float value)
    {
        stability = Mathf.Clamp(Mathf.RoundToInt(value), 0, 100); // ������������ �������� �� 0 �� 100
    }

    // ������ ��� ��������� �������� ��������
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

    // ����� ��� ��������� �������� ������������
    public int GetStability()
    {
        return stability;
    }
}
