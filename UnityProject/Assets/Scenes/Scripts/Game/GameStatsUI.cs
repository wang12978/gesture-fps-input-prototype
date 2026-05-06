using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameStatsUI : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    private int targetsHit = 0;
    private float startTime;

    private int milestoneStep = 5;
    private string milestoneText = "";
    void Start()
    {
        startTime = Time.time;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    public void AddTargetHit()
    {
        targetsHit++;
        float elapsedTime = Time.time - startTime;

        
        if (targetsHit % milestoneStep == 0)
        {
            milestoneText += $"Reached {targetsHit} targets: {elapsedTime:F1}s\n";
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        float elapsedTime = Time.time - startTime;

        statsText.text =
            $"Targets Hit: {targetsHit}\n" +
            $"Time: {elapsedTime:F1}s\n\n" +
            $"Records:\n{milestoneText}";
    }
}