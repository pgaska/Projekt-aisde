using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Priority_Queue;
using System.Drawing.Drawing2D;

namespace aisde
{
    class Graph
    {
        public List<Vertex> vertices = new List<Vertex>();
        public List<Edge> edges = new List<Edge>();
        public List<int> obowiazkowe = new List<int>();

        private double _path_cost;

        public Dictionary<int, List<Tuple<double, int>>> adjacencyList = new Dictionary<int, List<Tuple<double, int>>>(); // lista sasiedztwa - indeksowana przez id lista Tuple<koszt, wierzcholek docelowy>


        public void DrawGraph(PaintEventArgs e, Color backgroundColor)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(backgroundColor);
            SolidBrush drawBrush = new SolidBrush(backgroundColor);
            SolidBrush writeBrush = new SolidBrush(Color.White);
            foreach (Vertex v in vertices)
            {
                g.FillEllipse(drawBrush, v.position);
                g.DrawString(v.id.ToString(), new Font("Arial", 10), writeBrush, v.position);
            }
            foreach (Edge ed in edges)
            {
                g.DrawLine(pen, ed.beginning.center, ed.end.center);
            }
        }

        public void createAdjacencyList(bool undirected)
        {
            foreach (Vertex v in vertices)
            {
                if (!adjacencyList.ContainsKey(v.id))
                {
                    List<Tuple<double, int>> l = new List<Tuple<double, int>>();
                    adjacencyList.Add(v.id, l);
                }
            }

            foreach (Edge e in edges)
            {
                adjacencyList[e.beginning.id].Add(new Tuple<double, int>(e.length, e.end.id));

                if (undirected)
                {
                    adjacencyList[e.end.id].Add(new Tuple<double, int>(e.length, e.beginning.id));
                }
               
            }

            foreach(Vertex v in vertices)
            {
                if (v.obowiazkowy)
                {
                    obowiazkowe.Add(v.id);
                }
            }

        }

        /// <summary>
        /// Algorytm Dijkstry - znajdowanie najkrotszej sciezki
        /// </summary>
        /// <param name="start">id wierzcholka poczatkowego</param>
        /// <param name="end">id wierzcholka koncowego</param>
        /// <returns>koszt dotarcia do wierzcholka koncowego, moge dopisac tez wersje ze sciezka do niego</returns>
        public List<int> Dijkstra(int start, int end) 
        {
            const int inf = 2147483647; //nieskonczonosc = najwieksza wartosc inta
            Dictionary<int, double> cost = new Dictionary<int, double>(); //koszt do wierzcholka o danym id
            Dictionary<int, bool> visited = new Dictionary<int, bool>(); //tablica odwiedzonych
            int n = vertices.Count;
            int[] prev = new int[n+1];
            
            foreach (Vertex v in vertices)
            {
                visited.Add(v.id, false);
                cost.Add(v.id, inf);
            }
            SimplePriorityQueue<int> queue = new SimplePriorityQueue<int>(); // kolejka, priorytet ustawiany na podstawie kosztu

            cost[start] = 0;

            queue.Enqueue(start, (float)cost[start]);

            while (queue.Count != 0)
            {
                //odwiedzamy najtanszy wierzcholek na liscie
                int curr = queue.Dequeue();

                visited[curr] = true;

                foreach (Tuple<double, int> t in adjacencyList[curr]) //dla kazdego sasiada aktualnego wierzcholka
                {
                    int id = t.Item2;
                    if(cost[id] > cost[curr] + t.Item1) //sprawdz, czy mozna poprawic
                    {
                        cost[id] = cost[curr] + t.Item1; //popraw
                        if(!queue.Contains(id) && !visited[id]) //jezeli nie byl odwiedzony to dodaj na kolejke
                        {
                            queue.Enqueue(id, (float)cost[id]);
                            prev[id] = curr;
                        }
                        else if (queue.Contains(id)) //jezeli jest na kolejce to popraw koszt 
                        {
                            queue.UpdatePriority(id, (float)cost[id]);
                            prev[curr] = id;
                        }

                    }
                }
            }

            int step = end;
            List<int> path = new List<int>();
            path.Add(step);
            _path_cost = cost[end];
            while(step != start)
            {
                step = prev[step];
                path.Add(step);
            }
            path.Reverse();

            return path;
        }

