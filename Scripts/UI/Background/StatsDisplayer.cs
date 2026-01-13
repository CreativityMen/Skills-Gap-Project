/*
    Title: StatsDisplayer.cs

    Author: Afonso Marques

    Description: Script used to display numerous of different stats
    based on the MotorHandler. Instead of manually creating a GameObject
    with a TextLabel component each time, this automatically creates 
    several depending on how many you've inserted in the list.

    I wrote a bunch of comments to remind future self how I did certain
    things in case I need it when working on other projects, and to also
    show others how I did them in case they want to learn.

 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Stat
{
    public string statName;
    public string statValue;

    public Stat(string currentStatName, string currentStatValue)
    {
        if (currentStatValue == null) return;

        this.statValue = currentStatValue;
        this.statName = currentStatName;
    }
}

public class StatsDisplayer : MonoBehaviour
{
    // Parent
    public GameObject _statsDisplayer;

    // Other Objects
    private GameObject playerObject;
    private Rigidbody rigidBody;

    // Text
    private TextMeshProUGUI text;

    // Modules
    private MotorHandler motorHandler;

    // Stats
    [SerializeField]
    [Header("Stats List")]
    public List<Stat> stats = new List<Stat>()
    {
        new Stat("Ground Status:", "n/a"),
        new Stat("Action Status:", "n/a"),
        new Stat("Velocity:", "n/a")
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player"); 
        rigidBody = playerObject.GetComponent<Rigidbody>();

        motorHandler = playerObject.GetComponent<MotorHandler>();
        _statsDisplayer = gameObject;

        InstanceStats();
    }

    private void ManageStats()
    {
        foreach (Stat stat in stats)
        {
            switch (stat.statName)
            {
                case "Ground Status:":
                    if (motorHandler.currentGroundState != motorHandler.lastGroundState)
                    {
                        stat.statValue = motorHandler.currentGroundState.ToString();
                    }

                    break;

                case "Action Status:":
                    if (motorHandler.currentActionState != motorHandler.lastActionState)
                    {
                        stat.statValue = motorHandler.currentActionState.ToString();
                    }

                    break;

                case "Velocity:":
                    if (rigidBody.linearVelocity.sqrMagnitude > 0)
                    {
                        stat.statValue = rigidBody.linearVelocity.ToString();
                    }

                    break;
            }

            DisplayStats(stat.statName, stat.statValue);
        }

    }

    private void DisplayStats(string statName, string statValue)
    {
        if (_statsDisplayer.transform.Find(statName))
        {
            GameObject currentStatObject = _statsDisplayer.transform.Find(statName).gameObject;
            TextMeshProUGUI _text = currentStatObject.GetComponent<TextMeshProUGUI>();

            _text.text = statName + statValue;
        }
        else
        {
            return;
        }
    }

    private void InstanceStats()
    {
        // Properties
        float posAmount = 30f;
        float lastYPosition = 0f;

        foreach (Stat stat in stats)
        {
            GameObject currentStatus = Instantiate(_statsDisplayer.transform.Find("status").gameObject, _statsDisplayer.transform);
            currentStatus.GetComponent<TextMeshProUGUI>().text = stat.statName + stat.statValue;
            currentStatus.gameObject.SetActive(true);

            currentStatus.name = stat.statName;

            if (lastYPosition != 0f)
            {
                currentStatus.transform.position = new Vector3(currentStatus.transform.position.x, lastYPosition - posAmount);
            }

            lastYPosition = currentStatus.transform.position.y;
        }
    }

    private void LateUpdate()
    {
        if (!motorHandler) return;

        ManageStats();
    }
}
