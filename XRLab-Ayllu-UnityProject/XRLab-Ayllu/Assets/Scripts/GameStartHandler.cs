using UnityEngine;
using UnityEngine.Events;


public class GameStartHandler : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent _onStartGameEvent;

    private void Update()
    {
        if (!OVR_Helper.IsControllerConnected()) return;
        if (OVRInput.GetDown(OVRInput.Button.Start)) StartTheGame();
    }

    [ContextMenu("Start Game")]
    public void StartTheGame() => _onStartGameEvent?.Invoke();
}

