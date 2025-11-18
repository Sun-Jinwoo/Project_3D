using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive_Object : MonoBehaviour
{
    [Header("Configuración del movimiento")]
    public float amplitud = 0.5f;  
    public float velocidad = 1f;    

    private float posInicialY;

    void Start()
    {
     
        posInicialY = transform.position.y;
    }

    void Update()
    {
       
        float nuevaY = posInicialY + Mathf.Sin(Time.time * velocidad) * amplitud;

       
        transform.position = new Vector3(
            transform.position.x,
            nuevaY,
            transform.position.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
