using UnityEngine;
using UnityEngine.InputSystem;
using YG;

public class PlayerController : MonoBehaviour
{
    private Move move;
    [SerializeField] private InputActionAsset input;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction dashAction;
    private Vector2 Direction;

    //mobile

    private float dirX, dirY;
    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject canvasMobile;
    private void Awake()
    {
        if (YandexGame.EnvironmentData.isDesktop)
        {
            moveAction = input.FindAction("Move");

            sprintAction = input.FindAction("Sprint");

            dashAction = input.FindAction("Dash");
        }
        else
        {
            canvasMobile.SetActive(true);
        }
    }
    private void Start() => move = GetComponent<Move>();
    private void OnEnable()
    {
        if (YandexGame.EnvironmentData.isMobile)
            return;

        if (sprintAction == null)
            if (YandexGame.EnvironmentData.isDesktop)
            {
                moveAction = input.FindAction("Move");

                sprintAction = input.FindAction("Sprint");

                dashAction = input.FindAction("Dash");
            }

        moveAction.performed += context => Direction = context.ReadValue<Vector2>();
        moveAction.canceled += context => Direction = Vector2.zero;

        sprintAction.performed += context => move.SprintStart();
        sprintAction.canceled += context => move.SprintEnd();

        dashAction.performed += context => move.StartDash();

        dashAction.Enable();
        sprintAction.Enable();
        moveAction.Enable();
    }
    private void OnDisable()
    {
        if (YandexGame.EnvironmentData.isMobile)
            return;

        moveAction.performed -= context => Direction = context.ReadValue<Vector2>();
        moveAction.canceled -= context => Direction = Vector2.zero;

        sprintAction.performed -= context => move.SprintStart();
        sprintAction.canceled -= context => move.SprintEnd();

        dashAction.performed -= context => move.StartDash();

        dashAction.Disable();
        sprintAction.Disable();
        moveAction.Disable();

    }
    private void FixedUpdate()
    {
        if (move.dash == true || move.stan)
            return;

        if (YandexGame.EnvironmentData.isMobile)
        {
            dirX = joystick.Horizontal;
            dirY = joystick.Vertical;

            Direction = new Vector2(dirX, dirY);
        }

        if (Direction != Vector2.zero)
            move.RotationLogic(Direction);

        move.MovementLogic(Direction);

    }
}