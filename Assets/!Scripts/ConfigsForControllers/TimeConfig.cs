using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "TimeConfig", menuName = "Configs/TimeConfig")]
public class TimeConfig : ScriptableObject
{
    [SerializeField] private Light _morningLight;
    [SerializeField] private Light _dayLight;
    [SerializeField] private Light _eveningLight;

    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private TMP_Text _periodText;

    [SerializeField] private Image _blackoutImage;

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingRegularBuildings = 3;

    [Range(1f, 5f)]
    [SerializeField] private int _daysBetweenBombingSpecialBuildings = 3;

    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private MonoBehaviour[] _btnScripts;

    [SerializeField] private float _fadeDuration = 0.5f;

    public Light MorningLight => _morningLight;
    public Light DayLight => _dayLight;
    public Light EveningLight => _eveningLight;

    public TMP_Text DayText => _dayText;
    public TMP_Text PeriodText => _periodText;

    public Image BlackoutImage => _blackoutImage;

    public int DaysBetweenBombingRegularBuildings => _daysBetweenBombingRegularBuildings;
    public int DaysBetweenBombingSpecialBuildings => _daysBetweenBombingSpecialBuildings;

    public Button NextTurnBtn => _nextTurnBtn;
    public MonoBehaviour[] BtnScripts => _btnScripts;

    public float FadeDuration => _fadeDuration;

}
