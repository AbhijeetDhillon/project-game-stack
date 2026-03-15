using UnityEngine;
using System;

public enum BlockPatternType { None, Mandala, Stars }

[CreateAssetMenu(fileName ="Block Spawn Data", menuName = "Data/Block Spawn Data")]
public class GameSettingData : ScriptableObject
{
    public Vector3 initialBlockScale;
    public float blockMoveTime;
    public float blockSpawnDistance;
    public float minDistance;
    public float camMoveTime;
    public Color[] colorPalette;
    public float deltaColor;

    // Optional separate palette for the background gradient.
    // If empty, falls back to colorPalette (original behaviour).
    public Color[] bgColorPalette;

    // Procedural pattern stamped on top face of each block.
    public BlockPatternType blockPattern;

    public float blockOffsetHeight = 3.95f;
    public float endingSpeed = 20f;

    public int perfectCondition = 8;
    public float perfectScale = 0.5f;
}
