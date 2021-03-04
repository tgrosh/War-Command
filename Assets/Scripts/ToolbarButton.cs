using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarButton : MonoBehaviour
{
    public ToolbarAction toolbarAction;

    Button button;
    Color originalButtonColor;
    Color originalImageColor;

    private void Start()
    {
        button = GetComponent<Button>();
        originalButtonColor = button.GetComponentInChildren<Image>().color;
        originalImageColor = button.transform.Find("Image").GetComponent<Image>().color;
    }

    private void Update()
    {
        button.enabled = toolbarAction.cost <= ResourceBank.instance.currentIron;
        if (button.enabled)
        {
            button.transform.Find("Image").GetComponent<Image>().color = originalImageColor;
            button.GetComponentInChildren<Image>().color = originalButtonColor;
        }
        else
        {
            button.transform.Find("Image").GetComponent<Image>().color = button.GetComponentInChildren<Image>().color = Color.red;
        }
    }

    public void ToolbarButtonPressed()
    {
        EventManager.Emit(toolbarAction.eventType, toolbarAction);
    }
}
