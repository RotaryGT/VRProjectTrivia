using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour {
    public string Name = "Flames";
    public BeInvisible inv;
    public bool wasOverlapping;
    public bool isOverlapping;
    void Start()
    {
        isOverlapping = false;
        wasOverlapping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOverlapping && !wasOverlapping)
        {
            inv.ToggleVisible();
        }
        wasOverlapping = isOverlapping;
        isOverlapping = false;
    }
    // NOTE: Capital "O" in OnTriggerEnter
    public void OnTriggerEnter(Collider other)
    {
        isOverlapping = true;
    }
}
