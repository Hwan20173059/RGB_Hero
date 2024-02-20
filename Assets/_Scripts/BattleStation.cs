using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStation : MonoBehaviour
{
    public GameManager battlesystem;
    public Animator AnimBattleStation;

    void Update()
    {
        if (battlesystem.Stage < 6)
        {
            AnimBattleStation.SetTrigger("BattleStation1-1");
        }
        else if (battlesystem.Stage < 10)
        {
            AnimBattleStation.SetTrigger("BattleStation1-2");
        }
        else if (battlesystem.Stage == 10)
        {
            AnimBattleStation.SetTrigger("BattleStation1-3");
        }
    }
}
