using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioOcclusion : MonoBehaviour
{
    AudioListener listener;
    CapsulePawn player;
    public AudioSource source;
    int layerMask;
    public float startingDepth; // default 6000
    public float depthMultiplier; // default 1000

    // Start is called before the first frame update
    void Start()
    {
        listener = (AudioListener)FindObjectOfType(typeof(AudioListener), false);
        player = (CapsulePawn)FindObjectOfType(typeof(CapsulePawn), false);
        source.AddComponent<AudioLowPassFilter>();
        //source.GetComponent<AudioLowPassFilter>().cutoffFrequency = 3000;
        source.GetComponent<AudioLowPassFilter>().enabled = false;
        layerMask = ~(1 << 3);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, (player.transform.position - this.transform.position).normalized, out hit, Vector3.Distance(this.transform.position, player.transform.position), layerMask)){
            // cast from player to audio source to get object depth
            RaycastHit hit2;
            Physics.Raycast(player.transform.position, (this.transform.position - player.transform.position).normalized, out hit2, Vector3.Distance(player.transform.position, this.transform.position), layerMask);
            float depth = Vector3.Distance(hit.point, hit2.point);
            // 5000 - depth * 1k
            //Debug.Log("Hit, Depth: " + depth + " " + hit.collider.gameObject.name);
            source.GetComponent<AudioLowPassFilter>().cutoffFrequency = startingDepth - depth * depthMultiplier;
            source.GetComponent<AudioLowPassFilter>().enabled = true;
        }
        else{
            //Debug.Log("No");
            source.GetComponent<AudioLowPassFilter>().enabled = false;
        }

    }
}
