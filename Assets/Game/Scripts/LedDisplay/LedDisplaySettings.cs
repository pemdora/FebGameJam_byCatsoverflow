﻿using UnityEngine;

public enum LedDisplayOrder
{
    Sequenced,
    Randomized
}

[CreateAssetMenu(menuName = "LED/LED Display Setting", fileName = "New LED Display Setting")]
public class LedDisplaySettings : ScriptableObject
{
    public LedDisplayOrder order;
    public float speed = 750;
    public string[] sentences;
}