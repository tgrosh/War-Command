using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Targeter : MonoBehaviour
{
    public GameObject targetMarkerPrefab;
    public Targetable target;

    ActionQueue queue;
    GameObject marker;
    Selectable selectable;

    // Start is called before the first frame update
    void Start()
    {
        selectable = GetComponent<Selectable>();
        queue = GetComponent<ActionQueue>();
    }

    void Update()
    {
        if (queue != null && queue.Peek() != null && queue.Peek().actionTarget != null)
        {
            Targetable targetable = queue.Peek().actionTarget.GetComponentInParent<Targetable>();
            if (targetable && 
                queue.Peek().actionType == ActionType.Attack || 
                queue.Peek().actionType == ActionType.Build || 
                queue.Peek().actionType == ActionType.Collect)
            {
                target = targetable;
            } else
            {
                target = null;
            }
        } else
        {
            target = null;
        }

        if (target && selectable && selectable.IsSelected)
        {
            ShowMarker();
        } else
        {
            ClearMarker();
        }
    }

    void OnDestroy() {
        ClearMarker();
    }

    void ShowMarker()
    {
        if (marker)
        {
            Destroy(marker);
        }

        if (!target) return;
        marker = Instantiate(targetMarkerPrefab, new Vector3(target.transform.position.x, 0, target.transform.position.z), Quaternion.identity);
        marker.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        Bounds targetBounds = GetTargetBounds();
        marker.transform.localScale = new Vector3(targetBounds.extents.magnitude*2, 1, targetBounds.extents.magnitude * 2);
    }

    Bounds GetTargetBounds()
    {
        Bounds bounds = new Bounds(target.transform.position, Vector3.zero);

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    void ClearMarker()
    {
        if (marker)
        {
            Destroy(marker);
        }
    }

}
