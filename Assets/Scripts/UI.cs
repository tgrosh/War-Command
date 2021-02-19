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
    public Button producerButtonPrefab;

    object currentRegistered;
    int resources;

    void Awake()
    {
        EventManager.Subscribe(EventManager.Events.ResourceAmountChanged, ResourceAmountChanged);
        EventManager.Subscribe(EventManager.Events.RegisterBuilder, RegisterBuilder);
        EventManager.Subscribe(EventManager.Events.RegisterProducer, RegisterProducer);
    }

    private void RegisterProducer(object arg0)
    {
        Producer producer = arg0 as Producer;
        currentRegistered = producer;
        if (producer)
        {
            RegisterProducerMenuItems(producer.producerMenuItems);
        }
    }

    private void RegisterBuilder(object arg0)
    {
        Builder builder = arg0 as Builder;
        currentRegistered = builder;
        if (builder)
        {
            RegisterBuildMenuItems(builder.buildMenuItems);
        }
    }

    private void RegisterProducerMenuItems(List<ProducerAction> items)
    {
        foreach (ProducerAction item in items)
        {
            Button button = Instantiate(producerButtonPrefab, toolbar.transform);
            button.GetComponent<ProducerButton>().producerAction = item;
            button.transform.Find("Image").GetComponent<Image>().sprite = item.menuIcon;
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

    private void ResourceAmountChanged(object arg)
    {
        resources = (int)arg;
    }

    // Update is called once per frame
    void Update()
    {
        resourceCounter.text = resources.ToString();

        if (currentRegistered == null)
        {
            foreach (Transform child in toolbar.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

}
