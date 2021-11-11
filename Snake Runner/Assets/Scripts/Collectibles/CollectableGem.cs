using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableGem : CollectableObject
{
    public override void Collect(bool isFever)
    {
        PlayerController.Instance.Eat("Gem");
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