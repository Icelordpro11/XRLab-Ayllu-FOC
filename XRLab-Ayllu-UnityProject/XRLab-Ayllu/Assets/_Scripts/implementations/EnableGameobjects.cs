using UnityEngine;

public class EnableGameobjects : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    public void Enable()
    {
        foreach (GameObject i in _gameObjects)
        {
            i.SetActive(true);
        }
    }
    public void Disable()
    {
        foreach (GameObject i in _gameObjects)
        {
            i.SetActive(false);
        }
    }
}
