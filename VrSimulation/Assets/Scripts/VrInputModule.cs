using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VrInputModule : BaseInputModule
{
    public SteamVR_Action_Boolean ClickAction;
    public SteamVR_Input_Sources TargetSource;
    public Camera Camera;

    private PointerEventData PointerEventData;
    private GameObject CurrentObject;

    protected override void Awake()
    {
        base.Awake();

        PointerEventData = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        PointerEventData.Reset();
        PointerEventData.position = new Vector2(Camera.pixelWidth / 2, Camera.pixelHeight / 2);

        eventSystem.RaycastAll(PointerEventData, m_RaycastResultCache);
        PointerEventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        CurrentObject = PointerEventData.pointerCurrentRaycast.gameObject;

        m_RaycastResultCache.Clear();

        HandlePointerExitAndEnter(PointerEventData, CurrentObject);

        if (ClickAction.GetStateDown(TargetSource))
        {
            PointerEventData.pointerPressRaycast = PointerEventData.pointerCurrentRaycast;

            var newPointerPress = ExecuteEvents.ExecuteHierarchy(CurrentObject, PointerEventData, ExecuteEvents.pointerDownHandler)
                ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(CurrentObject);

            PointerEventData.pressPosition = PointerEventData.position;
            PointerEventData.pointerPress = newPointerPress;
            PointerEventData.rawPointerPress = CurrentObject;
        }

        if (ClickAction.GetStateUp(TargetSource))
        {
            ExecuteEvents.Execute(PointerEventData.pointerPress, PointerEventData, ExecuteEvents.pointerUpHandler);

            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(CurrentObject);

            if (PointerEventData.pointerPress == pointerUpHandler)
            {
                ExecuteEvents.Execute(PointerEventData.pointerPress, PointerEventData, ExecuteEvents.pointerClickHandler);
            }

            eventSystem.SetSelectedGameObject(null);

            PointerEventData.pressPosition = Vector2.zero;
            PointerEventData.pointerPress = null;
            PointerEventData.rawPointerPress = null;
        }

    }
}
