using System;
using System.Collections;
using System.Collections.Generic;
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
    public int lettersPerSecond = 60;

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

        rb.linearVelocity = Vector3.zero;

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
        foreach(CharacterDialogue c in dialogue.dialogues)
        {
            characterName.text = c.characterName;
            characterIcon.sprite = c.characterArt;
            foreach(string dialogueSection in c.speech)
            {
                yield return RunDialogueBox(dialogueSection);

                while(!interact.WasPressedThisFrame())
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        callback?.Invoke();
    }

    IEnumerator RunDialogueBox(string paragraph)
    {
        List<string> lines = SplitIntoLines(dialogueBox, paragraph);

        int startLine = 0;
        int endLine = 0;
        List<string> displayLines = new List<string>();

        int lineNumber = 0;
        foreach(string line in lines)
        {
            int lineLength = LengthWithoutSpaces(line);
            int offset = 0;

            string newLine = "";

            displayLines.Add(newLine);

            int currentWord = 0;
            float timeSinceLetter = 0;
            for(int i = 0; i < lineLength; i++)
            {
                timeSinceLetter += Time.deltaTime;
                float secondPerLetter = 1.0f / lettersPerSecond;
                Debug.Log($"Seconds/Letter {secondPerLetter}");
                while(timeSinceLetter < secondPerLetter)
                {
                    timeSinceLetter += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                
                timeSinceLetter -= Mathf.Floor(timeSinceLetter / secondPerLetter) * secondPerLetter;

                bool whitespace = false;
                while(line[i + offset] == ' ')
                {
                    whitespace = true;
                    offset += 1;
                    newLine += ' ';
                }
                if(whitespace)
                {
                    currentWord ++;
                }

                newLine += line[i + offset];

                displayLines[endLine] = newLine;

                dialogueBox.text = "";

                for(int a = startLine; a <= endLine; a++)
                {
                    dialogueBox.text += displayLines[a];
                    dialogueBox.text += '\n';
                }

                if(timeSinceLetter > secondPerLetter) continue;

                yield return new WaitForEndOfFrame();
            }

            endLine ++;

            if(endLine - startLine >= 3)
            {
                startLine ++;
            }
            
            lineNumber ++;
        }
    }
#endregion


    public static List<string> SplitIntoLines(TMP_Text textMeshPro, string paragraph)
    {
        List<string> lines = new List<string>();

        if (string.IsNullOrEmpty(paragraph))
            return lines;

        // Assign the text to TMP and force an update so textInfo is valid
        string originalText = textMeshPro.text;
        textMeshPro.text = paragraph;
        textMeshPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = textMeshPro.textInfo;

        for (int i = 0; i < textInfo.lineCount; i++)
        {
            int firstChar = textInfo.lineInfo[i].firstCharacterIndex;
            int lastChar = textInfo.lineInfo[i].lastCharacterIndex;

            // Substring of the paragraph that belongs to this line
            string lineText = paragraph.Substring(firstChar, lastChar - firstChar + 1);

            lines.Add(lineText);
        }

        // Restore original text (optional)
        textMeshPro.text = originalText;
        textMeshPro.ForceMeshUpdate();

        return lines;
    }

    private int LengthWithoutSpaces(String input)
    {
        string withoutSpaces = input.Replace(" ", "");
        return withoutSpaces.Length;
    }
}
