using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildMenuItem : ScriptableObject
{
    public string id;
    public string title;
    public Sprite menuIcon;
    public GameObject buildable;
}
