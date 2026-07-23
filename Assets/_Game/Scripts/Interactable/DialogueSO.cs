using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public Sprite CharacterArt;

    public string name;

    [TextArea(4, 10)]
    public List<string> dialogue;
}