        public List<Edge> Kruskal()
        {
            //int start = 1;
            List<Edge> E = edges.OrderBy(e => e.length).ToList();
            int n = vertices.Count; //liczba krawedzi w drzewie to zawsze liczba wierzcholkow - 1
            List<Edge> result = new List<Edge>();
            UnionFind uf = new UnionFind(n + 1);

            for (int i = 0; i < E.Count; ++i)
            {
                int from, to;
                from = E[i].beginning.id;
                to = E[i].end.id;

                if (uf.find(from) != uf.find(to))
                {
                    result.Add(E[i]);
                    uf.union(from, to);
                }
            }


            return result;
        }


        public int[,] FloydWarshall()
        {
            int n = vertices.Count;
            double[,] distance = new double[n + 1, n + 1];
            int[,] next = new int[n + 1, n + 1];

            for (int i = 0; i <= n; ++i)
            {
                for (int j = 0; j <= n; j++)
                {
                    distance[i, j] = 2147483647;
                }
            }
            for (int i = 0; i <= n; i++)
            {
                distance[i, i] = 0;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    next[i, j] = j;
                }
            }

            for (int i = 1; i <= n; ++i)
            {
                for (int j = 0; j < adjacencyList[i].Count; j++)
                {
                    distance[i, adjacencyList[i][j].Item2] = adjacencyList[i][j].Item1;

                }
            }


