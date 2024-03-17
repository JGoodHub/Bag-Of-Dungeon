using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DieItem : MonoBehaviour
{

    [SerializeField] private MeshRenderer _renderer;

    [SerializeField] private Material _redMat;
    [SerializeField] private Material _whiteMat;
    [SerializeField] private Material _blackMat;
    [SerializeField] private Material _greyMat;

    private Rigidbody _rigidbody;

    public float Speed => _rigidbody.velocity.magnitude;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetColour(DiceColour colour)
    {
        switch (colour)
        {
            default:
            case DiceColour.White:
                _renderer.material = _whiteMat;
                break;
            case DiceColour.Red:
                _renderer.material = _redMat;
                break;
            case DiceColour.Black:
                _renderer.material = _blackMat;
                break;
            case DiceColour.Grey:
                _renderer.material = _greyMat;
                break;
        }
    }

    public int GetResult()
    {
        List<KeyValuePair<int, float>> sideByAlignment = new List<KeyValuePair<int, float>>();

        sideByAlignment.Add(new KeyValuePair<int, float>(1, Vector3.Dot(Vector3.up, -transform.up)));
        sideByAlignment.Add(new KeyValuePair<int, float>(2, Vector3.Dot(Vector3.up, -transform.right)));
        sideByAlignment.Add(new KeyValuePair<int, float>(3, Vector3.Dot(Vector3.up, transform.forward)));
        sideByAlignment.Add(new KeyValuePair<int, float>(4, Vector3.Dot(Vector3.up, -transform.forward)));
        sideByAlignment.Add(new KeyValuePair<int, float>(5, Vector3.Dot(Vector3.up, transform.right)));
        sideByAlignment.Add(new KeyValuePair<int, float>(6, Vector3.Dot(Vector3.up, transform.up)));

        int bestSide = 1;
        float bestSideAlignment = float.MinValue;

        foreach ((int side, float alignment) in sideByAlignment)
        {
            if (alignment > bestSideAlignment)
            {
                bestSide = side;
                bestSideAlignment = alignment;
            }
        }

        return bestSide;
    }

    public bool IsSkewed()
    {
        List<KeyValuePair<int, float>> sideByAlignment = new List<KeyValuePair<int, float>>();

        sideByAlignment.Add(new KeyValuePair<int, float>(1, Vector3.Dot(Vector3.up, -transform.up)));
        sideByAlignment.Add(new KeyValuePair<int, float>(2, Vector3.Dot(Vector3.up, -transform.right)));
        sideByAlignment.Add(new KeyValuePair<int, float>(3, Vector3.Dot(Vector3.up, transform.forward)));
        sideByAlignment.Add(new KeyValuePair<int, float>(4, Vector3.Dot(Vector3.up, -transform.forward)));
        sideByAlignment.Add(new KeyValuePair<int, float>(5, Vector3.Dot(Vector3.up, transform.right)));
        sideByAlignment.Add(new KeyValuePair<int, float>(6, Vector3.Dot(Vector3.up, transform.up)));

        float bestSideAlignment = float.MinValue;

        foreach ((int _, float alignment) in sideByAlignment)
        {
            if (alignment > bestSideAlignment)
            {
                bestSideAlignment = alignment;
            }
        }

        return bestSideAlignment < 0.9f;
    }

    public void SelfRightDie()
    {
        // Re-rolling was a nicer looking solution instead of this
        // Keeping as the alignment checking is nice so should be used in the other two methods

        Dictionary<int, Vector3> sideDirection = new Dictionary<int, Vector3>
        {
            { 1, -transform.up },
            { 2, -transform.right },
            { 3, transform.forward },
            { 4, -transform.forward },
            { 5, transform.right },
            { 6, transform.up },
        };

        Dictionary<int, float> sideAlignment = new Dictionary<int, float>
        {
            { 1, Vector3.Dot(Vector3.up, sideDirection[1]) },
            { 2, Vector3.Dot(Vector3.up, sideDirection[2]) },
            { 3, Vector3.Dot(Vector3.up, sideDirection[3]) },
            { 4, Vector3.Dot(Vector3.up, sideDirection[4]) },
            { 5, Vector3.Dot(Vector3.up, sideDirection[5]) },
            { 6, Vector3.Dot(Vector3.up, sideDirection[6]) }
        };

        int bestSide = 1;
        float bestSideAlignment = float.MinValue;

        foreach ((int side, float alignment) in sideAlignment)
        {
            if (alignment > bestSideAlignment)
            {
                bestSide = side;
                bestSideAlignment = alignment;
            }
        }

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, sideDirection[bestSide]);
        float angle = Vector3.Angle(Vector3.up, sideDirection[bestSide]);

        Vector3 targetDirection = Quaternion.AngleAxis(angle, rotationAxis) * sideDirection[bestSide];

        Debug.DrawRay(transform.position, rotationAxis * 2f, Color.cyan, 5f);
        Debug.DrawRay(transform.position, sideDirection[bestSide] * 2f, Color.red, 5f);
        Debug.DrawRay(transform.position, targetDirection * 2f, Color.green, 5f);

        transform.Rotate(rotationAxis, angle);
    }

    public void SetFrozen(bool isFrozen)
    {
        _rigidbody.isKinematic = isFrozen;
    }

    public void BounceDie(float verticalForce, float rotationTorque, float sideForce)
    {
        _rigidbody.isKinematic = false;

        _rigidbody.AddForce(Vector3.up * verticalForce, ForceMode.Impulse);

        _rigidbody.AddTorque(VectorUtils.Random(1f, 1f, 1f).normalized * rotationTorque, ForceMode.Impulse);

        if (sideForce > 0f)
        {
            _rigidbody.AddForce(VectorUtils.Random(1f, 0f, 1f).normalized * sideForce, ForceMode.Impulse);
        }
    }

}

public enum DiceColour
{

    Red,
    White,
    Black,
    Grey

}