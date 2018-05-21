using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndrewScript : MonoBehaviour
{
    public PlayerScript Player;
    private FirewallScript _inPanelBounds = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (_inPanelBounds && Player.Active)
            {
                _inPanelBounds.Activate();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Panel")
        {
                _inPanelBounds = null;
                collision.GetComponentInParent<FirewallScript>().Instruction.SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Panel")
        {
            if (!collision.GetComponentInParent<FirewallScript>().Pressed)
            {
                _inPanelBounds = collision.gameObject.GetComponentInParent<FirewallScript>();
                collision.GetComponentInParent<FirewallScript>().Instruction.SetActive(true);
            }
        }
    }
}
