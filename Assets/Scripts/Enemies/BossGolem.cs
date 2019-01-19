using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolem : MonoBehaviour {
    public void DieAnimationEnded()
    {
        Destroy(transform.parent.gameObject);
    }
}
