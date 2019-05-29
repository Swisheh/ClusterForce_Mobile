using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    public float delay;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<AudioSource>().PlayDelayed(delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
