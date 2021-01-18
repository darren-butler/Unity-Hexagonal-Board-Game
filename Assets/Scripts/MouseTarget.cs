using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Each player gameobject has a component of this type
// This component uses raycasting to get a handle on the hextile the player has clicked on
// I followed this tutorial when implementing this feature - https://www.youtube.com/watch?v=p2_X_klweBw
public class MouseTarget : MonoBehaviour
{
    private Player player;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Throw a ray from the game camera to the mouse pointer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // if raycast hit a collider, info about the object hit will be stored @hit
        {
            if (Input.GetMouseButtonDown(0)) // if the player left clicked, and was at a hex tile (or any other object with a collider)
            {
                target = hit.collider.transform.parent.gameObject; // set our target game object to be the object we clicked on

                if (target != player.target && player.isTurn) // if target is a new target for the player, and it is the players turn
                {
                    player.target = target; // set the player target to be the gameobject we clicked on
                }
            }
        }
    }
}
