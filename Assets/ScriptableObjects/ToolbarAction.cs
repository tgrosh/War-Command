using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class ToolbarAction : ScriptableObject
{
    public string id;
    public string title;
    public Sprite menuIcon;
    public GameObject prefab;
    public int cost;
    public EventManager.EventMessage eventType;
}
