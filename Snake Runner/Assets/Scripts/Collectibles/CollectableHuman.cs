using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHuman : CollectableObject, IRecolourible
{
    [SerializeField] Material colorMaterial;
    private Color color;

    public void SetColorMaterial(Material newMaterial)
    {
        colorMaterial = newMaterial;
        color = colorMaterial.color;
        GetComponentInChildren<Renderer>().sharedMaterial = colorMaterial;
    }

    public override void Collect(bool isFever)
    {
        if (isFever)
        {
            PlayerController.Instance.Eat("Human");
        }
        else
        {
            if (color == PlayerController.Instance.color)
            {
                PlayerController.Instance.Eat("Human");
            }
            else
            {
                PlayerController.Instance.Death();
            }
        }
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
