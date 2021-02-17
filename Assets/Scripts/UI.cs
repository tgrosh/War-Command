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
    Builder currentBuilder;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Subscribe(EventManager.Events.ResourceCollected, ResourceCollected);
        EventManager.Subscribe(EventManager.Events.RegisterBuilder, RegisterBuilder);
    }

    private void RegisterBuilder(object arg0)
    {
        Builder builder = arg0 as Builder;
        currentBuilder = builder;
        if (builder)
        {
            RegisterBuildMenuItems(builder.buildMenuItems);
        }
    }

    private void RegisterBuildMenuItems(List<BuildAction> items)
    {
        foreach (BuildAction item in items)
        {
            Button button = Instantiate(buildButtonPrefab, toolbar.transform);
            button.GetComponent<BuildButton>().buildAction = item;
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

        if (currentBuilder == null)
        {
            foreach (Transform child in toolbar.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

}
