using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    //This script is refernced by the GrowPlot script.
    //The consumed vairable stops multiple plots from growing.
    [HideInInspector]
    public bool consumed = false;
}
