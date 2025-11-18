using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnHitPlayer(collision.gameObject);
            Destroy(gameObject);
        }
    }

    protected abstract void OnHitPlayer(GameObject player);
}
