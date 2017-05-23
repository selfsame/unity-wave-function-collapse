/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

public class SimpleTiledModel : Model
{
	public int[][][] propagator;
	public List<string> tiles;

	public SimpleTiledModel(string name, string subsetName, int width, int height, bool periodic)
        :base(width,height)
    {
		this.periodic = periodic;

		var xdoc = new XmlDocument();
		xdoc.Load(name);
		XmlNode xnode = xdoc.FirstChild;
		bool unique = xnode.Get("unique", false);
		xnode = xnode.FirstChild;

		List<string> subset = null;
		if (subsetName != "")
		{
			subset = new List<string>();
			foreach (XmlNode xsubset in xnode.NextSibling.NextSibling.ChildNodes) 
				if (xsubset.NodeType != XmlNodeType.Comment && xsubset.Get<string>("name") == subsetName)
					foreach (XmlNode stile in xsubset.ChildNodes) subset.Add(stile.Get<string>("name"));
		}


		Func<string, string> rotate = (n) =>{
			int rot = int.Parse(n.Substring(0,1))+1;
			return ""+rot+n.Substring(1);
		};

		tiles = new List<string>();
		var tempStationary = new List<double>();

		List<int[]> action = new List<int[]>();
		Dictionary<string, int> firstOccurrence = new Dictionary<string, int>();

		foreach (XmlNode xtile in xnode.ChildNodes)
		{
			string tilename = xtile.Get<string>("name");
			if (subset != null && !subset.Contains(tilename)) continue;

			Func<int, int> a, b;
			int cardinality;

			char sym = xtile.Get("symmetry", 'X');
			if (sym == 'L')
			{
				cardinality = 4;
				a = i => (i + 1) % 4;
				b = i => i % 2 == 0 ? i + 1 : i - 1;
			}
			else if (sym == 'T')
			{
				cardinality = 4;
				a = i => (i + 1) % 4;
				b = i => i % 2 == 0 ? i : 4 - i;
			}
			else if (sym == 'I')
			{
				cardinality = 2;
				a = i => 1 - i;
				b = i => i;
			}
			else if (sym == 'D')
			{
				cardinality = 2;
				a = i => 1 - i;
				b = i => 1 - i;
			}
			else
			{
				cardinality = 1;
				a = i => i;
				b = i => i;
			}

			T = action.Count;
			firstOccurrence.Add(tilename, T);
			
			int[][] map = new int[cardinality][];
			for (int t = 0; t < cardinality; t++)
			{
				map[t] = new int[8];

				map[t][0] = t;
				map[t][1] = a(t);
				map[t][2] = a(a(t));
				map[t][3] = a(a(a(t)));
				map[t][4] = b(t);
				map[t][5] = b(a(t));
				map[t][6] = b(a(a(t)));
				map[t][7] = b(a(a(a(t))));

				for (int s = 0; s < 8; s++) map[t][s] += T;

				action.Add(map[t]);
			}

			if (unique)
			{
				for (int t = 0; t < cardinality; t++)
				{
					tiles.Add(""+"0"+tilename);
				}
			}
			else
			{
				tiles.Add("0"+tilename);
				for (int t = 1; t < cardinality; t++) tiles.Add(rotate(tiles[T + t - 1]));
			}

			for (int t = 0; t < cardinality; t++) tempStationary.Add(xtile.Get("weight", 1.0f));
		}

		T = action.Count;
		stationary = tempStationary.ToArray();

        var tempPropagator = new bool[4][][];
        propagator = new int[4][][];
		for (int d = 0; d < 4; d++)
		{
            tempPropagator[d] = new bool[T][];
            propagator[d] = new int[T][];
            for (int t = 0; t < T; t++) tempPropagator[d][t] = new bool[T];
        }

        for (int x = 0; x < FMX; x++) for (int y = 0; y < FMY; y++) wave[x][y] = new bool[T];

        foreach (XmlNode xneighbor in xnode.NextSibling.ChildNodes)
		{
			string[] left = xneighbor.Get<string>("left").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string[] right = xneighbor.Get<string>("right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (subset != null && (!subset.Contains(left[0]) || !subset.Contains(right[0]))) continue;

            int L = action[firstOccurrence[string.Join(" ", left.Take(left.Length - 1).ToArray())]][left.Length == 1 ? 0 : int.Parse(left.Last())], D = action[L][1];
			int R = action[firstOccurrence[string.Join(" ", right.Take(right.Length - 1).ToArray())]][right.Length == 1 ? 0 : int.Parse(right.Last())], U = action[R][1];

            tempPropagator[0][R][L] = true;
            tempPropagator[0][action[R][6]][action[L][6]] = true;
            tempPropagator[0][action[L][4]][action[R][4]] = true;
            tempPropagator[0][action[L][2]][action[R][2]] = true;

            tempPropagator[1][U][D] = true;
            tempPropagator[1][action[D][6]][action[U][6]] = true;
            tempPropagator[1][action[U][4]][action[D][4]] = true;
            tempPropagator[1][action[D][2]][action[U][2]] = true;
		}

		for (int t2 = 0; t2 < T; t2++) for (int t1 = 0; t1 < T; t1++)
			{
                tempPropagator[2][t2][t1] = tempPropagator[0][t1][t2];
                tempPropagator[3][t2][t1] = tempPropagator[1][t1][t2];
			}

        List<int>[][] sparsePropagator = new List<int>[4][];
        for (int d = 0; d < 4; d++)
        {
            sparsePropagator[d] = new List<int>[T];
            for (int t = 0; t < T; t++) sparsePropagator[d][t] = new List<int>();
        }

        for (int d = 0; d < 4; d++) for (int t1 = 0; t1 < T; t1++)
            {
                List<int> sp = sparsePropagator[d][t1];
                bool[] tp = tempPropagator[d][t1];

                for (int t2 = 0; t2 < T; t2++) if (tp[t2]) sp.Add(t2);

                int ST = sp.Count;
                propagator[d][t1] = new int[ST];
                for (int st = 0; st < ST; st++) propagator[d][t1][st] = sp[st];
            }
        }

	protected override bool Propagate()
	{
		bool change = false, b;
		for (int x2 = 0; x2 < FMX; x2++) for (int y2 = 0; y2 < FMY; y2++) for (int d = 0; d < 4; d++)
				{
					int x1 = x2, y1 = y2;
					if (d == 0)
					{
						if (x2 == 0)
						{
							if (!periodic) continue;
							else x1 = FMX - 1;
						}
						else x1 = x2 - 1;
					}
					else if (d == 1)
					{
						if (y2 == FMY - 1)
						{
							if (!periodic) continue;
							else y1 = 0;
						}
						else y1 = y2 + 1;
					}
					else if (d == 2)
					{
						if (x2 == FMX - 1)
						{
							if (!periodic) continue;
							else x1 = 0;
						}
						else x1 = x2 + 1;
					}
					else
					{
						if (y2 == 0)
						{
							if (!periodic) continue;
							else y1 = FMY - 1;
						}
						else y1 = y2 - 1;
					}

					if (!changes[x1][y1]) continue;
					bool[] w1 = wave[x1][y1];
 					bool[] w2 = wave[x2][y2];

                    for (int t2 = 0; t2 < T; t2++)
                    {
                        if (!w2[t2]) continue;
                        b = false;
                        int[] prop = propagator[d][t2];
                        for (int i1 = 0; i1 < prop.Length && !b; i1++) b = w1[prop[i1]];
                        if (!b)
                        {
                            changes[x2][y2] = true;
                            change = true;
                            w2[t2] = false;
                        }
                    }
				}			

		return change;
	}

	public string Sample(int x, int y){
		bool found = false;
		string res = "?";
		for (int t = 0; t < T; t++) if (wave[x][y][t]){
			if (found) {return "?";}
			found = true;
			res = tiles[t];
		}
		return res;
	}

	protected override bool OnBoundary(int x, int y){
		return false;
	}

}