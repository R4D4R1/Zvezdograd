using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;

public class InfoPopUp : MonoBehaviour
{

    [SerializeField] protected Image _bgImage;
    [SerializeField] protected float scaleDuration = 0.5f;
    [SerializeField] protected float fadeDuration = 0.5f;
    [SerializeField] protected float scaleDownDuration = 0.2f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup _canvasGroup;

    public bool IsActive { get; protected set; } = false;

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        IsActive = true;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
        });
    }

    public void ShowPopUp(string Label, string Description)
    {
        IsActive = true;

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
        if (IsActive)
        {
            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(async () =>
            {
                IsActive = false;
                await UniTask.Delay(1000);
                Destroy(gameObject);
            });
        }
    }


    protected void SetAlpha(float alpha)
    {
        _canvasGroup.DOFade(alpha, fadeDuration);
    }
}