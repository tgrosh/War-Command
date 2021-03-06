using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public Dictionary<EventMessage, UnityEvent<object>> eventDictionary;
    public enum EventMessage
    {
        ResourcesDeposited,
        BuildButtonPressed,
        ResourcesWithdrawn,
        ProducerButtonPressed,
        IronAmountChanged,
        RegisterToolbarProvider,
        UnRegisterToolbarProvider,
        SelectionBoxUpdated,
        OilAmountChanged
    }

    private static EventManager eventManager;
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }

    }
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EventMessage, UnityEvent<object>>();
        }
    }

    public static void Subscribe(EventMessage eventName, UnityAction<object> listener)
    {
        UnityEvent<object> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<object>();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void Unsubscribe(EventMessage eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        UnityEvent<object> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void Emit(EventMessage eventName, object gameObject)
    {
        UnityEvent<object> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(gameObject);
        }
    }

}
