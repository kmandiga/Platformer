using UnityEngine;
using System.Collections;

public interface IHittable
{
    float percentage {get;set;}
    float weight {get;set;}
    void GotHit(Vector2 knockback, float hitstun, float damage);
    void CalculateVelocity();
}