using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player2D player2D;
    [SerializeField] private Player3D player3D;
    private InputController controls;


    private void Awake() 
    {
        
        
        controls = new InputController();

        //* Blob
        controls.Blob.Move.performed += ctx => player2D.SetMovement(ctx.ReadValue<Vector2>());
        controls.Blob.Move.canceled += ctx => player2D.SetMovement(ctx.ReadValue<Vector2>());
        
        controls.Blob.Menu.performed += _ => FindObjectOfType<MenuController>().MenuAction();

        //* Bus
        controls.Bus.Move.performed += ctx => player3D.SetMovement(ctx.ReadValue<Vector2>());
        controls.Bus.Move.canceled += ctx => player3D.SetMovement(ctx.ReadValue<Vector2>());
        controls.Bus.Fire.performed += ctx => player3D.ShotAction();

        controls.Bus.ChangeLane.performed += ctx => player3D.ChangeLane(ctx.ReadValue<float>());
        
    }


    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}
