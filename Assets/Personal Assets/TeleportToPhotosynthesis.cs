using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToPhotosynthesis : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Debug.Log("OverlapNotifier started!");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("OverlapNotifier Tick");
    }
    // NOTE: Capital "O" in OnTriggerEnter
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            Debug.Log("Camera overlapped me!");
        }
        else
        {
            Debug.Log("Something overlapped me!");
            SceneManager.LoadScene("Photosynthesis");
        }
    }
}