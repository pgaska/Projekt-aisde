using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace aisde
{
    class Vertex
    {
        public int id;
        public int x;
        public int y;
        public Rectangle position;
        public Point center;

        public Vertex(int id, int x, int y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            position = new Rectangle(x, y, 20, 20);
            center = new Point(position.Left+position.Width/2, position.Top+position.Height/2);
        }
    }
}
