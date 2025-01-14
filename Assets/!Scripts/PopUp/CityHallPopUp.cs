using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class CityHallPopUp : InfoPopUp
{
    private CityHallBuilding _buildingToUse;
    [SerializeField] private TextMeshProUGUI _errorText;

    [SerializeField] private TextMeshProUGUI _relationWithGovermentText;

    [SerializeField] private TextMeshProUGUI _militaryTimerText;

    // ������ ������

    // ������ �������� ���������� � ������ - ������� ����
    // �� ��������� � � ���� - ����� 2 ���� / �� �������� ���� 2


    //�������� ��� ����������� �����������
    //�������� ��������������
    //�������� ��������


    private void Start()
    {
        ControllersManager.Instance.timeController.OnNextDayEvent += TimeController_OnNextDayEvent;
    }

    private void TimeController_OnNextDayEvent()
    {
        if(_buildingToUse.DayPassed())
        {
            UpdateRelationWithGovermentText();
            UpdateMilitaryTimerText();
        }
        else
        {
            //gameover in DayPassed()
        }
    }

    private void UpdateRelationWithGovermentText()
    {
        _relationWithGovermentText.text = "��������� - " + _buildingToUse._relationWithGoverment.ToString();
    }

    private void UpdateMilitaryTimerText()
    {
        _militaryTimerText.text = "������� ���� �������� ����. ������ - " + _buildingToUse._daysLeftToSendArmyMaterials.ToString() + "��.";
    }



    public void ShowCityHallPopUp(CityHallBuilding cityHallBuilding)
    {
        _buildingToUse = cityHallBuilding;

        UpdateMilitaryTimerText();
        UpdateRelationWithGovermentText();

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
        });
    }

    public override void HidePopUp()
    {
        _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration);

        ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
        ControllersManager.Instance.mainGameUIController.TurnOnUI();
        ControllersManager.Instance.blurController.UnBlurBackGroundSmoothly();

        _errorText.enabled = false;

        SetAlpha(0);
    }
}
