using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public int gold;
    [SerializeField] private TMP_Text goldText;
    void OnEnable ()
    {
        EventManager.EnemyDeathEvent += addGoldOnEnemyDeath;
        EventManager.OnTowerBuyEvent += removeGoldOnTowerBuy;
        EventManager.OnTowerReturnEvent += addGoldOnTowerReturn;
        goldText.text = gold.ToString ();
    }

    void OnDisable ()
    {
        EventManager.EnemyDeathEvent -= addGoldOnEnemyDeath;
    }

    public void removeGold (int amount)
    {
        gold -= amount;
        goldText.text = gold.ToString ();
    }

    private void addGoldOnEnemyDeath (Enemy enemy)
    {
        gold += enemy.carriedMoney;
        goldText.text = gold.ToString();
    }

    private void removeGoldOnTowerBuy (Tower tower)
    {
        gold -= tower.cost;
        goldText.text = gold.ToString();
    }

    private void addGoldOnTowerReturn (Tower tower)
    {
        gold += tower.cost;
        goldText.text = gold.ToString ();
    }
}
