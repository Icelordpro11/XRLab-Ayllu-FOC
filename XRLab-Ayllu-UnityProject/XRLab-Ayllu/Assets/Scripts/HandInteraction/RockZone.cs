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
    
}