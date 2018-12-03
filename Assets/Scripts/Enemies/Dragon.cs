using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy {

    private void Awake()
    {
        enemyData = Def.Instance.EnemyDictionary[Declarations.EnemyType.Dragon];
        Init();
    }

    protected override void Died()
    {
        base.Died();
        anim.SetTrigger("Die");
    }

    internal override void DealDamage(int damage, Declarations.Effect effect = null)
    {
        base.DealDamage(damage, effect);
        if (Alive)
        {
            anim.SetTrigger("Take Damage");
        }
    }

    protected override void Rotate(bool instant = false)
    {
        #region Animate
        //var rotationAngle = transform.rotation.eulerAngles.y - Quaternion.Lerp(transform.rotation,
        //    Quaternion.LookRotation(currentTile.transform.position - transform.position), Time.deltaTime * rotationSpeed).eulerAngles.y;
        //if (Mathf.Abs(rotationAngle) > 5)
        //{
        //    if (rotationAngle < 0)
        //    {
        //        anim.SetBool("Fly Right", true);
        //    }
        //    else if (rotationAngle > 0)
        //    {
        //        anim.SetBool("Fly Left", true);
        //    }
        //}
        //else
        //{
        //    if (anim.GetBool("Fly Right"))
        //    {
        //        anim.SetBool("Fly Right", false);
        //    }
        //    if (anim.GetBool("Fly Left"))
        //    {
        //        anim.SetBool("Fly Left", false);
        //    }
        //}
        #endregion

        base.Rotate(instant);
    }

    public void DieAnimationEnded()
    {
        anim.speed = 0;
        Destroy(gameObject);
    }
}
