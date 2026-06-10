using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject npcBubble;
    [SerializeField] private TextMeshProUGUI npcText;
    [SerializeField] private GameObject playerBubble;
    [SerializeField] private TextMeshProUGUI playerText;

    private DialogueManager manager;
    private int lastChangeFrame;

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
        lastChangeFrame = Time.frameCount;
    }

    public void ShowPlayerLine(string text)
    {
        npcBubble.SetActive(false);
        playerText.text = text;
        playerBubble.SetActive(true);
        state = State.ShowingPlayer;
        lastChangeFrame = Time.frameCount;
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

        if (Input.GetMouseButtonDown(0))
        {
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
