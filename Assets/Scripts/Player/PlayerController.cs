using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private Vector2 targetPosition;
    private bool isMoving = false;

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = worldPoint;
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if ((Vector2)transform.position == targetPosition)
                isMoving = false;
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
