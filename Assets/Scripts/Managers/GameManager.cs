using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentBuildingName;
    public Waypoint currentDestinationWaypoint;
    public string currentLocation;

    [SerializeField] private BuildingInterior[] allBuildings;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI moneyLabel;

    [SerializeField] private int currentHour = 7;
    [SerializeField] private int currentMinute = 0;
    [SerializeField] private int currentDay = 2;

    [SerializeField] private int currentMoney = 2000;

    private float timeAccumulator;
    private const float REAL_SECONDS_PER_GAME_MINUTE = 1f;
    private int minutesToAdd;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLocation = "house";
        UpdateTimeDisplay();
        UpdateMoneyDisplay();

        foreach (var building in allBuildings)
        {
            if (building.BuildingName == currentLocation)
                building.Show();
            else
                building.Hide();
        }
    }

    private void Update()
    {
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator >= REAL_SECONDS_PER_GAME_MINUTE)
        {
            timeAccumulator -= REAL_SECONDS_PER_GAME_MINUTE;
            currentMinute++;
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour++;
                if (currentHour >= 24)
                {
                    currentHour = 0;
                    currentDay++;
                }
            }
            UpdateTimeDisplay();
        }

        if (minutesToAdd > 0)
        {
            AddMinutes(minutesToAdd);
            minutesToAdd = 0;
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeLabel != null)
        {
            string period = currentHour < 12 ? "am" : "pm";
            int displayHour = currentHour % 12;
            if (displayHour == 0) displayHour = 12;
            timeLabel.text = $"Día {currentDay} - {displayHour:D2} : {currentMinute:D2} {period}";
        }
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyLabel != null)
            moneyLabel.text = $"$ {currentMoney}";
    }

    public void AddMinutes(int minutes)
    {
        currentMinute += minutes;
        currentHour += currentMinute / 60;
        currentMinute %= 60;
        currentDay += currentHour / 24;
        currentHour %= 24;
        UpdateTimeDisplay();
    }

    public void ScheduleMinutes(int minutes)
    {
        minutesToAdd += minutes;
    }

    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        if (currentMoney < 0) currentMoney = 0;
        UpdateMoneyDisplay();
    }

    public void UpdateLocation()
    {
        currentLocation = currentBuildingName;

        foreach (var building in allBuildings)
        {
            if (building.BuildingName == currentLocation)
                building.Show();
            else
                building.Hide();
        }
    }
}
