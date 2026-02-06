using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class FishingHookController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;      // Movement/WASD (Vector2)
    [SerializeField] private InputActionReference interactAction;  // Movement/Interact (Button)

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private RingQTEController qteController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CatchCounterManager catchCounterManager;
    [SerializeField] private GameFlowManager gameFlowManager;

    [Header("Player Horizontal Move")]
    [SerializeField] private float playerSpeed = 6.0f;

    [Header("Hook Depth (line length)")]
    [SerializeField] private float minDepthLocalY = -6.0f;
    [SerializeField] private float maxDepthLocalY = -1.0f;
    [SerializeField] private float depthSpeed = 6.0f;

    [Header("Scoring")]
    [SerializeField] private int wildlifePenalty = 150;
    [SerializeField] private int perfectBonus = 50;
    [SerializeField] private int evidenceScore = 1;
    [SerializeField] private int corpseScore = 10;

    private readonly List<HookableObject> inRange = new List<HookableObject>();
    private int fallbackScore;

    private void Awake()
    {
        if (this.gameFlowManager == null)
        {
            this.gameFlowManager = FindObjectOfType<GameFlowManager>();
        }

        if (this.playerTransform == null)
        {
            this.playerTransform = this.transform.root;
        }

        if (this.qteController != null && this.hookTransform != null)
        {
            this.qteController.SetHookToFollow(this.hookTransform);
        }
    }

    private void OnEnable()
    {
        if (this.moveAction != null)
        {
            this.moveAction.action.Enable();
        }

        if (this.interactAction != null)
        {
            this.interactAction.action.Enable();
            this.interactAction.action.performed += this.OnInteractPerformed;
        }

        if (this.qteController != null)
        {
            this.qteController.OnRingQTEResult += this.HandleRingQTEResult;
        }
    }

    private void OnDisable()
    {
        if (this.interactAction != null)
        {
            this.interactAction.action.performed -= this.OnInteractPerformed;
            this.interactAction.action.Disable();
        }

        if (this.moveAction != null)
        {
            this.moveAction.action.Disable();
        }

        if (this.qteController != null)
        {
            this.qteController.OnRingQTEResult -= this.HandleRingQTEResult;
        }
    }

    private void Update()
    {
        if (!this.IsGameRunning())
        {
            return;
        }

        Vector2 move = Vector2.zero;

        if (this.moveAction != null)
        {
            move = this.moveAction.action.ReadValue<Vector2>();
        }

        this.UpdatePlayerHorizontal(move.x);
        this.UpdateHookDepth(move.y);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!this.IsGameRunning())
        {
            return;
        }

        if (this.qteController != null && this.qteController.IsBusy())
        {
            this.qteController.Press();
            return;
        }

        HookableObject target = this.SelectTargetClosestToHook();
        if (target == null)
        {
            Debug.Log("No target in hook range.");
            return;
        }

        Debug.Log("Starting QTE for: " + target.name);

        if (this.qteController != null)
        {
            this.qteController.BeginQTE(target);
        }
    }

    private void UpdatePlayerHorizontal(float xInput)
    {
        if (this.playerTransform == null)
        {
            return;
        }

        if (Mathf.Abs(xInput) < 0.01f)
        {
            return;
        }

        Vector3 pos = this.playerTransform.position;
        pos.x += xInput * this.playerSpeed * Time.deltaTime;
        this.playerTransform.position = pos;
    }

    private void UpdateHookDepth(float yInput)
    {
        if (this.hookTransform == null)
        {
            return;
        }

        if (Mathf.Abs(yInput) < 0.01f)
        {
            return;
        }

        Vector3 local = this.hookTransform.localPosition;
        local.y += yInput * this.depthSpeed * Time.deltaTime;
        local.y = Mathf.Clamp(local.y, this.minDepthLocalY, this.maxDepthLocalY);
        this.hookTransform.localPosition = local;
    }

    private HookableObject SelectTargetClosestToHook()
    {
        for (int i = this.inRange.Count - 1; i >= 0; i--)
        {
            if (this.inRange[i] == null)
            {
                this.inRange.RemoveAt(i);
            }
        }

        if (this.inRange.Count == 0)
        {
            return null;
        }

        HookableObject best = null;
        float bestDist = float.MaxValue;

        Vector3 hookPos = this.hookTransform.position;

        for (int i = 0; i < this.inRange.Count; i++)
        {
            HookableObject obj = this.inRange[i];
            if (obj == null)
            {
                continue;
            }

            if (!obj.IsHookable)
            {
                continue;
            }

            float d = Vector3.Distance(obj.transform.position, hookPos);
            if (d < bestDist)
            {
                bestDist = d;
                best = obj;
            }
        }

        return best;
    }

    private void HandleRingQTEResult(HookableObject target, RingQTEResult result)
    {
        if (target == null)
        {
            return;
        }

        if (result == RingQTEResult.Miss)
        {
            Debug.Log("REEL FAILED -> Losing item: " + target.name);
            target.MarkUnhookableAndFail();
            this.RemoveFromRange(target);
            this.scoreManager?.ResetCombo();
            return;
        }

        int gained = target.BaseScore;

        if (target.GrantsEvidence)
        {
            if (target.EvidenceType == EvidenceType.Corpse)
            {
                gained = this.corpseScore;
                this.catchCounterManager?.AddCatch(CatchType.Corpse);
            }
            else
            {
                gained = this.evidenceScore;
                this.catchCounterManager?.AddCatch(CatchType.Evidence);
            }
        }
        else
        {
            this.catchCounterManager?.AddCatch(CatchType.Fish);
        }

        if (result == RingQTEResult.Perfect)
        {
            gained += this.perfectBonus;
        }

        if (target.ObjectType == HookableType.Wildlife)
        {
            gained = -this.wildlifePenalty;
        }

        if (this.scoreManager != null)
        {
            this.scoreManager.AddScore(gained);
        }
        else
        {
            this.fallbackScore += gained;
        }

        int totalScore = this.scoreManager != null ? this.scoreManager.Score : this.fallbackScore;
        Debug.Log("REEL SUCCESS (" + result + ") -> +" + gained + " points | Total: " + totalScore);

        target.Consume();
        this.RemoveFromRange(target);
    }

    private void RemoveFromRange(HookableObject target)
    {
        for (int i = this.inRange.Count - 1; i >= 0; i--)
        {
            if (this.inRange[i] == target)
            {
                this.inRange.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HookableObject hookable = other.GetComponent<HookableObject>();
        if (hookable == null)
        {
            return;
        }

        if (!this.inRange.Contains(hookable))
        {
            Debug.Log("hookable item in range: " + hookable.name);
            this.inRange.Add(hookable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HookableObject hookable = other.GetComponent<HookableObject>();
        if (hookable == null)
        {
            return;
        }

        this.RemoveFromRange(hookable);
    }

    private bool IsGameRunning()
    {
        return this.gameFlowManager != null && this.gameFlowManager.CurrentState == GameState.Running;
    }
}
