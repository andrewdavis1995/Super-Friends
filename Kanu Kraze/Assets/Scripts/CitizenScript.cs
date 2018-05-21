using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenScript : MonoBehaviour
{
    public int Health = 100;
    public SpriteRenderer Renderer;
    public Sprite[] Sprites;
    public Transform[] Rubble;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool Hit(int hitPoints)
    {
        Health -= hitPoints;
        StartCoroutine(CreateRubble());
        if (Health < 0)
        {
            return true;
        }
        return false;
    }

    private IEnumerator CreateRubble()
    {
        yield return new WaitForSeconds(.3f);
        var iNumRubble = UnityEngine.Random.Range(2, 9);
        for (var i = 0; i < iNumRubble; i++)
        {
            var iRubble = UnityEngine.Random.Range(0, Rubble.Length);
            var obj = Instantiate(Rubble[iRubble], transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f, -1f), Quaternion.identity, transform);
            obj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(UnityEngine.Random.Range(-80, 80), UnityEngine.Random.Range(40, 100)));
        }

        if (Health < 0)
        {
            Destroy(gameObject);
        }
        else if (Health < 20)
        {
            Renderer.sprite = Sprites[2];
        }
        else if (Health < 50)
        {
            Renderer.sprite = Sprites[1];
        }
        else if (Health < 80)
        {
            Renderer.sprite = Sprites[0];
        }
    }
}
