using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private ArcingProjectile projectile;
    public bool collectionChecks = true;
    public int maxPoolSize = 10;
    IObjectPool<ArcingProjectile> m_Pool;

    public IObjectPool<ArcingProjectile> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                m_Pool = new ObjectPool<ArcingProjectile>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
            }
            return m_Pool;
        }
    }

    ArcingProjectile CreatePooledItem()
    {
        var proj =
            Instantiate(projectile, transform.position, Quaternion.identity);
        return proj;
    }

    // Called when an item is returned to the pool using Release
    public void OnReturnedToPool(ArcingProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    public void OnTakeFromPool(ArcingProjectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    public void OnDestroyPoolObject(ArcingProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
