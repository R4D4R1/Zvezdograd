using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PopUp : MonoBehaviour
{
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;

    [SerializeField] private Image _bgImage;
    [SerializeField] private float scaleDuration = 0.5f; // Длительность анимации скейла при открытии
    [SerializeField] private float fadeDuration = 0.5f;  // Длительность анимации прозрачности текста при открытии
    [SerializeField] private float scaleDownDuration = 0.2f; // Длительность анимации скейла при закрытии

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetTextAlpha(0);
        ShowPopUp();
    }

    public void ShowPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
        });
    }

    public void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
        {
            Destroy(gameObject); // Удаление поп-апа после завершения анимации
        });
    }

    private void SetTextAlpha(float alpha)
    {
        Color labelColor = LabelText.color;
        labelColor.a = alpha;
        LabelText.color = labelColor;

        Color descriptionColor = DescriptionText.color;
        descriptionColor.a = alpha;
        DescriptionText.color = descriptionColor;
    }
}
