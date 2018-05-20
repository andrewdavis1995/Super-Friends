using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndrewScript : MonoBehaviour
{

    private FirewallScript _inPanelBounds = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Panel")
        {
            _inPanelBounds = null;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Panel")
        {
            _inPanelBounds = collision.gameObject.GetComponentInParent<FirewallScript>();
            if (_inPanelBounds)
                _inPanelBounds.Activate();
        }
    }
}
