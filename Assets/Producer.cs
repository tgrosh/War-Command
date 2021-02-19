using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Producer : MonoBehaviour
{
    public List<ProducerAction> producerMenuItems = new List<ProducerAction>();
    public float producerRange;

    Selectable selectable;
    bool registered;

    private void Start()
    {
        selectable = GetComponent<Selectable>();
        EventManager.Subscribe(EventManager.Events.ProducerButtonPressed, ProducerButtonPressed);
    }

    private void ProducerButtonPressed(object arg0)
    {
        ProducerAction action = arg0 as ProducerAction;
        NavMeshHit navHit;
        NavMesh.SamplePosition(transform.position, out navHit, producerRange, NavMesh.AllAreas);

        if (navHit.hit)
        {
            GameObject obj = Instantiate(action.produceable, navHit.position, Quaternion.identity);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (selectable && selectable.isSelected && !registered)
        {
            EventManager.Emit(EventManager.Events.RegisterProducer, this);
            registered = true;
        }

        if (!selectable || (!selectable.isSelected && registered))
        {
            EventManager.Emit(EventManager.Events.RegisterProducer, null);
            registered = false;
        }
    }
}
