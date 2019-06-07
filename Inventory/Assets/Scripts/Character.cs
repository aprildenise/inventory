using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{

    // For dialogue
    public string characterName;
    [TextArea]
    public string[] characterSpeech;



}
