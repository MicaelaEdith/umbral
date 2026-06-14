using System;
using System.Collections.Generic;
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
    public bool CanSkipInput { get; set; } = true;

    private void Awake()
    {
        Instance = this;

        nodesByNpc.Clear();
        foreach (var node in allDialogueNodes)
        {
            if (node == null) continue;
            if (nodesByNpc.TryGetValue(node.NpcId, out var existing))
            {
                var expanded = new DialogueNode[existing.Length + 1];
                for (int i = 0; i < existing.Length; i++)
                    expanded[i] = existing[i];
                expanded[existing.Length] = node;
                nodesByNpc[node.NpcId] = expanded;
            }
            else
            {
                nodesByNpc[node.NpcId] = new DialogueNode[] { node };
            }
        }
    }

    public void StartDialogue(NPC npc)
    {
        if (!nodesByNpc.ContainsKey(npc.NpcId)) return;

        isDirectDialogue = false;
        currentNPC = npc;
        currentEntries = CollectEntries(npc.NpcId, GameManager.Instance.questProgress);
        currentEntryIndex = 0;

        if (currentEntries.Count == 0) return;

        IsDialogueActive = true;
        currentNPC.PlayTalkAnimation();

        if (player != null)
            player.SetActive(false);

        dialogueUI.ShowPlayerLine(currentEntries[0].playerLine, this);
    }

    public void PlayDialogueDirect(string npcId, Action onComplete, int requiredProgress)
    {
        if (!nodesByNpc.ContainsKey(npcId)) return;

        isDirectDialogue = true;
        onDialogueComplete = onComplete;
        currentNPC = null;
        currentEntries = CollectEntries(npcId, requiredProgress);
        currentEntryIndex = 0;

        if (currentEntries.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        IsDialogueActive = true;
        dialogueUI.ShowPlayerLine(currentEntries[0].playerLine, this);
    }

    private List<DialogueEntry> CollectEntries(string npcId, int progress)
    {
        var result = new List<DialogueEntry>();
        foreach (var node in nodesByNpc[npcId])
            foreach (var entry in node.Entries)
                if (entry.requiredProgress == progress)
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
        CanSkipInput = true;
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
