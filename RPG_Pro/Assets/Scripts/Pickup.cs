using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Gold,
    Health,
    Ghost
}

public class Pickup : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // did a player enter the trigger?
        if (collision.CompareTag("Player"))
        {
            // get the player 
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // give them the pickup value
            if (type == PickupType.Gold)
            {
                player.photonView.RPC("GiveGold", player.photonPlayer, value);
                PhotonNetwork.Destroy(gameObject);
            }
            else if (type == PickupType.Health)
            {
                player.photonView.RPC("Heal", player.photonPlayer, value);
                PhotonNetwork.Destroy(gameObject);
            }
            else if (type == PickupType.Ghost)
            {
                player.photonView.RPC("GhostPickup", player.photonPlayer, this);
            }

        }

            // destroy the pickup across the network
    }
}
