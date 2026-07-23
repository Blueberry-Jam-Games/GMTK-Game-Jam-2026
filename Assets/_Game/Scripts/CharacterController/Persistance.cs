using UnityEngine;

public class Persistance : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad (this.gameObject);        
    }
}
