using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    public int speed;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
