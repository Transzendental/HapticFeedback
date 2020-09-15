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
                DisconnectBolt();
            }
            _activeHand = value;
        }
    }

    private Quaternion PreviosRotation;
    private Bolt ConnectedBolt;

    public void SnapToBolt(Bolt bolt)
    {
        ConnectedBolt = bolt;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        transform.position = bolt.transform.position;
        PreviosRotation = ActiveHand.transform.rotation;
    }

    private void DisconnectBolt()
    {
        if (!ConnectedBolt)
            return;

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ConnectedBolt.InteractableDisconected();
        ConnectedBolt = null;
    }


    private void Update()
    {
        if (ConnectedBolt && ActiveHand)
        {
            // Break snap or set position to that of connected bolt
            if (Vector3.Distance(ConnectedBolt.transform.position, ActiveHand.transform.position) > ConnectedBolt.BreakDistance)
            {
                DisconnectBolt();
            }
            else
            {
                var tip = transform.GetChild(2);
                transform.position = ConnectedBolt.transform.position + transform.position - tip.position;

                // Apply rotation to connected bolt
                if (ActiveHand.HoldingFlag)
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
            PreviosRotation = ActiveHand.transform.rotation;
        }
    }
}