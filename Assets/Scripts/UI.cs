using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text resourceCounter;
    public GameObject toolbar;
    public Button toolbarButtonPrefab;

    ToolbarProvider currentProvider;
    int resources;

    void Awake()
    {
        EventManager.Subscribe(EventManager.EventMessage.ResourceAmountChanged, ResourceAmountChanged);
        EventManager.Subscribe(EventManager.EventMessage.RegisterToolbarProvider, RegisterToolbarProvider);
        EventManager.Subscribe(EventManager.EventMessage.UnRegisterToolbarProvider, UnRegisterToolbarProvider);
    }

    private void UnRegisterToolbarProvider(object arg0)
    {
        ToolbarProvider provider = arg0 as ToolbarProvider;
        if (currentProvider == provider)
        {
            currentProvider = null;
        }
    }

    private void RegisterToolbarProvider(object arg0)
    {
        ToolbarProvider provider = arg0 as ToolbarProvider;
        currentProvider = provider;
        if (provider)
        {
            RegisterMenuItems(provider.toolbarActions);
        }
    }

    private void RegisterMenuItems(List<ToolbarAction> items)
    {
        ClearToolbar();
        foreach (ToolbarAction item in items)
        {
            Button button = Instantiate(toolbarButtonPrefab, toolbar.transform);
            button.GetComponent<ToolbarButton>().toolbarAction = item;
            button.transform.Find("Image").GetComponent<Image>().sprite = item.menuIcon;
        }
    }

    void ClearToolbar()
    {        
        foreach (Transform child in toolbar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void ResourceAmountChanged(object arg)
    {
        resources = (int)arg;
    }

    void Update()
    {
        resourceCounter.text = resources.ToString();
        if(currentProvider == null)
        {
            ClearToolbar();
        }
    }

}
