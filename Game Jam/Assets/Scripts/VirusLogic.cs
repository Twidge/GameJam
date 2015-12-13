using UnityEngine;
using System.Collections;

public class VirusLogic : MonoBehaviour
{
    private float _randomAngle = 0f;
    private bool _left = false;
    private float _maxAdjustment = 67.5f;
    private float _speed = 0.2f;
    private const float TIME_BETWEEN_ANGLES = 0.3f;
    private const float TIME_BETWEEN_LEFT_RIGHT = 3f;
    private const int DEATH_FLASH_TIME = 4;
    private bool _newAngle = false;
    private bool _newDirection = false;
    private bool _deathFlashTriggered = false;

    private float _timeAlive = 0f;
    private const float TIME_OF_DEATH = 15f;

    void Start()
    {
        _randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        _newAngle = true;
        _newDirection = true;
    }

    void Update()
    {
        _timeAlive += Time.deltaTime;

        if (_newAngle)
        {
            if(_timeAlive < TIME_OF_DEATH - TIME_BETWEEN_ANGLES)
                StartCoroutine(PickNewAngle(TIME_BETWEEN_ANGLES));
        }

        if (_newDirection)
        {
            if(_timeAlive < TIME_OF_DEATH - TIME_BETWEEN_LEFT_RIGHT)
                StartCoroutine(SwapDirection(TIME_BETWEEN_LEFT_RIGHT));
        }

        transform.position = new Vector3(transform.position.x + (Mathf.Cos(_randomAngle) * _speed * Time.deltaTime), transform.position.y + (Mathf.Sin(_randomAngle) * _speed * Time.deltaTime));

        if (_timeAlive > TIME_OF_DEATH - DEATH_FLASH_TIME && !_deathFlashTriggered)
        {
            _deathFlashTriggered = true;
            StartCoroutine(DeathFlash(DEATH_FLASH_TIME));
        }

        if (_timeAlive > TIME_OF_DEATH)
            Destroy(this.gameObject);
    }

    IEnumerator PickNewAngle(float time)
    {
        _newAngle = false;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if (_left)
            _randomAngle = Random.Range((_randomAngle * Mathf.Rad2Deg) - _maxAdjustment, _randomAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

        else
            _randomAngle = Random.Range(_randomAngle * Mathf.Rad2Deg, (_randomAngle * Mathf.Rad2Deg) + _maxAdjustment) * Mathf.Deg2Rad;

        _newAngle = true;
    }

    IEnumerator SwapDirection(float time)
    {
        _newDirection = false;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        _left = !_left;
        _newDirection = true;
    }

    IEnumerator DeathFlash(int time)
    {
        for(int i = 0; i < time; i++)
        {
            Color existing = this.GetComponent<SpriteRenderer>().color;

            float foo = 1f;

            while(foo > 0.5f)
            {
                this.GetComponent<SpriteRenderer>().color = new Color(existing.r, existing.g, existing.b, (foo - 0.5f) / 0.5f);

                foo -= Time.deltaTime;

                yield return null;
            }

            while(foo <= 0.5f && foo > 0f)
            {
                this.GetComponent<SpriteRenderer>().color = new Color(existing.r, existing.g, existing.b, (0.5f - foo) / 0.5f);

                foo -= Time.deltaTime;

                yield return null;
            }
        }
    }
}