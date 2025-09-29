using UnityEngine;
using System.Collections.Generic;

public class DominationZone : MonoBehaviour
{
    public enum Team { None, Blue, Red }

    [Header("Configurações da Zona")]
    public Team controllingTeam = Team.None;
    public float captureSpeed = 10f; 

    [Header("Cores das Equipes")]
    public Color neutralColor = Color.gray;
    public Color blueTeamColor = Color.blue;
    public Color redTeamColor = Color.red;

    private float captureProgress = 0f; 
    private Renderer zoneRenderer;

    private List<Collider> playersInZone = new List<Collider>();

    private void Awake()
    {
        zoneRenderer = GetComponent<Renderer>();
        UpdateZoneColor();
    }

    private void Update()
    {
        int bluePlayers = 0;
        int redPlayers = 0;

        foreach (var player in playersInZone)
        {
            if (player.CompareTag("Player")) 
            {
                bluePlayers++;
            }
            else if (player.CompareTag("Enemy")) 
            {
                redPlayers++;
            }
        }

        if (bluePlayers > 0 && redPlayers == 0)
        {
            captureProgress += captureSpeed * Time.deltaTime;
        }
        else if (redPlayers > 0 && bluePlayers == 0)
        {
            captureProgress -= captureSpeed * Time.deltaTime;
        }

        captureProgress = Mathf.Clamp(captureProgress, -100f, 100f);

        UpdateControllingTeam();
        UpdateZoneColor();
    }

    private void UpdateControllingTeam()
    {
        if (captureProgress >= 100f)
        {
            controllingTeam = Team.Blue;
        }
        else if (captureProgress <= -100f)
        {
            controllingTeam = Team.Red;
        }
        else
        {
            controllingTeam = Team.None;
        }
    }

    private void UpdateZoneColor()
    {
        if (zoneRenderer != null)
        {
           
            if (captureProgress > 0)
            {
                zoneRenderer.material.color = Color.Lerp(neutralColor, blueTeamColor, captureProgress / 100f);
            }
            else
            {
                zoneRenderer.material.color = Color.Lerp(neutralColor, redTeamColor, -captureProgress / 100f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (!playersInZone.Contains(other))
            {
                playersInZone.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (playersInZone.Contains(other))
            {
                playersInZone.Remove(other);
            }
        }
    }
}