using UnityEngine;

public class SmokeTest
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnRuntimeInitialized()
    {
        Debug.Log("Unity Build Ready");
    }
}
