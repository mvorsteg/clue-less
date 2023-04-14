using UnityEngine;

public class DebugCanvasSetter : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject[] others;
    
    private void Awake()
    {
        startScreen.SetActive(true);
        foreach (GameObject other in others)
        {
            other.SetActive(false);
        }
    }
}