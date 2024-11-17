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

    public Image blackoutImage; // ������ ����������� ��� ���������� ������

    private enum TimeOfDay
    {
        ����,
        ����,
        �����
    }

    private DateTime startDate = new DateTime(1941, 8, 30);
    private DateTime endDate = new DateTime(1941, 9, 30);
    private DateTime currentDate;
    private TimeOfDay currentPeriod;

    private void Start()
    {
        currentDate = startDate;
        currentPeriod = TimeOfDay.����;
        UpdateLighting();
        UpdateText();
    }

    private void UpdateTime()
    {
        switch (currentPeriod)
        {
            case TimeOfDay.����:
                currentPeriod = TimeOfDay.����;
                break;
            case TimeOfDay.����:
                currentPeriod = TimeOfDay.�����;
                break;
            case TimeOfDay.�����:
                currentPeriod = TimeOfDay.����;
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
        morningLight.enabled = (currentPeriod == TimeOfDay.����);
        dayLight.enabled = (currentPeriod == TimeOfDay.����);
        eveningLight.enabled = (currentPeriod == TimeOfDay.�����);
    }

    private void UpdateText()
    {
        dayText.text = currentDate.ToString("d MMMM");
        periodText.text = currentPeriod.ToString();
    }

    public void OnEndTurnButtonClicked()
    {
        // ��������� ����� ����� ������ ������� ���
        blackoutImage.DOFade(1, 1.0f).OnComplete(() =>
        {
            UpdateTime(); // ������ ������ ��� ����� ����������
            blackoutImage.DOFade(0, 1.0f); // ������� ����������
        });
    }
}