            for (int k = 1; k <= n; k++)
            {
                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= n; j++)
                    {
                        if (distance[i, j] > distance[i, k] + distance[k, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                            next[i, j] = next[i, k];
                        }

                    }
                }
            }

            return next;
        }

        public List<int> getWarshallPath(int start, int end, int[,] next)
        {
            List<int> path = new List<int>();
            path.Add(start);
            while (start != end)
            {
                start = next[start, end];
                path.Add(start);
            }
            return path;
        }

        public void DrawDijkstra(List<int> wierzcholki ,PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Green, 2);
            SolidBrush drawBrush = new SolidBrush(Color.Green);
            SolidBrush writeBrush = new SolidBrush(Color.White);
            int i = 1;
            while (i<wierzcholki.Count())
            {
                int prev = i - 1;
                g.FillEllipse(drawBrush, vertices[wierzcholki[prev]-1].position);
                g.DrawString(vertices[wierzcholki[prev]-1].id.ToString(), new Font("Arial", 10), writeBrush, vertices[wierzcholki[prev]-1].position);
                g.FillEllipse(drawBrush, vertices[wierzcholki[i]-1].position);
                g.DrawString(vertices[wierzcholki[i]-1].id.ToString(), new Font("Arial", 10), writeBrush, vertices[wierzcholki[i]-1].position);
                g.DrawLine(pen, vertices[wierzcholki[prev]-1].center, vertices[wierzcholki[i]-1].center);
                i++;
            }
        }

        public void DrawKruskal(List<Edge> mst, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Green, 2);
            SolidBrush drawBrush = new SolidBrush(Color.Green);
            SolidBrush writeBrush = new SolidBrush(Color.White);
            foreach(Edge edge in mst)
            {
                g.DrawLine(pen, edge.beginning.center, edge.end.center);
                g.FillEllipse(drawBrush, edge.beginning.position);
                g.DrawString(edge.beginning.id.ToString(), new Font("Arial", 10), writeBrush, edge.beginning.position);
                g.FillEllipse(drawBrush, edge.end.position);
                g.DrawString(edge.end.id.ToString(), new Font("Arial", 10), writeBrush, edge.end.position);
            }
        }
        
        public void DrawFloyd(List<int> wierzcholki, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Random rnd = new Random();
            Pen pen = new Pen(Color.FromArgb(255, rnd.Next(0,255), rnd.Next(0, 255), rnd.Next(0, 255)), 2);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            SolidBrush writeBrush = new SolidBrush(Color.White);
            int i = 1;
            while (i < wierzcholki.Count())
            {
                int prev = i - 1;
                g.FillEllipse(drawBrush, vertices[wierzcholki[prev] - 1].position);
                g.DrawString(vertices[wierzcholki[prev] - 1].id.ToString(), new Font("Arial", 10), writeBrush, vertices[wierzcholki[prev] - 1].position);
                g.FillEllipse(drawBrush, vertices[wierzcholki[i] - 1].position);
                g.DrawString(vertices[wierzcholki[i] - 1].id.ToString(), new Font("Arial", 10), writeBrush, vertices[wierzcholki[i] - 1].position);
                g.DrawLine(pen, vertices[wierzcholki[prev] - 1].center, vertices[wierzcholki[i] - 1].center);
                i++;
            }
        }

        public List<Edge> Steiner()
        {
            List<Edge> res_edges = new List<Edge>();
            List<int> obo = new List<int>();
            List<int> result = new List<int>();
            List<int> dodane = new List<int>();

            dodane.Add(obowiazkowe[0]);

            for(int i = 1; i < obowiazkowe.Count; i++)
            {
                int[,] floyd = FloydWarshall();
                int min_obo = int.MaxValue;
                int min_obo_id = 0;
                for(int j = 1; j < obowiazkowe.Count; j++)
                {
                    if(!dodane.Contains(obowiazkowe[j]) && min_obo > floyd[obowiazkowe[0],obowiazkowe[j]])
                    {
                        min_obo = floyd[obowiazkowe[0], obowiazkowe[j]];
                        min_obo_id = obowiazkowe[j];
                    }
                }

                dodane.Add(min_obo_id);

                List<int> sciezka = getWarshallPath(obowiazkowe[0], min_obo_id, floyd);
                
                int id = 0;

                for(int g = 0; g < sciezka.Count-1; g++)
                {
                    Edge e = new Edge(0, vertices[sciezka[g]-1], vertices[sciezka[g + 1]-1]);
                    res_edges.Add(e);
                }

                for(int j = 0; j < sciezka.Count-1; j++)
                {
                    int curr_from = sciezka[j];
                    int curr_next = sciezka[j + 1];

                    for (int k = 0; k < adjacencyList[curr_from].Count; k++)
                    {
                        if (adjacencyList[curr_from][k].Item2 == curr_next)
                        {
                            adjacencyList[curr_from][k] = new Tuple<double, int>(0, curr_next);

                            break;
                        }
                    }

                }

            }

            return res_edges;
        }

        public void DrawSteiner(List<int> wierzcholki, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Green, 2);
            SolidBrush drawBrush = new SolidBrush(Color.Green);
            SolidBrush writeBrush = new SolidBrush(Color.White);
            int i = 1;
            while (i < wierzcholki.Count())
            {
                int prev = i - 1;

                g.FillEllipse(drawBrush, vertices[wierzcholki[prev] - 1].position);

                g.DrawString(vertices[wierzcholki[prev] - 1].id.ToString(), new Font("Arial", 10), writeBrush, vertices[wierzcholki[prev] - 1].position);

                g.FillEllipse(drawBrush, vertices[wierzcholki[i] - 1].position);

                g.DrawString(vertices[wierzcholki[i] - 1].id.ToString(), new Font("Arial", 10), writeBrush, vertices[wierzcholki[i] - 1].position);

                g.DrawLine(pen, vertices[wierzcholki[prev] - 1].center, vertices[wierzcholki[i] - 1].center);

                i++;
            }
        }
    }





}
