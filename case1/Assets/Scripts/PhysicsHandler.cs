using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: no reason for this class to derive monobehavior. fix it.
/*
 * the reason we do the 2d physics is that the problem in hand is suitable for solving in 2d.
 * we basically need to solve the collisions in XZ plane. it is cheaper this way.
 * although since we check every circle to every other circle there is a lot of unnecessary checks
 * I could've added spatial partitioning to minimize that, but I couldn't find the time.
 * also see: The Agent.cs 
 */
public class PhysicsHandler : MonoBehaviour
{
    public void Tick()
    {
        foreach (var mob in GameManager.instance.mobs)
        {
            mob.agent.Tick();
        }
        
        for (int j = 0; j < GameManager.instance.mobs.Count; j++)
        {
            for (int k = j+1; k < GameManager.instance.mobs.Count; k++)
            {
                var a = GameManager.instance.mobs[j].agent;
                var b = GameManager.instance.mobs[k].agent;
                Resolve(a, b);
            }
        }

        if (GameManager.instance.pushBox.isDropped == false)
        {
            foreach (var mob in GameManager.instance.mobs)
            {
                var result = ExPhysics2d.SolveCircleAABBCollisionBasedOnWeight(mob.agent.pos, mob.agent.radius, GameManager.instance.GetWeightValue(mob),
                    GameManager.instance.pushBox.pos, GameManager.instance.pushBox.size, GameManager.instance.pushBoxWeight);
                mob.agent.pos = result.Item1;
                GameManager.instance.pushBox.pos = result.Item2;
            }
        }
        
        foreach (var mob in GameManager.instance.mobs)
        {
            foreach (var staticWall in GameManager.instance.staticLevelWalls)
            {
                var newCirclePos = ExPhysics2d.SolveCircleAABBStaticDynamic(
                    staticWall.pos, staticWall.size,
                    mob.agent.pos, mob.agent.radius
                );
                mob.agent.pos = newCirclePos;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        foreach (var enemy in GameManager.instance.enemies)
        {
            enemy.agent.Tick();
        }
        
        for (int j = 0; j < GameManager.instance.enemies.Count; j++)
        {
            for (int k = j+1; k < GameManager.instance.enemies.Count; k++)
            {
                var a = GameManager.instance.enemies[j].agent;
                var b = GameManager.instance.enemies[k].agent;
                Resolve(a, b);
            }
        }

        if (GameManager.instance.pushBox.isDropped == false)
        {
            foreach (var enemy in GameManager.instance.enemies)
            {
                var result = ExPhysics2d.SolveCircleAABBCollisionBasedOnWeight(enemy.agent.pos, enemy.agent.radius, GameManager.instance.GetWeightValue(enemy),
                    GameManager.instance.pushBox.pos, GameManager.instance.pushBox.size, GameManager.instance.pushBoxWeight);
                enemy.agent.pos = result.Item1;
                GameManager.instance.pushBox.pos = result.Item2;
            }    
        }
        
        foreach (var enemy in GameManager.instance.enemies)
        {
            foreach (var staticWall in GameManager.instance.staticLevelWalls)
            {
                var newCirclePos = ExPhysics2d.SolveCircleAABBStaticDynamic(
                    staticWall.pos, staticWall.size,
                    enemy.agent.pos, enemy.agent.radius
                );
                enemy.agent.pos = newCirclePos;
            }
        }
    }

    void Resolve(Agent a, Agent b)
    {
        var result = ExPhysics2d.SolveCirclesCollisionBasedOnSize(a.pos, a.radius, b.pos, b.radius);
        a.pos = result.Item1;
        b.pos = result.Item2;
    }
}

/*
 * this class written by myself. I wrote this for another project that's why the naming is prefixed with Ex. 
 */
public static class ExPhysics2d
{
    public struct CirclesCollisionSolution
    {
        public bool isColliding;
        public Vector2 displacementADir;
        public Vector2 displacementBDir;
        public float overlap;

        public CirclesCollisionSolution(bool isColliding, Vector2 displacementADir, Vector2 displacementBDir, float overlap)
        {
            this.isColliding = isColliding;
            this.displacementADir = displacementADir;
            this.displacementBDir = displacementBDir;
            this.overlap = overlap;
        }
    }
    /// <summary>
    /// solves the collision by moving both circles equally.
    /// this will return the given positions if there is no collision.
    /// otherwise will return new positions for a and b in this order.
    /// </summary>
    public static (Vector2, Vector2) SolveCircles(Vector2 aPos, float aRadi, Vector2 bPos, float bRadi)
    {
        // since we want move both circles same amount we'll give weight equally.
        return SolveCirclesCollisionBasedOnWeight(aPos, aRadi, 0.5f, bPos, bRadi, 0.5f);
    }
    
    /// <summary>
    /// solves collision by only moving the dynamic circle.
    /// </summary>
    /// <param name="sPos">position of static circle</param>
    /// <param name="sRadi">radius of static circle</param>
    /// <param name="dPos">position of dynamic circle</param>
    /// <param name="dRadi">radius of dynamic circle</param>
    /// <returns>the new position of dynamic circles</returns>
    public static Vector2 SolveCirclesStaticDynamic(Vector2 sPos, float sRadi, Vector2 dPos, float dRadi)
    {
        return SolveCirclesCollisionBasedOnWeight(sPos, sRadi, 1.0f, dPos, dRadi, 0.0f).Item2;
    }
    /// <summary>
    /// solves the collision by moving circles based on radius.
    /// this will return the given positions if there is no collision.
    /// otherwise will return new positions for a and b in this order.
    /// </summary>
    public static (Vector2, Vector2) SolveCirclesCollisionBasedOnSize(Vector2 aPos, float aRadi, Vector2 bPos, float bRadi)
    {
        return SolveCirclesCollisionBasedOnWeight(aPos, aRadi, aRadi, bPos, bRadi, bRadi);
    }

    /// <summary>
    /// solves the collision by moving circles based on giving weight.
    /// the lighter one moves further.
    /// this will return the given positions back if there is no collision.
    /// otherwise will return new positions for a and b in this order.
    /// </summary>
    public static (Vector2, Vector2) SolveCirclesCollisionBasedOnWeight(Vector2 aPos, float aRadi, float aWeight, Vector2 bPos, float bRadi, float bWeight)
    {
        var result = CalcCirclesCollisionSolution(aPos, aRadi, bPos, bRadi);
        if(result.isColliding == false) return (aPos, bPos);
        
        float totalWeight = aWeight + bWeight;
        if (totalWeight <= Mathf.Epsilon) throw new NotSupportedException("The weight shouldn't be equal or smaller than zero");
        aWeight /= totalWeight;
        bWeight /= totalWeight;
        
        // we multiply with the other's weight since we want light one move more.
        aPos += result.displacementADir * (result.overlap * bWeight);
        bPos += result.displacementBDir * (result.overlap * aWeight);

        return (aPos, bPos);
    }

    public static CirclesCollisionSolution CalcCirclesCollisionSolution(Vector2 aPos, float aRadi, Vector2 bPos, float bRadi)
    {
        if (!CirclesCheck(aPos, aRadi, bPos, bRadi, out float overlap)) return new(false, Vector2.zero, Vector2.zero, 0);
        Vector2 AtoBdir = (bPos - aPos).normalized;
        Vector2 BtoAdir = (aPos - bPos).normalized;
        
        // When positions are identical, push in a random direction
        if (AtoBdir.sqrMagnitude < Mathf.Epsilon)
        {
            float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            AtoBdir = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            BtoAdir = -AtoBdir;
        }

        return new(true, BtoAdir, AtoBdir, overlap);
    }

    /// <summary>
    /// checks if two circles are colliding.
    /// </summary>
    public static bool CirclesCheck(Vector2 aPos, float aRadius, Vector2 bPos, float bRadius, out float overlap)
    {
        float dist = Vector2.Distance(aPos, bPos);
        float totalRadius = aRadius + bRadius;
        overlap = totalRadius - dist;
        return dist <= totalRadius;
    }
    
    /////////
    
    
    
    public struct CircleAABBCollisionSolution
    {
        public bool isColliding;
        public Vector2 displacementCircleDir;
        public Vector2 displacementAABBDir;
        public float overlap;

        public CircleAABBCollisionSolution(bool isColliding, Vector2 displacementCircleDir, Vector2 displacementAABBDir, float overlap)
        {
            this.isColliding = isColliding;
            this.displacementCircleDir = displacementCircleDir;
            this.displacementAABBDir = displacementAABBDir;
            this.overlap = overlap;
        }
    }

    /// <summary>
    /// Solves collision between a circle and an AABB by moving both equally.
    /// Returns original positions if there is no collision.
    /// Returns new positions for circle and AABB center in this order.
    /// </summary>
    public static (Vector2, Vector2) SolveCircleAABB(Vector2 circlePos, float radius, Vector2 aabbCenter, Vector2 aabbSize)
    {
        return SolveCircleAABBCollisionBasedOnWeight(circlePos, radius, 0.5f, aabbCenter, aabbSize, 0.5f);
    }

    /// <summary>
    /// Solves collision between a static AABB and a dynamic circle.
    /// Returns the new position of the circle.
    /// </summary>
    public static Vector2 SolveCircleAABBStaticDynamic(Vector2 aabbCenter, Vector2 aabbSize, Vector2 circlePos, float radius)
    {
        return SolveCircleAABBCollisionBasedOnWeight(circlePos, radius, 0.0f, aabbCenter, aabbSize, 1.0f).Item1;
    }

    /// <summary>
    /// Solves the collision between a circle and an AABB based on given weights.
    /// The lighter object moves further.
    /// Returns original positions if there is no collision.
    /// Returns new positions for circle and AABB center in this order.
    /// </summary>
    public static (Vector2, Vector2) SolveCircleAABBCollisionBasedOnWeight(Vector2 circlePos, float radius, float circleWeight, 
        Vector2 aabbCenter, Vector2 aabbSize, float aabbWeight)
    {
        var result = CalcCircleAABBCollisionSolution(circlePos, radius, aabbCenter, aabbSize);
        if (!result.isColliding) return (circlePos, aabbCenter);

        float totalWeight = circleWeight + aabbWeight;
        if (totalWeight <= Mathf.Epsilon) throw new NotSupportedException("The weight shouldn't be equal or smaller than zero");
        circleWeight /= totalWeight;
        aabbWeight /= totalWeight;

        // Multiply with the other's weight since we want the lighter one to move more
        circlePos += result.displacementCircleDir * (result.overlap * aabbWeight);
        aabbCenter += result.displacementAABBDir * (result.overlap * circleWeight);

        return (circlePos, aabbCenter);
    }

    public static CircleAABBCollisionSolution CalcCircleAABBCollisionSolution(Vector2 circlePos, float radius, Vector2 aabbCenter, Vector2 aabbSize)
    {
        if (!CircleAABBCheck(circlePos, radius, aabbCenter, aabbSize, out float overlap, out Vector2 normal))
            return new(false, Vector2.zero, Vector2.zero, 0);

        // When normal is zero (circle center inside AABB), push in direction of closest edge
        if (normal.sqrMagnitude < Mathf.Epsilon)
        {
            Vector2 toCenter = aabbCenter - circlePos;
            Vector2 halfSize = aabbSize * 0.5f;
            float absX = Mathf.Abs(toCenter.x);
            float absY = Mathf.Abs(toCenter.y);
            
            // Find closest edge
            if (absX / halfSize.x > absY / halfSize.y)
                normal = new Vector2(toCenter.x > 0 ? -1 : 1, 0);
            else
                normal = new Vector2(0, toCenter.y > 0 ? -1 : 1);
        }

        return new(true, normal, -normal, overlap);
    }

    /// <summary>
    /// Checks if a circle and AABB are colliding.
    /// </summary>
    public static bool CircleAABBCheck(Vector2 circlePos, float radius, Vector2 aabbCenter, Vector2 aabbSize, 
        out float overlap, out Vector2 normal)
    {
        normal = Vector2.zero;
        Vector2 halfSize = aabbSize * 0.5f;
        
        // Find the closest point on the AABB to the circle center
        Vector2 closest = new Vector2(
            Mathf.Clamp(circlePos.x, aabbCenter.x - halfSize.x, aabbCenter.x + halfSize.x),
            Mathf.Clamp(circlePos.y, aabbCenter.y - halfSize.y, aabbCenter.y + halfSize.y)
        );

        Vector2 distance = circlePos - closest;
        float distanceSqr = distance.sqrMagnitude;

        if (distanceSqr > 0)
            normal = distance.normalized;

        overlap = radius - Mathf.Sqrt(distanceSqr);
        return overlap > 0;
    }
    
    
    
    /////////
}

