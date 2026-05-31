using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewJunk", menuName = "SpaceJunk/JunkData")]
public class JunkData : ScriptableObject
{
    [Header("Identidad")]
    public string label = "PANEL SOLAR";
    public GameObject prefab;
    public Color glowColor = Color.cyan;

    [Header("Física")]
    public float mass = 2f;
    public float driftSpeed = 0.15f;
    public float spinSpeed = 8f;

    [Header("Gameplay")]
    public int points = 5;
    public JunkRarity rarity = JunkRarity.Common;

    [TextArea(2, 4)]
    public string flavourText = "Módulo flotante de la estación destruida.";
}

public enum JunkRarity { Common, Rare, Critical }