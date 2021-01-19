using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleEffect : MonoBehaviour
{
    public float duration = 1;
    [SerializeField] private GameObject[] circles;
    private Sequence sequence;

    void Start()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            float scale = Random.Range(1f, 3f);
            circles[i].transform.localScale = new Vector3(scale, scale, scale);
            circles[i].transform.localPosition = Vector3.zero;
            sequence = DOTween.Sequence()
                .Append(circles[i].transform.DOMove(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0).normalized * Random.Range(5, 10), duration).SetEase(Ease.OutExpo))
                //.AppendInterval(duration/4)
                .Insert(0, circles[i].transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), duration))
                .Append(circles[i].transform.DOScale(new Vector3(0, 0, 0), duration / 4))
                .AppendInterval(1)
                //.SetLoops(-1)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }


}
