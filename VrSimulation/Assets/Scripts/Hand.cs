using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(FixedJoint))]
public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean GrabAction;
    public SteamVR_Action_Boolean TeleportAction;

    public bool HoldingFlag { get; private set; }
    public bool PointingFlag { get; private set; }

    private SteamVR_Behaviour_Pose Pose;
    private FixedJoint Joint;
    public GameObject Pointer;

    private List<Interactable> PossibleInteractables = new List<Interactable>();
    private Interactable CurrentInteractable;

    private void Awake()
    {
        Pose = GetComponent<SteamVR_Behaviour_Pose>();
        Joint = GetComponent<FixedJoint>();
        Pointer = transform.Find("Pointer")?.gameObject;
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

        if (TeleportAction.GetStateDown(Pose.inputSource))
        {

            if (CurrentInteractable != null)
            {
                HoldingFlag = true;
            }
            else
            {
                SetPointerActive(true);
            }
        }

        if (TeleportAction.GetStateUp(Pose.inputSource))
        {
            HoldingFlag = false;
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
        if (!(interactable is null))
        {
            // Pick up interactable
            Joint.connectedBody = interactable.GetComponent<Rigidbody>();
            interactable.ActiveHand = this;
            CurrentInteractable = interactable;

            SetPointerActive(false);
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

        SetPointerActive(true);
    }

    private void SetPointerActive(bool active = true)
    {
        if (active && Pointer == null)
        {
            Pointer = GameObject.Find("Pointer");
            Pointer.transform.parent.GetComponent<Hand>().Pointer = null;
            Pointer.transform.SetParent(transform);
            Pointer.transform.localPosition = Vector3.zero;
            Pointer.transform.localRotation = Quaternion.identity;
        }

        Pointer.SetActive(active);
        PointingFlag = true;
    }
}
