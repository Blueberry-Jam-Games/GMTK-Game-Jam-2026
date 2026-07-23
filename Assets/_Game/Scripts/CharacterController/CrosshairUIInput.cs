using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CrosshairUIInput : MonoBehaviour
{
    private readonly List<RaycastResult> results = new();

    private EventSystem eventSystem;
    private PointerEventData pointer;
    private InputAction uiInput;

    private GameObject hovered;
    private GameObject pressed;

    private void Awake()
    {
        eventSystem = EventSystem.current;
        uiInput = InputSystem.actions.FindAction("UIInput", true);

        pointer = new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left
        };
    }

    private void OnEnable()
    {
        uiInput.started += Press;
        uiInput.canceled += Release;
    }

    private void OnDisable()
    {
        uiInput.started -= Press;
        uiInput.canceled -= Release;

        CancelPress();
        SetHovered(null);
    }

    private void Update()
    {
        RaycastResult hit = RaycastUI();

        GameObject newHovered =
            ExecuteEvents.GetEventHandler<IPointerEnterHandler>(
                hit.gameObject
            );

        SetHovered(newHovered);
    }

    private RaycastResult RaycastUI()
    {
        pointer.position = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );

        pointer.delta = Vector2.zero;

        results.Clear();
        eventSystem.RaycastAll(pointer, results);

        return results.Count > 0 ? results[0] : default;
    }

    private void SetHovered(GameObject target)
    {
        if (target == hovered)
            return;

        if (hovered != null)
        {
            ExecuteEvents.Execute(
                hovered,
                pointer,
                ExecuteEvents.pointerExitHandler
            );
        }

        hovered = target;
        pointer.pointerEnter = hovered;

        if (hovered != null)
        {
            ExecuteEvents.Execute(
                hovered,
                pointer,
                ExecuteEvents.pointerEnterHandler
            );
        }
    }

    private void Press(InputAction.CallbackContext context)
    {
        RaycastResult hit = RaycastUI();

        if (hit.gameObject == null)
            return;

        pointer.pressPosition = pointer.position;
        pointer.pointerPressRaycast = hit;
        pointer.eligibleForClick = true;

        pressed = ExecuteEvents.ExecuteHierarchy(
            hit.gameObject,
            pointer,
            ExecuteEvents.pointerDownHandler
        );

        pressed ??= ExecuteEvents.GetEventHandler<IPointerClickHandler>(
            hit.gameObject
        );

        pointer.pointerPress = pressed;
        pointer.rawPointerPress = hit.gameObject;
    }

    private void Release(InputAction.CallbackContext context)
    {
        if (pressed == null)
            return;

        ExecuteEvents.Execute(
            pressed,
            pointer,
            ExecuteEvents.pointerUpHandler
        );

        GameObject releasedOver =
            ExecuteEvents.GetEventHandler<IPointerClickHandler>(
                RaycastUI().gameObject
            );

        if (pointer.eligibleForClick && releasedOver == pressed)
        {
            ExecuteEvents.Execute(
                pressed,
                pointer,
                ExecuteEvents.pointerClickHandler
            );
        }

        ClearPress();
    }

    private void CancelPress()
    {
        if (pressed != null)
        {
            ExecuteEvents.Execute(
                pressed,
                pointer,
                ExecuteEvents.pointerUpHandler
            );
        }

        ClearPress();
    }

    private void ClearPress()
    {
        pressed = null;
        pointer.pointerPress = null;
        pointer.rawPointerPress = null;
        pointer.eligibleForClick = false;
    }
}
