using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Jobs;
using System.IO;
using System;

using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ex
{
    public static class Utils
    {
        public static void SetPosX(this Transform transform, float x)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = x;
            transform.position = newPosition;
        }
        
        public static void SetPosX(this TransformAccess transform, float x)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = x;
            transform.position = newPosition;
        }

        public static void SetPosY(this Transform transform, float y)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = y;
            transform.position = newPosition;
        }

        public static void SetPosY(this TransformAccess transform, float y)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = y;
            transform.position = newPosition;
        }

        public static void SetPosZ(this Transform transform, float z)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = z;
            transform.position = newPosition;
        }

        public static void SetPosZ(this TransformAccess transform, float z)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = z;
            transform.position = newPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RndVec2(float magnitude = 1) => new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * magnitude;

        public static float MapNum(float x, float originalMin, float originalMax, float newMin, float newMax)
        {
            // Apply the linear mapping formula
            return newMin + (x - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }

        /// <summary>
        /// this functions assumes your camera is tagged main and is a ortographic one.
        /// </summary>
        /// <returns></returns>
        public static Vector2 MousePos2D()
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new(pos.x, pos.y);
        }

        public static Mesh GenerateCircleMesh(float radius, int segmentCount = 32)
        {
            Mesh mesh = new();
            // Create vertices*
            int segments = segmentCount;
            Vector3[] vertices = new Vector3[segments + 1];
            vertices[0] = Vector3.zero; // Center vertex*
            for (int i = 0; i < segments; i++)
            {
                float angle = i * (2f * Mathf.PI / segments);
                vertices[i + 1] = new Vector3(

                     Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0
                    );
            }
            // Create triangles - reverse order to face forward*
            int[] triangles = new int[segments * 3];
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0; // Center vertex*
                triangles[i * 3 + 1] = (i + 2) % (segments + 1); // Next vertex, wrapping around*
                triangles[i * 3 + 2] = i + 1; // Current vertex*
            }
            // Fix the last triangle*
            triangles[^2] = 1; // Last triangle should connect to the first vertex*

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh GenerateQuadmesh(float width = 1f, float height = 1f)
        {
            Mesh mesh = new();

            // Create vertices
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(-width / 2, -height / 2, 0); // Bottom left
            vertices[1] = new Vector3(width / 2, -height / 2, 0);  // Bottom right
            vertices[2] = new Vector3(-width / 2, height / 2, 0);  // Top left
            vertices[3] = new Vector3(width / 2, height / 2, 0);   // Top right

            // Create triangles
            int[] triangles = new int[6];
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
            triangles[3] = 2;
            triangles[4] = 3;
            triangles[5] = 1;

            // Assign vertices and triangles to the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh GenerateLineMesh()
        {
            Mesh mesh = new();

            // Create vertices and indices for a simple line
            Vector3[] vertices = new Vector3[]
            {
                new (0, 0, 0),
                new (0, 0, 0)
            };

            int[] indices = new int[]
            {
                0, 1
            };

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0); // MeshTopolgy: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MeshTopology.html
            mesh.RecalculateBounds();

            return mesh;
        }

        public static (Vector2, Vector2) KeepWithinBounds2D(Bounds bounds, float radius, Vector2 pos, Vector2 vel)
        {
            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            if (pos.x > maxX || pos.x < minX)
            {
                pos.x = pos.x > maxX ? maxX : minX;
                vel.x *= -1;
            }
            if (pos.y > maxY || pos.y < minY)
            {
                pos.y = pos.y > maxY ? maxY : minY;
                vel.y *= -1;
            }

            return (pos, vel);
        }
