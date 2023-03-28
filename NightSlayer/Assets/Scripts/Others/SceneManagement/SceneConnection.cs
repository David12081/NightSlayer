using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Connection")]
public class SceneConnection : ScriptableObject
{
    public static SceneConnection activeConnection { get; set; }
}