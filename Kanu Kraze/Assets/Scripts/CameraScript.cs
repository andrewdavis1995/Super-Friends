using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private PlayerScript _target;
    private bool _transitioning = false;
    private float _transitionDuration = .65f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!_transitioning && _target.Alive)
        {
            transform.position = new Vector3(_target.transform.position.x + 5f, _target.transform.position.y + 2f, -25);
        }
    }

    public void ChangePlayer(PlayerScript target)
    {
        _target = target;
        _transitioning = true;
        StartCoroutine(Transition());
    }

    public bool IsTransitioning() { return _transitioning; }

    IEnumerator Transition()
    {
        float t = 0.0f;
        Vector3 startingPos = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * (Time.timeScale / _transitionDuration);
            //t +=(Time.timeScale / transitionDuration);

            var destination = new Vector3(_target.transform.position.x+ 5f, _target.transform.position.y + 2f, -25);

            transform.position = Vector3.Lerp(startingPos, destination, t);
            yield return 0;
        }
        _transitioning = false;
    }
}
