using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public GameManager battlesystem;
    public Animator Background;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (battlesystem.Stage < 6)
        {
            Background.SetTrigger("Stage1-1");
        }
        else if (battlesystem.Stage < 10)
        {
            Background.SetTrigger("Stage1-2");
        }
        else if (battlesystem.Stage == 10)
        {
            Background.SetTrigger("Stage1-3");
        }
    }
}
