using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class DraggableObstacle : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;

    private Vector3 dragOffset;
    
    private bool isDragging = false;
    
    private Vector3 targetPosition;
    
    private Rigidbody physicsBody;
    
    private BoxCollider boxCollider;

    private InputAction pointerInputAction;

    void Awake()
    {
        mainCamera = Camera.main;

        pointerInputAction = new InputAction(binding: "<Pointer>/position");

        pointerInputAction.Enable();

        physicsBody = GetComponent<Rigidbody>();

        boxCollider = GetComponent<BoxCollider>();

        targetPosition = physicsBody.position;
    }

    private void FixedUpdate()
    {
        if (!isDragging)
        {
            return;
        }

        TryMoveTo(targetPosition);
    }

    private void TryMoveTo(Vector3 desiredPosition)
    {
        Vector3 moveDirection = desiredPosition - transform.position;

        float moveDistance = moveDirection.magnitude;

        if (moveDistance < 0.001f)
        {
            return;
        }

        moveDirection.Normalize();

        Vector3 boxCastCenter = transform.position;
        
        Vector3 halfExtents = boxCollider.size * 0.5f;
        
        Quaternion orientation = transform.rotation;

        RaycastHit hit;

        if (Physics.BoxCast(boxCastCenter, halfExtents, moveDirection, out hit, orientation, moveDistance))
        {
            float safeDistance = Mathf.Max(hit.distance - 0.01f, 0f);

            Vector3 newPosition = transform.position + moveDirection * safeDistance;

            physicsBody.MovePosition(newPosition);
        }
        else
        {
            physicsBody.MovePosition(desiredPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        Vector3 pointerWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, -mainCamera.transform.position.z));

        pointerWorldPos.z = 0f;

        dragOffset = pointerWorldPos - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            return;
        }

        Vector3 pointerWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, -mainCamera.transform.position.z));
        
        pointerWorldPos.z = 0f;

        targetPosition = pointerWorldPos - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        Vector3 snappedPosition = SnapToGrid(transform.position);

        transform.position = snappedPosition;
    }
    private Vector3 SnapToGrid(Vector3 position)
    {
        float tileSize = 1f;

        float gridX = Mathf.Floor(position.x / tileSize); 

        float gridY = Mathf.Floor(position.y / tileSize); 

        Vector3 topLeft = new Vector3(gridX, gridY + 0.5f, position.z);       
        
        Vector3 topMid = new Vector3(gridX + 0.5f, gridY + 0.5f, position.z);  
        
        Vector3 topRight = new Vector3(gridX + 1f, gridY + 0.5f, position.z);  

        Vector3 midLeft = new Vector3(gridX, gridY, position.z);     
        
        Vector3 center = new Vector3(gridX + 0.5f, gridY, position.z);   
        
        Vector3 midRight = new Vector3(gridX + 1f, gridY, position.z);   
        
        Vector3 bottomLeft = new Vector3(gridX, gridY - 0.5f, position.z);      

        Vector3 bottomMid = new Vector3(gridX + 0.5f, gridY - 0.5f, position.z); 

        Vector3 bottomRight = new Vector3(gridX + 1f, gridY - 0.5f, position.z); 

        Vector3[] snapPoints = new Vector3[] {
        topLeft, topMid, topRight,
        midLeft, center, midRight,
        bottomLeft, bottomMid, bottomRight
    };

        Vector3 closestSnapPoint = snapPoints.OrderBy(p => (p - position).sqrMagnitude).First();

        return closestSnapPoint;
    }


    void OnDestroy()
    {
        pointerInputAction.Disable();
    }
}
