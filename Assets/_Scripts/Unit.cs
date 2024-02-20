using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	public string unitName;
	public int unitLevel;

	public int RP;
	public int BP;
	public int GP;

	public int damage;

	public int maxHP;
	public int currentHP;

	public int StatPoint;
	public int NeedEXP;
	public int CurretEXP;

	public int EXP;


    private void Update()
    {
		NeedEXP = 10 + (unitLevel * 4 / 3);

		if (currentHP > maxHP)
			currentHP = maxHP;
    }

    public bool TakeRDamage(int RP, int BP, int GP)
	{
		int Tdmg = RP + BP + GP;
		currentHP -= Tdmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

    public void Dead()
	{
		Destroy(gameObject);
	}


}
