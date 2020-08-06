using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(FixedJoint))]
public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean GrabAction;
    public SteamVR_Action_Boolean HoldAction;
    [HideInInspector]
    public bool HoldFlag;

    private SteamVR_Behaviour_Pose Pose;
    private FixedJoint Joint;

    private List<Interactable> PossibleInteractables = new List<Interactable>();
    private Interactable CurrentInteractable;

    private void Awake()
    {
        Pose = GetComponent<SteamVR_Behaviour_Pose>();
        Joint = GetComponent<FixedJoint>();
    }

    private void Update()
    {
        // Input picking up
        if (GrabAction.GetStateDown(Pose.inputSource))
        {
            Pickup();
        }

        if (GrabAction.GetStateUp(Pose.inputSource))
        {
            Drop();
        }

        // Input holding
        if (CurrentInteractable)
        {
            if (HoldAction.GetStateDown(Pose.inputSource))
            {
                HoldFlag = true;
            }

            if (HoldAction.GetStateUp(Pose.inputSource))
            {
                HoldFlag = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();
        if (interactable is null)
            return;

        PossibleInteractables.Add(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        PossibleInteractables.Remove(other.GetComponent<Interactable>());
    }

    private void Pickup()
    {
        var interactable = PossibleInteractables.FirstOrDefault();
        if (interactable is null) {
            // Use Pointer

        }
        else
        {
            // Pick up interactable
            Joint.connectedBody = interactable.GetComponent<Rigidbody>();
            interactable.ActiveHand = this;
            CurrentInteractable = interactable;
        }
        
    }

    private void Drop()
    {
        var rigidbodyInteractable = Joint.connectedBody;

        if (rigidbodyInteractable is null)
            return;

        rigidbodyInteractable.velocity = Pose.GetVelocity();
        rigidbodyInteractable.angularVelocity = Pose.GetAngularVelocity();
        CurrentInteractable.ActiveHand = null;
        Joint.connectedBody = null;
        CurrentInteractable = null;
    }
}
