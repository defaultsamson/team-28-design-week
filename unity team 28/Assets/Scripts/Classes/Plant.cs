using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Plant")]
public class Plant : ScriptableObject
{

    public Plant_GrowthStage[] growthStages = new Plant_GrowthStage[3];
}

[Serializable]
public class Plant_GrowthStage
{
    public Sprite sprite;
    public float timeLasts;
}