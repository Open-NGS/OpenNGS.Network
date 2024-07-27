using UnityEngine;
using System.Collections;

namespace OpenNGS.UI
{
    /// <summary>
    /// Class to read pixel data and create outline edges around opaque pixel areas.
    /// Idea From UCLA Game Lab 
    /// </summary>
    public class SimpleSurfaceEdge
    {
        private readonly Color[] m_Pixels; // the original pixel data from the image 
        private readonly int m_ImageHeight;
        private readonly int m_ImageWidth;

        readonly ArrayList m_Edges;
        private readonly ArrayList m_Vertices;
        private readonly ArrayList m_EdgeLoops;

        public SimpleSurfaceEdge(Color[] pixels, int imageWidth, int imageHeight, float threshold)
        {
            m_Pixels = pixels;
            m_ImageWidth = imageWidth;
            m_ImageHeight = imageHeight;

            m_Edges = new ArrayList();
            m_Vertices = new ArrayList();
            m_EdgeLoops = new ArrayList();

            float uvWidth = (float) m_ImageWidth;
            float uvHeight = (float) m_ImageHeight;

            for (int x = 0; x < m_ImageWidth; x++)
            {
                float uvX = x + 0.5f;
                for (int y = 0; y < m_ImageHeight; y++)
                {
                    float uvY = y + 0.5f;
                    // get the first pixel
                    Color pixel = m_Pixels[x + (m_ImageWidth * y)];
                    float pixelAlpha = pixel.a;

                    // only continue if the current pixel is opaque
                    if (pixelAlpha < threshold) continue;

                    // set up values for other possible pixel values
                    float pixelAboveAlpha = 0.0f;
                    float pixelBelowAlpha = 0.0f;
                    float pixelRightAlpha = 0.0f;
                    float pixelLeftAlpha = 0.0f;
                    float pixelAboveRightAlpha = 0.0f;
                    float pixelAboveLeftAlpha = 0.0f;
                    float pixelBelowRightAlpha = 0.0f;

                    // check x area, then the y. 
                    if (x > 0 && x < m_ImageWidth - 1)
                    {
                        if (y > 0 && y < m_ImageHeight - 1)
                        {
                            Color pixelAbove = m_Pixels[x + (m_ImageWidth * (y + 1))];
                            pixelAboveAlpha = pixelAbove.a;
                            Color pixelBelow = m_Pixels[x + (m_ImageWidth * (y - 1))];
                            pixelBelowAlpha = pixelBelow.a;
                            Color pixelRight = m_Pixels[x + 1 + (m_ImageWidth * y)];
                            pixelRightAlpha = pixelRight.a;
                            Color pixelLeft = m_Pixels[x - 1 + (m_ImageWidth * y)];
                            pixelLeftAlpha = pixelLeft.a;

                            Color pixelAboveRight = m_Pixels[x + 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveRightAlpha = pixelAboveRight.a;
                            Color pixelAboveLeft = m_Pixels[x - 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveLeftAlpha = pixelAboveLeft.a;
                            Color pixelBelowRight = m_Pixels[x + 1 + (m_ImageWidth * (y - 1))];
                            pixelBelowRightAlpha = pixelBelowRight.a;
                        }
                        else if (y == 0)
                        {
                            Color pixelAbove = m_Pixels[x + (m_ImageWidth * (y + 1))];
                            pixelAboveAlpha = pixelAbove.a;
                            Color pixelRight = m_Pixels[x + 1 + (m_ImageWidth * y)];
                            pixelRightAlpha = pixelRight.a;
                            Color pixelLeft = m_Pixels[x - 1 + (m_ImageWidth * y)];
                            pixelLeftAlpha = pixelLeft.a;

                            Color pixelAboveRight = m_Pixels[x + 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveRightAlpha = pixelAboveRight.a;
                            Color pixelAboveLeft = m_Pixels[x - 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveLeftAlpha = pixelAboveLeft.a;
                        }
                        else if (y == m_ImageHeight - 1)
                        {
                            Color pixelBelow = m_Pixels[x + (m_ImageWidth * (y - 1))];
                            pixelBelowAlpha = pixelBelow.a;
                            Color pixelRight = m_Pixels[x + 1 + (m_ImageWidth * y)];
                            pixelRightAlpha = pixelRight.a;
                            Color pixelLeft = m_Pixels[x - 1 + (m_ImageWidth * y)];
                            pixelLeftAlpha = pixelLeft.a;

                            Color pixelBelowRight = m_Pixels[x + 1 + (m_ImageWidth * (y - 1))];
                            pixelBelowRightAlpha = pixelBelowRight.a;
                        }
                        else
                        {
                            Debug.Log("SimpleSurfaceEdge:: error constructing pixel values, misinterpreted y values.");
                        }
                    }
                    else if (x == 0)
                    {
                        if (y > 0 && y < m_ImageHeight - 1)
                        {
                            Color pixelAbove = m_Pixels[x + (m_ImageWidth * (y + 1))];
                            pixelAboveAlpha = pixelAbove.a;
                            Color pixelBelow = m_Pixels[x + (m_ImageWidth * (y - 1))];
                            pixelBelowAlpha = pixelBelow.a;
                            Color pixelRight = m_Pixels[x + 1 + (m_ImageWidth * y)];
                            pixelRightAlpha = pixelRight.a;

                            Color pixelAboveRight = m_Pixels[x + 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveRightAlpha = pixelAboveRight.a;
                            Color pixelBelowRight = m_Pixels[x + 1 + (m_ImageWidth * (y - 1))];
                            pixelBelowRightAlpha = pixelBelowRight.a;
                        }
                        else if (y == 0)
                        {
                            Color pixelAbove = m_Pixels[x + (m_ImageWidth * (y + 1))];
                            pixelAboveAlpha = pixelAbove.a;
                            Color pixelRight = m_Pixels[x + 1 + (m_ImageWidth * y)];
                            pixelRightAlpha = pixelRight.a;

                            Color pixelAboveRight = m_Pixels[x + 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveRightAlpha = pixelAboveRight.a;
                        }
                        else if (y == m_ImageHeight - 1)
                        {
                            Color pixelBelow = m_Pixels[x + (m_ImageWidth * (y - 1))];
                            pixelBelowAlpha = pixelBelow.a;
                            Color pixelRight = m_Pixels[x + 1 + (m_ImageWidth * y)];
                            pixelRightAlpha = pixelRight.a;

                            Color pixelBelowRight = m_Pixels[x + 1 + (m_ImageWidth * (y - 1))];
                            pixelBelowRightAlpha = pixelBelowRight.a;
                        }
                        else
                        {
                            Debug.Log("SimpleSurfaceEdge:: error constructing pixel values, misinterpreted y values.");
                        }
                    }
                    else if (x == m_ImageWidth - 1)
                    {
                        if (y > 0 && y < m_ImageHeight - 1)
                        {
                            Color pixelAbove = m_Pixels[x + (m_ImageWidth * (y + 1))];
                            pixelAboveAlpha = pixelAbove.a;
                            Color pixelBelow = m_Pixels[x + (m_ImageWidth * (y - 1))];
                            pixelBelowAlpha = pixelBelow.a;
                            Color pixelLeft = m_Pixels[x - 1 + (m_ImageWidth * y)];
                            pixelLeftAlpha = pixelLeft.a;

                            Color pixelAboveLeft = m_Pixels[x - 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveLeftAlpha = pixelAboveLeft.a;
                        }
                        else if (y == 0)
                        {
                            Color pixelAbove = m_Pixels[x + (m_ImageWidth * (y + 1))];
                            pixelAboveAlpha = pixelAbove.a;
                            Color pixelLeft = m_Pixels[x - 1 + (m_ImageWidth * y)];
                            pixelLeftAlpha = pixelLeft.a;

                            Color pixelAboveLeft = m_Pixels[x - 1 + (m_ImageWidth * (y + 1))];
                            pixelAboveLeftAlpha = pixelAboveLeft.a;
                        }
                        else if (y == m_ImageHeight - 1)
                        {
                            Color pixelBelow = m_Pixels[x + (m_ImageWidth * (y - 1))];
                            pixelBelowAlpha = pixelBelow.a;
                            Color pixelLeft = m_Pixels[x - 1 + (m_ImageWidth * y)];
                            pixelLeftAlpha = pixelLeft.a;
                        }
                        else
                        {
                            Debug.Log(
                                "SimpleSurfaceEdge:: error constructing pixel values, misinterpreted y values.  Please create a new issue at https://github.com/uclagamelab/MeshCreator/issues.");
                        }
                    }

                    // try the up facing case
                    if (pixelAlpha >= threshold && pixelAboveAlpha >= threshold)
                    {
                        if (pixelAboveRightAlpha < threshold && pixelRightAlpha < threshold)
                        {
                            if (pixelAboveLeftAlpha >= threshold || pixelLeftAlpha >= threshold)
                            {
                                // add the vertical edge
                                Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                    GetVertex(x, y + 1, uvX / uvWidth, (uvY + 1) / uvHeight));
                                m_Edges.Add(e);
                            }
                        }
                        else if (pixelAboveLeftAlpha < threshold && pixelLeftAlpha < threshold)
                        {
                            if (pixelAboveRightAlpha >= threshold || pixelRightAlpha >= threshold)
                            {
                                // add the vertical edge
                                Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                    GetVertex(x, y + 1, uvX / uvWidth, (uvY + 1) / uvHeight));
                                m_Edges.Add(e);
                            }
                        }
                    }

                    // try the up diagonal case
                    if (pixelAlpha >= threshold && pixelAboveRightAlpha >= threshold)
                    {
                        if (pixelAboveAlpha < threshold && pixelRightAlpha >= threshold)
                        {
                            // add the up diagonal edge
                            Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                GetVertex(x + 1, y + 1, (uvX + 1) / uvWidth, (uvY + 1) / uvHeight));
                            m_Edges.Add(e);
                        }
                        else if (pixelAboveAlpha >= threshold && pixelRightAlpha < threshold)
                        {
                            // add the up diagonal edge
                            Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                GetVertex(x + 1, y + 1, (uvX + 1) / uvWidth, (uvY + 1) / uvHeight));
                            m_Edges.Add(e);
                        }
                    }

                    // try the right facing case
                    if (pixelAlpha >= threshold && pixelRightAlpha >= threshold)
                    {
                        if (pixelAboveAlpha < threshold && pixelAboveRightAlpha < threshold)
                        {
                            if (pixelBelowAlpha >= threshold || pixelBelowRightAlpha >= threshold)
                            {
                                // add the horizontal edge
                                Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                    GetVertex(x + 1, y, (uvX + 1) / uvWidth, uvY / uvHeight));
                                m_Edges.Add(e);
                            }
                        }
                        else if (pixelBelowAlpha < threshold && pixelBelowRightAlpha < threshold)
                        {
                            if (pixelAboveAlpha >= threshold || pixelAboveRightAlpha >= threshold)
                            {
                                // add the horizontal edge
                                Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                    GetVertex(x + 1, y, (uvX + 1) / uvWidth, uvY / uvHeight));
                                m_Edges.Add(e);
                            }
                        }
                    }

                    // try the down diagonal case
                    if (pixelAlpha >= threshold && pixelBelowRightAlpha >= threshold)
                    {
                        if (pixelRightAlpha < threshold && pixelBelowAlpha >= threshold)
                        {
                            // add the down diagonal edge
                            Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                GetVertex(x + 1, y - 1, (uvX + 1) / uvWidth, (uvY - 1) / uvHeight));
                            m_Edges.Add(e);
                        }
                        else if (pixelRightAlpha >= threshold && pixelBelowAlpha < threshold)
                        {
                            // ad the down diagonal edge
                            Edge e = new Edge(GetVertex(x, y, uvX / uvWidth, uvY / uvHeight),
                                GetVertex(x + 1, y - 1, (uvX + 1) / uvWidth, (uvY - 1) / uvHeight));
                            m_Edges.Add(e);
                        }
                    }
                }
            }

            MakeOutsideEdge();
            SimplifyEdge();
        }

