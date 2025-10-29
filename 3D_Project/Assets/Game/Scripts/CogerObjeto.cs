using UnityEngine;

public class CogerObjeto : MonoBehaviour
{

    public GameObject handPoint;
    private GameObject pickedObject = null;
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Objeto"))
    }

}
