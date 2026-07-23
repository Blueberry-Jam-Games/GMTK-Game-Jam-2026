using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.Multiplayer.Center.Common.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BJCharacterController : MonoBehaviour
{
    public Camera head;

    [Header("Player Settings")]
    [Range(0, 1)] 
    public float mouseSensitivaty = 0.5f;

    [Range(0, 1)] 
    public float mouseYSensitivaty = 0.9f;

    [Range(1, 5)] 
    public float speed = 3;
    public bool invertMouse = true;

    public float interactDistance = 10f;
    public float speakingRadius = 7f;
    public LayerMask speakableLayer;

    [Header("Visuals")]
    public TextMeshProUGUI crosshairInfo;
    public string interactableInfoText = "Interact [e]";
    public string speakableInfoText = "Speak [e]";

    public Canvas dialogueCanvas;
    public TextMeshProUGUI dialogueBox;
    public TextMeshProUGUI characterName;
    public Image characterIcon;

    [Header("Player Locks")]
    public bool enableMovement = true;
    public bool enableMouse = true;
    public bool enableInteraction = true;

    public bool inDialogue = false;


    // Internal
    private InputAction movement;
    private InputAction cursor;
    private InputAction interact;

    private Vector3 HeadRotation;
    private Rigidbody rb;

    private DialogueSO activeDialogue = null;

    public Action dialogueEnd;

    void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
        cursor = InputSystem.actions.FindAction("Look");
        interact = InputSystem.actions.FindAction("Interact");

        HeadRotation = new Vector3(0, 0, 0);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        #region Interactions
        if(enableInteraction)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            bool validInteractable = false;
            Interactable interactable;

            if(Physics.Raycast(ray, out hit, interactDistance))
            {
                if(hit.collider.TryGetComponent<Interactable>(out interactable))
                {
                    interactable.Highlighted = true;
                    validInteractable = true;
                    crosshairInfo.text = interactableInfoText;

                    if(interact.WasPressedThisFrame())
                    {
                        interactable.Interact();
                    }
                }
            }

            Collider[] speakableColliders = Physics.OverlapSphere(transform.position, speakingRadius, speakableLayer, QueryTriggerInteraction.Collide);

            float nearestNPC = speakingRadius + 1;
            Speakable nearestNPCSpeakable = null;

            foreach(Collider c in speakableColliders)
            {
                if(c.TryGetComponent<Speakable>(out Speakable speakable))
                {
                    if(Vector3.Distance(speakable.transform.position, transform.position) < nearestNPC)
                    {
                        nearestNPC = Vector3.Distance(speakable.transform.position, transform.position);
                        nearestNPCSpeakable = speakable;
                    }
                }
            }

            bool validSpeach = false;

            if(nearestNPCSpeakable != null)
            {
                nearestNPCSpeakable.speechBubble = true;

                if(!validInteractable)
                {
                    crosshairInfo.text = speakableInfoText;
                    validSpeach = true;
                    if(interact.WasPressedThisFrame())
                    {
                        activeDialogue = nearestNPCSpeakable.Interact(out Speakable s);
                        dialogueEnd += exitDialogue;
                        dialogueEnd += s.InteractionEnd;
                        enterDialogue(activeDialogue, dialogueEnd);
                    }
                }
            }

            if(!validSpeach && !validInteractable || inDialogue)
            {
                crosshairInfo.text = "";
            }
        }

        #endregion
    }

    void LateUpdate()
    {
        #region Rotation
        if(enableMouse)
        {
            Vector2 headMovement = cursor.ReadValue<Vector2>();

            headMovement *= mouseSensitivaty;
            headMovement.y *= mouseYSensitivaty;
            if(!invertMouse) headMovement.y *= -1;

            HeadRotation += new Vector3(headMovement.y, headMovement.x, 0);

            if(HeadRotation.x < -89) HeadRotation.x = -89;
            if(HeadRotation.x > 89) HeadRotation.x = 89;
            head.transform.rotation = Quaternion.Euler(HeadRotation);
        }
        #endregion
    }

    void FixedUpdate()
    {
        #region Movement
        if(enableMovement)
        {
            Vector2 moveAxis = movement.ReadValue<Vector2>();
            Vector3 moveDirection = Quaternion.Euler(new Vector3(0, HeadRotation.y, 0)) * new Vector3(moveAxis.x, 0, moveAxis.y);
            moveDirection *= speed;
            rb.linearVelocity = moveDirection;
        }
        #endregion
    }

    private void enterDialogue(DialogueSO dialogue, Action callback = null)
    {
        enableMouse = false;
        enableMovement = false;
        enableInteraction = false;
        inDialogue = true;

        dialogueCanvas.enabled = true;
        dialogueBox.text = "";

        characterIcon.sprite = dialogue.CharacterArt;

        characterName.text = dialogue.name;

        StartCoroutine(RunInteraction(activeDialogue, callback));
    }

    private void exitDialogue()
    {
        enableMouse = true;
        enableMovement = true;
        enableInteraction = true;
        inDialogue = false;

        dialogueCanvas.enabled = false;
    }

#region Interaction Handeler
    IEnumerator RunInteraction(DialogueSO dialogue, Action callback = null)
    {
        Debug.Log("RunInteraction");
        yield return new WaitForSeconds(2f);
        callback?.Invoke();
    }

#endregion
}
