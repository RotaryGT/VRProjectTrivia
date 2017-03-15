using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterScript : MonoBehaviour
{
    public string Destination = "VRTrivia";
    // Use this for initialization
    void Start()
    {
        Debug.Log("OverlapNotifier started!");
    }
    // NOTE: Capital "O" in OnTriggerEnter
    void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("Something overlapped me!");
            SceneManager.LoadScene(Destination);
        }
    }
}