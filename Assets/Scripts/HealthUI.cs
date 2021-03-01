using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image healthBar;

    Health health;
    Canvas canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponentInParent<Health>();
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas) canvas.enabled = ShowCanvas();
        if (health) healthBar.fillAmount = (float)health.currentHealth / (float)health.maxHealth;
    }

    bool ShowCanvas() {
        return health && health.maxHealth > health.currentHealth && health.currentHealth > 0;
    }
}
