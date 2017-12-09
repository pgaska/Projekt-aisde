using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace aisde
{
    class Graph
    {
        public List<Vertex> vertexes = new List<Vertex>();
        public List<Edge> edges= new List<Edge>();

        public Graph()
        {
           
        }

        public void DrawGraph(PaintEventArgs e, Color backgroundColor)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(backgroundColor);
            SolidBrush drawBrush = new SolidBrush(backgroundColor);
            SolidBrush writeBrush = new SolidBrush(Color.White);
            foreach (Vertex v in vertexes)
            {
                g.FillEllipse(drawBrush, v.position);
                g.DrawString(v.id.ToString(), new Font("Arial", 10), writeBrush, v.position);
            }
            foreach (Edge ed in edges)
            {
                g.DrawLine(pen, ed.beginning.center, ed.end.center);
            }
        }

        public int PrimMST(int start)
        {
               
            return 0;
            
        }

        public int Dijkstra(int start)
        {
            int[] cost = new int[];
            foreach(int c in cost) { cost[c] = 10000000; }
            SortedList<int, Vertex> vertices = new SortedList<int, Vertex>();
            bool[] visited = new bool[vertexes.Count];
            Vertex first = vertexes.Find(x => x.id == start);
            vertices.Add(0, first);
            cost[first.id] = 0;
            while(vertices.Count != 0)
            {
                Vertex curr = vertices[0];
                vertices.Remove(vertices.First().Key);
                visited[curr.id] = true;
                foreach (Vertex v in curr.)
            }
           
            
        }
    }
}
