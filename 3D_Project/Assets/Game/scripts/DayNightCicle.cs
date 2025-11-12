using UnityEngine;

public class DayNightCicle : MonoBehaviour
{
    
    void Start()
    {
        
    }

    //Nota: Se realiza para rotar la light y que cambie de dia a noche y viceversa
    public int rotationScale = 10; 

    void Update()
    {
        transform.Rotate(rotationScale * Time.deltaTime, 0, 0);
    }
}
