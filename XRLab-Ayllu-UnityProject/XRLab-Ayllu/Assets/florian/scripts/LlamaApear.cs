using System.Collections;
using UnityEngine;

public class LlamaApear : MonoBehaviour
{
    private static readonly string Dissapear = "Dissappear";
    [SerializeField] private Material[] _materials;
    private float _time = 1;

    private void Start()
    {
        StartCoroutine(c_Start());
    }

    private IEnumerator c_Start()
    {
        while (_time>-0.9)
        {
            Debug.Log(_time);
            foreach (var i in _materials)
            {
                i.SetFloat(Dissapear, _time);
            }
            _time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        foreach (var i in _materials)
        {
            i.SetFloat(Dissapear, 0.9f);
        }
    }
}