#if UNITY_EDITOR
        [MenuItem("Tools/Ex/Materials/InstancingSupported/Generate_Circle_Material")]
        public static void GenerateInstanceSupportingCircleMaterial()
        {
            string shaderCode = @"
            Shader ""Ex/InstancingSupported/Circle""
            {
                            SubShader
                {
                                Pass
                    {
                                    CGPROGRAM
#pragma vertex vert
#pragma fragment frag

# include ""UnityCG.cginc""

                        StructuredBuffer<float2> _PositionBuffer;

                        struct v2f
                    {
                        float4 pos : SV_POSITION;
                        };

                    float4x4 CreateMatrixFromPosition(float4 position)
                    {
                        float4x4 translationMatrix = float4x4(
                            1, 0, 0, position.x,
                            0, 1, 0, position.y,
                            0, 0, 1, position.z,
                            0, 0, 0, 1
                        );

                        return translationMatrix;
                    }

                    float4x4 CreateMatrixFrom2DPosition(float2 position, float z)
                    {
                        float4x4 translationMatrix = float4x4(
                            1, 0, 0, position.x,
                            0, 1, 0, position.y,
                            0, 0, 1, z,
                            0, 0, 0, 1
                        );

                        return translationMatrix;
                    }


                    v2f vert(appdata_base v, uint instanceID : SV_InstanceID)
                    {
                        v2f o;
                        // Fetch position directly from buffer
                        float2 instancePosition = _PositionBuffer[instanceID];

                        // Create matrix procedurally in shader
                        float4x4 instanceMatrix = CreateMatrixFrom2DPosition(instancePosition, 0);

                        // Transform vertex
                        o.pos = mul(UNITY_MATRIX_VP, mul(instanceMatrix, v.vertex));

                        return o;
                    }

                    float4 frag(v2f i) : SV_Target
                        {
                            return float4(1,1,1,1);
                }
                ENDCG
            }
                }
            }";
            string shaderFileName = "InstancedCircle";
            CreateFileIfNotExists(shaderFileName, shaderCode, "shader", "Ex/Shaders");
            Shader myShader = Shader.Find("Ex/InstancingSupported/Circle");
            if (myShader != null)
            {
                // Use the shader, for example, assign it to a material
                Material material = new (myShader);

                // Save material as an asset
                string materialPath = "Assets/Ex/Materials/CircleMaterial.mat";
                Directory.CreateDirectory(Path.GetDirectoryName(materialPath));
                AssetDatabase.CreateAsset(material, materialPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Material created and saved at: " + materialPath);
            }
            else
            {
                Debug.LogError("Failed to create shader from string.");
                throw new NotSupportedException();
            }
        }

        public static void CreateFileIfNotExists(string fileName, string fileContent, string fileExtension, string path)
        {
            if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileContent) || string.IsNullOrEmpty(fileExtension) || string.IsNullOrEmpty(path)) throw new NotSupportedException();
            // Convert file name to a valid file path
            fileName = fileName.Replace(" ", "_") + "." + fileExtension;

            // Determine the project path
            string filePath = Path.Combine(Application.dataPath, path, fileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Check if the file already exists
            if (!File.Exists(filePath))
            {
                // Write the shader content to the file
                File.WriteAllText(filePath, fileContent);

                // Refresh the AssetDatabase to make Unity recognize the new file
                AssetDatabase.Refresh();

                Debug.Log($"File created at: {filePath}");
            }
            else
            {
                Debug.Log($"File already exists at: {filePath}");
            }
        }
