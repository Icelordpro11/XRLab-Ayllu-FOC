using UnityEngine;
using System.Collections;
using UnityEngine.Events;


public class EventAfterTime : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private UnityEvent nextEvent;
    /// <summary>
    /// start a timer to invoke a new event after X seconds where X = time
    /// </summary>
    /// <param name="time">amount of seconds before next event</param>
    public void StartTimer(float time)
    {
        StartCoroutine(Timer(time));
    }
    
    private IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        nextEvent.Invoke();
    }
}