using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeInvisible : MonoBehaviour
{

    // Use this for initialization
    public void Start()
    {
        this.gameObject.GetComponent<Renderer>().enabled = false;
    }
    public void ToggleVisible()
    {
        if (this.gameObject.GetComponent<Renderer>().enabled == false) {
                this.gameObject.GetComponent<Renderer>().enabled = true;
        }
        else {
                this.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
}