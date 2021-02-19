using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProducerButton : MonoBehaviour
{
    public ProducerAction producerAction;

    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        button.enabled = producerAction.cost <= ResourceBank.instance.currentResources;
        if (button.enabled)
        {
            button.GetComponentInChildren<Image>().color = Color.white;
        } else
        {
            button.GetComponentInChildren<Image>().color = Color.red;
        }
    }

    public void ToolbarButtonPressed()
    {
        EventManager.Emit(EventManager.Events.ProducerButtonPressed, producerAction);
    }
}
