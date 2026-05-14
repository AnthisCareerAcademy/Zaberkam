using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Android;

// Don't mess with these; they're just naming the dropdowns.
[Serializable]
public struct AttackActionReferences
{
    public InputActionReference
        primary, secondary, firstAbility, secondAbility, thirdAbility, fourthAbility;
}

[Serializable]
public struct AttackCooldowns
{
    public float
        primary, secondary, firstAbility, secondAbility, thirdAbility, fourthAbility;
}

[Serializable]
public struct AttackIndicators
{
    public Image
        primary, secondary, firstAbility, secondAbility, thirdAbility, fourthAbility;
}

[Serializable]
public struct AttackHandlers
{
    public AttackTemplate
        primary, secondary, firstAbility, secondAbility, thirdAbility, fourthAbility;
}

public abstract class ClassTemplate : NetworkBehaviour
{
    [Header("Movement Options")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float gravity = -10f;
    [SerializeField] protected float scale = 1f;
    
    [Header("Camera Options")]
    [SerializeField] InputActionReference look;
    [SerializeField] InputActionReference pause;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] protected float defaultFOV = 60f;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject pauseMenu;
    
    [Header("Attack Options")]
    [SerializeField] AttackActionReferences attackInputs;
    [SerializeField] AttackCooldowns cooldowns;
    [SerializeField] AttackIndicators indicators;
    [SerializeField] protected AttackHandlers attackHandlers;
    [SerializeField] int attackRandomness = 5;

    [Header("Stats")]
    [SerializeField] protected float critMultiplier;
    [SerializeField] protected float critChance;

    private float[] activeActions = new float[6];
    
    protected CharacterController Controller;
    protected Health HealthManager;
    protected Transform CamTransform;
    
    // These are manipulated later.
    protected Vector3 Velocity;
    private float xRotation;
    private Camera camLens;

    private int attackBonus;  // This is a random value added to or subtracted from attacks.

    public virtual void Awake()
    {
        Controller = GetComponent<CharacterController>();
        if (Controller == null) Debug.LogError("CharacterController not found");
        
        HealthManager = GetComponent<Health>();
        if (HealthManager == null) Debug.LogError("Health not found");
        
        CamTransform = cam.transform;
        camLens = cam.GetComponent<Camera>();
        camLens.fieldOfView = defaultFOV;
        
        // Change everything to match scale.
        moveSpeed *= scale;
        jumpHeight *= scale;
        gravity *= scale;
        
        transform.localScale = Vector3.one * scale;
        
        // Resize hitboxes.
        attackHandlers.primary.scale = scale;
        attackHandlers.secondary.scale = scale;
        attackHandlers.firstAbility.scale = scale;
        attackHandlers.secondAbility.scale = scale;
        attackHandlers.thirdAbility.scale = scale;
        attackHandlers.fourthAbility.scale = scale;
        
        Controller.minMoveDistance *= scale;
        Controller.skinWidth = 0.05f * scale;
        Controller.stepOffset *= scale;
        
        camLens.nearClipPlane *= scale;
        camLens.farClipPlane *= scale;
        
        // Set all the images to be filled so they can display cooldown properly.
        indicators.primary.type = Image.Type.Filled;
        indicators.secondary.type = Image.Type.Filled;
        indicators.firstAbility.type = Image.Type.Filled;
        indicators.secondAbility.type = Image.Type.Filled;
        indicators.thirdAbility.type = Image.Type.Filled;
        indicators.fourthAbility.type = Image.Type.Filled;

        Unpause();
    }

    public virtual void Update()
    {
        if (!IsOwner) return;

        CheckPause();
        
        DoLook();
        DoMove();

        attackBonus = Random.Range(-attackRandomness, attackRandomness);

        if (!Cursor.visible)
        {
            // I tried to make this a for-loop, but the structs weren't cooperating, so this works for now.
            HandleAction(attackInputs.primary, DoPrimary, cooldowns.primary, 0);
            HandleAction(attackInputs.secondary, DoSecondary, cooldowns.secondary, 1);
            HandleAction(attackInputs.firstAbility, DoFirstAbility, cooldowns.firstAbility, 2);
            HandleAction(attackInputs.secondAbility, DoSecondAbility, cooldowns.secondAbility, 3);
            HandleAction(attackInputs.thirdAbility, DoThirdAbility, cooldowns.thirdAbility, 4);
            HandleAction(attackInputs.fourthAbility, DoFourthAbility, cooldowns.fourthAbility, 5);
        }
        
        // I don't think the for loop would be any smaller here...
        if (indicators.primary) indicators.primary.fillAmount = FixIcons((activeActions[0] - Time.time) / cooldowns.primary);
        if (indicators.secondary) indicators.secondary.fillAmount = FixIcons((activeActions[1] - Time.time) / cooldowns.secondary);
        if (indicators.firstAbility) indicators.firstAbility.fillAmount = FixIcons((activeActions[2] - Time.time) / cooldowns.firstAbility);
        if (indicators.secondAbility) indicators.secondAbility.fillAmount = FixIcons((activeActions[3] - Time.time) / cooldowns.secondAbility);
        if (indicators.thirdAbility) indicators.thirdAbility.fillAmount = FixIcons((activeActions[4] - Time.time) / cooldowns.thirdAbility);
        if (indicators.fourthAbility) indicators.fourthAbility.fillAmount = FixIcons((activeActions[5] - Time.time) / cooldowns.fourthAbility);
    }

