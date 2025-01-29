using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI provisionText;
    [SerializeField] private TextMeshProUGUI medicineText;
    [SerializeField] private TextMeshProUGUI rawMaterialsText;
    [SerializeField] private TextMeshProUGUI readyMaterialsText;
    [SerializeField] private TextMeshProUGUI stabilityText;

    [SerializeField, Range(0, 10)] private int provision = 0;
    [SerializeField, Range(0, 10)] private int medicine = 0;
    [SerializeField, Range(0, 10)] private int rawMaterials = 0;
    [SerializeField, Range(0, 10)] private int readyMaterials = 0;
    [SerializeField, Range(0, 100)] private int stability = 100;

    [SerializeField, Range(0, 10)] private int maxProvision = 10;
    [SerializeField, Range(0, 10)] private int maxMedicine = 10;
    [SerializeField, Range(0, 10)] private int maxRawMaterials = 10;
    [SerializeField, Range(0, 10)] private int maxReadyMaterials = 10;
    [SerializeField, Range(0, 100)] private int maxStability = 100;

    [SerializeField] private Color minColor = Color.red;
    [SerializeField] private Color maxColor = Color.blue;

    [SerializeField] private Image stabilityFillImage;

    [SerializeField] private Slider provisionSlider;
    [SerializeField] private Slider medicineSlider;
    [SerializeField] private Slider rawMaterialsSlider;
    [SerializeField] private Slider readyMaterialsSlider;
    [SerializeField] private Slider stabilitySlider;

    public bool IsStabilityZero { get; private set; }

    void Start()
    {
        InitializeSliders();
        UpdateAllUI();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDestroy()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        foreach (var building in ControllersManager.Instance.buildingController.RegularBuildings)
        {
            if (building.CurrentState == RepairableBuilding.State.Damaged && !IsStabilityZero)
            {
                AddOrRemoveStability(-2);
            }
        }
    }

    private void InitializeSliders()
    {
        SetupSlider(provisionSlider, maxProvision);
        SetupSlider(medicineSlider, maxMedicine);
        SetupSlider(rawMaterialsSlider, maxRawMaterials);
        SetupSlider(readyMaterialsSlider, maxReadyMaterials);
        SetupSlider(stabilitySlider, maxStability);
    }

    private void SetupSlider(Slider slider, int maxValue)
    {
        slider.maxValue = maxValue;
        slider.minValue = 0;
        slider.interactable = false;
    }

    private void UpdateAllUI()
    {
        UpdateSliderUI(provisionSlider, provision, maxProvision, provisionText, "ÏÐÎÂÈÇÈß");
        UpdateSliderUI(medicineSlider, medicine, maxMedicine, medicineText, "ÌÅÄÈÊÀÌÅÍÒÛ");
        UpdateSliderUI(rawMaterialsSlider, rawMaterials, maxRawMaterials, rawMaterialsText, "ÑÛÐÜÅ");
        UpdateSliderUI(readyMaterialsSlider, readyMaterials, maxReadyMaterials, readyMaterialsText, "ÑÒÐÎÉÌÀÒÅÐÈÀËÛ");
        UpdateStabilityUI();
    }

    private void UpdateSliderUI(Slider slider, int value, int maxValue, TextMeshProUGUI text, string label)
    {
        slider.value = value;
        text.text = $"{label} {value}/{maxValue}";
    }

    private void UpdateStabilityUI()
    {
        stabilitySlider.value = stability;
        stabilityText.text = $"ÑÒÀÁÈËÜÍÎÑÒÜ {stability}/{maxStability}";
        UpdateStabilityFillColor();
    }

    private void UpdateStabilityFillColor()
    {
        if (stabilityFillImage != null)
        {
            float t = stability / (float)maxStability;
            stabilityFillImage.color = Color.Lerp(minColor, maxColor, t);
        }
    }

    public void AddOrRemoveProvision(int value) => ModifyResource(ref provision, value, maxProvision, provisionSlider, provisionText, "ÏÐÎÂÈÇÈß");
    public void AddOrRemoveMedicine(int value) => ModifyResource(ref medicine, value, maxMedicine, medicineSlider, medicineText, "ÌÅÄÈÊÀÌÅÍÒÛ");
    public void AddOrRemoveRawMaterials(int value) => ModifyResource(ref rawMaterials, value, maxRawMaterials, rawMaterialsSlider, rawMaterialsText, "ÑÛÐÜÅ");
    public void AddOrRemoveReadyMaterials(int value) => ModifyResource(ref readyMaterials, value, maxReadyMaterials, readyMaterialsSlider, readyMaterialsText, "ÑÒÐÎÉÌÀÒÅÐÈÀËÛ");

    public void AddOrRemoveStability(int value)
    {
        stability = Mathf.Clamp(stability + value, 0, maxStability);
        UpdateStabilityUI();
        if (stability == 0) IsStabilityZero = true;
    }

    private void ModifyResource(ref int resource, int value, int maxValue, Slider slider, TextMeshProUGUI text, string label)
    {
        resource = Mathf.Clamp(resource + value, 0, maxValue);
        UpdateSliderUI(slider, resource, maxValue, text, label);
    }

    public int GetProvision() => provision;
    public int GetMedicine() => medicine;
    public int GetRawMaterials() => rawMaterials;
    public int GetReadyMaterials() => readyMaterials;
    public int GetStability() => stability;

    public int GetMaxProvision() => maxProvision;
    public int GetMaxMedicine() => maxMedicine;
    public int GetMaxRawMaterials() => maxRawMaterials;
    public int GetMaxReadyMaterials() => maxReadyMaterials;
    public int GetMaxStability() => maxStability;
}
