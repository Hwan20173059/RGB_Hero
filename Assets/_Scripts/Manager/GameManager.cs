using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, TALK, NEXTSTAGE, WON, LOST }

public class GameManager : MonoBehaviour
{
    [Header("Manager")]
    public RoulletManager roulletManager;
    public TalkManager talkManager;
    public SoundManager soundManager;

    [Header("State")]
    public BattleState state;

    [Header("Stage")]
    public int Stage = 1;

	bool isCombat = true;

    [Header("Animation")]
	public Animator PlayerAnim;
    public Animator EnemyAnim;

    [Header("Prefab")]
    public GameObject playerPrefab;
	public GameObject[] enemyPrefab;

    [Header("Battle Field")]
    public Transform playerBattleStation;
	public Transform enemyBattleStation;
	Unit playerUnit;
	Unit enemyUnit;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("Battle UI")]
    public Text dialogueText;
    public Text StageText;

    public Text PlayerLevel;
    public Slider EXPSlider;
    public Text EXPText;

    [Header("Talk UI")]
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

    [Header("Inventory")]
    public int Fruit;
    public Text FruitText;


    void Start()
    {
        isCombat = false;
        state = BattleState.START;
        textId = 100;
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        // 경험치 검사 및 레벨 업
        if (playerUnit.CurretEXP >= playerUnit.NeedEXP)
        {
            playerUnit.CurretEXP -= playerUnit.NeedEXP;
            playerUnit.unitLevel++;
            playerUnit.StatPoint += 5;
        }

        EXPSlider.maxValue = playerUnit.NeedEXP;
        EXPSlider.value = playerUnit.CurretEXP;
        EXPText.text = playerUnit.CurretEXP.ToString() + " / " + playerUnit.NeedEXP.ToString();

        // 레벨, 스텟, 스텟포인트 표시
        PlayerLevel.text = playerUnit.unitLevel.ToString();
        roulletManager.RP.text = playerUnit.RP.ToString();
        roulletManager.GP.text = playerUnit.GP.ToString();
        roulletManager.BP.text = playerUnit.BP.ToString();
        roulletManager.PointText.text = "Point : " + playerUnit.StatPoint.ToString();
        playerHUD.SetHUD(playerUnit);

        // 가방 아이템 텍스트
        FruitText.text = Fruit.ToString();

        // 스테이지 표시
        StageText.text = "Stage " + Stage.ToString();
    }

    // 첫 전투 (튜토리얼은 슬라임 enemyPrefab[0] 확정 등장)
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

    // 룰렛 버튼 가동
    IEnumerator PlayerAttack()
	{
        soundManager.PlaySound("Button");

        if (isCombat)
		{
            isCombat = false;

            int TotalDamage = 0;
            bool isDead = true;

            // 공격 타입 랜덤 결정
            int AttackType1 = Random.Range(0, 3);
			int AttackType2 = Random.Range(0, 3);
			int AttackType3 = Random.Range(0, 3);

            // 총 데미지 계산
            TotalDamage += AttackType(AttackType1);
            TotalDamage += AttackType(AttackType2);
            TotalDamage += AttackType(AttackType3);            


            // 룰렛 텍스트 처리
            dialogueText.text = "룰렛 가동!";

            yield return new WaitForSeconds(1.5f);

            roulletManager.RoulletText(roulletManager.Roullet1,AttackType1);
            soundManager.PlaySound("Button");

            yield return new WaitForSeconds(0.5f);

            roulletManager.RoulletText(roulletManager.Roullet2, AttackType2);
            soundManager.PlaySound("Button");

            yield return new WaitForSeconds(0.5f);

            roulletManager.RoulletText(roulletManager.Roullet3, AttackType3);
            soundManager.PlaySound("Button");


            // 타입에 따른 공격 처리 텍스트 표시 ( RRR, GGG, BBB 특수 패턴 + 일반 패턴 )
            AttackText(AttackType1, AttackType2, AttackType3);
            yield return new WaitForSeconds(0.5f);

            AttackAction(AttackType1, AttackType2, AttackType3);
            yield return new WaitForSeconds(1f);

            HitAction(AttackType1, AttackType2, AttackType3);

            if (AttackType1 == AttackType2 && AttackType2 == AttackType3)
            {
                isDead = enemyUnit.TakeDamage(TotalDamage * 2);
                dialogueText.text = TotalDamage * 2 + "의 데미지!";
            }
            else
            {
                isDead = enemyUnit.TakeDamage(TotalDamage);
                dialogueText.text = TotalDamage + "의 데미지!";
            }


            // 타입에 따른 공격 처리
            enemyHUD.SetHP(enemyUnit.currentHP);
            yield return new WaitForSeconds(1f);

            // 룰렛 비우기
            roulletManager.RoulletClear();

            // 사망 처리 검사
            if (isDead)
			{
				state = BattleState.WON;
				EndBattle();
			}
			else // 턴 넘기기
			{
				state = BattleState.ENEMYTURN;
				StartCoroutine(EnemyTurn());
			}
		}
    }

    // 적의 턴
	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + "의 공격!";
        EnemyAnim.SetBool("isAttack", true);
        PlayerAnim.SetBool("isHit", true);
        soundManager.PlaySound("Attack");

        yield return new WaitForSeconds(1f);
        EnemyAnim.SetBool("isAttack", false);
        PlayerAnim.SetBool("isHit", false);

        dialogueText.text = enemyUnit.damage + "의 데미지!";
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
		playerHUD.SetHP(playerUnit.currentHP);
        soundManager.PlaySound("Hit");

        yield return new WaitForSeconds(1f);

        // 캐릭터 사망 처리 검사
		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} 
        else // 턴 넘기기
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}
	}

    // 다음 스테이지 진행
    IEnumerator NEXTSTAGE()
	{
        yield return new WaitForSeconds(1f);
        
		enemyUnit.Dead();
        dialogueText.text = "STAGE " + Stage + "도착";

        yield return new WaitForSeconds(1f);

        int n = 0;

        // 스테이지에 따라 몬스터 랜덤 결정
        if (Stage == 10)
        {
            n = 5; // 보스 몬스터
        }
        else if (Stage < 10 && Stage >= 6) 
        {
            n = Random.Range(2, 5);
        }
        else if (Stage < 6)
        {
            n = Random.Range(0, 3);
        }

        // 몬스터 소환, 세팅
        GameObject enemyGO = Instantiate(enemyPrefab[n], enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        EnemyAnim = enemyGO.GetComponent<Animator>();

        dialogueText.text = enemyUnit.unitName + "가 나타났다!";

        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // 대화 UI 상태
    IEnumerator TALK(int textId)
    {
        talkBackground.SetActive(true);
        talkPanel.SetBool("isShow",true);

        // Talk Index를 받아 Talk UI 표시 (EndBattle 함수 참고)
        string talkData = talkManager.GetTalk(textId, talkIndex);

        // Talk Data를 모두 사용할때 Talk UI를 종료함
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

    // 배틀에서 승리
    void EndBattle()
	{
        if (state == BattleState.WON)
		{
            EnemyAnim.SetBool("isDie", true);
            dialogueText.text = "배틀에서 이겼다!";

            SetEXP(playerUnit.EXP);
            playerUnit.CurretEXP += enemyUnit.EXP;

            Stage++;

            soundManager.PlaySound("Clear");

            // 지정된 스테이지를 클리어 할시 Talk 이벤트 발생
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
		else if (state == BattleState.LOST) // 배틀에서 졌을 때
		{
            PlayerAnim.SetBool("isDie", true);
            dialogueText.text = "배틀에서 져버렸다...";

            Invoke("GameOver", 1f);
        }
	}


    int AttackType(int AttackType)
    {
        if (AttackType == 0)
            return playerUnit.RP;
        else if (AttackType == 1)
            return playerUnit.GP;
        else if (AttackType == 2)
            return playerUnit.BP;
        else
            return 0;
    }

    public void AttackText(int AttackType1, int AttackType2, int AttackType3)
    {
        if (AttackType1 == 0 && AttackType2 == 0 && AttackType3 == 0)
            dialogueText.text = "인페르노 레드!!";
        else if (AttackType1 == 1 && AttackType2 == 1 && AttackType3 == 1)
            dialogueText.text = "토네이도 그린!!";
        else if (AttackType1 == 2 && AttackType2 == 2 && AttackType3 == 2)
            dialogueText.text = "레비아탄 블루!!";
        else
            return;
    }

    public void AttackAction(int AttackType1, int AttackType2, int AttackType3)
    {
        if (AttackType1 == 0 && AttackType2 == 0 && AttackType3 == 0)
        {
            PlayerAnim.SetBool("RRRAttack", true);
            EnemyAnim.SetBool("isHit", true);
            soundManager.PlaySound("Attack");
        }
        else if (AttackType1 == 1 && AttackType2 == 1 && AttackType3 == 1)
        {
            PlayerAnim.SetBool("BBBAttack", true);
            EnemyAnim.SetBool("isHit", true);
            soundManager.PlaySound("Attack");
        }
        else if (AttackType1 == 2 && AttackType2 == 2 && AttackType3 == 2)
        {
            PlayerAnim.SetBool("GGGAttack", true);
            EnemyAnim.SetBool("isHit", true);
            soundManager.PlaySound("Attack");
        }
        else 
        {
            PlayerAnim.SetBool("isAttack", true);
            EnemyAnim.SetBool("isHit", true);
            soundManager.PlaySound("Attack");
        }
    }

    public void HitAction(int AttackType1, int AttackType2, int AttackType3)
    {
        if (AttackType1 == 0 && AttackType2 == 0 && AttackType3 == 0)
        {
            PlayerAnim.SetBool("RRRAttack", false);
            EnemyAnim.SetBool("isHit", false);
            soundManager.PlaySound("Hit");
        }
        else if (AttackType1 == 1 && AttackType2 == 1 && AttackType3 == 1)
        {
            PlayerAnim.SetBool("BBBAttack", false);
            EnemyAnim.SetBool("isHit", false);
            soundManager.PlaySound("Hit");
        }
        else if (AttackType1 == 2 && AttackType2 == 2 && AttackType3 == 2)
        {
            PlayerAnim.SetBool("GGGAttack", false);
            EnemyAnim.SetBool("isHit", false);
            soundManager.PlaySound("Hit");
        }
        else
        {
            PlayerAnim.SetBool("isAttack", false);
            EnemyAnim.SetBool("isHit", false);
            soundManager.PlaySound("Hit");
        }
    }

    // 플레이어 턴 (조작 가능)
    void PlayerTurn()
	{
		isCombat = true;
        dialogueText.text = "무엇을 할까? ";
	}

    // 룰렛 가동 버튼 누를시
	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}

    // Text UI 클릭시
	public void TextButtonClick()
	{
        StartCoroutine(TALK(textId));
    }

    // 스텟 버튼 클릭시
    public void StatButtonClick()
    {
        if (isCombat)
        {
            soundManager.PlaySound("Button");
            StatUI.SetActive(true);
        }
    }

    // 스텟 UI 나가기 버튼 클릭시
    public void UICloseButtonClick()
    {
        soundManager.PlaySound("Button");
        StatUI.SetActive(false);
    }

    // 스텟 R 포인트 상승 버튼
    public void RPup()
    {
        if (playerUnit.StatPoint > 0)
        {
            soundManager.PlaySound("Button");
            playerUnit.RP++;
            playerUnit.StatPoint--;
        }
        return;
    }

    // 스텟 G 포인트 상승 버튼
    public void GPup()
    {
        if (playerUnit.StatPoint > 0)
        {
            soundManager.PlaySound("Button");
            playerUnit.GP++;
            playerUnit.StatPoint--;
        }
        return;
    }

    // 스텟 B 포인트 상승 버튼
    public void BPup()
    {
        if (playerUnit.StatPoint > 0)
        {
            soundManager.PlaySound("Button");
            playerUnit.BP++;
            playerUnit.StatPoint--;
        }
        return;
    }

    // 메뉴 버튼 클릭
    public void MenuButtonClick()
    {
        soundManager.PlaySound("Button");
        GameMenuUI.SetActive(true);
    }

    // 메뉴 나가기 버튼
    public void MenuCloseButtonClick()
    {
        soundManager.PlaySound("Button");
        GameMenuUI.SetActive(false);
    }

    // 경험치 세팅
    public void SetEXP(int EXP)
    {
        EXPSlider.value = EXP;
    }

    // 가방 버튼 클릭
    public void BagButtonClick()
    {
        if (isCombat)
        {
            soundManager.PlaySound("Button");
            GameBagUI.SetActive(true);
        }
    }

    // 가방 나가기 버튼
    public void BagCloseButtonClick()
    {
        soundManager.PlaySound("Button");
        GameBagUI.SetActive(false);
    }

    // 과일 아이템 사용
    public void UseFruit()
    { 
        if (Fruit > 0)
        {
            soundManager.PlaySound("Button");
            playerUnit.currentHP += 10;
            Fruit--;
        }
        return;
    }

    // 게임 다시하기 버튼
    public void GaemRetry()
    {
        SceneManager.LoadScene("MainScene");
    }

    // 게임 나가기 버튼
    public void GaemOver()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 게임 오버 UI 출력
    void GameOver() 
    {
        GameOverUI.SetActive(true);
    }

    // 메뉴 버튼 클릭
    void MenuButton()
    {
        GameMenuUI.SetActive(true);
    }
}
