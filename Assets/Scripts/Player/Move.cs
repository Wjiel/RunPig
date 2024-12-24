using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class Move : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource sourceTop;
    [SerializeField] private AudioClip clipTop;
    [SerializeField] private float delayTop = 0.3f;
    [SerializeField] private float delayTopWalk = 0.3f;
    [SerializeField] private float delayTopRun = 0.15f;
    private float timeDelayTop;

    [Header("Player Speed")]
    [SerializeField] private float MaxSpeed = 8;
    [SerializeField] private float DefaulSpeed = 5;
    private float _currentSpeed;
    [SerializeField] private float RotationSpeed = 10;
    [SerializeField] private AnimationCurve curveDash;

    [SerializeField] private float TimeToResetDash = 2;

    private float _timeToResetDash;
    private bool _canDash = true;

    [Header("Stamin")]
    [SerializeField] private Image[] StaminaField;
    [SerializeField] private float StaminaSpeed = 5;
    [Header("Visual")]
    [SerializeField] private ParticleSystem particleSystemRun;
    [SerializeField] private ParticleSystem particleSystemDash;
    [SerializeField] private ParticleSystem particleSystemStan;
    [SerializeField] private Animator animator;

    [SerializeField] private Image DashReloadImage;
    private Camera _camera;
    private float _staminaCount = 100;
    private Rigidbody2D _rigidBody;
    private Transform _transform;

    public bool dash { get; private set; }
    public bool stan { get; private set; }

    private Coroutine run;
    private Coroutine resetStamina;
    private Coroutine dashRun;
    private Tween dOTween;
    private Tween dOTween1;

    [SerializeField] private float MaxCamerSize;
    [SerializeField] private float DefaulCamerSize;
    [SerializeField] private float MinCamerSize;
    private void Start()
    {
        _transform = transform;
        _camera = Camera.main;

        _rigidBody = _transform.GetComponent<Rigidbody2D>();
        _currentSpeed = DefaulSpeed;
        _transform.localScale = Vector2.zero;
    }
    private void OnEnable()
    {
        transform.DOScale(1.5f, 1f);
    }
    private void OnDisable()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        _transform.DOScale(0, 0.5f);
    }

    public void MovementLogic(Vector2 direction)
    {
        setAnimationWalk();

        _rigidBody.linearVelocity = direction * _currentSpeed;

        if (direction != Vector2.zero)
            if (timeDelayTop < delayTop)
            {
                timeDelayTop += Time.deltaTime;
            }
            else
            {
                timeDelayTop = 0;

                sourceTop.pitch = Random.Range(0.8f, 1.2f);

                sourceTop.PlayOneShot(clipTop);
            }
    }
    private void setAnimationWalk()
    {
        if (_rigidBody.linearVelocity != Vector2.zero)
            animator.SetBool("Walk", true);
        else
            animator.SetBool("Walk", false);
    }
    public void RotationLogic(Vector2 direction)
    {
        Quaternion target = Quaternion.LookRotation(_transform.forward, direction);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, target, RotationSpeed);
    }
    #region Dash
    public void StartDash()
    {
        if (_canDash == false)
            return;

        dashRun = StartCoroutine(DashRun());

        StartCoroutine(DashReset());
    }
    private IEnumerator DashReset()
    {
        DashReloadImage.fillAmount = 0;

        DashReloadImage.DOFillAmount(1, TimeToResetDash);

        while (_timeToResetDash < TimeToResetDash)
        {
            _timeToResetDash += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        _canDash = true;
    }

    private IEnumerator DashRun()
    {
        _canDash = false;
        _timeToResetDash = 0;
        dash = true;
        float time = 0;
        float speed;

        _camera.DOOrthoSize(MinCamerSize, 1f);
        particleSystemDash.Play();

        animator.SetBool("Run", true);

        while (time < 1)
        {
            time += Time.deltaTime * 2;
            speed = curveDash.Evaluate(time) * 25;

            _rigidBody.linearVelocity = _transform.up * speed;

            yield return new WaitForFixedUpdate();
        }

        particleSystemDash.Stop();
        _camera.DOOrthoSize(DefaulCamerSize, 1f);
        animator.SetBool("Run", false);

        dash = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (dash && stan == false)
        {
            StartCoroutine(getStan());

            if (other.transform.TryGetComponent(out IDamagable iDamagble))
            {
                iDamagble.ToHit();
            }
        }

    }

    private IEnumerator getStan()
    {
        stan = true;

        _rigidBody.linearVelocity = Vector2.zero;

        StopCoroutine(dashRun);
        particleSystemDash.Stop();
        animator.SetBool("Run", false);
        dash = false;

        particleSystemStan.Play();

        _camera.DOOrthoSize(MinCamerSize, 1);
        _camera.DOShakePosition(1, 0.5f);

        animator.SetBool("Walk", false);

        _rigidBody.AddForce(-_transform.up * DefaulSpeed);

        yield return new WaitForSeconds(1.5f);

        _camera.DOOrthoSize(DefaulCamerSize, 1);

        particleSystemStan.Stop();
        stan = false;
    }

    #endregion

    #region Stamin
    public void SprintStart()
    {
        if (stan || dash)
            return;

        _currentSpeed = MaxSpeed;
        delayTop = delayTopRun;

        particleSystemRun.Play();
        animator.SetBool("Run", true);

        _camera.DOOrthoSize(MaxCamerSize, 1);

        dOTween.Kill();
        dOTween1.Kill();

        StaminaField[0].DOFade(100, 0.5f);
        StaminaField[1].DOFade(100, 0.5f);

        if (resetStamina != null)
            StopCoroutine(resetStamina);

        run = StartCoroutine(StaminaRun());
    }
    public void SprintEnd()
    {
        _currentSpeed = DefaulSpeed;
        delayTop = delayTopWalk;


        animator.SetBool("Run", false);

        particleSystemRun.Stop();

        if (stan || dash)
            return;

        _camera.DOOrthoSize(DefaulCamerSize, 1);


        if (run != null)
            StopCoroutine(run);

        resetStamina = StartCoroutine(StaminaReset());
    }

    private IEnumerator StaminaRun()
    {
        while (_staminaCount > 0 && _rigidBody.linearVelocity != Vector2.zero)
        {
            _staminaCount -= Time.deltaTime * StaminaSpeed;
            StaminaField[0].fillAmount = _staminaCount / 100;
            StaminaField[1].fillAmount = _staminaCount / 100;

            if (dash == true)
            {
                SprintEnd();
                StopCoroutine(run);
            }

            yield return new WaitForFixedUpdate();
        }
        SprintEnd();
    }
    private IEnumerator StaminaReset()
    {
        while (_staminaCount < 100)
        {
            _staminaCount += Time.deltaTime * 3;
            StaminaField[0].fillAmount = _staminaCount / 100;
            StaminaField[1].fillAmount = _staminaCount / 100;

            yield return new WaitForFixedUpdate();
        }

        if (_staminaCount > 95)
        {
            dOTween = StaminaField[0].DOFade(0, 1f);
            dOTween1 = StaminaField[1].DOFade(0, 1f);
        }
    }

    #endregion
}
