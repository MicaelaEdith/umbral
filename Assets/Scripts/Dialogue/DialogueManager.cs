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
    private DialogueNode currentNode;
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
        DialogueNode firstNode = nodes.FirstOrDefault(n => n.NodeId == "root")
                            ?? nodes[0];

        if (firstNode == null) return;

        IsDialogueActive = true;
        currentNode = firstNode;
        currentNPC.PlayTalkAnimation();

        if (player != null)
            player.SetActive(false);

        dialogueUI.ShowNpcLine(firstNode.NpcLine, this);
    }

    public void OnNpcClicked()
    {
        if (currentNode == null) return;
        dialogueUI.ShowPlayerLine(currentNode.PlayerLine);
    }

    public void OnPlayerClicked()
    {
        if (currentNode == null) return;

        if (currentNode.IsLast)
        {
            EndDialogue();
            return;
        }

        DialogueNode[] nodes = nodesByNpc[currentNPC.NpcId];
        DialogueNode nextNode = nodes.FirstOrDefault(n => n.NodeId == currentNode.NextNodeId);

        if (nextNode == null)
        {
            EndDialogue();
            return;
        }

        currentNode = nextNode;
        currentNPC.PlayTalkAnimation();
        dialogueUI.ShowNpcLine(nextNode.NpcLine, this);
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
        currentNode = null;
    }
}
