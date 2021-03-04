using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text ironCounter;
    public Text oilCounter;
    public GameObject toolbar;
    public Button toolbarButtonPrefab;
    public RectTransform selectionBox;

    ToolbarProvider currentProvider;
    int currentIronAmount;
    int currentOilAmount;

    void Awake()
    {
        EventManager.Subscribe(EventManager.EventMessage.IronAmountChanged, ResourceAmountChanged);
        EventManager.Subscribe(EventManager.EventMessage.OilAmountChanged, OilAmountChanged);
        EventManager.Subscribe(EventManager.EventMessage.RegisterToolbarProvider, RegisterToolbarProvider);
        EventManager.Subscribe(EventManager.EventMessage.UnRegisterToolbarProvider, UnRegisterToolbarProvider);
        EventManager.Subscribe(EventManager.EventMessage.SelectionBoxUpdated, SelectionBoxUpdated);
    }

    private void SelectionBoxUpdated(object arg0)
    {
        Vector2[] vectors = arg0 as Vector2[];
        bool isValidVectorArray = vectors != null && vectors.Length == 2;

        selectionBox.gameObject.SetActive(isValidVectorArray);

        if (isValidVectorArray)
        {
            selectionBox.sizeDelta = vectors[0];
            selectionBox.anchoredPosition = vectors[1];
        }
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

    private void OilAmountChanged(object arg0)
    {
        currentOilAmount = (int)arg0;
    }

    private void ResourceAmountChanged(object arg)
    {
        currentIronAmount = (int)arg;
    }

    void Update()
    {
        ironCounter.text = currentIronAmount.ToString();
        oilCounter.text = currentOilAmount.ToString();
        if (currentProvider == null)
        {
            ClearToolbar();
        }
    }

}
