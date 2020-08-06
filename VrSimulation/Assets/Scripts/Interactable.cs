using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand _activeHand;
    public Hand ActiveHand
    {
        get { return _activeHand; }
        set
        {
            if (value == null)
            {
                OnInteractableDropped?.Invoke(_activeHand);
            }
            _activeHand = value;
        }
    }

    private Quaternion PreviosRotation;
    private Bolt ConnectedBolt;

    public event InteractableDroppedHandler OnInteractableDropped;

    public delegate void InteractableDroppedHandler(Hand hand);

    public void SnapToBolt(Bolt bolt)
    {
        ConnectedBolt = bolt;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        transform.position = bolt.transform.position;
        PreviosRotation = ActiveHand.transform.rotation;
    }

    private void Awake()
    {
        OnInteractableDropped += DisconnectBolt;
        InvokeRepeating("LogRotation", 2.0f, 0.5f);
    }

    private void Update()
    {
        if (ConnectedBolt && ActiveHand)
        {
            // Break snap or set position to that of connected bolt
            if (Vector3.Distance(ConnectedBolt.transform.position, ActiveHand.transform.position) > ConnectedBolt.BreakDistance)
            {
                DisconnectBolt(ActiveHand);
            }
            else
            {
                var tip = transform.GetChild(2);
                transform.position = ConnectedBolt.transform.position + transform.position - tip.position;
                // transform.LookAt(ConnectedBolt.transform, transform.up);

                // Apply rotation to connected bolt
                if (ActiveHand.HoldFlag)
                {
                    var deltaAngleZ = PreviosRotation.eulerAngles.z - ActiveHand.transform.rotation.eulerAngles.z;
                    if (deltaAngleZ >= 180)
                    {
                        deltaAngleZ -= 360;
                    }
                    if (deltaAngleZ <= -180)
                    {
                        deltaAngleZ += 360;
                    }
                    ConnectedBolt.IncreaseTension(deltaAngleZ);
                }
            }
        }

        PreviosRotation = ActiveHand.transform.rotation;
    }

    private void DisconnectBolt(Hand hand)
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ConnectedBolt = null;
    }
}

