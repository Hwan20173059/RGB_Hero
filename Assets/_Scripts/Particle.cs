using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("SetActiveFalse", 1f);
    }

    void SetActiveFalse()
    {
        this.gameObject.SetActive(false);
    }
}
