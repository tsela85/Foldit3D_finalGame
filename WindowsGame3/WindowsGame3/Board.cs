﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Foldit3D
{
    class Board
    {
        public enum BoardState { chooseEdge1, onEdge1, chooseEdge2, onEdge2,preFold, folding1, folding2 };

        public struct DividingVert
        {
            public float distance;
            public int small;
            public int big;
            public Vector3 position;
        }
        private VertexPositionNormalTexture[] vertices;        
        private short[] indices;
        private short[] invertIndices;
        private int vertNum;
        private Texture2D texture;
        private Vector3 pointOnEdge;
        private DividingVert[] p;        
        private VertexPositionColor[] verOnEdge;
        public Board one, two;
        private float angle = 0;
        private Vector3 center;
        private Effect effect;
        private Camera camera;
        private Matrix worldMatrix;
        private GraphicsDevice device;
        private InputHandler input;
        private BoardState state;
        private List<VertexPositionNormalTexture> oldLineList;
        private List<short> oldLineindices;
        VertexPositionColor[] lineList;
        private List<short> lineIndices;

        public Board(Texture2D tex, Effect eff)
        {
            texture = tex;
            effect = eff;
        }

        #region properties

        public VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public short[] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        public BoardState State
        {
            get { return state; }
            set { state = value; }
        }

        public Vector3 Center
        {
            get { return center; }            
        }

        public Vector3 getAxis()
        {
            Vector3 axis;
            if (((p[0].small + p[0].big) > (p[1].small + p[1].big)) || (p[0].big == 0))
                axis = p[0].position - p[1].position;
            else
                axis = p[1].position - p[0].position;
            axis.Normalize();
            return axis;
        }

        public Vector3 getAxisPoint()
        {
            return new Vector3((p[0].position.X + p[1].position.X) / 2, 0, (p[0].position.Z + p[1].position.Z) / 2);
            //return p[0].position;
        }

        public void getfoldPoints(out Vector3 p1, out Vector3 p2)
        {
            if (((p[0].small + p[0].big) > (p[1].small + p[1].big)) || (p[0].big == 0))
            {
                p1 = p[0].position;
                p2 = p[1].position;
            }
            else
            {
                p1 = p[1].position;
                p2 = p[0].position;
            }            
        }



        public float getAngle()
        {
            return angle;
        }
        #endregion

        #region Levels

        public void initLevel(List<IDictionary<string, string>> data)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector2> texCords = new List<Vector2>();
            int numOfVertix = 0;
            foreach (IDictionary<string, string> item in data)
            {
                Vector3 point = new Vector3((float)Convert.ToDouble(item["x"]), (float)Convert.ToDouble(item["y"]), (float)Convert.ToDouble(item["z"]));
                Vector2 cord = new Vector2((float)Convert.ToDouble(item["texX"]), (float)Convert.ToDouble(item["texY"]));
                points.Add(point);
                texCords.Add(cord);
                numOfVertix++;
            }
            
            Initialize(numOfVertix,points.ToArray(),texCords.ToArray());
        }

        #endregion

        public void Initialize(int vNum, Vector3[] points, Vector2[] texCords)
        {
            pointOnEdge = Vector3.Zero;
            center = Vector3.Zero;
            foreach (Vector3 vec in points)
                center += vec;
            center /= vNum;
            int iCount = (vNum - 3) * 3 + 3;
            vertNum = vNum;
            
            vertices = new VertexPositionNormalTexture[vNum];
            oldLineList = new List<VertexPositionNormalTexture>();
            oldLineindices = new List<short>();
            lineList = new VertexPositionColor[2];
            lineList[0].Color = Color.Orange;
            lineList[1].Color = Color.Orange;
            lineIndices = new List<short>();
            camera = Game1.camera;
            device = Game1.device;
            input = Game1.input;
            worldMatrix = Matrix.Identity;
            state = BoardState.chooseEdge1;

            indices = new short[iCount];
            invertIndices = new short[iCount];
            for (int i = 0; i < vNum; i++)
            {
                vertices[i].Position = points[i];
                vertices[i].TextureCoordinate = texCords[i];
                vertices[i].Normal = Vector3.Up;
            }
            //create the indices. every 2 consecutive points makes an outer edge (x,y,z) -> xy , yz (not xz)
            short j = 0;
            for (short i = 0; i < iCount; i += 3)
            {
                invertIndices[iCount - (i + 1)] =  indices[i] = (short)(j % vNum);
                invertIndices[iCount - (i + 1) - 1] = indices[i + 1] = (short)((j + 1) % vNum);
                invertIndices[iCount - (i + 1) - 2] = indices[i + 2] = (short)(((j != 4 ? j : 5) + 2) % vNum);
                j += 2;
            }
            verOnEdge = new VertexPositionColor[6];
            verOnEdge[0].Color = Color.Purple;
            verOnEdge[1].Color = Color.Purple;
            verOnEdge[2].Color = Color.Purple;
            verOnEdge[3].Color = Color.Purple;
            verOnEdge[4].Color = Color.Purple;
            verOnEdge[5].Color = Color.Purple;
            p = new DividingVert[2];
        }

        //public void initLevel(List<IDictionary<string, string>> data)
        //{
        //    List<Vector3> vecs = new List<Vector3>();
        //    List<Vector2> tex = new List<Vector2>();
        //    foreach (IDictionary<string, string> item in data)
        //    {
        //       Vector3 temp = new Vector3();
        //       temp.X = Convert.To (item["x"])  ( item["x"];

        //    }
        //}
                                   

        public void DrawfoldPart()
        {
            if (state == BoardState.folding1)
            {
                one.foldShape(angle);                
                one.Draw();
                angle -= Game1.closeRate;
                if (angle < -MathHelper.Pi + 1.8f * Game1.closeRate)
                    state = BoardState.folding2;
            }
            else
                if (state == BoardState.folding2)
                {
                    one.foldShape(angle);
                    one.Draw();
                    angle += Game1.openRate;
                    if (angle > 0)
                        state = BoardState.chooseEdge1;
                }
        }

        
        public void Draw()
        {
            if ((state == BoardState.folding1) || (state == BoardState.folding2))
            {       
                two.Draw();    
                }
                else
                {

                    effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
                    effect.Parameters["xWorld"].SetValue(worldMatrix);
                    effect.Parameters["xView"].SetValue(camera.View);
                    effect.Parameters["xProjection"].SetValue(camera.Projection);
                    effect.Parameters["xTexture"].SetValue(texture);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        //normal board
                        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0,
                            vertices.Length, indices, 0, indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
                        //invert board
                        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0,
                           vertices.Length, invertIndices, 0, indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);

                        if ((state != BoardState.folding1) && (state != BoardState.folding1) && (oldLineList.Count > 0))
                        {

                            device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.LineList,
                            oldLineList.ToArray(), 0, oldLineList.Count, oldLineindices.ToArray(), 0
                            , oldLineList.Count / 2, VertexPositionNormalTexture.VertexDeclaration);

                        }
                    }
                    effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
                    effect.Parameters["xWorld"].SetValue(worldMatrix);
                    effect.Parameters["xView"].SetValue(camera.View);
                    effect.Parameters["xProjection"].SetValue(camera.Projection);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        //Draw line
                        if ((state == BoardState.chooseEdge2) || (state == BoardState.onEdge2))
                        {
                            device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                            lineList, 0, 2, new short[2] {0,1}, 0
                            , 1, VertexPositionColor.VertexDeclaration);
                        }
                        if ((state == BoardState.onEdge1) || (state == BoardState.onEdge2))
                        {
                            device.DrawUserPrimitives(PrimitiveType.TriangleList, verOnEdge, 0, 2,        VertexPositionColor.VertexDeclaration);
                        }
                    }
                }
        }

        #region Collision
        public bool collideWithEdge(Vector3 point)
        {
            DividingVert minVert = LineIntersectPoint(vertices[0].Position, vertices[1].Position, point);
            minVert.small = 0;
            minVert.big = 1;
            for (int i = 1; i < vertNum; i++)
            {
                DividingVert currentVert = LineIntersectPoint(vertices[i].Position, vertices[(i + 1) % vertNum].Position, point);
                if (currentVert.distance < minVert.distance)
                {
                    minVert = currentVert;
                    minVert.small = i;
                    minVert.big = (i + 1) % vertNum;
                }
            }
            if (minVert.distance < 2f)
            {
                pointOnEdge = minVert.position;
                verOnEdge[0].Position = new Vector3(pointOnEdge.X - 0.3f, -0.1f, pointOnEdge.Z - 0.3f);                
                verOnEdge[1].Position = new Vector3(pointOnEdge.X, -0.1f, pointOnEdge.Z + 0.3f);                
                verOnEdge[2].Position = new Vector3(pointOnEdge.X + 0.3f, -0.1f, pointOnEdge.Z - 0.3f);                
                if ((state == BoardState.chooseEdge1) || (state == BoardState.onEdge1))
                    p[0] = minVert;
                else
                {
                    // both points not on the same edge
                    if ((p[0].small != minVert.small) && (p[0].big != minVert.big))
                        p[1] = minVert;
                    else
                        return false;
                }
                
                return true;
            }
            return false;
        }

        private DividingVert LineIntersectPoint(Vector3 v1, Vector3 v2, Vector3 point)
        {
            Vector2 A = new Vector2(v1.X, v1.Z);
            Vector2 B = new Vector2(v2.X, v2.Z);
            Vector2 p = new Vector2(point.X, point.Z);

            DividingVert edgePoint = new DividingVert();
            //get the normalized line segment vector
            Vector2 v = B - A;
            v.Normalize();

            //determine the point on the line segment nearest to the point p
            float distanceAlongLine = Vector2.Dot(p, v) - Vector2.Dot(A, v);
            Vector2 nearestPoint;
            if (distanceAlongLine < 0)
            {
                //closest point is A
                nearestPoint = A;
            }
            else if (distanceAlongLine > Vector2.Distance(A, B))
            {
                //closest point is B
                nearestPoint = B;
            }
            else
            {
                //closest point is between A and B... A + d  * ( ||B-A|| )
                nearestPoint = A + distanceAlongLine * v;
            }

            //Calculate the distance between the two points
            float actualDistance = Vector2.Distance(nearestPoint, p);
            edgePoint.distance = actualDistance;
            edgePoint.big = edgePoint.small = -1;
            edgePoint.position = new Vector3(nearestPoint.X, 0, nearestPoint.Y);
            return edgePoint;
        }

        //----------------------------------------------------------------
        // GetPickedPosition() - gets 3D position of mouse pointer
        //                     - always on the the Y = 0 plane     
        //----------------------------------------------------------------

        public Vector3 GetPickedPosition(Vector2 mousePosition)
        {

            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 10 is as far away as possible
            Vector3 nearSource = new Vector3(mousePosition, 0f);
            Vector3 farSource = new Vector3(mousePosition, 1f);

            // find the two screen space positions in world space
            Vector3 nearPoint = device.Viewport.Unproject(nearSource, camera.Projection, camera.View, Matrix.Identity);

            Vector3 farPoint = device.Viewport.Unproject(farSource,
                                camera.Projection, camera.View, Matrix.Identity);

            // compute normalized direction vector from nearPoint to farPoint
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // create a ray using nearPoint as the source
            Ray r = new Ray(nearPoint, direction);

            // calculate the ray-plane intersection point
            Vector3 n = new Vector3(0f, 1f, 0f);
            Plane p = new Plane(n, 0f);

            // calculate distance of intersection point from r.origin
            float denominator = Vector3.Dot(p.Normal, r.Direction);
            float numerator = Vector3.Dot(p.Normal, r.Position) + p.D;
            float t = -(numerator / denominator);

            // calculate the picked position on the y = 0 plane
            Vector3 pickedPosition = nearPoint + direction * t;


            return pickedPosition;
        }


        //// Test if point P lies inside the counterclockwise triangle ABC
        //public bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        //{
        //    // Translate point and triangle so that point lies at origin
        //    a -= p; b -= p; c -= p;
        //    // Compute normal vectors for triangles pab and pbc
        //    Vector2 u = Vector2.Cross(b, c);
        //    Vector2 v = Vector2.Cross(c, a);
        //    // Make sure they are both pointing in the same direction
        //    if (Vector2.Dot(u, v) < 0.0f)
        //        return false;
        //    // Compute normal vector for triangle pca
        //    Vector2 w = Vector2.Cross(a, b);
        //    // Make sure it points in the same direction as the first two
        //    if (Vector2.Dot(u, w) < 0.0f)
        //        return false;
        //    // Otherwise P must be in (or on) the triangle
        //    return true;
        //}

        // Compute the 2D pseudo cross product Dot(Perp(u), v)
        private float Cross2D(Vector2 u, Vector2 v)
        {
            return ((u.Y) * (v.X)) - ((u.X) * (v.Y));
        }

        // Test if 2D point P lies inside the counterclockwise 2D triangle ABC
        public bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            // If P to the right of AB then outside triangle
            if (Cross2D(p - a, b - a) < 0.0f) return false;
            // If P to the right of BC then outside triangle
            if (Cross2D(p - b, c - b) < 0.0f) return false;
            // If P to the right of CA then outside triangle
            if (Cross2D(p - c, a - c) < 0.0f) return false;
            // Otherwise P must be in (or on) the triangle
            return true;
        }
        
        public bool PointInBeforeFold(Vector3 p)
        {            
            for (int i = 0; i < one.indices.Length ; i+=3)
            {
                Vector2 a = new Vector2(one.vertices[one.indices[i+2]].Position.X,
                    one.vertices[one.indices[i+2]].Position.Z);
                Vector2 b = new Vector2(one.vertices[one.indices[i + 1]].Position.X,
                    one.vertices[one.indices[i + 1]].Position.Z);
                Vector2 c = new Vector2(one.vertices[one.indices[i]].Position.X, 
                    one.vertices[one.indices[i]].Position.Z);
                Vector2 point = new Vector2(p.X, p.Z);
                if (PointInTriangle(point, a,b,c))
                    return true;
            }
            return false;
        }

        public bool PointInAfterFold(Vector3 p)
        {
            Vector3[] folded = new Vector3[one.vertNum];
            Vector3 axis = one.vertices[0].Position - one.vertices[one.vertNum - 1].Position;            
            Matrix foldMatrix = Matrix.Identity;

            axis.Normalize();
            foldMatrix *= Matrix.CreateTranslation(-one.vertices[0].Position);
            foldMatrix *= Matrix.CreateFromAxisAngle(axis, MathHelper.Pi);
            foldMatrix *= Matrix.CreateTranslation(one.vertices[0].Position);
            for (int i = 0; i < one.vertNum; i++)
                folded[i] = Vector3.Transform(one.vertices[i].Position, foldMatrix);                        
            
            for (int i = 0; i < one.indices.Length; i += 3)
            {
                Vector2 a = new Vector2(folded[one.indices[i]].X ,folded[one.indices[i]].Z);
                Vector2 b = new Vector2(folded[one.indices[i + 1]].X, folded[one.indices[i + 1]].Z);
                Vector2 c = new Vector2(folded[one.indices[i+2]].X, folded[one.indices[i+2]].Z);
                Vector2 point = new Vector2(p.X, p.Z);
                if (PointInTriangle(point, a, b, c))
                    return true;
            }
            return false;
        }

        public bool PointInBoard(Vector3 p)
        {
            for (int i = 0; i < two.indices.Length; i += 3)
            {
                Vector2 a = new Vector2(two.vertices[two.indices[i + 2]].Position.X,
                    two.vertices[two.indices[i + 2]].Position.Z);
                Vector2 b = new Vector2(two.vertices[two.indices[i + 1]].Position.X,
                    two.vertices[two.indices[i + 1]].Position.Z);
                Vector2 c = new Vector2(two.vertices[two.indices[i]].Position.X,
                    two.vertices[two.indices[i]].Position.Z);
                Vector2 point = new Vector2(p.X, p.Z);
                if (PointInTriangle(point, a, b, c))
                    return true;
            }
            return false;
        }

        #endregion

        #region Divide Shape
        public void Divide(DividingVert first, DividingVert second, out Board partOne, out Board partTwo)
        {
            //
            /* ADD TEST TO FIND IF NOT ON THE SAME EDGE*/
            //
            Board part1 = new Board(texture,effect);
            Board part2 = new Board(texture, effect);
            int p1_pNum, p2_pNum;
            p1_pNum = Math.Abs(first.big - second.small) + 3;
            if ((first.big == vertNum - 1) && (second.small == 0))
                p1_pNum -= 2;
            p2_pNum = indices.Length - p1_pNum + 2;
            Vector3[] p1_points = new Vector3[p1_pNum];
            Vector2[] p1_texCords = new Vector2[p1_pNum];
            p1_points[0] = first.position;
            p1_texCords[0] = findTexCords(first);
            p1_points[p1_pNum - 1] = second.position;
            p1_texCords[p1_pNum - 1] = findTexCords(second);
            for (int i = 0; i < p1_pNum - 2; i++)
            {
                p1_points[i + 1] = vertices[(first.big + i) % vertNum].Position;
                p1_texCords[i + 1] = vertices[(first.big + i) % vertNum].TextureCoordinate;
            }
            Vector3[] p2_points = new Vector3[p2_pNum];
            Vector2[] p2_texCords = new Vector2[p2_pNum];
            p2_points[0] = second.position;
            p2_texCords[0] = p1_texCords[p1_pNum - 1]; //alreadt calculated before a moment
            p2_points[p2_pNum - 1] = first.position;
            p2_texCords[p2_pNum - 1] = p1_texCords[0]; //alreadt calculated before a moment
            for (int i = 0; i < p2_pNum - 2; i++)
            {
                p2_points[i + 1] = vertices[(second.big + i) % vertNum].Position;
                p2_texCords[i + 1] = vertices[(second.big + i) % vertNum].TextureCoordinate;
            }
            part1.Initialize(p1_pNum, p1_points, p1_texCords);
            part2.Initialize(p2_pNum, p2_points, p2_texCords);
            // storing the smaller part in parOne , bigger -> partTwo
            if (Vector3.Distance(center,part1.center) >= Vector3.Distance(center,part2.center))
            {
                partOne = part1;
                partTwo = part2;
            } else
            {
                partOne = part2;
                partTwo = part1;
            }
        }

        private Vector2 findTexCords(DividingVert divVert)
        {
            float smallX = vertices[divVert.small].Position.X;
            float smallCordX = vertices[divVert.small].TextureCoordinate.X;
            float bigX = vertices[divVert.big].Position.X;
            float bigCordX = vertices[divVert.big].TextureCoordinate.X;
            float vertX = divVert.position.X;

            float smallZ = vertices[divVert.small].Position.Z;
            float smallCordZ = vertices[divVert.small].TextureCoordinate.Y;
            float bigZ = vertices[divVert.big].Position.Z;
            float bigCordZ = vertices[divVert.big].TextureCoordinate.Y;
            float vertZ = divVert.position.Z;

            float xSize = Math.Abs(bigX - smallX);
            float zSize = Math.Abs(bigZ - smallZ);
            //the point from wich you maser the distance
            float relativeX = (bigCordX < smallCordX ? bigX : smallX);
            float relativeZ = (bigCordZ < smallCordZ ? bigZ : smallZ);

            float xRel = (xSize != 0 ? Math.Abs(relativeX - vertX) / xSize : smallX);
            float zRel = (zSize != 0 ? Math.Abs(relativeZ - vertZ) / zSize : smallZ);
            float xCsize = Math.Abs(bigCordX - smallCordX);
            float zCsize = Math.Abs(bigCordZ - smallCordZ);
            float xCrel = (xCsize != 0 ? xRel * xCsize : smallCordX);
            float zCrel = (zCsize != 0 ? zRel * zCsize : smallCordZ);
            return new Vector2(xCrel, zCrel);
        }

        #endregion

        #region Folding
        public void foldShape(float angle)
        {
            Vector3 axis = vertices[0].Position - vertices[vertNum - 1].Position;
            axis.Normalize();

            worldMatrix = Matrix.Identity;
            worldMatrix *= Matrix.CreateTranslation(-vertices[0].Position);
            worldMatrix *= Matrix.CreateFromAxisAngle(axis, angle);
            worldMatrix *= Matrix.CreateTranslation(vertices[0].Position);            
        }

        #endregion
        public BoardState update()
        {
            Vector3 mouse = GetPickedPosition(
                new Vector2((float)input.MouseHandler.MouseState.X, (float)input.MouseHandler.MouseState.Y));            
            bool onEdge = collideWithEdge(mouse);
            lineList[1].Position = mouse;
            lineList[0].Color = Color.OrangeRed;
            lineList[1].Color = Color.OrangeRed;
            if ((state == BoardState.chooseEdge1) && (onEdge))
            {               
                    if (input.MouseHandler.WasLeftButtonClicked())
                        state = BoardState.chooseEdge2;
                    else
                        state = BoardState.onEdge1;                                    
            } else
            if ((state == BoardState.onEdge1))
            {
                if (onEdge)                
                {
                    if (input.MouseHandler.WasLeftButtonClicked())
                    {                        
                        lineList[0].Position = p[0].position;       
                        state = BoardState.chooseEdge2;
                    }
                } else
                    state = BoardState.chooseEdge1;
            } else
            if ((state == BoardState.chooseEdge2) && (onEdge))
            {
                lineList[1].Position = mouse;
                if (input.MouseHandler.WasLeftButtonClicked())
                    state = BoardState.preFold;                
                else                                          
                    state = BoardState.onEdge2;               
            } else
            if ((state == BoardState.onEdge2))
            {
                lineList[1].Position = p[1].position;
                lineList[0].Color = Color.Green;
                lineList[1].Color = Color.Green;
                if (onEdge)
                    {
                        if (input.MouseHandler.WasLeftButtonClicked())
                            state = BoardState.preFold;
                    } else
                        state = BoardState.chooseEdge2;
            } else
            if (state == BoardState.preFold)
            {
                Divide(p[0], p[1], out one, out two);
                //Tom -- array of all the old fold lines                
                oldLineindices.Add((short)oldLineindices.Count);
                oldLineList.Add(new VertexPositionNormalTexture(p[0].position, new Vector3(0,0,0), new Vector2(0f, 0.5f)));
                oldLineindices.Add((short)oldLineindices.Count);
                oldLineList.Add(new VertexPositionNormalTexture(p[1].position,new Vector3(0,0,0), new Vector2(1f, 0.5f))); 

                state = BoardState.folding1;
            }
            if ((input.MouseHandler.WasRightButtonClicked()))
            {
                state = BoardState.chooseEdge1;
                angle = 0;                
            }

            return state;

        }
    }
}

