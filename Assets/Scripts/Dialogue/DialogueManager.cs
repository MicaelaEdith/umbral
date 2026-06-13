using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueNode[] allDialogueNodes;
    [SerializeField] private GameObject player;

    private NPC currentNPC;
    private DialogueEntry currentEntry;
    private readonly Dictionary<string, DialogueNode[]> nodesByNpc = new();

    public bool IsDialogueActive { get; private set; }

    private void Awake()
    {
        Instance = this;
        BuildNodeIndex();
    }

    private void BuildNodeIndex()
    {
        nodesByNpc.Clear();
        foreach (var node in allDialogueNodes)
        {
            if (node == null) continue;
            if (!nodesByNpc.ContainsKey(node.NpcId))
                nodesByNpc[node.NpcId] = new DialogueNode[] { node };
            else
                nodesByNpc[node.NpcId] = nodesByNpc[node.NpcId].Append(node).ToArray();
        }
    }

    public void StartDialogue(NPC npc)
    {
        if (!nodesByNpc.ContainsKey(npc.NpcId)) return;

        currentNPC = npc;
        DialogueNode[] nodes = nodesByNpc[npc.NpcId];
        currentEntry = FindBestEntry(nodes, GameManager.Instance.questProgress);

        if (currentEntry == null) return;

        IsDialogueActive = true;
        currentNPC.PlayTalkAnimation();

        if (player != null)
            player.SetActive(false);

        dialogueUI.ShowPlayerLine(currentEntry.playerLine, this);
    }

    private DialogueEntry FindBestEntry(DialogueNode[] nodes, int progress)
    {
        DialogueEntry best = null;
        int bestProgress = -1;
        foreach (var node in nodes)
        {
            foreach (var entry in node.Entries)
            {
                if (entry.requiredProgress <= progress && entry.requiredProgress > bestProgress)
                {
                    best = entry;
                    bestProgress = entry.requiredProgress;
                }
            }
        }
        return best;
    }

    public void OnPlayerClicked()
    {
        if (currentEntry == null) return;
        currentNPC.PlayTalkAnimation();
        dialogueUI.ShowNpcLine(currentEntry.npcLine, this);
    }

    public void OnNpcClicked()
    {
        currentNPC.PlayIdleAnimation();
        EndDialogue();
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialogueUI.Hide();

        if (currentNPC != null)
            currentNPC.PlayIdleAnimation();

        if (player != null)
            player.SetActive(true);

        currentNPC = null;
        currentEntry = null;
    }
}
