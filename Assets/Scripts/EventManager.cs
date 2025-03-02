using System;
using UnityEngine;
using System.Diagnostics;


public static class EventManager
{
    // Define an event that takes an Enemy as parameter
    public static event Action<Enemy> EnemyDeathEvent;
    public static event Action OnCastleDeathEvent;

    public static event Action<Tower> OnTowerBuyEvent;
    public static event Action<Tower> OnTowerReturnEvent;

    public static event Action<float> OnCaslteHitEvent;

    public static event Action OnGameWonEvent;
    // Method to trigger the enemy death event
    public static void OnEnemyDeath(Enemy enemy)
    {
        EnemyDeathEvent?.Invoke(enemy);
    }

    public static void OnCastleDeath ()
    {
        OnCastleDeathEvent?.Invoke ();
    }    

    public static void OnTowerBuy (Tower tower)
    {
        OnTowerBuyEvent?.Invoke (tower);
    }

    public static void OnTowerReturn (Tower tower)
    {
        OnTowerReturnEvent?.Invoke (tower);
    }

    public static void OnCastleHit (float damage)
    {
        OnCaslteHitEvent?.Invoke (damage);
    }

    public static void OnGameWon ()
    {
        OnGameWonEvent?.Invoke ();
    }
}
