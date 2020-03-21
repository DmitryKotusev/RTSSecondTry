using UnityEngine;
using Sirenix.OdinInspector;

abstract public class BigGun : Gun
{
    [SerializeField]
    [Required]
    Transform leftHandTransform;

    [SerializeField]
    [Required]
    Transform catridgeClipHolder = null;

    [SerializeField]
    Transform catridgeClip = null;

    protected Vector3 catridgeDefaultLocalPosition;

    protected Quaternion catridgeDefaultLocalRotation;

    protected virtual void Start()
    {
        catridgeDefaultLocalPosition = catridgeClip.localPosition;
        catridgeDefaultLocalRotation = catridgeClip.localRotation;
    }

    public Transform LeftHandTransform
    {
        get
        {
            return leftHandTransform;
        }
    }

    public Transform CatridgeClipHolder
    {
        get
        {
            return catridgeClipHolder;
        }
    }

    public Transform CatridgeClip
    {
        get
        {
            return catridgeClip;
        }
    }

    public Vector3 СatridgeDefaultLocalPosition
    {
        get
        {
            return catridgeDefaultLocalPosition;
        }
    }

    public Quaternion CatridgeDefaultLocalRotation
    {
        get
        {
            return catridgeDefaultLocalRotation;
        }
    }

    public void ReturnCatridgeToPlace()
    {
        if (catridgeClip == null)
        {
            return;
        }

        catridgeClip.SetParent(catridgeClipHolder, true);

        catridgeClip.localPosition = catridgeDefaultLocalPosition;

        catridgeClip.localRotation = catridgeDefaultLocalRotation;
    }
}
