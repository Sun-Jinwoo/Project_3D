using Unity.Mathematics;
using UnityEngine;
public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float fast = 8f;
    public float JumpForce = 5f;
    public float sensibility = 2f;
    public float limitCamX = 45f;
    public Transform cam;
    public int salud = 5;

    private Rigidbody rb;  
    private bool grounded;

    private float rotX = 0f;

    public Transform SpawnPoint;

    public float VelocidadEscala = 0.01f;
    float x = 1;
    float y = 1;
    float z = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //Raycast
        Debug.DrawRay(transform.position, Vector3.down * 1.8f, Color.red);
        Gizmos.color = Color.red;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1.8f);

        //Mover
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);

        //Saltar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                rb = GetComponent<Rigidbody>();
                rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                //rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
        }
        
        //Camara
        rotX += -Input.GetAxis("Mouse Y") * sensibility;
        rotX = Mathf.Clamp(rotX, -limitCamX, limitCamX);
        cam.localRotation = Quaternion.Euler(rotX,0,0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensibility, 0);

        Debug.DrawRay(cam.position, cam.forward * 5f, Color.blue);
        /*
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Pew Pew");

            RaycastHit hitted;
            if (Physics.Raycast(cam.position, cam.forward, out hitted, 5f))
            {
                if (hitted.transform.gameObject.CompareTag("Item"))
                {
                    Destroy(hitted.transform.gameObject);
                    Debug.Log("Saved");
                }
            }
        }*/


        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        { speed = fast; }
        else
        { speed = 5f; }


    }

    //Collision con objetos
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
        if (collision.gameObject.tag == "DeathZone")
        {
            transform.position = SpawnPoint.position;
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }

    //Rotaciones
    private void RotarX()
    { transform.rotation = Quaternion.Euler(x,0,0); x += 45; }
    private void RotarY()
    { transform.rotation = Quaternion.Euler(0,y,0); y += 45; }
    private void RotarZ()
    { transform.rotation = Quaternion.Euler(0,0,z); z += 45; }
}
