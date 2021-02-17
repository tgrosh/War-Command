using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text resourceCounter;
    public GameObject toolbar;
    public Button buildButtonPrefab;

    int resources;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Subscribe(EventManager.Events.ResourceCollected, ResourceCollected);
        EventManager.Subscribe(EventManager.Events.RegisterBuildMenuItems, RegisterBuildMenuItems);
    }

    private void RegisterBuildMenuItems(object arg0)
    {
        List<BuildMenuItem> items = arg0 as List<BuildMenuItem>;

        foreach (BuildMenuItem item in items)
        {
            Button button = Instantiate(buildButtonPrefab, toolbar.transform);
            button.GetComponent<BuildButton>().buildId = item.id;
            button.transform.Find("Image").GetComponent<Image>().sprite = item.menuIcon;
        }
    }

    private void ResourceCollected(object arg)
    {
        resources += (int)arg;
    }

    // Update is called once per frame
    void Update()
    {
        resourceCounter.text = resources.ToString();
    }

}
