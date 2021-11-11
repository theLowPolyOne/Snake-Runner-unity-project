using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGUI : MonoBehaviour
{
    [SerializeField] private bool isShaking = false;
    [SerializeField] private float shakeSpeed = 50f;
    private RectTransform rTransform;

    private void Start()
    {
        rTransform = gameObject.GetComponent<RectTransform>();
    }
}
