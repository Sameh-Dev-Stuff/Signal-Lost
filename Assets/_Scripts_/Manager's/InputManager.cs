using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton — persists across scenes.
/// Single source of truth for Action Map switching and all input reads.
/// Cursor management lives here only; UiManager must NOT touch the cursor.
/// </summary>
public class InputManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Singleton
    // ─────────────────────────────────────────────

    public static InputManager Instance { get; private set; }

    // ─────────────────────────────────────────────
    //  Private — Input System
    // ─────────────────────────────────────────────

    private InputActionsData _actionsData;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _sprintAction;
    private InputAction _crouchAction;
    private InputAction _aimDirectionAction;
    private InputAction _aimPointAction;
    private InputAction _pauseAction;
    private InputAction _interactAction;

    // ═════════════════════════════════════════════
    //  Unity callbacks
    // ═════════════════════════════════════════════

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        
        _actionsData = new InputActionsData();

        _moveAction = _actionsData.PlayerMap.Move;
        _attackAction = _actionsData.PlayerMap.Attack;
        _aimPointAction = _actionsData.PlayerMap.AimPoint;
        _aimDirectionAction = _actionsData.PlayerMap.AimDirection;
        _sprintAction = _actionsData.PlayerMap.Sprint;
        _crouchAction = _actionsData.PlayerMap.Crouch;
        _pauseAction = _actionsData.PlayerMap.Pause;
        _interactAction = _actionsData.PlayerMap.Interact;
        
        ApplyMapForScene(SceneManager.GetActiveScene());
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ═════════════════════════════════════════════
    //  Scene handling
    // ═════════════════════════════════════════════

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMapForScene(scene);
    }

    /// <summary>
    /// Decides which Action Map should be active for a given scene.
    /// Main Menu (index 0) → UI map.  Everything else → Player map.
    /// </summary>
    private void ApplyMapForScene(Scene scene)
    {
        if (scene.buildIndex == 0)
        {
            SwitchToUiActionMap();
            SwitchToPlayerActionMap(); // -- -- --  jast for test -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 
        }
        else
        {
            SwitchToPlayerActionMap();
        }
    }

    // ═════════════════════════════════════════════
    //  Action Map switching  (single source of truth)
    // ═════════════════════════════════════════════

    /// <summary>Enables UI map, disables Player map, unlocks cursor.</summary>
    public void SwitchToUiActionMap()
    {
        _actionsData.PlayerMap.Disable();
        _actionsData.UiMap.Enable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>Enables Player map, disables UI map, locks cursor.</summary>
    public void SwitchToPlayerActionMap()
    {
        _actionsData.UiMap.Disable();
        _actionsData.PlayerMap.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // ═════════════════════════════════════════════
    //  Input reads
    // ═════════════════════════════════════════════

    public Vector2 MoveInput()            => _moveAction.ReadValue<Vector2>();
    public Vector2 AimDirectionInput()    => _aimDirectionAction.ReadValue<Vector2>();
    public Vector2 AimPointInput()            => _aimPointAction.ReadValue<Vector2>();

    public bool AttackInputIsPressed()    => _attackAction.IsPressed();
    public bool AttackInputClick()        => _attackAction.WasPressedThisFrame();
    public bool SprintInput()             => _sprintAction.IsPressed();
    public bool CrouchInput()             => _crouchAction.WasPressedThisFrame();  // toggle — only fires once per press
    public bool PauseInput()              => _pauseAction.WasPressedThisFrame();
    public bool InteractInput()           => _interactAction.WasPressedThisFrame();
}