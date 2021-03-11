using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedObject : MonoBehaviour
{
    public PETSTATE toState = PETSTATE.Eating;

    public Transform[] NeedPos;

    [Range(0, 1)]
    public float nutritionGain, energyGain, moodGain; 

    public Transform ClosestPos(Vector2 pos)
    {
        Transform _out = NeedPos[0];
        float dist = Vector2.Distance(pos, NeedPos[0].position);

        for(int i = 1; i < NeedPos.Length; i++)
        {
            if(Vector2.Distance(pos, NeedPos[i].position)< dist)
            {
                _out = NeedPos[i];
            }
        }
        return _out;
    }

    public void Lock()
    {
        GetComponent<DragDrop>().Locked = true;
    }

    public void Unlock()
    {
        GetComponent<DragDrop>().Locked = false;
    }
}