    float FixIcons(float value)
    {
        // This is a fix for the icons; if the value is less than 0, set it to 1 so the icon shows.
        return value < 0 ? 1 : value;
    }

    void DoLook()
    {
        Vector2 lookInput = look.action.ReadValue<Vector2>() * (mouseSensitivity * Time.deltaTime);

        // Limit vertical rotation.
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Turn the camera and the player.
        if (!Cursor.visible)
        {
            CamTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * lookInput.x);
        }
    }

    void DoMove()
    {
        Vector2 movement = move.action.ReadValue<Vector2>().normalized;

        if (!Cursor.visible)
        {
            Velocity.x = movement.x * moveSpeed;
            Velocity.z = movement.y * moveSpeed;
        }

        bool isGrounded = Controller.isGrounded;

        // Apply gravity.
        if (!isGrounded) Velocity.y += gravity * Time.deltaTime;
        
        // Perform jumps.
        if (isGrounded && jump.action.IsPressed())
        {
            Velocity.y = jumpHeight;
        }

        Velocity = transform.rotation * Velocity;
        Controller.Move(Velocity * Time.deltaTime);
    }

    void HandleAction(InputActionReference input, Action action, float cooldown, int id)
    {
        // Check if the action can be activated. This can also be used to hide/show UI elements.
        if (input.action.IsPressed() && Time.time > activeActions[id])
        {
            action();
            activeActions[id] = Time.time + cooldown;
        }
    }

    void CheckPause()
    {
        // Unlock cursor on pause.
        if (pause.action.WasCompletedThisFrame())
        {
            Pause();
        }
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
    }
    
    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }
    
    // These are the empty attack actions, to be overridden in child classes.
    protected virtual void DoPrimary()
    {
        if (Random.value < critChance) attackHandlers.primary.DoAttack(attackBonus, critMultiplier);
        else attackHandlers.primary.DoAttack(attackBonus);
    }
    protected virtual void DoSecondary() 
    {
        attackHandlers.secondary.DoAttack(attackBonus);
    }
    protected virtual void DoFirstAbility() 
    {
        attackHandlers.firstAbility.DoAttack(attackBonus);
    }
    protected virtual void DoSecondAbility() 
    {
        attackHandlers.secondAbility.DoAttack(attackBonus);
    }
    protected virtual void DoThirdAbility() 
    {
        attackHandlers.thirdAbility.DoAttack(attackBonus);
    }
    protected virtual void DoFourthAbility() 
    {
        attackHandlers.fourthAbility.DoAttack(attackBonus);
    }
    
    // Status effects. May eventually move to separate class.
    protected void ChangeFOV(float newFOV = 0f, float zoomRate = 1f)
    {
        if (newFOV == 0) newFOV = defaultFOV;
        camLens.fieldOfView = Mathf.Lerp(camLens.fieldOfView, newFOV, zoomRate * Time.deltaTime);
    }
    
    protected IEnumerator CritChanceUp(float time = 1f, float amount = 0.15f)
    {
        float originalCritChance = critChance;
        critChance += amount;
        yield return new WaitForSeconds(time);
        critChance = originalCritChance;
    }
    
    protected IEnumerator CritUp(float time = 1f, float amount = 0.5f)
    {
        float originalCrit = critMultiplier;
        critMultiplier += amount;
        yield return new WaitForSeconds(time);
        critMultiplier = originalCrit;
    }

    protected IEnumerator Dash(Vector3 direction, float time = 0.5f, float speed = 10f)
    {
        float timeElapsed = 0f;
        while (timeElapsed < time)
        {
            Controller.Move(direction * (speed * Time.deltaTime));
            yield return null;
            timeElapsed += Time.deltaTime;
        }
    }
    
    protected IEnumerator Invincibility(float time = 1)
    {
        HealthManager.invincible = true;
        yield return new WaitForSeconds(time);
        HealthManager.invincible = false;
    }
}
