using System.Collections.Generic;
using UnityEngine;

public class RockZone : MonoBehaviour
{
    [Header("Stack Manager")]
    [SerializeField] private int _stonesPerPack;
    [SerializeField] private StackManager _stackManager;
    [SerializeField] private Transform _stackOrigin;
    [SerializeField] private List<Transform> _stackPositions = new List<Transform>();
    
    private List<Transform> _stones = new List<Transform>();

    private void Start()
    {
        _stackManager = FindAnyObjectByType<StackManager>();
    }

    private void OnTriggerEnter(Collider stone)
    {
        TryAddStone(stone);
    }

    private void OnTriggerExit(Collider other)
    {
        _stones.Remove(other.transform);
        UpdateStonePositions();
    }

    private void TryAddStone(Collider stone)
    {
        if (!stone.CompareTag("Grabbable") || _stones.Contains(stone.transform))
            return;

        _stones.Add(stone.transform);
        UpdateStonePositions();
        CheckStonesAmount();
    }

    private void UpdateStonePositions()
    {
        for (int i = 0; i < _stones.Count; i++)
        {
            if (i >= _stackPositions.Count) break;

            Transform stone = _stones[i];
            Transform targetPos = _stackPositions[i];

            stone.transform.position = targetPos.position;
            stone.transform.rotation = targetPos.rotation;
        }
    }

    private void CheckStonesAmount()
    {
        if (_stones.Count != _stonesPerPack) return;

        _stackManager.AddCompletedStack();
        Debug.Log("Volledige stapel klaar!");
    }
    
    // OLD VARIABLES
    
    // [Header("Stone")]
    // [SerializeField] private Transform _snapPoint;
    // [SerializeField] private float _newHeight;
    // [SerializeField] private float _stoneSpeed;
    // [SerializeField] private float _minDist = 0.05f; // Minimum distance from target to consider stone "snapped"
    
    // OLD FUNCTIONS
    
    
    // private void SetStoneInsideZone(Collider stone)
    // {
    //     Rigidbody rb = stone.attachedRigidbody;
    //     rb.isKinematic = true; // Stop physics movement
    //     StartCoroutine(MoveStone(stone.transform)); // Start moving it toward the snap point
    // }
    //
    // // Coroutine that smoothly moves the stone into the snap position
    // private IEnumerator MoveStone(Transform stone)
    // {
    //     Vector3 endPos = _snapPoint.position; // Destination position for this stone
    //     
    //     // Continue moving until the stone is close enough to the target position
    //     while (Vector3.Distance(stone.position, _snapPoint.position) > _minDist)
    //     {
    //         // Move a small step toward the snap point each frame
    //        stone.position = Vector3.MoveTowards(
    //            stone.position,
    //            endPos,
    //            _stoneSpeed * Time.deltaTime
    //         );
    //        
    //         yield return null; // Wait until next frame to continue
    //     }
    // }
    //
    // // Raises the snap point upward to prepare for the next stone in the stack
    // private void SetNewPosition()
    // {
    //     _snapPoint.position = new Vector3(
    //         _snapPoint.position.x,
    //         _snapPoint.position.y + _newHeight, // Increase height for next placement
    //         _snapPoint.position.z
    //     );
    // }
}