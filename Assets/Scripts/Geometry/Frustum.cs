using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Frustum : MonoBehaviour
{
    [Tooltip("Whether to divide vertices when building mesh or not")]
    [SerializeField]
    bool splitVertices;

    [Tooltip("Mesh filter to use to show mesh")]
    [SerializeField]
    MeshFilter meshFilter;

    private List<Vector3> meshVertices = new List<Vector3>();
    private List<int> meshTriangles = new List<int>();

    /// <summary>
    /// The nearest frustrum plane coordinates in local space of frustrum object.
    /// Must be 4.
    /// </summary>
    private List<Vector3> nearPlanePoints = new List<Vector3>();
    /// <summary>
    /// Far frustrum plane coordinates in local space of frustrum object.
    /// Must be 4.
    /// </summary>
    private List<Vector3> farPlanePoints = new List<Vector3>();

    private Mesh mesh;

    public Mesh FrustumMesh
    {
        get
        {
            return mesh;
        }
    }

    public void Awake()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
    }

    /// <summary>
    /// Sets nearPlanePoints list
    /// </summary>
    /// <param name="nearPlanePointsWorld">Near plane points coordiantes in world space</param>
    public void SetNearPlanePoints(IEnumerable<Vector3> nearPlanePointsWorld)
    {
        nearPlanePoints = new List<Vector3>(nearPlanePointsWorld.Select((worldPoint) =>
        {
            return transform.InverseTransformPoint(worldPoint);
        }));
    }

    /// <summary>
    /// Sets farPlanePoints list
    /// </summary>
    /// <param name="nearPlanePointsWorld">Near far points coordiantes in world space</param>
    public void SetFarPlanePoints(IEnumerable<Vector3> farPlanePointsWorld)
    {
        farPlanePoints = new List<Vector3>(farPlanePointsWorld.Select((worldPoint) =>
        {
            return transform.InverseTransformPoint(worldPoint);
        }));
    }

    public bool GenerateNewMesh()
    {
        if (nearPlanePoints.Count < 4)
        {
            return false;
        }

        if (farPlanePoints.Count < 4)
        {
            return false;
        }

        meshVertices.Clear();
        meshTriangles.Clear();

        //0
        Vector3 A1 = nearPlanePoints[0];
        //1
        Vector3 A2 = nearPlanePoints[1];
        //2
        Vector3 A3 = nearPlanePoints[2];
        //3
        Vector3 A4 = nearPlanePoints[3];
        //4
        Vector3 B1 = farPlanePoints[0];
        //5
        Vector3 B2 = farPlanePoints[1];
        //6
        Vector3 B3 = farPlanePoints[2];
        //7
        Vector3 B4 = farPlanePoints[3];

        // Mesh vertices and triangles setting
        #region
        if (!splitVertices)
        {

            meshVertices.Add(A1);
            meshVertices.Add(A2);
            meshVertices.Add(A3);
            meshVertices.Add(A4);
            meshVertices.Add(B1);
            meshVertices.Add(B2);
            meshVertices.Add(B3);
            meshVertices.Add(B4);



            //A1,A2,A3
            meshTriangles.Add(0);
            meshTriangles.Add(1);
            meshTriangles.Add(2);

            //A1,A4,A3
            meshTriangles.Add(2);
            meshTriangles.Add(3);
            meshTriangles.Add(0);



            //B4,B3,B2
            meshTriangles.Add(7);
            meshTriangles.Add(6);
            meshTriangles.Add(5);

            //B2,B1,B4
            meshTriangles.Add(5);
            meshTriangles.Add(4);
            meshTriangles.Add(7);



            // ======================= SIDE 1

            //A2,B2,B3
            meshTriangles.Add(1);
            meshTriangles.Add(5);
            meshTriangles.Add(6);

            //B3,A3,A2
            meshTriangles.Add(6);
            meshTriangles.Add(2);
            meshTriangles.Add(1);

            // ======================= SIDE 2

            //B3,B4,A4
            meshTriangles.Add(6);
            meshTriangles.Add(7);
            meshTriangles.Add(3);

            //A4,A3,B3
            meshTriangles.Add(3);
            meshTriangles.Add(2);
            meshTriangles.Add(6);

            // ======================= SIDE 3

            //B4,B1,A1
            meshTriangles.Add(7);
            meshTriangles.Add(4);
            meshTriangles.Add(0);

            //A1,A4,B4
            meshTriangles.Add(0);
            meshTriangles.Add(3);
            meshTriangles.Add(7);

            // ======================= SIDE 4

            //A1,B1,B2
            meshTriangles.Add(0);
            meshTriangles.Add(4);
            meshTriangles.Add(5);

            //B2,A2,A1
            meshTriangles.Add(5);
            meshTriangles.Add(1);
            meshTriangles.Add(0);
        }
        else
        {
            //0-3
            meshVertices.Add(A1);
            meshVertices.Add(A2);
            meshVertices.Add(A3);
            meshVertices.Add(A4);

            //A1,A2,A3
            meshTriangles.Add(0);
            meshTriangles.Add(1);
            meshTriangles.Add(2);
            //A1,A4,A3
            meshTriangles.Add(2);
            meshTriangles.Add(3);
            meshTriangles.Add(0);



            //4-7
            meshVertices.Add(B1);
            meshVertices.Add(B2);
            meshVertices.Add(B3);
            meshVertices.Add(B4);

            //B4,B3,B2
            meshTriangles.Add(7);
            meshTriangles.Add(6);
            meshTriangles.Add(5);

            //B2,B1,B4
            meshTriangles.Add(5);
            meshTriangles.Add(4);
            meshTriangles.Add(7);



            // ======================= SIDE 1

            //8-11
            meshVertices.Add(A2);
            meshVertices.Add(A3);
            //10
            meshVertices.Add(B2);
            meshVertices.Add(B3);

            //A2,B2,B3
            meshTriangles.Add(8);
            meshTriangles.Add(10);
            meshTriangles.Add(11);

            //B3,A3,A2
            meshTriangles.Add(11);
            meshTriangles.Add(9);
            meshTriangles.Add(8);

            // ======================= SIDE 2

            //12-15
            meshVertices.Add(A3);
            meshVertices.Add(A4);
            //14
            meshVertices.Add(B3);
            meshVertices.Add(B4);

            //B3,B4,A4
            meshTriangles.Add(14);
            meshTriangles.Add(15);
            meshTriangles.Add(13);

            //A4,A3,B3
            meshTriangles.Add(13);
            meshTriangles.Add(12);
            meshTriangles.Add(14);

            // ======================= SIDE 3

            //16-19
            meshVertices.Add(A1);
            meshVertices.Add(A4);
            //18
            meshVertices.Add(B1);
            meshVertices.Add(B4);

            //B4,B1,A1
            meshTriangles.Add(19);
            meshTriangles.Add(18);
            meshTriangles.Add(16);

            //A1,A4,B4
            meshTriangles.Add(16);
            meshTriangles.Add(17);
            meshTriangles.Add(19);

            // ======================= SIDE 4

            //20-23
            meshVertices.Add(A1);
            meshVertices.Add(A2);
            //22
            meshVertices.Add(B1);
            meshVertices.Add(B2);

            //A1,B1,B2
            meshTriangles.Add(20);
            meshTriangles.Add(22);
            meshTriangles.Add(23);

            //B2,A2,A1
            meshTriangles.Add(23);
            meshTriangles.Add(21);
            meshTriangles.Add(20);
        }
        #endregion

        //optimization to reduce garbage
        //int count = meshTriangles.Count;
        //if (m_trianglesArray == null || m_trianglesArray.Length != count)
        //{
        //    m_trianglesArray = new int[count];
        //}
        //for (int i = 0; i < count; i++)
        //{
        //    m_trianglesArray[i] = meshTriangles[i];
        //}

        mesh.Clear();
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshTriangles.ToArray(), 0);

        if (splitVertices)
        {
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        meshFilter.mesh = mesh;

        return true;
    }
}
