using UnityEngine;

public class FirewallScript : MonoBehaviour
{
    public BoxCollider2D WallCollider;
    private bool _activated = false;

    // Use this for initialization
    void Start()
    {

    }

    public void Activate()
    {
        _activated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_activated)
        {
            WallCollider.size = WallCollider.size - new Vector2(0, 2 * Time.deltaTime);
            WallCollider.offset = WallCollider.offset + new Vector2(0, 1 * Time.deltaTime);
        }
    }
}
