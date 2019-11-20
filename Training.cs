using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

class Training : MonoBehaviour{
	public int gridsize = 1;
	public int width = 12;
	public int depth = 12;
	public UnityEngine.Object[] tiles = new UnityEngine.Object[0];
	public int[] RS = new int[0];
	public Dictionary<string, byte> str_tile;
	Dictionary<string, int[]> neighbors;
	public byte[,] sample; 

	public static byte Get2DByte(byte[,] ar, int x, int y){
		return ar[x, y];
	}

	public int Card(int n){
		return (n%4 + 4)%4;
	}

	public void RecordNeighbors() {
		Compile();
		neighbors = new Dictionary<string, int[]>();
		for (int y = 0; y < depth; y++){
			for (int x = 0; x < width; x++){
				for (int r = 0; r < 2; r++){
					int idx = (int)sample[x, y];
					int rot = Card(RS[idx] + r);
                    int rx = x + 1 - r;
                    int ry = y + r;
                    if (rx < width && ry < depth)
                    {
                        int ridx = (int)sample[rx, ry];
                        int rrot = Card(RS[ridx] + r);
                        string key = "" + idx + "." + rot + "|" + ridx + "." + rrot;
                        if (!neighbors.ContainsKey(key) && tiles[idx] && tiles[ridx])
                        {
                            neighbors.Add(key, new int[] { idx, rot, ridx, rrot });
                            Debug.DrawLine(
                                transform.TransformPoint(new Vector3((x + 0f) * gridsize, (y + 0f) * gridsize, 1f)),
                                transform.TransformPoint(new Vector3((x + 1f - r) * gridsize, (y + 0f + r) * gridsize, 1f)), Color.red, 9.0f, false);
                        }
                    }
				}
			}
		}
		System.IO.File.WriteAllText(Application.dataPath+"/"+this.gameObject.name+".xml", NeighborXML());
	}

	public string AssetPath(UnityEngine.Object o){
		#if UNITY_EDITOR
		return AssetDatabase.GetAssetPath(o).Trim().Replace("Assets/Resources/", "").Replace(".prefab", "");
		#else
		return "";
		#endif
	}

	public string NeighborXML(){
		Dictionary<UnityEngine.Object,int> counts = new Dictionary<UnityEngine.Object,int>();
		string res = "<set>\n  <tiles>\n";
		for (int i = 0; i < tiles.Length; i++){
			UnityEngine.Object o = tiles[i];
			if (o && !counts.ContainsKey(o)){
				counts[o] = 1;
				string assetpath = AssetPath(o);
				string sym = "X";
				string last = assetpath.Substring(assetpath.Length - 1);
				if (last == "X" || last == "I" || last == "L" || last == "T" || last == "D"){
					sym = last;
				}
				res += "<tile name=\""+assetpath+"\" symmetry=\""+sym+"\" weight=\"1.0\"/>\n";
			}
		}
		res += "	</tiles>\n<neighbors>";
		Dictionary<string, int[]>.ValueCollection v = neighbors.Values;
		foreach( int[] link in v ) {
	    	res += "  <neighbor left=\""+AssetPath(tiles[link[0]])+" "+link[1]+
	    				   "\" right=\""+AssetPath(tiles[link[2]])+" "+link[3]+"\"/>\n";
		}
		return res + "	</neighbors>\n</set>";
	}

	public bool hasWhitespace(){
		byte ws = (byte)0;
		for (int y = 0; y < depth-1; y++){
			for (int x = 0; x < width-1; x++){
				if (sample[x, y] == ws){
					return true;
				}
			}
		}
		return false;
	}

	public void Compile() {
		str_tile = new Dictionary<string, byte>();
		sample = new byte[width, depth]; 
		int cnt = this.transform.childCount;
		tiles = new UnityEngine.Object[500];
		RS = new int[1000];
		tiles[0] = null;
		RS[0] = 0;
		for (int i = 0; i < cnt; i++){
			GameObject tile = this.transform.GetChild(i).gameObject;
			Vector3 tilepos = tile.transform.localPosition;
			
			if ((tilepos.x > -0.55f) && (tilepos.x <= width*gridsize-0.55f) &&
				  (tilepos.y > -0.55f) && (tilepos.y <= depth*gridsize-0.55f)){
				UnityEngine.Object fab = tile;
				#if UNITY_EDITOR
				fab = PrefabUtility.GetCorrespondingObjectFromSource(tile);
				// if (fab == null){
				// 	PrefabUtility.RevertPrefabInstance(tile);
				// 	fab = PrefabUtility.GetCorrespondingObjectFromSource(tile);
				// }
				if (fab == null){
					fab = (GameObject)Resources.Load(tile.name);
					if (!fab){
						fab = tile;
					}
				}

				tile.name = fab.name;
				#endif
				int X = (int)(tilepos.x) / gridsize;
				int Y = (int)(tilepos.y) / gridsize;
				int R = (int)((360 - tile.transform.localEulerAngles.z)/90);
				if (R == 4) {R = 0;};
				if (!str_tile.ContainsKey(fab.name+R)){
					int index = str_tile.Count+1;
					str_tile.Add(fab.name+R, (byte)index);
					tiles[index] = fab;
					RS[index] = R;
					sample[X, Y] = str_tile[fab.name+R];
				} else {
					sample[X, Y] = str_tile[fab.name+R];
				}
			}
		}
		tiles = tiles.SubArray(0 , str_tile.Count+1);
		RS = RS.SubArray(0 , str_tile.Count+1);
	}

	void OnDrawGizmos(){
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(new Vector3((width*gridsize/2f)-gridsize*0.5f, (depth*gridsize/2f)-gridsize*0.5f, 0f),
							new Vector3(width*gridsize, depth*gridsize, gridsize));
		Gizmos.color = Color.cyan;
		for (int i = 0; i < this.transform.childCount; i++){
			GameObject tile = this.transform.GetChild(i).gameObject;
			Vector3 tilepos = tile.transform.localPosition;
			if ((tilepos.x > -0.55f) && (tilepos.x <= width*gridsize-0.55f) &&
				(tilepos.y > -0.55f) && (tilepos.y <= depth*gridsize-0.55f)){
				Gizmos.DrawSphere(tilepos, gridsize*0.2f);
			}
		}
	}
}

#if UNITY_EDITOR
[CustomEditor (typeof(Training))]
public class TrainingEditor : Editor {
	public override void OnInspectorGUI () {
		Training training = (Training)target;
		if(GUILayout.Button("compile")){
			training.Compile();
		}
		if(GUILayout.Button("record neighbors")){
			training.RecordNeighbors();
		}
		DrawDefaultInspector ();
	}
}
#endif