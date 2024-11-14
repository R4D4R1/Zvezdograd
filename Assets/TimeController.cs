using System.Globalization;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _dayPeriod;

    private int _date;

    private void Start()
    {
        Calendar calendar = new Calendar();
    }
    private enum DayPeriod
    {
        Morning,
        MidDay,
        Evenening
    }

    public void ChangeDate()
    {
        
    }
}