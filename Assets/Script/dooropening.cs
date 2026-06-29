using UnityEngine;
using System.Collections;

public class dooropening : MonoBehaviour
{
    public float openangle = 90f;
    public float openSpeed = 2f;
    public bool isOpen = false;

    [Header("Player Detection")]
    public string playerTag = "Player";   // make sure your sphere/player has this tag
    public bool closeWhenPlayerLeaves = false; // set true if you want it to auto-close after player walks away

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine doorCoroutine;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openangle, 0);
    }

    // Requires this object's Collider to have "Is Trigger" checked,
    // and the player object to have a Collider + Rigidbody, and the tag set in "playerTag".
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag(playerTag) && !isOpen)
        {
            Debug.Log("Player detected — opening door.");
            OpenDoor();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && isOpen && closeWhenPlayerLeaves)
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        SetDoorState(true);
    }

    public void CloseDoor()
    {
        SetDoorState(false);
    }

    private void SetDoorState(bool open)
    {
        isOpen = open;

        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }
        doorCoroutine = StartCoroutine(RotateDoor(open ? openRotation : closedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}
