using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavGridGroup : MonoBehaviour {

    List<GameObject>  node = new List<GameObject>();

    public void AddNode(Vector3 pos)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        obj.transform.SetParent(this.transform);
        node.Add(obj);
        obj.name = node.Count.ToString();
        if (pos == Vector3.down)
        {
            if (node.Count != 0 )
            {
                obj.transform.position = new Vector3(node[0].transform.position.x + 0.5f, 0.2f,
                    node[0].transform.position.z + 0.5f);
            }
        }
        else
        {
            obj.transform.position = pos;
        }
        flushMesh = true;
    }

    public void DeleteNode(GameObject obj)
    {
        if (node.Contains(obj))
        {
            node.Remove(obj);
            DestroyImmediate(obj);
        }
    }

	
	// Update is called once per frame
	void Update () {

	}

    private MeshFilter meshFilter = null;
    private MeshRenderer meshRenderer = null;
    private Mesh mesh = null;
    private bool flushMesh = true;
    private bool bInit = false;
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        for (int i = 0; i < node.Count; i++)
        {
            Vector3 startpos = node[i].transform.position;
            startpos.y = 0.2f;
            Vector3 endpos;
            if (i >= node.Count-1)
            {
                endpos = node[0].transform.position;
            }
            else
            {
                endpos = node[i + 1].transform.position;
            }
            endpos.y = 0.2f;
            UnityEngine.Debug.DrawLine(startpos, endpos, new Color(1, 0, 0));
        }

        if (node.Count>3 && flushMesh)
        {
            meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            meshFilter = this.gameObject.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                meshFilter = this.gameObject.AddComponent<MeshFilter>();
            }

            if (meshRenderer == null)
            {
                meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            }


            if (meshFilter != null)
            {
                CreateMesh();
            }
            flushMesh = false;
        }
    }

    void CreateMesh()
    {
        int points = node.Count;
        Vector3[] newVertices = new Vector3[points];

        int i;
        for (i = 0; i < node.Count; i++)
        {
            node[i].transform.position = new Vector3(node[i].transform.position.x,0.2f, node[i].transform.position.z);
            newVertices[i] = node[i].transform.position;
        }
        Triangulator triangulator = new Triangulator(newVertices);
        int[] newTriangles = triangulator.Triangulate();

        // add mesh
        meshFilter.mesh = new Mesh();
        meshFilter.sharedMesh.vertices = newVertices;
        meshFilter.sharedMesh.triangles = newTriangles;
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateBounds();
    }
#endif

    public void ReflushMesh()
    {
        this.flushMesh = true;
    }

    public Vector3[] GetNavGroupData()
    {
        Transform[] objects = this.GetComponentsInChildren<Transform>();
        if (objects.Length - 1 != node.Count)
        {
            node.Clear();
            foreach (Transform o in objects)
            {
                if (o != this.transform)
                {
                    node.Add(o.gameObject);
                }
            }
        }

        Vector3[] vertices = new Vector3[node.Count];
        for (int i = 0; i < node.Count; i++)
        {
            node[i].transform.position = new Vector3(node[i].transform.position.x, 0.2f, node[i].transform.position.z);
            vertices[i] = node[i].transform.position;
        }
        return vertices;
    }
}


public class Triangulator
{
    private List<Vector3> m_points;

    public Triangulator(Vector3[] points)
    {
        m_points = new List<Vector3>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector3 pval = m_points[p];
            Vector3 qval = m_points[q];
            A += pval.x * qval.z - qval.x * pval.z;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = m_points[V[u]];
        Vector3 B = m_points[V[v]];
        Vector3 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.z - A.z)) - ((B.z - A.z) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.z - B.z;
        bx = A.x - C.x; by = A.z - C.z;
        cx = B.x - A.x; cy = B.z - A.z;
        apx = P.x - A.x; apy = P.z - A.z;
        bpx = P.x - B.x; bpy = P.z - B.z;
        cpx = P.x - C.x; cpy = P.z - C.z;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}

