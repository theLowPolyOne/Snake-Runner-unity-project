using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour, ICollectable
{
    [SerializeField] protected float collectDuration = 1f;
    [SerializeField] protected AnimationCurve collectAnimation;
    protected bool isCollecting = false;
    protected float collectSpeed = 50f;
    protected Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }
    public virtual void Collect(bool isFever) { }

    protected virtual IEnumerator CollectAnimation(float duration, float speed)
    {
        isCollecting = true;
        _collider.enabled = false;
        float expiredSeconds = 0;
        float progress = 0;
        Transform target = PlayerController.Instance.eatPoint;
        Vector3 startScale = transform.localScale;
        transform.parent = target;
        while (progress < 1)
        {
            expiredSeconds += Time.deltaTime;
            progress = expiredSeconds / duration;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * collectAnimation.Evaluate(progress) * Time.deltaTime);
            Vector3 direction = target.position - transform.position + new Vector3(0, -90, 0);
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, collectAnimation.Evaluate(progress));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, collectAnimation.Evaluate(progress));
            yield return null;
        }
        transform.parent = null;
        isCollecting = false;
        gameObject.SetActive(false);
    }
}
