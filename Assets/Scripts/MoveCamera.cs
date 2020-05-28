using UnityEngine;
using Mirror;

public class MoveCamera : NetworkBehaviour
{
    public Transform head;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(head.transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localEulerAngles = Vector3.zero;

    }
}