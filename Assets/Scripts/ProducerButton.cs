using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerButton : MonoBehaviour
{
    public ProducerAction producerAction;

    public void ToolbarButtonPressed()
    {
        EventManager.Emit(EventManager.Events.ProducerButtonPressed, producerAction);
    }
}
