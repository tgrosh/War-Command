using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class ProducerAction : ScriptableObject
{
    public string id;
    public string title;
    public Sprite menuIcon;
    public GameObject produceable;
    public int cost;
}
