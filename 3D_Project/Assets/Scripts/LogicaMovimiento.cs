using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogicaMovimiento : MonoBehaviour
{

    public float movementSpeed = 5.0f;
    public float rotationSpeed = 200.0f;

    private Animator anim;
    public float x, y;

    public Rigidbody rb;
    public float fuerzaDeSalto = 8f;
    public bool puedoSaltar;

    public float velocidadInicial;
    public float velocidadAgachado;
    internal bool destruirAutomatico;

    public bool destruirConCursor { get; internal set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        puedoSaltar = false;
        anim = GetComponent<Animator>();

        velocidadInicial = movementSpeed;
        velocidadAgachado = movementSpeed * 0.5f;
    }

    private void FixedUpdate()
    {
        // Rotación suave
        float rotation = x * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        // Movimiento
        Vector3 movement = transform.forward * y * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");


        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", y);

        if (puedoSaltar)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("salte", true);
                rb.AddForce(new Vector3(0, fuerzaDeSalto, 0), ForceMode.Impulse);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                anim.SetBool("agachado", true);
                movementSpeed = velocidadAgachado;
            }
            else
            {
                anim.SetBool("agachado", false);
                movementSpeed = velocidadInicial;
            }
            anim.SetBool("tocoSuelo", true);
        }
        else
        {
            EstoyCayendo();

        }

    }
    public void EstoyCayendo()
    {
        anim.SetBool("tocoSuelo", false);
        anim.SetBool("salte", false);
    }

    internal void SumarPunto()
    {
        throw new NotImplementedException();
    }
}


