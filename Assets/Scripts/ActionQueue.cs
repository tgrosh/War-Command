using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    public List<Action> actions = new List<Action>();

    Collector collector;
    Attacker attacker;
    Mover mover;
    Builder builder;

    // Start is called before the first frame update
    void Start()
    {
        collector = GetComponent<Collector>();
        attacker = GetComponent<Attacker>();
        mover = GetComponent<Mover>();
        builder = GetComponent<Builder>();
    }

    public void Add(Action action)
    {
        if (action.actionType == ActionType.Collect && !collector) return;
        if (action.actionType == ActionType.Attack && !attacker) return;
        if (action.actionType == ActionType.Move && !mover) return;
        if (action.actionType == ActionType.Build && !builder) return;

        actions.Add(action);
    }

    public void Clear()
    {
        actions.Clear();
    }
}
