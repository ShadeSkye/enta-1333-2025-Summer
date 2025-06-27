using UnityEngine;
using UnityEngine.Pool;

public class ShootOverTime : MonoBehaviour
{
	[Header("Projectile")]
	[SerializeField] private ArcingProjectile ProjectilePrefab;
	[SerializeField] private Transform ProjectileStartPoint;

	// Replace this with some kind of targeted enemy position
	[SerializeField] private Transform DebugEndPoint;

	[Header("Flight Settings")]
	[SerializeField] [Min(0.1f)] private float ProjectileSpeed = 5f; // units / second
	[SerializeField] private float ArcHeight = 2f; // world units

	[SerializeField] private PoolManager poolManager;

	/// <summary>
	///     Fires the projectile
	/// </summary>
	public void Shoot()
	{
		// Instantiate with generic overload so no GetComponent is needed.

		ArcingProjectile proj = poolManager.Pool.Get();

		proj.Launch(transform.position, DebugEndPoint.position, ProjectileSpeed, ArcHeight);
	}
}
