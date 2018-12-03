using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy {

    private void Awake()
    {
        enemyData = Def.Instance.EnemyDictionary[Declarations.EnemyType.Golem];
        Init();
    }

    protected override void Died()
    {
        base.Died();
        anim.SetTrigger("Died");
    }

    public void DieAnimationEnded()
    {
        Destroy(gameObject);
    }
}
