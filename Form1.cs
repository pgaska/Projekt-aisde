using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aisde
{
    public partial class Form1 : Form
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        bool fileSelected = false;
        string path;

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            SelectFile();
            if(fileSelected)
            {
                GraphParser graphParser = new GraphParser();
                Graph g = graphParser.ParseGraph(openFileDialog.FileName);
                g.createAdjacencyList(false);
                g.DrawGraph(e, Color.Black);
                if (graphParser.algorithm == GraphParser.Algorithm.DIJKSTRA)
                {
                    List<int> l = g.Dijkstra(graphParser.dijkstraVerticesId[0], graphParser.dijkstraVerticesId[1]);
                    g.DrawDijkstra(l, e);
                }
                else if (graphParser.algorithm == GraphParser.Algorithm.MST)
                {
                    List<Edge> edges = g.Kruskal();
                    g.DrawKruskal(edges, e);
                }
                else if (graphParser.algorithm == GraphParser.Algorithm.FLOYD)
                {
                    int[,] floyd = g.FloydWarshall();
                    int floydPathsAmount = graphParser.floydVerticesIds.Count();
                    for(int i=0;i<floydPathsAmount; i++)
                    {
                        List<int> l = g.getWarshallPath(graphParser.floydVerticesIds[i].Item1, graphParser.floydVerticesIds[i].Item2, floyd);
                        g.DrawFloyd(l, e);
                    }
                }
                else if (graphParser.algorithm == GraphParser.Algorithm.STEINER)
                {
                    List<Edge> l = g.Steiner();
                    g.DrawKruskal(l, e);
                }
            }
        }

        private void SelectFile()
        {
            openFileDialog.Filter = "TXT|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                fileSelected = true;
            }
        }
    }
}
