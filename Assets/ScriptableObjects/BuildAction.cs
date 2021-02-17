using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildAction : ScriptableObject
{
    public string id;
    public string title;
    public Sprite menuIcon;
    public GameObject buildable;
}
