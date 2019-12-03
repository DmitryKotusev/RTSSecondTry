using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Basic class for controller entity.
/// It can either be human player or
/// AI.
/// </summary>
abstract public class Controller : MonoBehaviour
{
    static HashSet<short> controllerIdsPool = new HashSet<short>();

    [SerializeField]
    Team team;

    [SerializeField]
    [ReadOnly]
    short controllerId;

    public Controller()
    {
        controllerId = GetFreeId();
        RegisterNewId(controllerId);
    }

    // Static methods
    #region
    /// <summary>
    /// Returns free id.
    /// Returns 0 if no free ids are available.
    /// If it returns 0 something might actually
    /// went wrong.
    /// </summary>
    /// <returns></returns>
    static short GetFreeId()
    {
        for (short i = 1; i <= short.MaxValue; i++)
        {
            if (!controllerIdsPool.Contains(i))
            {
                return i;
            }
        }
        return 0;
    }

    static bool IsIdFree(short id)
    {
        return !controllerIdsPool.Contains(id);
    }

    static void RegisterNewId(short id)
    {
        if (!IsIdFree(id))
        {
            throw new System.Exception($"Id {id} already belongs to other controller");
        }
    }

    static bool UnregisterId(short id)
    {
        return controllerIdsPool.Remove(id);
    }
    #endregion

    // Getters and setters
    #region
    public void SetTeam(Team team)
    {
        this.team = team;
    }

    public Team GetTeam()
    {
        return team;
    }

    public void SetControllerId(short controllerId)
    {
        if (this.controllerId == controllerId)
        {
            return;
        }

        RegisterNewId(controllerId);
        UnregisterId(this.controllerId);

        this.controllerId = controllerId;
    }

    public short GetControllerId()
    {
        return controllerId;
    }
    #endregion

    ~Controller()
    {
        UnregisterId(controllerId);
        Debug.Log($"Controller with id {controllerId} destroyed");
    }
}
