using UnityEngine;
using System.Collections;

public interface IHittable
{
    float playerPercentage {get;set;}
    float playerWeight {get;set;}
    void GotHit(Vector2 knockback, float hitstun, float damage);
}