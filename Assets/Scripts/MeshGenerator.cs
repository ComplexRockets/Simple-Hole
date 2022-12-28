using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [Range(0, 1)]
    public float

            corner1Radius,
            corner2Radius,
            corner3Radius,
            corner4Radius;

    public float

            radius,
            depth,
            test;

    public int cornerRes;

    private Mesh

            depthMesh,
            bottomMesh;

    private MeshRenderer

            depthMeshRenderer,
            bottomMeshRenderer;

    private MeshFilter

            depthMeshFilter,
            bottomMeshFilter;

    public float RoundEdges = 0.5f;

    public float RoundTopLeft = 0.0f;

    public float RoundTopRight = 0.0f;

    public float RoundBottomLeft = 0.0f;

    public float RoundBottomRight = 0.0f;

    public bool UsePercentage = true;

    public Rect rect = new Rect(-0.5f, -0.5f, 1f, 1f);

    public float Scale = 1f;

    public int CornerVertexCount = 8;

    public bool CreateUV = true;

    public bool FlipBackFaceUV = false;

    public bool DoubleSided = false;

    public bool AutoUpdate = false;

    private MeshFilter m_MeshFilter;

    private Mesh m_Mesh;

    private Vector3[] m_Vertices;

    private Vector3[] m_Normals;

    private Vector2[] m_UV;

    private int[] m_Triangles;

    void Start()
    {
        //Creating Meshes
        m_Mesh = depthMesh = new Mesh();
        bottomMesh = new Mesh();

        //Creating the gamobjects holding the meshes and setting them as children of this one
        GameObject depthObj = new GameObject();
        GameObject bottomObj = new GameObject();
        depthObj.transform.SetParent(this.transform);
        bottomObj.transform.SetParent(this.transform);
        depthObj.name = "DepthMaskMesh";
        bottomObj.name = "BottomMesh";

        //Creating and setting up the mesh renderers and mesh filters
        depthMeshRenderer = depthObj.AddComponent<MeshRenderer>();
        depthMeshRenderer.material = new Material(Shader.Find("Standard"));
        m_MeshFilter = depthMeshFilter = depthObj.AddComponent<MeshFilter>();
        depthMeshFilter.mesh = depthMesh;
        m_MeshFilter.sharedMesh = m_Mesh;

        bottomMeshRenderer = bottomObj.AddComponent<MeshRenderer>();
        bottomMeshRenderer.materials =
            new Material[2]
            {
                new Material(Shader.Find("Standard")),
                new Material(Shader.Find("Standard"))
            };
        bottomMeshFilter = bottomObj.AddComponent<MeshFilter>();
        bottomMeshFilter.mesh = bottomMesh;
    }

    public Mesh UpdateMesh()
    {
        if (CornerVertexCount < 2) CornerVertexCount = 2;
        int sides = DoubleSided ? 2 : 1;
        int vCount = CornerVertexCount * 4 * sides + sides; //+sides for center vertices
        int triCount = (CornerVertexCount * 4) * sides;
        if (m_Vertices == null || m_Vertices.Length != vCount)
        {
            m_Vertices = new Vector3[vCount];
            m_Normals = new Vector3[vCount];
        }
        if (m_Triangles == null || m_Triangles.Length != triCount * 3)
            m_Triangles = new int[triCount * 3];
        if (CreateUV && (m_UV == null || m_UV.Length != vCount))
        {
            m_UV = new Vector2[vCount];
        }
        int count = CornerVertexCount * 4;
        if (CreateUV)
        {
            m_UV[0] = Vector2.one * 0.5f;
            if (DoubleSided) m_UV[count + 1] = m_UV[0];
        }
        float tl = Mathf.Max(0, RoundTopLeft + RoundEdges);
        float tr = Mathf.Max(0, RoundTopRight + RoundEdges);
        float bl = Mathf.Max(0, RoundBottomLeft + RoundEdges);
        float br = Mathf.Max(0, RoundBottomRight + RoundEdges);
        float f = Mathf.PI * 0.5f / (CornerVertexCount - 1);
        float a1 = 1f;
        float a2 = 1f;
        float x = 1f;
        float y = 1f;
        Vector2 rs = Vector2.one;
        if (UsePercentage)
        {
            rs = new Vector2(rect.width, rect.height) * 0.5f;
            if (rect.width > rect.height)
                a1 = rect.height / rect.width;
            else
                a2 = rect.width / rect.height;
            tl = Mathf.Clamp01(tl);
            tr = Mathf.Clamp01(tr);
            bl = Mathf.Clamp01(bl);
            br = Mathf.Clamp01(br);
        }
        else
        {
            x = rect.width * 0.5f;
            y = rect.height * 0.5f;
            if (tl + tr > rect.width)
            {
                float b = rect.width / (tl + tr);
                tl *= b;
                tr *= b;
            }
            if (bl + br > rect.width)
            {
                float b = rect.width / (bl + br);
                bl *= b;
                br *= b;
            }
            if (tl + bl > rect.height)
            {
                float b = rect.height / (tl + bl);
                tl *= b;
                bl *= b;
            }
            if (tr + br > rect.height)
            {
                float b = rect.height / (tr + br);
                tr *= b;
                br *= b;
            }
        }
        m_Vertices[0] = rect.center * Scale;
        if (DoubleSided) m_Vertices[count + 1] = rect.center * Scale;
        for (int i = 0; i < CornerVertexCount; i++)
        {
            float s = Mathf.Sin((float)i * f);
            float c = Mathf.Cos((float)i * f);
            Vector2 v1 =
                new Vector3(-x + (1f - c) * tl * a1, y - (1f - s) * tl * a2);
            Vector2 v2 =
                new Vector3(x - (1f - s) * tr * a1, y - (1f - c) * tr * a2);
            Vector2 v3 =
                new Vector3(x - (1f - c) * br * a1, -y + (1f - s) * br * a2);
            Vector2 v4 =
                new Vector3(-x + (1f - s) * bl * a1, -y + (1f - c) * bl * a2);

            m_Vertices[1 + i] = (Vector2.Scale(v1, rs) + rect.center) * Scale;
            m_Vertices[1 + CornerVertexCount + i] =
                (Vector2.Scale(v2, rs) + rect.center) * Scale;
            m_Vertices[1 + CornerVertexCount * 2 + i] =
                (Vector2.Scale(v3, rs) + rect.center) * Scale;
            m_Vertices[1 + CornerVertexCount * 3 + i] =
                (Vector2.Scale(v4, rs) + rect.center) * Scale;
            if (CreateUV)
            {
                if (!UsePercentage)
                {
                    Vector2 adj =
                        new Vector2(2f / rect.width, 2f / rect.height);
                    v1 = Vector2.Scale(v1, adj);
                    v2 = Vector2.Scale(v2, adj);
                    v3 = Vector2.Scale(v3, adj);
                    v4 = Vector2.Scale(v4, adj);
                }
                m_UV[1 + i] = v1 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + CornerVertexCount * 1 + i] =
                    v2 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + CornerVertexCount * 2 + i] =
                    v3 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + CornerVertexCount * 3 + i] =
                    v4 * 0.5f + Vector2.one * 0.5f;
            }
            if (DoubleSided)
            {
                m_Vertices[1 + CornerVertexCount * 8 - i] = m_Vertices[1 + i];
                m_Vertices[1 + CornerVertexCount * 7 - i] =
                    m_Vertices[1 + CornerVertexCount + i];
                m_Vertices[1 + CornerVertexCount * 6 - i] =
                    m_Vertices[1 + CornerVertexCount * 2 + i];
                m_Vertices[1 + CornerVertexCount * 5 - i] =
                    m_Vertices[1 + CornerVertexCount * 3 + i];
                if (CreateUV)
                {
                    m_UV[1 + CornerVertexCount * 8 - i] =
                        v1 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + CornerVertexCount * 7 - i] =
                        v2 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + CornerVertexCount * 6 - i] =
                        v3 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + CornerVertexCount * 5 - i] =
                        v4 * 0.5f + Vector2.one * 0.5f;
                }
            }
        }
        for (int i = 0; i < count + 1; i++)
        {
            m_Normals[i] = -Vector3.forward;
            if (DoubleSided)
            {
                m_Normals[count + 1 + i] = Vector3.forward;
                if (FlipBackFaceUV)
                {
                    Vector2 uv = m_UV[count + 1 + i];
                    uv.x = 1f - uv.x;
                    m_UV[count + 1 + i] = uv;
                }
            }
        }
        for (int i = 0; i < count; i++)
        {
            m_Triangles[i * 3] = 0;
            m_Triangles[i * 3 + 1] = i + 1;
            m_Triangles[i * 3 + 2] = i + 2;
            if (DoubleSided)
            {
                m_Triangles[(count + i) * 3] = count + 1;
                m_Triangles[(count + i) * 3 + 1] = count + 1 + i + 1;
                m_Triangles[(count + i) * 3 + 2] = count + 1 + i + 2;
            }
        }
        m_Triangles[count * 3 - 1] = 1;
        if (DoubleSided) m_Triangles[m_Triangles.Length - 1] = count + 1 + 1;

        m_Mesh.Clear();
        m_Mesh.vertices = m_Vertices;
        m_Mesh.normals = m_Normals;
        if (CreateUV) m_Mesh.uv = m_UV;
        m_Mesh.triangles = m_Triangles;
        return m_Mesh;
    }

    void Update()
    {
        UpdateMesh();
        //depthMesh.Clear();

        //bottomMesh.Clear();
        //GenerateCapMesh (depthMesh);
        //GenerateCapMesh(bottomMesh, generateTriangles: false);
        //GenerateCapMesh(bottomMesh, depth: depth, doubleSided: true);
        //GenerateSideMesh(bottomMesh);

        //bottomMesh.RecalculateNormals();
    }

    private void GenerateCapMesh(
        Mesh mesh,
        float depth = 0f,
        bool generateTriangles = true,
        bool doubleSided = false
    )
    {
        bool[] cornerRounded =
            new bool[4]
            {
                corner1Radius != 0,
                corner2Radius != 0,
                corner3Radius != 0,
                corner4Radius != 0
            };
        float[] cornerRadii =
            new float[4]
            { corner1Radius, corner2Radius, corner3Radius, corner4Radius };
        int roundCorners =
            (cornerRounded[0] ? 1 : 0) +
            (cornerRounded[1] ? 1 : 0) +
            (cornerRounded[2] ? 1 : 0) +
            (cornerRounded[3] ? 1 : 0);
        int vertexCount = 9 + roundCorners * cornerRes;
        Vector3[] vertices = new Vector3[vertexCount];
        float diagonalRadius = Mathf.Sqrt(radius * radius + radius * radius);

        float angle = 0;
        vertices[0] = new Vector3(0, 0, 0);
        for (
            int
                corner = 0,
                vertex = 1;
            corner < 4;
            corner++
        )
        {
            vertices[vertex] =
                new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            vertex++;

            if (cornerRounded[corner])
            {
                float circleRadius = radius * cornerRadii[corner];
                float startAngle = Mathf.Atan2((radius - circleRadius), radius);
                float endAngle = Mathf.PI / 2 - startAngle;
                Vector3 p0 =
                    vertices[vertex] =
                        new Vector3(Mathf.Cos(startAngle + angle),
                            0,
                            Mathf.Sin(startAngle + angle)) *
                        Squareradius(startAngle, corner, true);
                Vector3 p2 =
                    vertices[vertex + cornerRes] =
                        new Vector3(Mathf.Cos(endAngle + angle),
                            0,
                            Mathf.Sin(endAngle + angle)) *
                        Squareradius(endAngle, corner, false);
                angle += Mathf.PI / 4;
                Vector3 p1 =
                    new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) *
                    diagonalRadius *
                    test;
                vertex += 1;

                for (int i = 1; i < cornerRes; i++, vertex++)
                {
                    vertices[vertex] =
                        CalculateQuadraticBezierPoint((float)(i - 1) /
                        (cornerRes - 2),
                        p0,
                        p1,
                        p2);
                }
                vertex += 1;
                angle += Mathf.PI / 4;
            }
            else
            {
                angle += Mathf.PI / 4;
                vertices[vertex] =
                    new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) *
                    diagonalRadius;
                vertex++;
                angle += Mathf.PI / 4;
            }
        }

        if (depth != 0)
        {
            Vector3 depthvector = new Vector3(0, -depth, 0);
            Vector3[] verts = new Vector3[vertexCount];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertic = vertices[i];
                verts[i] = vertic + depthvector;
            }
            vertices = verts;
        }

        if (doubleSided) vertices = vertices.Concat(vertices).ToArray();

        int vertexCountOffset = mesh.vertices.Count();
        mesh.SetVertices(mesh.vertices.Concat(vertices).ToArray());

        if (generateTriangles)
        {
            int triCount =
                (roundCorners * (cornerRes + 2) + (4 - roundCorners) * 2) * 3;
            int[] triangles1 = new int[triCount];
            int[] triangles2 = new int[triCount];

            for (
                int
                    tri = 0,
                    vertex = 1;
                tri < triCount;
                tri += 3, vertex += 1
            )
            {
                triangles1[tri] = vertexCountOffset;
                triangles1[tri + 1] = vertexCountOffset + vertex + 1;
                triangles1[tri + 2] = vertexCountOffset + vertex;

                if (doubleSided)
                {
                    triangles2[tri] = vertexCountOffset + vertexCount;
                    triangles2[tri + 1] =
                        vertexCountOffset + vertexCount + vertex;
                    triangles2[tri + 2] =
                        vertexCountOffset + vertexCount + vertex + 1;
                }
            }
            triangles1[triCount - 2] = vertexCountOffset + 1;
            if (doubleSided)
                triangles2[triCount - 1] = vertexCountOffset + vertexCount + 1;

            mesh.subMeshCount = doubleSided ? 2 : 1;
            mesh
                .SetTriangles(mesh.GetTriangles(0).Concat(triangles1).ToArray(),
                0);
            if (doubleSided)
                mesh
                    .SetTriangles(mesh
                        .GetTriangles(1)
                        .Concat(triangles2)
                        .ToArray(),
                    1);
        }
    }

    private void GenerateSideMesh(Mesh mesh)
    {
        bool[] cornerRounded =
            new bool[4]
            {
                corner1Radius != 0,
                corner2Radius != 0,
                corner3Radius != 0,
                corner4Radius != 0
            };
        int roundCorners =
            (cornerRounded[0] ? 1 : 0) +
            (cornerRounded[1] ? 1 : 0) +
            (cornerRounded[2] ? 1 : 0) +
            (cornerRounded[3] ? 1 : 0);
        int vertexPerSide =
            roundCorners * (cornerRes + 2) + (4 - roundCorners) * 2 + 1;
        int triCount =
            (2 * roundCorners * (cornerRes + 2) + (4 - roundCorners) * 4) * 3;
        int[] triangles1 = new int[triCount];
        int[] triangles2 = new int[triCount];

        for (
            int
                tri = 0,
                vertex = 1;
            tri < triCount;
            tri += 6, vertex += 1
        )
        {
            triangles1[tri] = vertex;
            triangles1[tri + 1] = triangles1[tri + 3] = vertex + 1;
            triangles1[tri + 2] = triangles1[tri + 5] = vertex + vertexPerSide;
            triangles1[tri + 4] = vertex + vertexPerSide + 1;

            triangles2[tri] = vertex;
            triangles2[tri + 2] = triangles2[tri + 3] = vertex + 1;
            triangles2[tri + 1] = triangles2[tri + 4] = vertex + vertexPerSide;
            triangles2[tri + 5] = vertex + vertexPerSide + 1;
        }
        triangles1[triCount - 5] = triangles1[triCount - 3] = 1;
        triangles1[triCount - 2] = vertexPerSide + 1;
        triangles2[triCount - 4] = triangles2[triCount - 3] = 1;
        triangles2[triCount - 1] = vertexPerSide + 1;

        mesh.subMeshCount = 4;
        mesh.SetTriangles(mesh.triangles.Concat(triangles1).ToArray(), 2);
        mesh.SetTriangles(mesh.triangles.Concat(triangles2).ToArray(), 3);

        // Vector3[] normals1 = Enumerable.Repeat<Vector3>(new Vector3(0, 1, 0), vertexPerSide).ToArray();
        // Vector3[] normals2 = Enumerable.Repeat<Vector3>(new Vector3(0, -1, 0), vertexPerSide).ToArray();
        // mesh.SetNormals(normals1.Concat(normals1.Concat(normals2)).ToArray());
    }

    private float Squareradius(float angle, float corner, bool firstHalf)
    {
        if (firstHalf) return radius / Mathf.Cos(angle);
        return radius / Mathf.Sin(angle);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }
}
