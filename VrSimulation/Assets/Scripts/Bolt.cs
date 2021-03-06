﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class Bolt : MonoBehaviour
{
    public float Tension;
    public float BreakDistance;
    public int MaxTurn = 3;
    public Text Label;
    public bool DisplayColor = true;

    public event BoltStateChangedHandler OnBoltStateChanged;

    public delegate void BoltStateChangedHandler();

    private FixedJoint Joint;
    private Gradient Gradient;
    private Material Material;
    private bool TensionChanged = false;

    private void Awake()
    {
        Joint = GetComponent<FixedJoint>();
        Material = GetComponent<Renderer>().material;

        Gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        var colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.blue;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.green;
        colorKey[1].time = 0.5f;
        colorKey[2].color = Color.red;
        colorKey[2].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1 (0 and 100%)
        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        Gradient.SetKeys(colorKey, alphaKey);
        Material.color = Gradient.Evaluate(Tension);
    }

    private void Update()
    {
        // Update color of bold depending on its tension
        if (DisplayColor)
            Material.color = Gradient.Evaluate(Tension);
    }

    public void IncreaseTension(float increase)
    {
        transform.Rotate(0, 0, increase, Space.Self);
        Tension += increase / (360 * MaxTurn);
        TensionChanged = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();
        if (interactable == null)
            return;

        interactable.SnapToBolt(this);
    }

    internal void InteractableDisconected()
    {
        if (TensionChanged)
            OnBoltStateChanged?.Invoke();
        TensionChanged = false;
    }
}
