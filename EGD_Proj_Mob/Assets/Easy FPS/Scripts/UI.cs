using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Color interfaceColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUI()
    {
        //GameObject.Find("Points Frame").GetComponent<Image>().color = interfaceColor;
        this.transform.Find("Enemy Frames").GetComponent<Image>().color = interfaceColor;
        this.transform.Find("Bullets Frame").GetComponent<Image>().color = interfaceColor;
        this.transform.Find("Character Portrait").GetComponent<Image>().color = interfaceColor;
        this.transform.Find("Bullet1").GetComponent<Image>().color = interfaceColor;
        //GameObject.Find("Bullet2").GetComponent<Image>().color = interfaceColor;
        //GameObject.Find("Bullet3").GetComponent<Image>().color = interfaceColor;
        //this.transform.Find("Health").GetComponent<Text>().color = interfaceColor;
        //this.transform.Find("Ammo").GetComponent<Text>().color = interfaceColor;
        transform.Find("Points Frame").GetComponent<Image>().color = interfaceColor;
        transform.Find("Health Frame").GetComponent<Image>().color = interfaceColor;
    }
}