        Vertex GetVertex(float x, float y, float u, float v)
        {
            for (int i = 0; i < m_Vertices.Count; i++)
            {
                Vertex ver = (Vertex) m_Vertices[i];
                if (ver.x == x && ver.y == y)
                {
                    return ver;
                }
            }

            Vertex newver = new Vertex(x, y, u, v);
            m_Vertices.Add(newver);
            return newver;
        }

        public void MergeClosePoints(float mergeDistance)
        {
            foreach (EdgeLoop edgeLoop in m_EdgeLoops)
            {
                edgeLoop.MergeClosePoints(mergeDistance);
            }
        }

        void SimplifyEdge()
        {
            foreach (EdgeLoop edgeLoop in m_EdgeLoops)
            {
                edgeLoop.SimplifyEdge();
            }
        }

        void MakeOutsideEdge()
        {
            // order the edges
            // start first edge loop with the first outside edge
            EdgeLoop currentEdgeLoop = new EdgeLoop((Edge) m_Edges[0]);
            m_Edges.RemoveAt(0);
            m_EdgeLoops.Add(currentEdgeLoop);

            while (m_Edges.Count > 0)
            {
                // if the currentEdgeLoop is fully closed make a new edge loop
                if (currentEdgeLoop.IsClosed())
                {
                    EdgeLoop nextEdgeLoop = new EdgeLoop((Edge) m_Edges[0]);
                    m_Edges.RemoveAt(0);
                    m_EdgeLoops.Add(nextEdgeLoop);
                    currentEdgeLoop = nextEdgeLoop;
                }

                // test each edge to see if it fits into the edgeloop
                ArrayList deleteEdges = new ArrayList();
                for (int i = 0; i < m_Edges.Count; i++)
                {
                    Edge e = (Edge) m_Edges[i];
                    if (currentEdgeLoop.AddEdge(e))
                    {
                        // try to add the edge
                        deleteEdges.Add(e);
                    }
                }

                // delete the added edges
                for (int i = 0; i < deleteEdges.Count; i++)
                {
                    m_Edges.Remove((Edge) deleteEdges[i]);
                }
            }
        }

