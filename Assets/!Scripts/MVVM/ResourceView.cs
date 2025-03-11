using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI provisionText;
    [SerializeField] private TextMeshProUGUI medicineText;
    [SerializeField] private TextMeshProUGUI rawMaterialsText;
    [SerializeField] private TextMeshProUGUI readyMaterialsText;
    [SerializeField] private TextMeshProUGUI stabilityText;
    [SerializeField] private Slider provisionSlider;
    [SerializeField] private Slider medicineSlider;
    [SerializeField] private Slider rawMaterialsSlider;
    [SerializeField] private Slider readyMaterialsSlider;
    [SerializeField] private Slider stabilitySlider;

    private ResourceViewModel _viewModel;
    private PopUpFactory _popUpFactory;

    [Inject]
    public void Construct(ResourceViewModel viewModel,PopUpFactory popUpFactory)
    {
        _viewModel = viewModel;
        _popUpFactory = popUpFactory;
        BindUI();
    }

    private void BindUI()
    {
        provisionSlider.maxValue = _viewModel.Model.MaxProvision;
        medicineSlider.maxValue = _viewModel.Model.MaxMedicine;
        rawMaterialsSlider.maxValue = _viewModel.Model.MaxRawMaterials;
        readyMaterialsSlider.maxValue = _viewModel.Model.MaxReadyMaterials;
        stabilitySlider.maxValue = _viewModel.Model.MaxStability;

        SubscribeToResource(_viewModel.Provision, provisionSlider, provisionText, "ПРОВИЗИЯ", _viewModel.Model.MaxProvision);
        SubscribeToResource(_viewModel.Medicine, medicineSlider, medicineText, "МЕДИКАМЕНТЫ", _viewModel.Model.MaxMedicine);
        SubscribeToResource(_viewModel.RawMaterials, rawMaterialsSlider, rawMaterialsText, "СЫРЬЕ", _viewModel.Model.MaxRawMaterials);
        SubscribeToResource(_viewModel.ReadyMaterials, readyMaterialsSlider, readyMaterialsText, "СТРОЙМАТЕРИАЛЫ", _viewModel.Model.MaxReadyMaterials);
        SubscribeToResource(_viewModel.Stability, stabilitySlider, stabilityText, "СТАБИЛЬНОСТЬ", _viewModel.Model.MaxStability);
    }

    private void SubscribeToResource(IReadOnlyReactiveProperty<int> resource, Slider slider, TextMeshProUGUI text, string name, int maxValue)
    {
        int previousValue = resource.Value; // Запоминаем начальное значение

        resource.Subscribe(newValue =>
        {
            slider.value = newValue;
            text.text = $"{name} {newValue}/{maxValue}";

            if (previousValue != newValue)
            {
                int difference = newValue - previousValue;
                string action = difference > 0 ? "Добавлено" : "Убавлено";
                string message = $"{action} {name.ToLower()} {Mathf.Abs(difference)}";

                _popUpFactory.CreateNotification(message, difference > 0);

                previousValue = newValue; // Обновляем предыдущее значение
            }
        }).AddTo(this);
    }


    public void ModifyResource(ResourceModel.ResourceType type, int value)
    {
        _viewModel.ModifyResource(type, value);
    }
}

