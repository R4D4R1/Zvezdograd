using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class TimeController : MonoBehaviour
{
    public Light morningLight;
    public Light dayLight;
    public Light eveningLight;

    public TMP_Text dayText;
    public TMP_Text periodText;

    public Image blackoutImage; // Черное изображение для затемнения экрана

    private enum TimeOfDay
    {
        Утро,
        День,
        Вечер
    }

    private DateTime startDate = new DateTime(1941, 8, 30);
    private DateTime endDate = new DateTime(1941, 9, 30);
    private DateTime currentDate;
    private TimeOfDay currentPeriod;

    private void Start()
    {
        currentDate = startDate;
        currentPeriod = TimeOfDay.Утро;
        UpdateLighting();
        UpdateText();
    }

    private void UpdateTime()
    {
        switch (currentPeriod)
        {
            case TimeOfDay.Утро:
                currentPeriod = TimeOfDay.День;
                break;
            case TimeOfDay.День:
                currentPeriod = TimeOfDay.Вечер;
                break;
            case TimeOfDay.Вечер:
                currentPeriod = TimeOfDay.Утро;
                currentDate = currentDate.AddDays(1);
                if (currentDate > endDate)
                {
                    currentDate = startDate;
                }
                break;
        }
        UpdateLighting();
        UpdateText();
    }

    private void UpdateLighting()
    {
        morningLight.enabled = (currentPeriod == TimeOfDay.Утро);
        dayLight.enabled = (currentPeriod == TimeOfDay.День);
        eveningLight.enabled = (currentPeriod == TimeOfDay.Вечер);
    }

    private void UpdateText()
    {
        dayText.text = currentDate.ToString("d MMMM");
        periodText.text = currentPeriod.ToString();
    }

    public void OnEndTurnButtonClicked()
    {
        // Затемнить экран перед сменой периода дня
        blackoutImage.DOFade(1, 1.0f).OnComplete(() =>
        {
            UpdateTime(); // Меняем период дня после затемнения
            blackoutImage.DOFade(0, 1.0f); // Убираем затемнение
        });
    }
}
