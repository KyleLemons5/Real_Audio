using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEcho : MonoBehaviour
{
    public float minEchoSurfaces;
    public float echoSurfaceMultiplier;
    public float extraSurfaceMultiplier;
    public float minEchoDistance;
    public float maxEchoDistance;
    RaycastHit[] hitArr = new RaycastHit[6];
    private int surfacesHit;
    private float totalEchoMultiplier;
    private float averageDistance;
    public float wetMix;
    public float dryMix;
    public float echoSpeedMultiplier;
    private float echoSpeedDivisor;

    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source.AddComponent<AudioEchoFilter>();
        source.GetComponent<AudioEchoFilter>().enabled = false;
        echoSpeedDivisor = echoSpeedMultiplier / 343;
        setEchoAmount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void scanEchoArea(){
        // Ray in XYZ and -XYZ
        // create vect3s for all directions
        surfacesHit = 0;
        totalEchoMultiplier = 0;
        averageDistance = 0;

        Vector3 direction = new Vector3(maxEchoDistance, 0, 0);
        Physics.Raycast(transform.position, direction, out hitArr[0], maxEchoDistance);
        direction = new Vector3(-maxEchoDistance, 0, 0);
        Physics.Raycast(transform.position, direction, out hitArr[1], maxEchoDistance);
        direction = new Vector3(0, maxEchoDistance, 0);
        Physics.Raycast(transform.position, direction, out hitArr[2], maxEchoDistance);
        direction = new Vector3(0, -maxEchoDistance, 0);
        Physics.Raycast(transform.position, direction, out hitArr[3], maxEchoDistance);
        direction = new Vector3(0, 0, maxEchoDistance);
        Physics.Raycast(transform.position, direction, out hitArr[4], maxEchoDistance);
        direction = new Vector3(0, 0, -maxEchoDistance);
        Physics.Raycast(transform.position, direction, out hitArr[5], maxEchoDistance);

        for(int i = 0; i < hitArr.Length; i++){
            if(hitArr[i].transform != null){
                //Debug.Log("Something hit " + hitArr[i].transform.gameObject.name);
                if(hitArr[i].transform.gameObject != null){
                    IEchoableObject obj = hitArr[i].transform.gameObject.GetComponent<IEchoableObject>();
                    if(obj != null){
                        Debug.Log("obj found " + i + " distance " + hitArr[i].distance);
                        surfacesHit++;
                        totalEchoMultiplier = totalEchoMultiplier + obj.echoMultiplier;
                        averageDistance = averageDistance + hitArr[i].distance;
                    }
                }
            }
        }
        averageDistance = averageDistance / surfacesHit;
    }

    private void setEchoAmount(){
        scanEchoArea();
        extraSurfaceMultiplier = surfacesHit / minEchoSurfaces * extraSurfaceMultiplier;
        source.GetComponent<AudioEchoFilter>().decayRatio = Mathf.Clamp(totalEchoMultiplier/surfacesHit * echoSurfaceMultiplier * extraSurfaceMultiplier, 0f, 1f);
        source.GetComponent<AudioEchoFilter>().delay = Mathf.Clamp(averageDistance / echoSpeedDivisor, 10f, 5000f);
        source.GetComponent<AudioEchoFilter>().dryMix = dryMix;
        source.GetComponent<AudioEchoFilter>().wetMix = wetMix;
        Debug.Log("Average Distance = " + averageDistance);
        Debug.Log("Echo Speed Divisor = " + echoSpeedDivisor);
        Debug.Log("Delay = " + Mathf.Clamp(averageDistance / echoSpeedDivisor, 10f, 5000f));
        source.GetComponent<AudioEchoFilter>().enabled = true;
    }
}
