using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpeed : MonoBehaviour
{
    public Text _currentSpeed;
    public CharacterController cc;

    private void Start()
    {
        _currentSpeed.text = "Speed: 0";
    }

    private void Update()
    {
        _currentSpeed.text = "Speed: " + cc.velocity.magnitude.ToString("F1");
    }
}
