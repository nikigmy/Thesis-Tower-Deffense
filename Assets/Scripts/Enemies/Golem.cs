using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy {

    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    protected override void Died()
    {
        Alive = false;
        GameManager.instance.SpawnManager.EnemyDestroyed(this);
        GameManager.instance.AddMoney(enemyData.Award);
        anim.SetTrigger("Died");
    }

    public void DieAnimationEnded()
    {
        Destroy(gameObject);
    }
}
