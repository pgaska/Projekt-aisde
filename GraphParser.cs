using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace aisde
{
    class GraphParser
    {
        public enum Algorithm { MST, DIJKSTRA, FLOYD, STEINER};  //typ algorytmu, moze sie przyda
        public Algorithm algorithm;

        private const int startIndexVertex = 8;
        private const int startIndexEdge = 8;
        private const int startIndexAlgorithm = 11;

        public int[] dijkstraVerticesId = new int[2];    //wierzcholki start i end dijkstry, tablica 2-elementowa
        public List<Tuple<int, int>> floydVerticesIds = new List<Tuple<int, int>>();    //pary wierzcholkow do floyda

        public Graph ParseGraph(string path)
        {
            Graph graph=new Graph();

            StreamReader streamReader = new StreamReader(path);

            graph.vertices = ParseVertices(streamReader);   //wczytaj wierzchołki
            ParseEdges(streamReader, graph.vertices, graph);    //wczytaj krawedzie

            string s = ParseAlgorithmType(streamReader); //wczytaj algorytm

            switch(s)
            {
                case ("MST"):
                    algorithm = Algorithm.MST;
                    break;
                case ("SCIEZKA"):
                    algorithm = Algorithm.DIJKSTRA;
                    ParseDijkstra(streamReader);
                    break;
                case ("FLOYD"):
                    algorithm = Algorithm.FLOYD;
                    ParseFloyd(streamReader);
                    break;
                case ("STEINER"):
                    algorithm = Algorithm.STEINER;
                    break;
                default:
                    break;
            }

            streamReader.Close();
            return graph;
        }

        private List<Vertex> ParseVertices(StreamReader streamReader)
        {
            List<Vertex> vertices = new List<Vertex>();
            int amountVertices = GetAmount(streamReader, startIndexVertex);

            string line;
            string[] numbers;
            int id, x, y, i=0;
            int obowiazkowy_int;
            bool obowiazkowy;

            while(i<amountVertices)
            {
                line = streamReader.ReadLine();

                if(!Comment(line))
                {
                    numbers = line.Split(' ');//rozdziela dane pobrane z linii

                    id = int.Parse(numbers[0]);
                    x = int.Parse(numbers[1]);
                    y = int.Parse(numbers[2]);
                    obowiazkowy_int = int.Parse(numbers[3]);
                    if(obowiazkowy_int ==1)
                    {
                        obowiazkowy = true;
                    }
                    else
                    {
                        obowiazkowy = false;
                    }
                    vertices.Add(new Vertex(id, x, y, obowiazkowy));
                    i++;
                }
            }

            return vertices;
        }

        private void ParseEdges(StreamReader streamReader, List<Vertex> vertices, Graph graph)
        {
            int amountEdges = GetAmount(streamReader, startIndexEdge);

            string line;
            string[] numbers;

            int id, beginning, end, i = 0;

            while(i<amountEdges)
            {
                line = streamReader.ReadLine();

                if(!Comment(line))
                {
                    numbers = line.Split(' '); //rozdziela dane pobrane z linii

                    id = int.Parse(numbers[0]);
                    beginning = int.Parse(numbers[1]);
                    end = int.Parse(numbers[2]);

                    graph.edges.Add(new Edge(id, vertices[beginning - 1], vertices[end - 1])); //tutaj zakładam że wierzchołki są dobrze ponumerowane
                    i++;
                }
            }
        }
        //jaki algorytm
        private string ParseAlgorithmType(StreamReader streamReader)
        {
            string line;
            while (streamReader != null)
            {
                line = streamReader.ReadLine();

                if (!Comment(line))
                {
                    line = line.Substring(startIndexAlgorithm);
                    return line;
                }
            }
            return null;
        }
        //wierzchołki dijkstry
        private void ParseDijkstra(StreamReader streamReader)
        {
            string line;
            string[] numbers;
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();

                if (!Comment(line))
                {
                    numbers = line.Split(' ');
                    dijkstraVerticesId[0] = int.Parse(numbers[0]);
                    dijkstraVerticesId[1] = int.Parse(numbers[1]);
                }
            }
        }

        //pary wierzchołków do algorytmu floyda
        private void ParseFloyd(StreamReader streamReader)
        {
            string line;
            string[] numbers;
            line = streamReader.ReadLine();
            while(!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
                if (!Comment(line))
                {
                    numbers = line.Split(' ');
                    var ids = Tuple.Create(int.Parse(numbers[0]), int.Parse(numbers[1]));
                    floydVerticesIds.Add(ids);
                }
            }
        }

        //pobiera ilosc danych elementow
        private int GetAmount(StreamReader streamReader, int start)
        {
            int amount;
            string line;

            while(streamReader!=null)
            {
                line = streamReader.ReadLine();

                if(!Comment(line))
                {
                    line = line.Substring(start);
                    amount = int.Parse(line);
                    return amount;
                }
            }
            return 0;
        }

        //czy linia jest komentarzem
        private bool Comment(string line)
        {
            return (line.Substring(0, 1) == "#");
        }
    }
}
