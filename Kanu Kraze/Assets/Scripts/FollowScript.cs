using System;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    private float _xPosition = -9999;
    private float _timeCount = 1;

    private PlayerScript _target;
    private PlayerScript _player;

    // Use this for initialization
    void Start()
    {
        _player = GetComponent<PlayerScript>();
    }

    public void SetTarget(PlayerScript target)
    {
        _target = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null || !_target.Alive)
            return;

        if (_player.JohnScript != null && _player.JohnScript.Climbing)
            return;

        _timeCount += Time.deltaTime;
        if (_timeCount > 2.5f)
        {
            _timeCount = 0;
            if (_target.onGround || (_target.JohnScript != null && _target.JohnScript.Climbing == true))
            {
                _xPosition = _target.transform.position.x;
            }
        }

        if (_xPosition == -9999) return;


        if (transform.position.x < _xPosition - 1.25f)
        {
            if (!_player.Walking)
                _player.BeginWalk();
            transform.Translate(new Vector3(2.7f * Time.deltaTime, 0, 0));
            foreach (var r in _player.Renderers)
            {
                r.flipX = false;
            }
            _player.Walking = true;
        }
        else if (transform.position.x > _xPosition + 1.25f)
        {
            if (!_player.Walking)
                _player.BeginWalk();
            transform.Translate(new Vector3(-2.7f * Time.deltaTime, 0, 0));
            foreach (var r in _player.Renderers)
            {
                r.flipX = true;
            }
            _player.Walking = true;
        }
        else
        {
            _xPosition = -9999;
            if (_player.Walking)
                _player.StopWalk();
            _player.Walking = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "JumpTrigger" && _player != null && !_player.Active)
        {
            _player.Jump(1);
        }
        if (collision.transform.tag == "JumpStopper" && _player != null && !_player.Active)
        {
            _xPosition = transform.position.x - 1.5f;
        }
    }

    internal void Reset()
    {
        enabled = false;
        _xPosition = -9999;
    }
}
