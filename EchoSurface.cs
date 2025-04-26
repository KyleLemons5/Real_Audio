using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoSurface : MonoBehaviour, IEchoableObject
{
    public float _echoMultiplier;

    void Start (){
    }

    float IEchoableObject.echoMultiplier{
        get => _echoMultiplier;
        set => _echoMultiplier = value;
    }
}