#endif
    }

    class ListPool
    {
        private readonly Stack<List<int>> pool = new();
        readonly int listInitialCapacity;

        public ListPool(int _listInitialCapacity = 0)
        {
            listInitialCapacity = _listInitialCapacity;
        }

        public List<int> Get()
        {
            return pool.Count > 0 ? pool.Pop() : new List<int>(listInitialCapacity);
        }

        public void Return(List<int> list)
        {
            list.Clear();
            pool.Push(list);
        }
    }

    struct Grid
    {
        public int columnCount;
        public int rowCount;
        public float cellWidth;
        public float cellHeight;
        public Vector2 centerPos;
        public readonly int cellCount => rowCount * columnCount;
        public readonly float width => columnCount * cellWidth;
        public readonly float height => rowCount * cellHeight;
        public readonly Vector2 bottomLeftPos => new(centerPos.x - width / 2, centerPos.y - height / 2);
        public readonly Vector2 topLeftPos => new(centerPos.x - width / 2, centerPos.y + height / 2);
        public readonly Vector2 bottomRightPos => new(centerPos.x + width / 2, centerPos.y - height / 2);
        public readonly Vector2 topRightPos => new(centerPos.x + width / 2, centerPos.y + height / 2);


        public Grid(int _columnCount, int _rowCount, Vector2 _centerPos, float _cellWidth, float _cellHeight)
        {
            columnCount = _columnCount;
            rowCount = _rowCount;
            cellWidth = _cellWidth;
            cellHeight = _cellHeight;
            centerPos = _centerPos;
        }

        public readonly (bool, Vector2Int) MapToGrid(Vector2 pos)
        {
            if (pos.x < bottomLeftPos.x || pos.x > bottomRightPos.x || pos.y < bottomLeftPos.y || pos.y > topLeftPos.y) return (false, new Vector2Int(-1, -1)); // out of bounds.

            int gridX = Mathf.FloorToInt((pos.x - bottomLeftPos.x) / cellWidth);
            int gridY = Mathf.FloorToInt((pos.y - bottomLeftPos.y) / cellHeight);

            // Clamp values to avoid off-by-one errors. (The calculation of gridX and gridY in MapToGrid uses Mathf.FloorToInt, which could cause an off-by-one error for edge cases near the grid’s boundaries. If pos.x is exactly on the grid’s right edge (bottomRightPos.x), the calculated gridX might exceed columnCount - 1. Similarly, if pos.y is exactly on the top edge, gridY might exceed rowCount -1.
            gridX = Mathf.Clamp(gridX, 0, columnCount - 1);
            gridY = Mathf.Clamp(gridY, 0, rowCount - 1);

            return (true, new Vector2Int(gridX, gridY));
        }

        public readonly void Draw()
        {
            Debug.DrawRay(bottomLeftPos, Vector2.left, Color.magenta);
            Debug.DrawRay(topLeftPos, Vector2.left, Color.magenta);
            Debug.DrawRay(bottomRightPos, Vector2.right, Color.magenta);
            Debug.DrawRay(topRightPos, Vector2.right, Color.magenta);

            for (int y = 0; y <= rowCount; y++)
            {
                Debug.DrawLine(bottomLeftPos + new Vector2(0, cellHeight) * y, bottomRightPos + new Vector2(0, cellHeight) * y, Color.red);
            }

            for (int x = 0; x <= columnCount; x++)
            {
                Debug.DrawLine(bottomLeftPos + new Vector2(cellWidth, 0) * x, topLeftPos + new Vector2(cellWidth, 0) * x, Color.green);
            }
        }
    }

    class ExGraphics
    {
        readonly Mesh circleMesh;
        RenderParams circleRp;
        ComputeBuffer circlePositionBuffer;

        readonly Mesh lineMesh;
        RenderParams lineRp;
        ComputeBuffer linePositionBuffer;

        public ExGraphics(Material circleMat, Material lineMat)
        {
            circleMesh = Utils.GenerateCircleMesh(1);
            circlePositionBuffer = new ComputeBuffer(1, sizeof(float) * 2);
            circleRp = new(circleMat)
            {
                worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one), // use tighter bounds
                matProps = new MaterialPropertyBlock()
            };

            lineMesh = Utils.GenerateLineMesh();
            linePositionBuffer = new ComputeBuffer(1, sizeof(float) * 4);
            lineRp = new(lineMat)
            {
                worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one), // use tighter bounds
                matProps = new MaterialPropertyBlock()
            };
        }

        public void DrawCircles(List<Vector2> positions)
        {
            if (positions.Count == 0) return;
            if (circlePositionBuffer.count < positions.Count)
            {
                // Dispose of the old buffer
                circlePositionBuffer.Dispose();

                // Create a new buffer with increased capacity
                // Use the next power of 2 to minimize frequent reallocations
                int newCapacity = Mathf.NextPowerOfTwo(positions.Count);
                circlePositionBuffer = new ComputeBuffer(newCapacity, sizeof(float) * 2);
            }

            circlePositionBuffer.SetData(positions.ToArray());
            circleRp.matProps.SetBuffer("_PositionBuffer", circlePositionBuffer);
            Graphics.RenderMeshPrimitives(circleRp, circleMesh, 0, circlePositionBuffer.count);
        }

        public void DrawLines(List<Vector4> lines)
        {
            if (lines.Count == 0) return;
            if (linePositionBuffer.count < lines.Count)
            {
                linePositionBuffer.Dispose();
                int newCap = Mathf.NextPowerOfTwo(lines.Count);
                linePositionBuffer = new ComputeBuffer(newCap, sizeof(float) * 4);
            }

            linePositionBuffer.SetData(lines.ToArray());
            lineRp.matProps.SetBuffer("_LinesBuffer", linePositionBuffer);
            Graphics.RenderMeshPrimitives(lineRp, lineMesh, 0, linePositionBuffer.count);
        }

        public void Dispose()
        {
            circlePositionBuffer?.Dispose();
            circlePositionBuffer?.Release();

            linePositionBuffer?.Dispose();
            linePositionBuffer?.Release();
        }
    }

    public class ResizableArray<T>
    {
        public T[] array;
        public int Count { get; private set; }

        public ResizableArray(int initialCapacity = 4)
        {
            array = new T[initialCapacity];
            Count = 0;
        }

        public void Add(T item)
        {
            if (Count == array.Length)
            {
                Array.Resize(ref array, array.Length * 2);
            }
            array[Count++] = item;
        }
    }

    public class Spring
    {
        [System.Serializable]
        public struct SpringFeatures
        {
            public float springLength;
            public float springStiffness;
            public float damping;
        }
        public float springLength = 1;          // Length of the spring
        public float springStiffness = 0.5f;    // Controls how "stiff" the spring is
        public float damping = 0.5f;            // Controls how quickly oscillations settle
        public float mass = 1f;                 // Mass of the point
        public Vector3 velocity = Vector3.zero; // Current velocity of the point

        public Spring()
        {

        }

        public Spring(SpringFeatures springFeatures)
        {
            springLength = springFeatures.springLength;
            springStiffness = springFeatures.springStiffness;
            damping = springFeatures.damping;
            mass = 1f; // ? mass is not properly supported yet 
        }

        public void PointPointSpring(Transform pointA, Transform pointB)
        {
            var velocity = SpringCore(pointA.position, pointB.position);

            pointB.position += velocity;
            pointA.position -= velocity;
        }

        public void AnchorPointSpring(Transform anchor, Transform point)
        {
            var velocity = SpringCore(anchor.position, point.position);
            point.position += velocity;
        }

        // TODO: make this frame rate independent.
        public Vector3 SpringCore(Vector3 p1, Vector3 p2)
        {
            // Calculate spring force using Hooke's Law: F = -kx
            Vector3 displacement = p2 - p1;

            // Calculate the current distance between anchor and point
            float currentDistance = displacement.magnitude;
            // Normalize the displacement vector
            Vector3 direction = displacement.normalized;
            // Calculate the spring force using Hooke's Law: F = -k * (x - springLength)
            Vector3 springForce = -springStiffness * (currentDistance - springLength) * direction;

            // Calculate damping force: F = -cv
            Vector3 dampingForce = -damping * velocity;

            // Sum up forces
            Vector3 totalForce = springForce + dampingForce;

            // Calculate acceleration (F = ma)
            Vector3 acceleration = totalForce / mass;

            // Update velocity (integrate acceleration)
            velocity += acceleration;

            velocity.y = 0;

            return velocity;
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

    namespace Verlet
    {
        public class Point
        {
            public Transform tr;
            public Vector3 oldPos;
            public bool pinned;
            readonly float radius;

            public Point(Transform transform, bool pinned = false)
            {
                tr = transform;
                oldPos = tr.position;
                this.pinned = pinned;
                radius = tr.localScale.x / 2;
            }

            public void Tick()
            {
                if (pinned)
                {
                    oldPos = tr.position; // otherwise the oldPos will be the its value when we pinned the point and when it got unpinned this will cause to point move unexpectedly since its oldPos is not updated correctly.
                    return;
                }
                Vector3 v = tr.position - oldPos;
                oldPos = tr.position;
                tr.position += v;

                tr.position += new Vector3(0, -0.1f, 0);
            }

            public void ConstrainWorldBounds(Bounds bounds)
            {
                if (pinned) return;

                float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
                float minX = bounds.center.x - bounds.extents.x / 2 + radius;
                float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
                float minY = bounds.center.y - bounds.extents.y / 2 + radius;

                Vector3 pos = tr.position;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                tr.position = pos;
            }
        }

        public class Stick
        {
            public Point pointA;
            public Point pointB;
            public LineRenderer lineRnd;
            readonly float length;

            public Stick(Point a, Point b, LineRenderer lineRenderer)
            {
                pointA = a;
                pointB = b;
                lineRnd = lineRenderer;
                lineRnd.positionCount = 2;
                length = Vector2.Distance(pointA.tr.position, pointB.tr.position);
            }

            public void Tick()
            {
                float distance = Vector3.Distance(pointA.tr.position, pointB.tr.position);
                float difference = length - distance;
                float percent = difference / distance;

                float dx = pointB.tr.position.x - pointA.tr.position.x;
                float dy = pointB.tr.position.y - pointA.tr.position.y;
                Vector3 offset = Vector3.zero;
                offset.x = dx * percent / 2;
                offset.y = dy * percent / 2;

                if (!pointA.pinned)
                {
                    pointA.tr.position -= offset;
                }
                if (!pointB.pinned)
                {
                    pointB.tr.position += offset;
                }
            }
        }
    }
}