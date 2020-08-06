using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour
{
    public float DefaultLenght = 5f;

    private Transform Tip;
    private LineRenderer LineRenderer;

    private void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
        Tip = transform.GetChild(0);
    }

    private void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit);

        var endPostition = hit.collider == null ? transform.position + transform.forward * DefaultLenght : hit.point;

        Tip.position = endPostition;
        LineRenderer.SetPosition(0, transform.position);
        LineRenderer.SetPosition(1, endPostition);
    }
}
