namespace Assets.Scripts.Craft.Parts.Modifiers
{
    using Assets.Scripts.Craft.Parts.Modifiers.Fuselage;
    using Assets.Scripts.Design;
    using ModApi.Craft;
    using ModApi.Craft.Parts;
    using UnityEngine;

    public class HoleScript : PartModifierScript<HoleData>
    {
        public int cornerRes => Data.cornerRes;
        private GameObject depthObj;
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private BoxCollider _collider;
        private MeshFilter _meshFilter; //private BoxCollider boxCollider;
        private Vector3[] vertices, normals;
        private Vector2[] uvs;
        private int[] triangles;
        private bool createUV = true;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //Creating Mesh
            _mesh = new Mesh();

            //Finding the gamobjects holding the meshes and setting them as children of this one
            depthObj = this.transform.Find("DepthMaskMesh").gameObject;

            //Creating and setting up the mesh renderers and mesh filters
            _meshRenderer = depthObj.GetComponent<MeshRenderer>();
            _meshRenderer.material = new Material(Shader.Find("Jundroo/DepthMask"));
            _collider = depthObj.GetComponent<BoxCollider>();
            _meshFilter = depthObj.GetComponent<MeshFilter>();

            UpdateMesh();
        }

        public void UpdateMesh()
        {
            Rect rect = new Rect(0f, 0f, 1f, 1f);

            int sides = Data.doubleSided ? 2 : 1;
            int vCount = cornerRes * 4 * sides + sides; //+sides for center vertices
            int triCount = (cornerRes * 4) * sides;
            if (vertices == null || vertices.Length != vCount)
            {
                vertices = new Vector3[vCount];
                normals = new Vector3[vCount];
            }
            if (triangles == null || triangles.Length != triCount * 3)
                triangles = new int[triCount * 3];
            if (createUV && (uvs == null || uvs.Length != vCount))
            {
                uvs = new Vector2[vCount];
            }
            int count = cornerRes * 4;
            if (createUV)
            {
                uvs[0] = Vector2.one * 0.5f;
                if (Data.doubleSided)
                    uvs[count + 1] = uvs[0];
            }
            float tl = Mathf.Max(0, Data.cornerRadius1);
            float tr = Mathf.Max(0, Data.cornerRadius2);
            float bl = Mathf.Max(0, Data.cornerRadius3);
            float br = Mathf.Max(0, Data.cornerRadius4);
            float f = Mathf.PI * 0.5f / (cornerRes - 1);
            float a1 = 1f;
            float a2 = 1f;
            float x = 1f;
            float y = 1f;
            Vector2 rs = Vector2.one;

            rs = new Vector2(rect.width, rect.height) * 0.5f;
            if (rect.width > rect.height)
                a1 = rect.height / rect.width;
            else
                a2 = rect.width / rect.height;
            tl = Mathf.Clamp01(tl);
            tr = Mathf.Clamp01(tr);
            bl = Mathf.Clamp01(bl);
            br = Mathf.Clamp01(br);

            vertices[0] = new Vector3();
            if (Data.doubleSided)
                vertices[count + 1] = new Vector3();
            for (int i = 0; i < cornerRes; i++)
            {
                float s = Mathf.Sin((float)i * f);
                float c = Mathf.Cos((float)i * f);
                Vector2 v1 = new Vector3(-x + (1f - c) * tl * a1, y - (1f - s) * tl * a2);
                Vector2 v2 = new Vector3(x - (1f - s) * tr * a1, y - (1f - c) * tr * a2);
                Vector2 v3 = new Vector3(x - (1f - c) * br * a1, -y + (1f - s) * br * a2);
                Vector2 v4 = new Vector3(-x + (1f - s) * bl * a1, -y + (1f - c) * bl * a2);

                vertices[1 + i] = (Vector2.Scale(v1, rs));
                vertices[1 + cornerRes + i] = (Vector2.Scale(v2, rs));
                vertices[1 + cornerRes * 2 + i] = (Vector2.Scale(v3, rs));
                vertices[1 + cornerRes * 3 + i] = (Vector2.Scale(v4, rs));
                if (createUV)
                {
                    uvs[1 + i] = v1 * 0.5f + Vector2.one * 0.5f;
                    uvs[1 + cornerRes * 1 + i] = v2 * 0.5f + Vector2.one * 0.5f;
                    uvs[1 + cornerRes * 2 + i] = v3 * 0.5f + Vector2.one * 0.5f;
                    uvs[1 + cornerRes * 3 + i] = v4 * 0.5f + Vector2.one * 0.5f;
                }
                if (Data.doubleSided)
                {
                    vertices[1 + cornerRes * 8 - i] = vertices[1 + i];
                    vertices[1 + cornerRes * 7 - i] = vertices[1 + cornerRes + i];
                    vertices[1 + cornerRes * 6 - i] = vertices[1 + cornerRes * 2 + i];
                    vertices[1 + cornerRes * 5 - i] = vertices[1 + cornerRes * 3 + i];
                    if (createUV)
                    {
                        uvs[1 + cornerRes * 8 - i] = v1 * 0.5f + Vector2.one * 0.5f;
                        uvs[1 + cornerRes * 7 - i] = v2 * 0.5f + Vector2.one * 0.5f;
                        uvs[1 + cornerRes * 6 - i] = v3 * 0.5f + Vector2.one * 0.5f;
                        uvs[1 + cornerRes * 5 - i] = v4 * 0.5f + Vector2.one * 0.5f;
                    }
                }
            }
            for (int i = 0; i < count + 1; i++)
            {
                normals[i] = -Vector3.forward;
                if (Data.doubleSided)
                {
                    normals[count + 1 + i] = Vector3.forward;
                    // if (FlipBackFaceUV)
                    // {
                    //     Vector2 uv = uvs[count + 1 + i];
                    //     uv.x = 1f - uv.x;
                    //     uvs[count + 1 + i] = uv;
                    // }
                }
            }
            for (int i = 0; i < count; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
                if (Data.doubleSided)
                {
                    triangles[(count + i) * 3] = count + 1;
                    triangles[(count + i) * 3 + 1] = count + 1 + i + 1;
                    triangles[(count + i) * 3 + 2] = count + 1 + i + 2;
                }
            }
            triangles[count * 3 - 1] = 1;
            if (Data.doubleSided)
                triangles[triangles.Length - 1] = count + 1 + 1;

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.normals = normals;
            if (createUV)
                _mesh.uv = uvs;
            _mesh.triangles = triangles;

            //_collider.size = new Vector3(rect.width, rect.height, 0.5f);

            _meshFilter.sharedMesh = null;
            _meshFilter.sharedMesh = _mesh;

            depthObj.transform.localScale = new Vector3(Data.width, Data.height, 1);

            PartScript.PartMaterialScript.UpdateRenderers();
            PartScript.PartMaterialScript.UpdateTextureData();
            PartScript.PartMaterialScript.OnMaterialsChanged();
            if (Game.InDesignerScene)
            {
                Symmetry.SynchronizePartModifiers(base.PartScript);
            }
        }
    
