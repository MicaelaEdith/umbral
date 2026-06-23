using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private GameObject npcBubble;
    [SerializeField]
    private TextMeshProUGUI npcText;
    [SerializeField]
    private GameObject playerBubble;
    [SerializeField]
    private TextMeshProUGUI playerText;
    [SerializeField]
    private float autoAdvanceDelay = 1.5f;

    private DialogueManager manager;
    private int lastChangeFrame;
    private float autoTimer;

    private enum State { Hidden, ShowingNpc, ShowingPlayer }
    private State state = State.Hidden;

    public float AutoAdvanceDelay { get => autoAdvanceDelay; set => autoAdvanceDelay = value; }

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
        lastChangeFrame = Time.frameCount;
        autoTimer = 0f;
    }

    public void ShowPlayerLine(string text, DialogueManager mgr)
    {
        manager = mgr;
        npcBubble.SetActive(false);
        playerText.text = text;
        playerBubble.SetActive(true);
        state = State.ShowingPlayer;
        lastChangeFrame = Time.frameCount;
        autoTimer = 0f;
    }

    public void Hide()
    {
        npcBubble.SetActive(false);
        playerBubble.SetActive(false);
        state = State.Hidden;
    }

    private void Update()
    {
        if (state == State.Hidden) return;
        if (Time.frameCount <= lastChangeFrame) return;

        autoTimer += Time.deltaTime;

        bool advance = autoTimer >= autoAdvanceDelay;

        if (manager != null && manager.CanSkipInput)
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                advance = true;

        if (!advance) return;

        autoTimer = 0f;
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
