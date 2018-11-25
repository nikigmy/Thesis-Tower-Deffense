using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Enemy {

    private void Awake()
    {
        enemyData = Def.Instance.EnemyDictionary[Declarations.EnemyType.Swordsman];
        Init();
    }

    protected override void Died()
    {
        Alive = false;
        GameManager.instance.SpawnManager.EnemyDestroyed(this);
        GameManager.instance.AddMoney(enemyData.Award);
        anim.speed = 1;
        anim.SetTrigger("Died");
    }

    public void DieAnimationEnded()
    {
        Destroy(gameObject);
    }
}
