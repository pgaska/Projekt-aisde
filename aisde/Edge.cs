using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aisde
{
    class Edge
    {
        public int id;
        public Vertex beginning;
        public Vertex end;
        public double length;

        public Edge(int id, Vertex beginning, Vertex end)
        {
            this.id = id;
            this.beginning = beginning;
            this.end = end;
            length = Math.Sqrt(Math.Pow(end.x - beginning.x, 2) + Math.Pow(end.y - beginning.y, 2));
        }
    }
}
