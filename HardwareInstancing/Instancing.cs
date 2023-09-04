using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace HardwareInstancing
{
    public class Instancing
    {
        Texture2D texture;
        Effect effect;

        VertexDeclaration instanceVertexDeclaration;

        VertexBuffer instanceBuffer;
        VertexBuffer geometryBuffer;
        IndexBuffer indexBuffer;

        VertexBufferBinding[] bindings;
        InstanceInfo[] instances;

        private int _index = 0;

        struct InstanceInfo
        {
            public Vector4 World;
            public Vector2 AtlasCoordinate;
        };

        Int32 instanceCount = 1_000_000;

        public void Initialize(GraphicsDevice device)
        {
            GenerateInstanceVertexDeclaration();
            GenerateGeometry(device);
            GenerateInstanceInformation(device, instanceCount);

            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(geometryBuffer);
            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
        }

        public void Load(ContentManager Content)
        {
            effect = Content.Load<Effect>("InstancingShader");
            texture = Content.Load<Texture2D>("default_256");
        }

        private void GenerateInstanceVertexDeclaration()
        {
            VertexElement[] instanceStreamElements = new VertexElement[2];

            instanceStreamElements[0] =
                new VertexElement(0, VertexElementFormat.Vector4,
                    VertexElementUsage.Position, 1);

            instanceStreamElements[1] =
                new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate, 1);

            instanceVertexDeclaration = new VertexDeclaration(instanceStreamElements);
        }

        private void GenerateBackwards(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(1, 1));
    }
    
    private void GenerateForwards(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(1, 1));
    }
    
    private void GenerateRight(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1));
    }
    
    private void GenerateLeft(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(1, 1));
    }
    
    private void GenerateUp(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 0)); //0
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)); //1
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, 1)); //2

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, 1)); //2
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)); //1
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 1)); //3
    }
    
    private void GenerateDown(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1));
    }

        //This creates a cube!
        public void GenerateGeometry(GraphicsDevice device)
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[36];

            GenerateUp(ref vertices);
            
            GenerateBackwards(ref vertices);
            GenerateForwards(ref vertices);
            GenerateRight(ref vertices);
            GenerateDown(ref vertices);
            GenerateLeft(ref vertices);


            geometryBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration,
                36, BufferUsage.WriteOnly);
            geometryBuffer.SetData(vertices);

            var indices = new short [36];
            for (short i = 0; i < 36; i++)
            {
                indices[i] = i;
            }

            indexBuffer = new IndexBuffer(device, typeof(short), 36, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        private void GenerateInstanceInformation(GraphicsDevice device, Int32 count)
        {
            instances = new InstanceInfo[count];
            Random rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                //random position example
                instances[i].World = new Vector4(rnd.Next(-500, 500),
                    rnd.Next(-500, 500),
                    rnd.Next(-500, 500), 1);

                instances[i].AtlasCoordinate = new Vector2(rnd.Next(0, 2), rnd.Next(0, 2));
            }

            instanceBuffer = new VertexBuffer(device, instanceVertexDeclaration,
                count, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);
        }

        //view and projection should come from your camera
        public void Draw(ref Matrix view, ref Matrix projection, GraphicsDevice device)
        {
            device.Clear(Color.Black);

            effect.CurrentTechnique = effect.Techniques["Instancing"];
            effect.Parameters["WVP"].SetValue(view * projection);
            effect.Parameters["cubeTexture"].SetValue(texture);

            device.Indices = indexBuffer;

            effect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffers(bindings);
            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 12, instanceCount);
        }
    }
}