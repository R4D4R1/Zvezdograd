using TMPro; // Импортируем TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Импортируем DOTween

public class PeopleUnit : MonoBehaviour
{
    // Перечисление, описывающее возможные состояния юнита
    public enum UnitState
    {
        Ready,   // Готов к работе
        Busy,    // Занят
        Injured, // Травмирован
        Resting  // Отдыхает
    }

    [SerializeField] private UnitState currentState = UnitState.Ready; // Текущее состояние юнита, по умолчанию Ready
    [SerializeField] private TextMeshProUGUI _text; // Текстовое поле для отображения состояния
    [SerializeField] private TextMeshProUGUI _restingTimeText; // Текстовое поле для отображения времени отдыха
    [SerializeField] private Image _image; // Изображение юнита

    public int restingTime; // Переменная для хранения времени отдыха

    private void Awake()
    {
        // При инициализации скрываем текст времени отдыха и включаем юнита
        _restingTimeText.gameObject.SetActive(false);
        EnableUnit();
    }

    // Метод для включения юнита и изменения его цвета на белый
    public void EnableUnit()
    {
        Color whiteColor = Color.white;
        _text.DOColor(whiteColor, 0.5f); // Анимация изменения цвета текста на белый за 0.5 секунд
        _image.DOColor(whiteColor, 0.5f); // Анимация изменения цвета изображения на белый за 0.5 секунд

        // Если юнит травмирован или отдыхает, переводим его в состояние готовности
        if (currentState == UnitState.Injured || currentState == UnitState.Resting)
        {
            currentState = UnitState.Ready;
        }
    }

    // Метод для отключения юнита и изменения его цвета на серый
    public void DisableUnit()
    {
        Color grayColor = new Color(0.392f, 0.392f, 0.392f);
        _text.DOColor(grayColor, 0.5f); // Анимация изменения цвета текста на серый за 0.5 секунд
        _image.DOColor(grayColor, 0.5f); // Анимация изменения цвета изображения на серый за 0.5 секунд
    }

    // Метод для получения текущего состояния юнита
    public UnitState GetCurrentState()
    {
        return currentState;
    }

    // Метод для установки состояния юнита на занят
    public void SetBusy()
    {
        currentState = UnitState.Busy;
    }

    // Метод для установки состояния юнита на травмирован
    public void SetInjured()
    {
        currentState = UnitState.Injured;
    }

    // Метод для установки состояния юнита на готов
    public void SetReady()
    {
        currentState = UnitState.Ready;
    }

    // Метод для установки времени отдыха юнита
    public void SetRestingTime(int restingTurns)
    {
        restingTime = restingTurns;
    }

    // Метод для перевода юнита в состояние отдыха
    public void UnitResting()
    {
        if (currentState == UnitState.Busy)
        {
            currentState = UnitState.Resting;

            // Обновляем текст времени отдыха и показываем его
            UpdateRestingText();
            _restingTimeText.gameObject.SetActive(true);
        }
    }

    // Метод для обновления времени отдыха юнита
    public void UpdateRestingTime()
    {
        if (currentState == UnitState.Resting)
        {
            restingTime--; // Уменьшаем время отдыха

            // Обновляем текст времени отдыха
            UpdateRestingText();

            // Если время отдыха закончилось, переводим юнита в состояние готовности
            if (restingTime <= 0)
            {
                currentState = UnitState.Ready;
                restingTime = -1; // Сбрасываем время отдыха
                _restingTimeText.gameObject.SetActive(false); // Скрываем текст времени отдыха
                EnableUnit(); // Включаем юнита
            }
        }
    }

    // Метод для обновления текста времени отдыха
    private void UpdateRestingText()
    {
        _restingTimeText.text = restingTime.ToString();
    }
}
