using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    public bool GetKey(string keyName)
    {
        return Input.GetKey(keyName);
    }
}
