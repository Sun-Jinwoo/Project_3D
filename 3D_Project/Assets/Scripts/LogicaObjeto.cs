using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogicaObjeto : MonoBehaviour
{
    public bool destruirConCursor;
    public bool destruirAutomatico;
    public LogicaMovimiento logicaMovimiento;

    public int tipo;

    //1 = crece
    //2 = aumenta velocidad
    //3 = aumenta salto


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("player");
        if (players.Length > 0)
        {
            logicaMovimiento = players[0].GetComponent<LogicaMovimiento>();
            if (logicaMovimiento == null)
            {
                Debug.LogError("El objeto con tag 'player' no tiene el componente LogicaMovimiento.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con tag 'player' en la escena.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Efecto()
    {
        switch (tipo)
        {
            case 1:
                logicaMovimiento.gameObject.transform.localScale = new Vector3(3, 3, 3); 
                break;
            case 2:
                logicaMovimiento.velocidadInicial += 5;
                break;
            case 3:
                // En vez de aumentar fuerza de salto: sumar punto al jugador y destruir este objeto
                if (logicaMovimiento != null)
                {
                    logicaMovimiento.SumarPunto();
                }
                Destroy(gameObject);
                break;
            case 4:
                logicaMovimiento.fuerzaDeSalto += 10;
                break;
            default:
                Debug.Log("sin efecto");
                break;
        }
    }
}
