using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BigGun : Gun
{
    [SerializeField]
    Transform leftHandTransform;
    public Transform LeftHandTransform
    {
        get
        {
            return leftHandTransform;
        }
    }
}
