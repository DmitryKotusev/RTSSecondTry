using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Controller controller;

    public Controller Controller
    {
        get => controller;
        set => controller = value;
    }

    public void SpawnGroup(SpawnGroup spawnGroup)
    {

    }
}
