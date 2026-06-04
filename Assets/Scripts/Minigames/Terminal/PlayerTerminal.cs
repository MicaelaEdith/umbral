using UnityEngine;

public class PlayerTerminal : MonoBehaviour
{
    [SerializeField] private TerminalMonitor[] monitors;
    [SerializeField] private GameObject parentToClose;
    [SerializeField] private GameObject ticketButton;
    [SerializeField] private MinigameTrigger triggerToDisable;
    [SerializeField] private MinigameTrigger triggerToEnable;

    private int currentMonitorIndex;
    private int mistakeCount;
    private Ticket currentTicket;

    public int MistakeCount => mistakeCount;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        currentMonitorIndex = 0;
        mistakeCount = 0;
        currentTicket = null;

        foreach (var m in monitors)
            m.Deactivate();

        foreach (var monitor in monitors)
        {
            foreach (var btn in monitor.buttons)
                btn.OnClick += OnButtonClicked;
        }

        if (monitors.Length > 0)
            monitors[0].Activate();
    }

    private void OnButtonClicked(ClickableButton button)
    {
        if (button.IsCancel)
        {
            mistakeCount++;
            GameManager.Instance.ScheduleMinutes(2);
            ResetToStart();
            return;
        }

        if (currentMonitorIndex >= monitors.Length) return;

        TerminalMonitor current = monitors[currentMonitorIndex];
        bool correct = current.ProcessClick(button);

        if (correct)
        {
            if (current.IsComplete)
            {
                bool isLast = currentMonitorIndex >= monitors.Length - 1;

                if (isLast)
                {
                    current.ShowTicketOnComplete();
                    currentMonitorIndex++;
                    SubscribeToTicket(current);
                }
                else
                {
                    current.Deactivate();
                    currentMonitorIndex++;
                    monitors[currentMonitorIndex].Activate();
                }
            }
        }
        else
        {
            mistakeCount++;
            GameManager.Instance.ScheduleMinutes(2);
            Debug.Log($"PlayerTerminal — error #{mistakeCount}");
        }
    }

    private void SubscribeToTicket(TerminalMonitor monitor)
    {
        if (monitor.ticketOnComplete == null) return;

        currentTicket = monitor.ticketOnComplete.GetComponent<Ticket>();
        if (currentTicket != null)
            currentTicket.OnCollected += OnTicketCollected;
    }

    private void ResetToStart()
    {
        if (currentTicket != null)
        {
            currentTicket.OnCollected -= OnTicketCollected;
            currentTicket = null;
        }

        foreach (var m in monitors)
            m.Deactivate();

        currentMonitorIndex = 0;

        foreach (var monitor in monitors)
        {
            foreach (var btn in monitor.buttons)
                btn.gameObject.SetActive(true);
        }

        if (monitors.Length > 0)
            monitors[0].Activate();
    }

    private void OnTicketCollected()
    {
        if (currentTicket != null)
        {
            currentTicket.OnCollected -= OnTicketCollected;
            currentTicket = null;
        }

        if (triggerToDisable != null)
        {
            triggerToDisable.SetCanActivate(false);
            triggerToDisable.NotifyClosed();
        }

        if (triggerToEnable != null)
            triggerToEnable.SetCanActivate(true);

        if (GameManager.Instance != null)
            GameManager.Instance.shameActive = true;

        if (ticketButton != null)
            ticketButton.SetActive(true);

        if (parentToClose != null)
            parentToClose.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (currentTicket != null)
            currentTicket.OnCollected -= OnTicketCollected;

        foreach (var monitor in monitors)
        {
            if (monitor == null) continue;
            foreach (var btn in monitor.buttons)
            {
                if (btn != null)
                    btn.OnClick -= OnButtonClicked;
            }
        }
    }
}
