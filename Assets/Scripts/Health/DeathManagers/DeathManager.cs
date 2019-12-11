using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class DeathManager : MonoBehaviour
{
    [SerializeField]
    protected Vector3 localOffset = new Vector3(0, 1, 0);

    abstract public void Die();
}
