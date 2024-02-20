using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoulletManager : MonoBehaviour
{
    [Header("Roullet")]
    public Text Roullet1;
    public Text Roullet2;
    public Text Roullet3;

    public Text RP;
    public Text GP;
    public Text BP;
    public Text PointText;

    public void RoulletPlay(Text index)
    {
        int AttackType = Random.Range(0, 3);
        RoulletText(index, AttackType);
    }

    public void RoulletClear()
    {
        Roullet1.text = "";
        Roullet2.text = "";
        Roullet3.text = "";
    }

    public void RoulletText(Text index, int type)
    {
        switch(type)
        {
            case 0:
                index.text = "<color=red>R</color>";
                break;

            case 1:
                index.text = "<color=green>G</color>";
                break;

            case 2:
                index.text = "<color=blue>B</color>";
                break;
        }
    }
}
