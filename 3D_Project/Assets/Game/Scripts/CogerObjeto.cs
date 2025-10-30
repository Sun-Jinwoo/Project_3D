using UnityEngine;

public class CogerObjeto : MonoBehaviour
{

    public GameObject handPoint;
    private GameObject pickedObject = null;
    // Update is called once per frame
    void Update()
    {
        
        if (pickedObject != null)
        {
            if (Input.GetKey("q"))
            {
                pickedObject.GetComponent<Rigidbody>().useGravity = true;
                pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                pickedObject.transform.SetParent(null);
                pickedObject = null;
            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ObjetoAgarrable"))
        {
            if (Input.GetKey("e")) { 
                other.GetComponent<Rigidbody>().useGravity = false;

                other.GetComponent<Rigidbody>().useGravity = true;

                other.transform.position = handPoint.transform.position;

                other.gameObject.transform.SetParent(handPoint.gameObject.transform);

                pickedObject = other.gameObject;
            }
        }
    }

}
