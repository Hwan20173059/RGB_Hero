using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, TALK, NEXTSTAGE, WON, LOST }

public class GameManager : MonoBehaviour
{
    [Header("Stage")]
    public int Stage = 1;

	bool isCombat = true;

    [Header("Animation")]
	public Animator PlayerAnim;
    public Animator EnemyAnim;

    [Header("Prefab")]
    public GameObject playerPrefab;
	public GameObject[] enemyPrefab;

    [Header("Roullet")]
    public Text Roullet1;
    public Text Roullet2;
    public Text Roullet3;

    public Text RP;
    public Text GP;
    public Text BP;
    public Text PointText;

    [Header("Battle Field")]
    public Transform playerBattleStation;
	public Transform enemyBattleStation;
	Unit playerUnit;
	Unit enemyUnit;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("UI")]
    public Text dialogueText;
    public Text StageText;

    public Text PlayerLevel;
    public Slider EXPSlider;
    public Text EXPText;

    public TalkManager talkManager;

    public Image portraitImg;
    public Animator portraitEffect;
    public Sprite prevSprite;

    int textId;
    public int talkIndex;

    [Header("UI Object")]
    public Animator talkPanel;
    public GameObject talkBackground;
    public GameObject StatUI;
    public GameObject GameOverUI;
    public GameObject GameMenuUI;
    public GameObject GameBagUI;

    [Header("Type Effect")]
    public TypeEffect talkText;

    [Header("State")]
    public BattleState state;

    [Header("Bag")]
    public int Fruit;
    public Text FruitText;

    [Header("Sound")]
    public AudioSource Audio;
	public AudioClip Attack, Hit, Clear, Button;

    void Start()
    {
        isCombat = false;
        state = BattleState.START;
        textId = 100;
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (playerUnit.CurretEXP >= playerUnit.NeedEXP)
        {
            playerUnit.CurretEXP -= playerUnit.NeedEXP;
            playerUnit.unitLevel++;
            playerUnit.StatPoint += 5;
        }

        PlayerLevel.text = playerUnit.unitLevel.ToString();
        RP.text = playerUnit.RP.ToString();
        GP.text = playerUnit.GP.ToString();
        BP.text = playerUnit.BP.ToString();
        PointText.text = "Point : " + playerUnit.StatPoint.ToString();
        playerHUD.SetHUD(playerUnit);

        FruitText.text = Fruit.ToString();

        StageText.text = "Stage " + Stage.ToString();

        EXPSlider.maxValue = playerUnit.NeedEXP;
        EXPSlider.value = playerUnit.CurretEXP;
        EXPText.text = playerUnit.CurretEXP.ToString() + " / " + playerUnit.NeedEXP.ToString();
    }

    IEnumerator SetupBattle()
	{
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
        PlayerAnim = playerGO.GetComponent<Animator>();

        GameObject enemyGO = Instantiate(enemyPrefab[0], enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();
        EnemyAnim = enemyGO.GetComponent<Animator>();

        enemyHUD.SetHUD(enemyUnit);

        dialogueText.text = enemyUnit.unitName + "가 나타났다!";

		yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;

        textId = 100;
        StartCoroutine(TALK(textId));
    }
    IEnumerator PlayerAttack()
	{
        Audio.PlayOneShot(Button);

        if (isCombat)
		{
            isCombat = false;
            int TotalDamage = 0;

			int AttackType1 = Random.Range(0, 3);
			int AttackType2 = Random.Range(0, 3);
			int AttackType3 = Random.Range(0, 3);

			// 데미지 계산 처리
			if (AttackType1 == 0) { TotalDamage += playerUnit.RP; }
			else if (AttackType1 == 1) { TotalDamage += playerUnit.BP; }
			else { TotalDamage += playerUnit.GP; }

			if (AttackType2 == 0) { TotalDamage += playerUnit.RP; }
			else if (AttackType2 == 1) { TotalDamage += playerUnit.BP; }
			else { TotalDamage += playerUnit.GP; }

			if (AttackType3 == 0) { TotalDamage += playerUnit.RP; }
			else if (AttackType3 == 1) { TotalDamage += playerUnit.BP; }
			else { TotalDamage += playerUnit.GP; }

			// 적 처치 상태 bool 처리
			bool isDead = enemyUnit.TakeDamage(TotalDamage);

            // 룰렛 텍스트 처리
            dialogueText.text = "룰렛 가동!";

            yield return new WaitForSeconds(1.5f);

            if (AttackType1 == 0) { Roullet1.text = "<color=red>R</color>"; }
			else if (AttackType1 == 1) { Roullet1.text = "<color=blue>B</color>"; }
			else { Roullet1.text = "<color=green>G</color>"; }
			Audio.PlayOneShot(Button);

            yield return new WaitForSeconds(0.5f);

            if (AttackType2 == 0) { Roullet2.text = "<color=red>R</color>"; }
			else if (AttackType2 == 1) { Roullet2.text = "<color=blue>B</color>"; }
			else { Roullet2.text = "<color=green>G</color>"; }
            Audio.PlayOneShot(Button);

            yield return new WaitForSeconds(0.5f);

			if (AttackType3 == 0) { Roullet3.text = "<color=red>R</color>"; }
			else if (AttackType3 == 1) { Roullet3.text = "<color=blue>B</color>"; }
			else { Roullet3.text = "<color=green>G</color>"; }
            Audio.PlayOneShot(Button);

            yield return new WaitForSeconds(0.5f);

            if (AttackType1 == 0 && AttackType2 == 0 && AttackType3 == 0)
            {
                dialogueText.text = "인페르노 레드!!";

                yield return new WaitForSeconds(1f);

                // 애니메이션 처리
                PlayerAnim.SetBool("isAttack", true);
                EnemyAnim.SetBool("isHit", true);
                Audio.PlayOneShot(Attack);

                yield return new WaitForSeconds(1f);

                PlayerAnim.SetBool("isAttack", false);
                EnemyAnim.SetBool("isHit", false);
                Audio.PlayOneShot(Hit);

                isDead = enemyUnit.TakeDamage(TotalDamage);

                dialogueText.text = TotalDamage * 2 + "의 데미지!";
                enemyHUD.SetHP(enemyUnit.currentHP);
            }
            else if (AttackType1 == 1 && AttackType2 == 1 && AttackType3 == 1)
            {
                dialogueText.text = "레비아탄 블루!!";

                yield return new WaitForSeconds(1f);

                // 애니메이션 처리
                PlayerAnim.SetBool("isAttack", true);
                EnemyAnim.SetBool("isHit", true);
                Audio.PlayOneShot(Attack);

                yield return new WaitForSeconds(1f);

                PlayerAnim.SetBool("isAttack", false);
                EnemyAnim.SetBool("isHit", false);
                Audio.PlayOneShot(Hit);

                isDead = enemyUnit.TakeDamage(TotalDamage);

                dialogueText.text = TotalDamage * 2 + "의 데미지!";
                enemyHUD.SetHP(enemyUnit.currentHP);
            }
            else if (AttackType1 == 2 && AttackType2 == 2 && AttackType3 == 2)
            {
                dialogueText.text = "토네이도 그린!!";

                yield return new WaitForSeconds(1f);

                // 애니메이션 처리
                PlayerAnim.SetBool("isAttack", true);
                EnemyAnim.SetBool("isHit", true);
                Audio.PlayOneShot(Attack);

                yield return new WaitForSeconds(1f);

                PlayerAnim.SetBool("isAttack", false);
                EnemyAnim.SetBool("isHit", false);
                Audio.PlayOneShot(Hit);

                isDead = enemyUnit.TakeDamage(TotalDamage);

                dialogueText.text = TotalDamage * 2 + "의 데미지!";
                enemyHUD.SetHP(enemyUnit.currentHP);
            }
            else
            {
                // 애니메이션 처리
                PlayerAnim.SetBool("isAttack", true);
                EnemyAnim.SetBool("isHit", true);
                Audio.PlayOneShot(Attack);

                yield return new WaitForSeconds(1f);

                PlayerAnim.SetBool("isAttack", false);
                EnemyAnim.SetBool("isHit", false);
                Audio.PlayOneShot(Hit);

                // 로그 출력 처리
                dialogueText.text = TotalDamage + "의 데미지!";
                enemyHUD.SetHP(enemyUnit.currentHP);
            }

            yield return new WaitForSeconds(1f);

            // 룰렛 비우기
            Roullet1.text = " ";
            Roullet2.text = " ";
            Roullet3.text = " ";

            if (isDead)
			{
				state = BattleState.WON;
				EndBattle();
			}
			else
			{
				state = BattleState.ENEMYTURN;
				StartCoroutine(EnemyTurn());
			}
		}
    }
	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + "의 공격!";
        EnemyAnim.SetBool("isAttack", true);
        PlayerAnim.SetBool("isHit", true);
        Audio.PlayOneShot(Attack);

        yield return new WaitForSeconds(1f);
        EnemyAnim.SetBool("isAttack", false);
        PlayerAnim.SetBool("isHit", false);

        dialogueText.text = enemyUnit.damage + "의 데미지!";
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
		playerHUD.SetHP(playerUnit.currentHP);
        Audio.PlayOneShot(Hit);

        yield return new WaitForSeconds(1f);

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}
	}
    IEnumerator NEXTSTAGE()
	{
        yield return new WaitForSeconds(1f);
        
		enemyUnit.Dead();
        dialogueText.text = "STAGE " + Stage + "도착";

        yield return new WaitForSeconds(1f);

        int n = 0;

        if (Stage == 10)
        {
            n = 5;
            
        }
        else if (Stage < 10 && Stage >= 6) 
        {
            n = Random.Range(2, 5);
        }
        else if (Stage < 6)
        {
            n = Random.Range(0, 3);
        }

        GameObject enemyGO = Instantiate(enemyPrefab[n], enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        EnemyAnim = enemyGO.GetComponent<Animator>();

        dialogueText.text = enemyUnit.unitName + "가 나타났다!";

        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    IEnumerator TALK(int textId)
    {
        talkBackground.SetActive(true);
        talkPanel.SetBool("isShow",true);

        string talkData = talkManager.GetTalk(textId, talkIndex);

        if (Stage == 1)
        {
            if (talkData == null)
            {
                talkBackground.SetActive(false);
                talkPanel.SetBool("isShow", false);
                talkIndex = 0;
                PlayerTurn();
            }
        }
        else if (Stage == 2)
        {
            if (talkData == null)
            {
                talkBackground.SetActive(false);
                talkPanel.SetBool("isShow", false);
                talkIndex = 0;
                StartCoroutine(NEXTSTAGE());
            }
        }
        else if (Stage == 5)
        {
            if (talkData == null)
            {
                talkBackground.SetActive(false);
                talkPanel.SetBool("isShow", false);
                talkIndex = 0;
                StartCoroutine(NEXTSTAGE());
            }
        }
        else if (Stage == 10)
        {
            if (talkData == null)
            {
                talkBackground.SetActive(false);
                talkPanel.SetBool("isShow", false);
                talkIndex = 0;
                StartCoroutine(NEXTSTAGE());
            }
        }

        talkText.SetMsg(talkData.Split(':')[0]);

        portraitImg.sprite = talkManager.GetPortrait(textId, int.Parse(talkData.Split(':')[1]));
        if (prevSprite != portraitImg.sprite)
        {
            portraitEffect.SetTrigger("doEffect");
            prevSprite = portraitImg.sprite;
        }

        yield return new WaitForSeconds(0.1f);
    }


    void EndBattle()
	{
        if (state == BattleState.WON)
		{
            EnemyAnim.SetBool("isDie", true);
            dialogueText.text = "배틀에서 이겼다!";

            SetEXP(playerUnit.EXP);
            playerUnit.CurretEXP += enemyUnit.EXP;

            Stage++;

            Audio.PlayOneShot(Clear);

            if (Stage == 2)
            {
                textId = 200;
                StartCoroutine(TALK(textId));
            }
            else if (Stage == 5)
            {
                textId = 300;
                StartCoroutine(TALK(textId));
            }
            else if (Stage == 10)
            {
                textId = 400;
                StartCoroutine(TALK(textId));
            }
            else
            {
                StartCoroutine(NEXTSTAGE());
            }
        } 
		else if (state == BattleState.LOST)
		{
            PlayerAnim.SetBool("isDie", true);
            dialogueText.text = "배틀에서 져버렸다...";

            Invoke("GameOver", 1f);
        }
	}
	void PlayerTurn()
	{
		isCombat = true;
        dialogueText.text = "무엇을 할까? ";
	}


	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}
	public void TextButtonClick()
	{
        StartCoroutine(TALK(textId));
    }
    public void StatButtonClick()
    {
        if (isCombat)
        {
            Audio.PlayOneShot(Button);
            StatUI.SetActive(true);
        }
    }
    public void UICloseButtonClick()
    {
        Audio.PlayOneShot(Button);
        StatUI.SetActive(false);
    }
    public void RPup()
    {
        if (playerUnit.StatPoint > 0)
        {
            Audio.PlayOneShot(Button);
            playerUnit.RP++;
            playerUnit.StatPoint--;
        }
        return;
    }
    public void GPup()
    {
        if (playerUnit.StatPoint > 0)
        {
            Audio.PlayOneShot(Button);
            playerUnit.GP++;
            playerUnit.StatPoint--;
        }
        return;
    }
    public void BPup()
    {
        if (playerUnit.StatPoint > 0)
        {
            Audio.PlayOneShot(Button);
            playerUnit.BP++;
            playerUnit.StatPoint--;
        }
        return;
    }
    public void MenuButtonClick()
    {
        Audio.PlayOneShot(Button);
        GameMenuUI.SetActive(true);
    }
    public void MenuCloseButtonClick()
    {
        Audio.PlayOneShot(Button);
        GameMenuUI.SetActive(false);
    }
    public void SetEXP(int EXP)
    {
        EXPSlider.value = EXP;
    }
    public void BagButtonClick()
    {
        if (isCombat)
        {
            Audio.PlayOneShot(Button);
            GameBagUI.SetActive(true);
        }
    }
    public void BagCloseButtonClick()
    {
        Audio.PlayOneShot(Button);
        GameBagUI.SetActive(false);
    }
    public void UseFruit()
    { 
        if (Fruit > 0)
        {
            Audio.PlayOneShot(Button);
            playerUnit.currentHP += 10;
            Fruit--;
        }
        return;
    }
    public void GaemRetry()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void GaemOver()
    {
        SceneManager.LoadScene("TitleScene");
    }
    void GameOver() 
    {
        GameOverUI.SetActive(true);
    }
    void MenuButton()
    {
        GameMenuUI.SetActive(true);
    }
}
