using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    public Transform destino;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            other.transform.position = destino.position;
            other.transform.rotation = destino.rotation;

            if (controller != null)
                controller.enabled = true;
        }
    }
}
