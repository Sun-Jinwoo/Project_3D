using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    public Camera MyCamera;
    public Transform Model;
    public Transform Head;
    public float MoveSpeed = 5f;
    public float TurnSmoothing = 5f;

    public static Ninja Instance;

    Rigidbody myRigidbody;
    Vector3 moveInput;
    Vector3 moveFactor;

    private void Awake()
    {
        Instance = this;
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Vault")
        {
            GameManager.Instance.Vault = true;
        }
        else if (other.name == "Escape" && GameManager.Instance.Vault)
        {
            GameManager.Instance.GameOver = true;
            GameManager.Instance.VictoryPanel.SetActive(true);
        }
        Debug.Log(other.name);
    }

    void Update()
    {
        // Movement
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.z = Input.GetAxisRaw("Vertical");

        float moveMagnitude = moveInput.magnitude;
        moveFactor = MyCamera.transform.TransformDirection(moveInput);
        moveFactor.y = 0f;
        moveFactor.Normalize();
        moveFactor *= moveMagnitude;
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.GameOver)
        {
            myRigidbody.MovePosition(transform.position + moveFactor * MoveSpeed * Time.deltaTime);
            if (moveFactor != Vector3.zero)
            {
                Model.rotation = Quaternion.Lerp(Model.rotation, Quaternion.LookRotation(moveFactor), Time.deltaTime * TurnSmoothing);
            }
        }
    }
}
