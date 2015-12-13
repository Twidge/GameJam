using UnityEngine;
using System.Collections;

public class OrbAI : MonoBehaviour
{
    private float _randomAngle = 0f;
    private bool _left = false;
    private float _maxAdjustment = 67.5f;
    private float _speed = 0.2f;
    private const float TIME_BETWEEN_ANGLES = 0.3f;
    private const float TIME_BETWEEN_LEFT_RIGHT = 3f;
    private bool _newAngle = false;
    private bool _newDirection = false;

	void Start ()
    {
        _randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        _newAngle = true;
        _newDirection = true;
	}
	
	void Update ()
    {
        if(_newAngle)
        {
            StartCoroutine(PickNewAngle(TIME_BETWEEN_ANGLES));
        }

        if(_newDirection)
        {
            StartCoroutine(SwapDirection(TIME_BETWEEN_LEFT_RIGHT));
        }

        transform.position = new Vector3(transform.position.x + (Mathf.Cos(_randomAngle) * _speed * Time.deltaTime), transform.position.y + (Mathf.Sin(_randomAngle) * _speed * Time.deltaTime));
	}

    IEnumerator PickNewAngle(float time)
    {
        _newAngle = false;

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if(_left)
            _randomAngle = Random.Range((_randomAngle * Mathf.Rad2Deg) - _maxAdjustment, _randomAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

        else
            _randomAngle = Random.Range(_randomAngle * Mathf.Rad2Deg, (_randomAngle * Mathf.Rad2Deg) + _maxAdjustment) * Mathf.Deg2Rad;

        _newAngle = true;
    }

    IEnumerator SwapDirection(float time)
    {
        _newDirection = false;

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        _left = !_left;
        _newDirection = true;
    }
}
