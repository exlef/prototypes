using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    List<Agent> agents;
    void Start()
    {
        agents = FindObjectsByType<Agent>(FindObjectsSortMode.None).ToList();
        Debug.Log(agents.Count);
    }

    // Update is called once per frame
    void Update()
    {
        for (int j = 0; j < agents.Count; j++)
        {
            for (int k = j+1; k < agents.Count; k++)
            {
                var a = agents[j];
                var b = agents[k];
                var result = ExPhysics2d.SolveCirclesCollisionBasedOnSize(a.pos, a.radius, b.pos, b.radius);
                a.pos = result.Item1;
                b.pos = result.Item2;
            }
        }
    }
}

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
    }
