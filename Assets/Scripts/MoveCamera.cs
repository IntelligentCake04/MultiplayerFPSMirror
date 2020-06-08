using UnityEngine;
using Mirror;

public class MoveCamera : NetworkBehaviour
{
    public GameObject weaponHolder;
    public Transform head;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(head.transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localEulerAngles = Vector3.zero;

        weaponHolder.transform.SetParent(Camera.main.transform);
    }
}