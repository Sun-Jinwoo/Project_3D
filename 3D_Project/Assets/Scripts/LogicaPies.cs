using UnityEngine;

public class LogicaPies : MonoBehaviour
{
    public LogicaMovimiento logicaMovimiento;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (logicaMovimiento != null)
        {
            logicaMovimiento.puedoSaltar = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (logicaMovimiento != null)
        {
            logicaMovimiento.puedoSaltar = false;
        }
    }
}
