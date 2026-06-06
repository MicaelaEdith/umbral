using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MoneyFeedback moneyFeedback;
    [SerializeField] private TimeFeedback timeFeedback;

    public string currentBuildingName;
    public Waypoint currentDestinationWaypoint;
    public string currentLocation;

    public bool shameActive;
    [Range(0f, 1f)] public float shameLevel;

    [SerializeField] private BuildingInterior[] allBuildings;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI moneyLabel;

    [SerializeField] private int remainingMinutes = 660;
    [SerializeField] private int currentMoney = 2000;

    private float timeAccumulator;
    private const float REAL_SECONDS_PER_GAME_MINUTE = 1f;
    private int timeToSpend;

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
            remainingMinutes--;
            if (remainingMinutes < 0) remainingMinutes = 0;
            UpdateTimeDisplay();

            if (remainingMinutes <= 0)
            {
                Debug.Log("Se acabó el tiempo del día");
            }
        }

        if (timeToSpend > 0)
        {
            SpendTime(timeToSpend);
            timeToSpend = 0;
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeLabel != null)
        {
            int hours = remainingMinutes / 60;
            int minutes = remainingMinutes % 60;
            timeLabel.text = $"{hours:D2} : {minutes:D2} hs restantes";
        }
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyLabel != null)
            moneyLabel.text = $"$ {currentMoney}";
    }

    public void SpendTime(int minutes)
    {
        remainingMinutes -= minutes;
        if (remainingMinutes < 0) remainingMinutes = 0;
        UpdateTimeDisplay();
        if (timeFeedback != null)
            timeFeedback.Flash();
    }

    public void ScheduleTime(int minutes)
    {
        timeToSpend += minutes;
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
        if (moneyFeedback != null)
            moneyFeedback.Flash();
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
