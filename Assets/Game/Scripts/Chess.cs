using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour
{
    [SerializeField] private GameObject chessOpen;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            transform.gameObject.SetActive(false);
            chessOpen.SetActive(true);
        }
    }
}
