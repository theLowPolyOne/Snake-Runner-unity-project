using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObstacle : CollectableObject, IDamaging
{
    public void Damage()
    {
        PlayerController.Instance.Death();
    }

    public override void Collect(bool isFever)
    {
        if (isFever)
        {
            PlayerController.Instance.Eat("Human");
            if (!isCollecting)
            {
                float duration = collectDuration;
                float speed = collectSpeed;
                if (isFever)
                {
                    duration /= 3;
                    speed *= 3;
                }
                StartCoroutine(CollectAnimation(duration, speed));
            }
        }
    }
}