        public override void OnConnectedToPart(PartConnectedEventData e)
        {
            base.OnConnectedToPart(e);
            ManageAutoResize(e.TargetPart, e.TargetAttachPoint);
        }


        //Auto resize (removed because it was breaking the mod)
        public override void OnCraftStructureChanged(ICraftScript craftScript)
        {
            // base.OnCraftStructureChanged(craftScript);
            // if (Game.InDesignerScene)
            //     if (PartScript.AttachPointScripts[0].AttachPoint.PartConnections.Count == 1)
            //         if (!ManageAutoResize(PartScript.AttachPointScripts[0].AttachPoint.PartConnections[0].GetOtherPart(PartScript.Data), PartScript.AttachPointScripts[0].AttachPoint.PartConnections[0].Attachments[0].GetOtherAttachPoint(PartScript.AttachPointScripts[0].AttachPoint)) && PartScript.AttachPointScripts[1].AttachPoint.PartConnections.Count == 1)
            //             ManageAutoResize(PartScript.AttachPointScripts[1].AttachPoint.PartConnections[0].GetOtherPart(PartScript.Data), PartScript.AttachPointScripts[1].AttachPoint.PartConnections[0].Attachments[0].GetOtherAttachPoint(PartScript.AttachPointScripts[1].AttachPoint));
        }

        private bool ManageAutoResize(PartData data, AttachPoint targetAttachPoint)
        {
            FuselageData targetData = data.GetModifier<FuselageData>();
            Debug.Log(targetAttachPoint.Name);
            if (targetData != null)
            {
                bool top = targetAttachPoint.Name.Contains("Top");
                bool bottom = targetAttachPoint.Name.Contains("Bottom");
                if (top)
                {
                    Data.cornerRadius1 = targetData.CornerRadiuses[0];
                    Data.cornerRadius2 = targetData.CornerRadiuses[1];
                    Data.cornerRadius3 = targetData.CornerRadiuses[2];
                    Data.cornerRadius4 = targetData.CornerRadiuses[3];

                    Data.width = targetData.TopScale.x * 2;
                    Data.height = targetData.TopScale.y * 2;
                }
                else if (bottom)
                {
                    Data.cornerRadius1 = targetData.CornerRadiuses[4];
                    Data.cornerRadius2 = targetData.CornerRadiuses[5];
                    Data.cornerRadius3 = targetData.CornerRadiuses[6];
                    Data.cornerRadius4 = targetData.CornerRadiuses[7];

                    Data.width = targetData.BottomScale.x * 2;
                    Data.height = targetData.BottomScale.y * 2;
                }

                UpdateMesh();
                if (top || bottom) return true;
            }
            return false;
        }
    }
}