using UnityEngine;

public class DisableBarriers : MonoBehaviour
{
    void Awake()
    {
        if(GameplayManager.Instance.removeBarriers)
        {
            this.gameObject.SetActive(false);
        }
    }
}
