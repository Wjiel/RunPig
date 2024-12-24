using System;
using DG.Tweening;
using UnityEngine;

public class Tower : MonoBehaviour, IDamagable
{
    private Transform Target;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform pointToBulletSpawn;
    [SerializeField] private float delayAttack;
    [SerializeField] private float bulletPower;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private ParticleSystem partcl;
    private float _time;
    public int indexOfPlace { get; set; }
    public static event Action<int> removeArray;
    public void Start()
    {
        Target = FindAnyObjectByType<HP>().transform;
        transform.localScale = Vector2.zero;
        transform.DOScale(1, 1f).SetEase(Ease.OutBounce);
    }
    public void ToHit()
    {
        transform.DOScale(0, 1).SetEase(Ease.InOutQuint);

        removeArray?.Invoke(indexOfPlace);

        Destroy(gameObject, 1);
        enabled = false;
    }
    void FixedUpdate()
    {

        if (_time < delayAttack)
        {
            _time += Time.deltaTime;
        }
        else
        {
            _time = 0;

            partcl.Play();

            _transform.DOPunchPosition(_transform.right * 0.2f, 0.5f, 1).SetEase(Ease.InCirc);

            Instantiate(bulletPrefab, pointToBulletSpawn.position, Quaternion.identity)
            .GetComponent<Rigidbody2D>().AddForce(-_transform.right * bulletPower, ForceMode2D.Impulse);

        }

        if (_time > delayAttack - 0.2f || _time < 0.2f)
            return;

        RotationLogick();
    }

    private void RotationLogick()
    {
        var newRotation = Quaternion.Euler(0, 0,
           Mathf.Atan2(Target.position.y - _transform.position.y, Target.position.x - _transform.position.x) * Mathf.Rad2Deg - 180);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, newRotation, 2 * Time.deltaTime);
    }
}