        public Vector2[] GetOutsideEdgeVertices()
        {
            EdgeLoop eL = (EdgeLoop) m_EdgeLoops[0];
            return eL.GetVertexList();
        }

        public ArrayList GetAllEdgeVertices()
        {
            ArrayList edgeLoopVertices = new ArrayList();
            foreach (EdgeLoop el in m_EdgeLoops)
            {
                edgeLoopVertices.Add(el.GetVertexList());
            }

            return edgeLoopVertices;
        }

        public bool ContainsIslands()
        {
            if (m_EdgeLoops.Count > 1) return true;
            return false;
        }

        public Vector2 GetUVForIndex(int loopIndex, int i)
        {
            EdgeLoop eL = (EdgeLoop) m_EdgeLoops[loopIndex];
            return eL.GetUVForIndex(i);
        }

        public Vector2 GetUVForIndex(int i)
        {
            EdgeLoop eL = (EdgeLoop) m_EdgeLoops[0];
            return eL.GetUVForIndex(i);
        }
    }

    class Vertex
    {
        public float x, y;
        public float u, v;

        public Vertex(float _x, float _y, float _u, float _v)
        {
            x = _x;
            y = _y;
            u = _u;
            v = _v;
        }

        /// <summary>
        /// GetString() returns a descriptive string about info in this object, Useful for debugging.
        ///	</summary>>
        public string GetString()
        {
            return "Vertex(x,y:" + x + "," + y + ", uv:" + u + "," + v + ")";
        }
    }

