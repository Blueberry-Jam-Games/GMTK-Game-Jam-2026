using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterDialogue
{
    public string characterName;
    public Sprite characterArt;

    [TextArea(4, 10)]
    public List<string> speech;
}
