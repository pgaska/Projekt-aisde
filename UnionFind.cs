using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aisde
{
    public class UnionFind
    {
        public int[] id { get; set; }
        public int[] sz { get; set; }
        public int cnt { get; set; }

        public UnionFind(int N)
        {
            cnt = N;

            id = new int[N];
            sz = new int[N];

            for (int i = 0; i < N; i++)
            {
                id[i] = i;
                sz[i] = 1;
            }
        }

        // Return the id of component corresponding to object p.
        public int find(int p)
        {

            int root = p;
            while (root != id[root])
                root = id[root];

            while (p != root)
            {
                int newp = id[p];
                id[p] = root;
                p = newp;
            }

            return root;
        }

        public void union(int x, int y)
        {
            int i = find(x);
            int j = find(y);
            if (i == j) return;

            if (sz[i] < sz[j])
            {
                id[i] = j;
                sz[j] += sz[i];
            }
            else
            {
                id[j] = i;
                sz[i] += sz[j];
            }

            cnt--;
        }


        public bool connected(int x, int y)
        {
            return find(x) == find(y);
        }

        public int count()
        {
            return cnt;
        }
    }
}
