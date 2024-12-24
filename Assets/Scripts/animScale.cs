using DG.Tweening;
using UnityEngine;

public class animScale : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector2.zero;
        transform.DOScale(1, 1).SetEase(Ease.OutBounce);
    }
   
}
