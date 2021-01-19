using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
