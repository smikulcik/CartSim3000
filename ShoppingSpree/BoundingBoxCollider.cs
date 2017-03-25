using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShoppingSpree
{
    public class BoundingBoxCollider
    {
        GameObject gameObject;
        Vector3[] rawVerts;
        Vector3[] verts;
        public BoundingBox bb;

        public bool EnableBounce = true;
        public bool EnableRotate = true;

        public BoundingBoxCollider(GameObject g)
        {
            gameObject = g;
            extractVerts();
            Update();
        }
        public void extractVerts()
        {
            int numVerts = 0;
            int offset = 0;
            foreach (ModelMesh mesh in gameObject.Model.Meshes)
            {

                //there may be multiple parts ... different materials etc...
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    numVerts += part.NumVertices;
                }
            }
            
            rawVerts = new Vector3[numVerts];
            verts = new Vector3[numVerts];
            foreach (ModelMesh mesh in gameObject.Model.Meshes)
            {

                //there may be multiple parts ... different materials etc...
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    //lets get the array of Vector3...  this will be an interlaced list of Position Normal Texture Position Normal Texture ....
                    Vector3[] tempVerts = new Vector3[part.NumVertices];

                    part.VertexBuffer.GetData(part.VertexOffset * stride, tempVerts, 0, tempVerts.Length, stride);

                    //the array of vertices are unique.  If a triangle reuses an exisiting vertex, it will not "reappear" in the list
                    //therefore we need to make a copy of each vertex used in every triangle...
                    ushort[] indices = new ushort[part.IndexBuffer.IndexCount];
                    part.IndexBuffer.GetData<ushort>(indices);

                    for(int i = 0; i < tempVerts.Length; i++) {
                        rawVerts[offset] = tempVerts[i];
                        offset++;
                    }
                }
            }

            // dont have bounding boxes with zero dimenstions
            //bb = BoundingBox.CreateMerged(bb, new BoundingBox(bb.Min - new Vector3(-0.001f), bb.Max + new Vector3(0.001f)));
        }

        //use new world transform
        public void Update()
        {
            Matrix transform = gameObject.Model.Meshes[0].ParentBone.Transform * gameObject.World;
            Vector3.Transform(rawVerts, ref transform, verts);

            bb = BoundingBox.CreateFromPoints(verts);
        }
        
        private double greatestMagInt(double n)
        {
            return Math.Sign(n) * Math.Floor(Math.Abs(n));
        }

        public bool checkCollision(GameObject e)
        {
            //A quad is defined by 6 vertices (3 per triangle).  Since we assume both triangles are on the
            // same plane, it is not required to check the collision with the other triangle, so skip by 6.

            BoundingBox enemyBB = e.Collider.bb;

            Vector3 enemyCenter = (enemyBB.Max + enemyBB.Min) / 2;
            float enemyRadius = (enemyBB.Max - enemyBB.Min).Length() / 2;
            Vector3 enemyDims = enemyBB.Max - enemyBB.Min;

            Vector3 myCenter = (bb.Max + bb.Min) / 2;
            Vector3 myDims = bb.Max - bb.Min;

            //if too far away to collide
            if ((enemyCenter - myCenter).Length() > (myDims + enemyDims).Length())
                return false;

            Boolean collision = false;
            
            if (bb.Intersects(enemyBB))  //collision
            {
                Vector3 toEnemy = enemyCenter - myCenter;
                if(toEnemy.Length() == 0)
                {
                    return false;
                }

                //direction of intersection, must be scaled to my bounding box
                Vector3 scaledToEnemy = new Vector3(toEnemy.X / myDims.X, toEnemy.Y / myDims.Y, toEnemy.Z / myDims.Z);

                float maxDim = (float)Math.Max(Math.Max(Math.Abs(scaledToEnemy.X), Math.Abs(scaledToEnemy.Y)), Math.Abs(scaledToEnemy.Z))*.999f;
                Vector3 n = Vector3.Normalize(
                    new Vector3(
                        (float)(greatestMagInt(scaledToEnemy.X / maxDim)),
                        (float)(greatestMagInt(scaledToEnemy.Y / maxDim)),
                        (float)(greatestMagInt(scaledToEnemy.Z / maxDim))
                    )
                );
                //n = Vector3.Normalize(toEnemy);


                // box analog of myRad + eRad - (center - e.center);
                //this is how much we must correct the enemy position
                Vector3 dist = Math.Abs(Vector3.Dot((myDims + enemyDims) / 2, n)) * n - Vector3.Dot(toEnemy, n) * n;

                e.Pos = e.Pos + dist * 1.01f ;
                /*if (gameObject.Model.Meshes[0].Name != "Plane")
                {
                    Console.WriteLine(dist);
                    Console.WriteLine(n);
                }*/

                if (e.Collider.EnableBounce)
                {
                    Vector3 V = e.Vel;
                    V.Normalize();
                    V = 2 * (Vector3.Dot(-V, n))
                             * n + V;
                    if (!float.IsNaN(e.Vel.Length()) && !float.IsNaN(V.X))
                        e.Vel = e.Vel.Length() * V * .8f;
                    /*if(gameObject.Model.Meshes[0].Name != "Plane")
                        Console.WriteLine(e.Model.Meshes[0].Name + " bounced on " + gameObject.Model.Meshes[0].Name);*/
                }
                collision = true;
                //return collision;
            }
            return collision;
        }
    }
}
