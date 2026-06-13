using System;
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
    private List<DialogueEntry> currentEntries;
    private int currentEntryIndex;
    private Action onDialogueComplete;
    private readonly Dictionary<string, DialogueNode[]> nodesByNpc = new();
    private bool isDirectDialogue;

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

        isDirectDialogue = false;
        currentNPC = npc;
        DialogueNode[] nodes = nodesByNpc[npc.NpcId];
        currentEntries = GetEntriesForProgress(nodes, GameManager.Instance.questProgress);
        currentEntryIndex = 0;

        if (currentEntries == null || currentEntries.Count == 0) return;

        IsDialogueActive = true;
        currentNPC.PlayTalkAnimation();

        if (player != null)
            player.SetActive(false);

        dialogueUI.ShowPlayerLine(currentEntries[0].playerLine, this);
    }

    public void PlayDialogueDirect(string npcId, Action onComplete)
    {
        if (!nodesByNpc.ContainsKey(npcId)) return;

        isDirectDialogue = true;
        onDialogueComplete = onComplete;
        currentNPC = null;
        DialogueNode[] nodes = nodesByNpc[npcId];
        currentEntries = new List<DialogueEntry>(nodes[0].Entries);
        currentEntryIndex = 0;

        if (currentEntries == null || currentEntries.Count == 0) return;

        IsDialogueActive = true;
        dialogueUI.ShowPlayerLine(currentEntries[0].playerLine, this);
    }

    private List<DialogueEntry> GetEntriesForProgress(DialogueNode[] nodes, int progress)
    {
        int bestProgress = -1;
        foreach (var node in nodes)
            foreach (var entry in node.Entries)
                if (entry.requiredProgress <= progress && entry.requiredProgress > bestProgress)
                    bestProgress = entry.requiredProgress;

        var result = new List<DialogueEntry>();
        if (bestProgress < 0) return result;

        foreach (var node in nodes)
            foreach (var entry in node.Entries)
                if (entry.requiredProgress == bestProgress)
                    result.Add(entry);

        return result;
    }

    public void OnPlayerClicked()
    {
        if (currentEntries == null) return;

        if (!isDirectDialogue && currentNPC != null)
            currentNPC.PlayTalkAnimation();

        dialogueUI.ShowNpcLine(currentEntries[currentEntryIndex].npcLine, this);
    }

    public void OnNpcClicked()
    {
        currentEntryIndex++;
        if (currentEntryIndex < currentEntries.Count)
        {
            dialogueUI.ShowPlayerLine(currentEntries[currentEntryIndex].playerLine, this);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialogueUI.Hide();

        if (!isDirectDialogue)
        {
            if (currentNPC != null)
                currentNPC.PlayIdleAnimation();
            if (player != null)
                player.SetActive(true);
        }

        Action cb = onDialogueComplete;
        currentNPC = null;
        currentEntries = null;
        currentEntryIndex = 0;
        isDirectDialogue = false;
        onDialogueComplete = null;

        cb?.Invoke();
    }
}
