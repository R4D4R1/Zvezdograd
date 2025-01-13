using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class InfoPopUp : MonoBehaviour
{

    [SerializeField] protected Image _bgImage;
    [SerializeField] protected float scaleDuration = 0.5f;
    [SerializeField] protected float fadeDuration = 0.5f;
    [SerializeField] protected float scaleDownDuration = 0.2f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
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

            SetAlpha(1);
        });
    }

    public virtual void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
        {
            SetAlpha(0);
            Destroy(gameObject);
        });
    }

    protected void SetAlpha(float alpha)
    {
        _canvasGroup.DOFade(alpha, fadeDuration);
    }
}