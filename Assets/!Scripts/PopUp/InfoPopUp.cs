using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class InfoPopUp : MonoBehaviour
{

    [SerializeField] protected Image _bgImage;
    [SerializeField] protected float scaleDuration = 0.5f;
    [SerializeField] protected float fadeDuration = 0.5f;
    [SerializeField] protected float scaleDownDuration = 0.2f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetTextAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
        });
    }

    public void ShowPopUp(string Label, string Description)
    {
        LabelText.text = "";
        DescriptionText.text = "";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            LabelText.text = Label;
            DescriptionText.text = Description;

            LabelText.DOFade(1, fadeDuration);
            DescriptionText.DOFade(1, fadeDuration);
        });
    }

    public virtual void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
        {
            SetTextAlpha(0);
            Destroy(gameObject);
        });
    }

    protected virtual void SetTextAlpha(float alpha)
    {
        Color labelColor = LabelText.color;
        labelColor.a = alpha;
        LabelText.color = labelColor;

        Color descriptionColor = DescriptionText.color;
        descriptionColor.a = alpha;
        DescriptionText.color = descriptionColor;
    }
}