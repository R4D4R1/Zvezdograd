using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;  // Для анимации перемещения

public class PeopleUnitsController : MonoBehaviour
{
    [SerializeField] private List<PeopleUnit> allUnits;  // Список всех юнитов
    private List<PeopleUnit> readyUnits = new();  // Список готовых юнитов
    private Vector3[] initialPositions;  // Массив для хранения начальных позиций

    // Возвращает количество готовых юнитов и выводит это значение в консоль
    public int GetReadyUnits()
    {
        return readyUnits.Count;
    }

    // Вызывается при инициализации объекта
    private void Awake()
    {
        // Получаем начальные позиции всех юнитов с RectTransform
        initialPositions = new Vector3[allUnits.Count];
        for (int i = 0; i < allUnits.Count; i++)
        {
            initialPositions[i] = allUnits[i].GetComponent<RectTransform>().position;  // Начальная позиция юнита
        }

        UpdateReadyUnits();
    }

    // Подписка на событие перехода к следующему ходу при старте объекта
    void Start()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurn;
    }

    // Отписка от события при отключении объекта
    private void OnDisable()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurn;
    }

    // Обновляет список готовых юнитов
    void UpdateReadyUnits()
    {
        readyUnits.Clear();
        foreach (var unit in allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Ready)
            {
                readyUnits.Add(unit);
            }
        }
    }

    // Проверяет, достаточно ли готовых юнитов для выполнения задания
    public bool AreUnitsReady(int units)
    {
        return units <= readyUnits.Count;
    }

    // Назначает юнитов на задание, если их достаточно
    public void AssignUnitsToTask(int requiredUnits, int restingTurns)
    {
        if (AreUnitsReady(requiredUnits))
        {
            int assignedUnits = 0;
            UpdateReadyUnits();

            foreach (var unit in readyUnits)
            {
                if (assignedUnits < requiredUnits)
                {
                    unit.SetBusy();
                    unit.DisableUnit();
                    unit.SetRestingTime(restingTurns);
                    assignedUnits++;
                }
            }
        }
        else
        {
            Debug.Log("Not enough units");
        }

        SortUnitsByRestingTime();
    }

    // Метод для сортировки юнитов по времени отдыха по возрастанию
    public void SortUnitsByRestingTime()
    {
        // Сортируем всех юнитов по времени отдыха
        readyUnits.Sort((unit1, unit2) => unit1.restingTime.CompareTo(unit2.restingTime));

        // Теперь распределяем юнитов по начальным позициям
        for (int i = 0; i < readyUnits.Count; i++)
        {
            PeopleUnit unit = readyUnits[i];

            // Перемещаем юнита на доступную позицию
            if (i < initialPositions.Length)
            {
                // Применяем анимацию перемещения на позицию
                RectTransform rectTransform = unit.GetComponent<RectTransform>();
                rectTransform.DOMove(initialPositions[i], 0.5f).SetEase(Ease.InOutSine);
            }
        }
    }

    // Метод, вызываемый при переходе к следующему ходу
    public void NextTurn()
    {
        UpdateRestingTimeForAllUnits();
        RestAllBusyUnits();
        SortUnitsByRestingTime();  // После обновления времени отдыха, сортируем юнитов
    }

    // Переводит все занятые юниты в состояние отдыха
    public void RestAllBusyUnits()
    {
        foreach (var unit in allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Busy)
            {
                unit.UnitResting();
            }
        }
    }

    // Обновляет оставшееся время отдыха для всех юнитов, находящихся в состоянии отдыха
    public void UpdateRestingTimeForAllUnits()
    {
        foreach (var unit in allUnits)
        {
            if (unit.GetCurrentState() == PeopleUnit.UnitState.Resting)
            {
                unit.UpdateRestingTime();
            }
        }
    }
}