    class Edge
    {
        public Vertex v1;
        public Vertex v2;
        public bool isShared; // indicate if there are two of these?

        ArrayList attachedFaces;

        public Edge(Vertex _v1, Vertex _v2)
        {
            v1 = _v1;
            v2 = _v2;
            isShared = false;
            attachedFaces = new ArrayList();
        }

        public void AttachFace(Face f)
        {
            attachedFaces.Add(f);
        }

        public bool OtherFaceCentered(Face f)
        {
            if (attachedFaces.Count > 1)
            {
                Face face1 = (Face) attachedFaces[0];
                Face face2 = (Face) attachedFaces[1];
                if (face1 == null && face2 == f)
                    return true; // faces already deleted????
                if (face2 == null && face1 == f)
                    return true;
                if (face1 != null && face1 != f && face1.IsCentered())
                    return true;
                if (face2 != null && face2 != f && face2.IsCentered())
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Swap the value v1, v2
        /// </summary>
        public void SwitchVertices()
        {
            (v1, v2) = (v2, v1);
        }
    }

    class Face
    {
        public Vertex v1, v2, v3;
        public Edge e1, e2, e3;

        public Face(Edge _e1, Edge _e2, Edge _e3)
        {
            e1 = _e1;
            e2 = _e2;
            e3 = _e3;
            e1.AttachFace(this);
            e2.AttachFace(this);
            e3.AttachFace(this);
            v1 = e1.v1;
            v2 = e1.v2;
            if (e2.v1 != v1 && e2.v1 != v2)
                v3 = e2.v1;
            else
                v3 = e2.v2;
        }

        public bool ContainsEdge(Edge e)
        {
            if (e1 == e || e2 == e || e3 == e)
            {
                return true;
            }

            return false;
        }

        public bool IsCentered()
        {
            if (e1.isShared && e2.isShared && e3.isShared)
            {
                return true;
            }

            return false;
        }

        public bool IsCenteredCentered()
        {
            if (IsCentered())
            {
                if (e1.OtherFaceCentered(this) && e2.OtherFaceCentered(this) && e3.OtherFaceCentered(this))
                {
                    return true;
                }
            }

            return false;
        }
    }
    
    /// <summary>
    /// Ordered list of edges 
    /// </summary>
    class EdgeLoop
    {
        public ArrayList orderedEdges;

        public EdgeLoop(Edge e)
        {
            orderedEdges = new ArrayList();
            orderedEdges.Add(e);
        }

        public bool AddEdge(Edge e)
        {
            // see if it shares with the last edge
            Vertex lastVertex = ((Edge) orderedEdges[orderedEdges.Count - 1]).v2;
            if (e.v1 == lastVertex)
            {
                // this is the correct vertex order
                orderedEdges.Add(e);
                return true;
            }
            else if (e.v2 == lastVertex)
            {
                // incorrect order, switch before adding 
                e.SwitchVertices();
                orderedEdges.Add(e);
                return true;
            }

            // see if it shares with the first edge
            Vertex firstVertex = ((Edge) orderedEdges[0]).v1;
            if (e.v2 == firstVertex)
            {
                // this is the correct vertex order
                orderedEdges.Insert(0, e);
                return true;
            }
            
            if (e.v1 == firstVertex)
            {
                // incorrect order, switch before adding
                e.SwitchVertices();
                orderedEdges.Insert(0, e);
                return true;
            }

            return false;
        }

        public bool IsClosed()
        {
            if (orderedEdges.Count < 2) return false;
            Vertex lastVertex = ((Edge) orderedEdges[orderedEdges.Count - 1]).v2;
            Vertex firstVertex = ((Edge) orderedEdges[0]).v1;
            if (firstVertex.x == lastVertex.x && firstVertex.y == lastVertex.y) 
                return true;
            
            return false;
        }

        public Vector2[] GetVertexList()
        {
            Vector2[] verts = new Vector2[orderedEdges.Count];
            for (int i = 0; i < orderedEdges.Count; i++)
            {
                Vertex v = ((Edge) orderedEdges[i]).v1;
                verts[i] = new Vector2(v.x, v.y);
            }

            return verts;
        }

        public Vector2 GetUVForIndex(int i)
        {
            if (i >= orderedEdges.Count)
            {
                return new Vector2();
            }

            Vertex v = ((Edge) orderedEdges[i]).v1;
            return new Vector2(v.u, v.v);
        }

        /// <summary>
        /// Search for edges in which the shared vertex is a point
        ///	on a line between the two outer points.
        /// </summary>
        public void SimplifyEdge()
        {
            ArrayList newOrderedEdges = new ArrayList(); // list to stick the joined edges
            Edge currentEdge = (Edge) orderedEdges[0];
            for (int i = 1; i < orderedEdges.Count; i++)
            {
                // start with the second edge for comparison
                Edge testEdge = (Edge) orderedEdges[i];
                Vertex v1 = currentEdge.v1;
                Vertex v2 = testEdge.v2;
                Vertex sharedPoint = currentEdge.v2;
                if (sharedPoint != testEdge.v1)
                {
                    // oops, bad list, it should be closed by now
                    Debug.LogError(
                        "Mesh Creator EdgeLoop Error: list is not ordered when simplifying edge.  Please create a new issue at https://github.com/uclagamelab/MeshCreator/issues.");
                    return;
                }

                if (v1 == v2)
                {
                    Debug.LogError(
                        "Mesh Creator EdgeLoop Error: found matching endpoints for a line when simplifying.  Please create a new issue at https://github.com/uclagamelab/MeshCreator/issues.");
                    return;
                }

                // determine if sharedPoint is on a line between the two endpoints
                // The point (x3, y3) is on the line determined by (x1, y1) and (x2, y2) if and only if (x3-x1)*(y2-y1)==(x2-x1)*(y3-y1). 
                float slope1 = (sharedPoint.x - v1.x) * (v2.y - v1.y);
                float slope2 = (v2.x - v1.x) * (sharedPoint.y - v1.y);
                if (slope1 == slope2)
                {
                    // combine the two lines into current
                    currentEdge.v2 = v2;
                }
                else
                {
                    // there isn't a continuation of line, so add current to new ordered and set current to testEdge
                    newOrderedEdges.Add(currentEdge);
                    currentEdge = testEdge;
                }
            }

            newOrderedEdges.Add(currentEdge);
            orderedEdges = newOrderedEdges;
        }

        
        
        /// <summary>
        /// Very simple edge smoothing by comparing distance between adjacent
        /// points on edge and merging if close enough
        /// </summary>
        public void MergeClosePoints(float mergeDistance)
        {
            if (mergeDistance < 0.0f) return;

            ArrayList newOrderedEdges = new ArrayList(); // list to stick the joined edges
            // int originalCount = orderedEdges.Count;
            Edge currentEdge = (Edge) orderedEdges[0];
            for (int i = 1; i < orderedEdges.Count; i++)
            {
                // start with the second edge for comparison
                Edge testEdge = (Edge) orderedEdges[i];
                float dist = Vector2.Distance(new Vector2(currentEdge.v1.x, currentEdge.v1.y),
                    new Vector2(testEdge.v2.x, testEdge.v2.y));
                Vertex v2 = testEdge.v2;

                if (dist < mergeDistance)
                {
                    // combine the two lines into current
                    currentEdge.v2 = v2;
                }
                else
                {
                    // there isn't a continuation of line, so add current to new ordered and set current to testEdge
                    newOrderedEdges.Add(currentEdge);
                    currentEdge = testEdge;
                }
            }

            newOrderedEdges.Add(currentEdge);
            orderedEdges = newOrderedEdges;
        }
    }
}