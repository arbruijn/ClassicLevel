using Classic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ClassicLevelComp : MonoBehaviour {
    ClassicLevel level;

    // Use this for initialization
    void Start () {
        /*
        GetClassic
        var ri = 0;
        foreach (var go in ClassicRobots) {
            var lgo = UnityEngine.Object.Instantiate<GameObject>(go);
            lgo.transform.parent = gameObject.transform;
            lgo.transform.localPosition = Vector3.right * ri++ * 10f;
        }
        */
        //Debug.Log("NumRobots " + GetClassic.NumRobots);
        //var bot = GetClassic.InstantiateRobot(1);
        //transform.localScale = Vector3.one * .001f;
        //bot.transform.localRotation = Quaternion.Euler(0,-180,0); 
        //bot.transform.parent = transform;
        if (!GetClassic.MyInit())
            return;
        level = new ClassicLevel();
        level.Read(new BinaryReader(new MemoryStream(GetClassic.hog.ItemData("level01.rdl"))));


        var tmapSet = new Dictionary<int, int>();
        var tmapList = new List<int>();
        var tmapCount = 0;
        foreach (var segment in level.mine.Segments) {
            var segVerts = segment.verts;
            for (int sidenum = 0; sidenum < Segment.NUM_SIDES; sidenum++)
                if (segment.children[sidenum] == -1) {
                    int tmap_num = segment.sides[sidenum].tmap_num;
                    if (!tmapSet.ContainsKey(tmap_num)) {
                        tmapSet.Add(tmap_num, tmapCount++);
                        tmapList.Add(tmap_num);
                    }
                    int tmap_num2 = segment.sides[sidenum].tmap_num2 & 0x3fff;
                    if (tmap_num2 != 0 && !tmapSet.ContainsKey(tmap_num2)) {
                        //Classic.Debug.Log("tmap 2 " + tmap_num2);
                        tmapSet.Add(tmap_num2, tmapCount++);
                        tmapList.Add(tmap_num2);
                    }
                }
        }

        var vertices = level.mine.Vertices.Select(x => x.ToVector3() / 5).ToArray();
        //mesh.vertices = vertices;
        var mverts = new List<Vector3>();
        var tris = new List<int>[tmapCount].Select(x => new List<int>()).ToArray();
        var uvs = new List<Vector2>();
        var norms = new List<Vector3>();
        
        int n = 0;
        foreach (var segment in level.mine.Segments) {
            var segVerts = segment.verts;
            for (int sidenum = 0; sidenum < Segment.NUM_SIDES; sidenum++)
                //if (segment.HasSide(sidenum))
                if (segment.children[sidenum] == -1)
                {
                    //if (tmapSet[segment.sides[sidenum].tmap_num] != 0)
                    //    continue;
                    //var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    //segment.sides[sidenum].
                    var mvertOfs = mverts.Count;
                    for (int i = 0; i < 4; i++)
                    {
                        mverts.Add(vertices[segVerts[Segment.Side_to_verts[sidenum, i]]]);
                        uvs.Add(segment.sides[sidenum].uvls[i].ToVector2());
                    }
                    var norm = Vector3.Cross(mverts[mvertOfs + 1] - mverts[mvertOfs], mverts[mvertOfs + 2] - mverts[mvertOfs]);
                    norm.Normalize();
                    norms.Add(norm);
                    norms.Add(norm);
                    norms.Add(norm);
                    norms.Add(norm);
                    var mtris = tris[tmapSet[segment.sides[sidenum].tmap_num]];
                    for (int i = 0; i < 2; i++)
                    {
                        mtris.Add(mvertOfs);
                        mtris.Add(mvertOfs + 1 + i);
                        mtris.Add(mvertOfs + 2 + i);
                    }
                    int tmap_num2 = segment.sides[sidenum].tmap_num2;
                    if (false && tmap_num2 != 0)
                    {
                        if ((tmap_num2 & 0xc000) != 0) {
                            var rot = tmap_num2 >> 14;
                            mvertOfs = mverts.Count;
                            for (int i = 0; i < 4; i++)
                            {
                                mverts.Add(vertices[segVerts[Segment.Side_to_verts[sidenum, i]]]);
                                var uv = segment.sides[sidenum].uvls[i].ToVector2();
                                switch (rot) {
                                    case 1:
                                        uv = new Vector2(-uv.y, uv.x);
                                        break;
                                    case 2:
                                        uv = new Vector2(-uv.x, -uv.y);
                                        break;
                                    case 3:
                                        uv = new Vector2(uv.y, -uv.x);
                                        break;
                                }
                                uvs.Add(uv);
                            }
                            norms.Add(norm);
                            norms.Add(norm);
                            norms.Add(norm);
                            norms.Add(norm);
                        }
                        var mtris2 = tris[tmapSet[segment.sides[sidenum].tmap_num2 & 0x3fff]];
                        for (int i = 0; i < 2; i++)
                        {
                            mtris2.Add(mvertOfs);
                            mtris2.Add(mvertOfs + 1 + i);
                            mtris2.Add(mvertOfs + 2 + i);
                        }
                    }
                }
            //if (n++ == 50)
            //    break;
        }

        var mineGO = new GameObject();
        mineGO.transform.parent = transform;
        var mesh = new Mesh();
        mesh.subMeshCount = tmapCount;
        mesh.SetVertices(mverts);
        for (int i = 0; i < tris.Length; i++)
            mesh.SetTriangles(tris[i], i);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(norms);
        //mesh.RecalculateNormals();
        mineGO.AddComponent<MeshFilter>().sharedMesh = mesh;

        var mats = new Material[tmapCount];
        for (int i = 0; i < tmapCount; i++) {
            var tex = GetClassic.MkTex(GetClassic.ClassicData.Textures[tmapList[i]].index);
            Classic.Debug.Log("mat " + i + " tex " + tmapList[i] + " bm " + GetClassic.ClassicData.Textures[tmapList[i]].index);
            var mat = new Material(Shader.Find("Standard"));
            //mat.color = Color.white;
            //mat.
            mat.mainTexture = tex;
            mats[i] = mat;
        }
        mineGO.AddComponent<MeshRenderer>().materials = mats;
        mineGO.AddComponent<MeshCollider>();
        var lights = new GameObject("lights");
        lights.transform.parent = transform;
        var lightsTransform = lights.transform;
        foreach (var segment in level.mine.Segments) {
            Vector3 pos = new Vector3(), min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < 8; i++) {
                var v = vertices[segment.verts[i]];
                pos += v;
                for (int j = 0; j < 3; j++) {
                    if (v[j] < min[j])
                        min[j] = v[j];
                    if (v[j] > max[j])
                        max[j] = v[j];
                }
            }
            pos *= 0.125f;
            var lightGO = new GameObject();
            lightGO.transform.parent = lightsTransform;
            lightGO.transform.localPosition = pos;
            var light = lightGO.AddComponent<Light>();
            light.color = Color.white;
            var range = light.range = (max - min).magnitude / 2f * 1.1f;
            //var range = light.range = Mathf.Max(Mathf.Max(max.x - min.x, max.y - min.y), max.z - min.z) / 2f;
            //if (range == 9.75f)
            //    Classic.Debug.Log("min=" + min + " max=" + max);
            //(max - min).magnitude / 2f;
            //light.intensity = 1.5f;
            //light.type = LightType.Point;
            //light.intensity = Mathf.Min(2f, Mathf.Max(range / 3f, 1 + Mathf.Log(segment.static_light.ToFloat() / 10f)));
            light.intensity =segment.static_light.ToFloat();
            //Mathf.Log(segment.static_light.ToFloat()) * range;
            //Mathf.Min(range / 8f, Mathf.Log(segment.static_light.ToFloat()) * 4f);
            light.renderMode = LightRenderMode.ForcePixel;
            //light.transform.localPosition = pos;
        }
        UnityEngine.Debug.Log("player rad: " + GetClassic.ClassicData.PolygonModels[GetClassic.ClassicData.PlayerShip.model_num].rad);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Classic.Debug.Log("collision");
    }
}
