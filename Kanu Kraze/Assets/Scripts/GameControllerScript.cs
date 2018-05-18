using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    public CameraScript CameraScript;

    private List<PlayerScript> _players = new List<PlayerScript>();
    private int _selectedPlayer = 0;

    // Use this for initialization
    void Start()
    {
        var goPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in goPlayers)
        {
            var script = player.GetComponentInChildren<PlayerScript>();
            _players.Add(script);
        }

        if (_players.Count > 0)
        {
            _players[0].Active = true;
            CameraScript.ChangePlayer(_players[0].transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!CameraScript.IsTransitioning())
        {
            if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl))
            {
                _players[_selectedPlayer].Active = false;

                if (Input.GetKey(KeyCode.LeftShift))
                    _selectedPlayer--;
                else
                    _selectedPlayer++;

                if (_selectedPlayer >= _players.Count)
                    _selectedPlayer = 0;
                if (_selectedPlayer < 0)
                    _selectedPlayer = _players.Count - 1;

                _players[_selectedPlayer].Active = true;
                CameraScript.ChangePlayer(_players[_selectedPlayer].transform);
            }
        }
    }
}
