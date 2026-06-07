using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject npcBubble;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private GameObject playerBubble;
    [SerializeField] private TextMeshProUGUI playerText;

    private DialogueManager manager;
    private bool waitingForClick;

    private enum State { Hidden, ShowingNpc, ShowingPlayer }
    private State state = State.Hidden;

    private void Awake()
    {
        npcBubble.SetActive(false);
        playerBubble.SetActive(false);
    }

    public void ShowNpcLine(string text, DialogueManager mgr)
    {
        manager = mgr;
        playerBubble.SetActive(false);
        npcText.text = text;
        npcBubble.SetActive(true);
        state = State.ShowingNpc;
        waitingForClick = true;
    }

    public void ShowPlayerLine(string text)
    {
        npcBubble.SetActive(false);
        playerText.text = text;
        playerBubble.SetActive(true);
        state = State.ShowingPlayer;
        waitingForClick = true;
    }

    public void Hide()
    {
        npcBubble.SetActive(false);
        playerBubble.SetActive(false);
        state = State.Hidden;
        waitingForClick = false;
    }

    private void Update()
    {
        if (!waitingForClick) return;

        if (Input.GetMouseButtonDown(0))
        {
            waitingForClick = false;

            switch (state)
            {
                case State.ShowingNpc:
                    manager.OnNpcClicked();
                    break;
                case State.ShowingPlayer:
                    manager.OnPlayerClicked();
                    break;
            }
        }
    }
}
