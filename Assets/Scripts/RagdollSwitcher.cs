using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSwitcher : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other?.attachedRigidbody?.GetComponent<BasePlayer>();

        if (player)
        {
            Debug.Log($"RagdollSwitcher: {player.name}");
        }
    }
}
