using UnityEngine;

[CreateAssetMenu(menuName = "Umbral/Dialogue Node", fileName = "NewDialogueNode")]
public class DialogueNode : ScriptableObject
{
    [SerializeField] private string nodeId;
    [SerializeField] private string npcId;
    [SerializeField] private string npcLine;
    [SerializeField] private string playerLine;
    [SerializeField] private string nextNodeId;

    public string NodeId => nodeId;
    public string NpcId => npcId;
    public string NpcLine => npcLine;
    public string PlayerLine => playerLine;
    public string NextNodeId => nextNodeId;
    public bool IsLast => string.IsNullOrEmpty(nextNodeId);
}
