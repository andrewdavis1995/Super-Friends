using UnityEngine;

public class FirewallScript : MonoBehaviour
{
    public BoxCollider2D WallCollider;
    private bool _activated = false;
    public bool Pressed = false;
    public GameObject Instruction;

    // Use this for initialization
    void Start()
    {

    }

    public void Activate()
    {
        _activated = true;
        Pressed = true;
        Instruction.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_activated)
        {
            WallCollider.isTrigger = true;
            return;
        }

        if (_activated)
        {
            WallCollider.size = WallCollider.size - new Vector2(0, 2 * Time.deltaTime);
            WallCollider.offset = WallCollider.offset + new Vector2(0, 1 * Time.deltaTime);
        }
    }
}
