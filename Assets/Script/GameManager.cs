using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text curScoreText;
    public Text BestText;
    public GameObject menuSet;
    
    public AudioSource bgm1;
    public AudioSource bgm2;


    
    void Awake()
    { 
        bgm1.Play();
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    { 
      
       
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);

    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            BestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        bgm2.Stop();
        bgm1.Play();
        SceneManager.LoadScene(0);
    }


    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuSet.activeSelf)
            {
                menuSet.SetActive(false);
            }
            else
            {
                menuSet.SetActive(true);
            }
        }

    }

    public void GameSave()
    {
        PlayerPrefs.SetInt("Coin",player.coin);
        PlayerPrefs.SetInt("Ammo",player.ammo);
        PlayerPrefs.SetInt("Health",player.health);
        PlayerPrefs.SetInt("Score",player.score);
        PlayerPrefs.SetInt("Stage", stage);
        int a =Convert.ToInt32(player.hasWeapons[0]);
        int b =Convert.ToInt32(player.hasWeapons[1]);
        int c =Convert.ToInt32(player.hasWeapons[2]);
        PlayerPrefs.SetInt("HasGrenades", player.hasGrenades);

        PlayerPrefs.SetInt("hasWeapons1",a);
        PlayerPrefs.SetInt("hasWeapons2",b);
        PlayerPrefs.SetInt("hasWeapons3",c);
        menuSet.SetActive(false);

    }

    public void GameLoad()
    {
       int a= PlayerPrefs.GetInt("Coin");
       int b= PlayerPrefs.GetInt("Ammo");
       int c=PlayerPrefs.GetInt("Health");
       int d=PlayerPrefs.GetInt("Score");
       int e= PlayerPrefs.GetInt("hasWeapons1");
       int f= PlayerPrefs.GetInt("hasWeapons2");
       int g= PlayerPrefs.GetInt("hasWeapons3");
       int i=PlayerPrefs.GetInt("HasGrenades");

       stage =  PlayerPrefs.GetInt("Stage");
       player.coin = a;
       player.ammo = b;
       player.health = c;
       player.score = d;
       player.hasWeapons[0] = Convert.ToBoolean(e);;
       player.hasWeapons[1] = Convert.ToBoolean(f);;
       player.hasWeapons[2] = Convert.ToBoolean(g);;
       player.hasGrenades = i;
    }

    public void GameExit()
    {
        Application.Quit();
    }
    public void StageStart()
    {
 
       
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {      
       bgm2.Stop();
       bgm1.Play();
        player.transform.position = Vector3.up * 1.18f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;

    }

    IEnumerator InBattle()
    {
        bgm1.Stop();
        bgm2.loop = true;
        bgm2.Play();
        
        if(stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for(int index=0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while(enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }
        
        while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    void LateUpdate()
    {
        //상단 UI
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int minute = (int)(playTime / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", minute)
                            + ":" + string.Format("{0:00}", second);

        //플레이어 UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if(player.equipWeapon == null)
            playerAmmoTxt.text = "- / " + player.ammo;
        
        else if(player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        //무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //몬스터 숫자 UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();
        
        //보스 체력 UI
        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else{
            bossHealthGroup.anchoredPosition = Vector2.up * 200;

        }
    }
}
