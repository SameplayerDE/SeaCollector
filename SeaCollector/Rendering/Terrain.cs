using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public class Terrain
    {
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private GraphicsDevice _graphicsDevice;
        private int _height;
        private int _width;

        public Vector3 Position;
        
        private VertexPositionNormalColorTexture[] _vertices;

        public void Generate(GraphicsDevice graphicsDevice, int width, int height)
        {
            _graphicsDevice = graphicsDevice;
            _height = height;
            _width = width;
            
            const float max = 2;
            const float min = 0;

            _vertices = new VertexPositionNormalColorTexture[width * height];
            var indices = new short[(width - 1) * (height) * 6];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var index = j + width * i;

                    //var vertex0 = new VertexPositionColor(new Vector3(x + 0, y + 0, z + 0), color);
                    _vertices[index].Position = new Vector3(j, 0, i);
                    _vertices[index].Color = Color.Bisque;
                    //vertices[index].TextureCoordinate.X = j;
                    //vertices[index].TextureCoordinate.Y = -i;
                }
            }

            var counter = 0;
            for (var i = 0; i < height - 1; i++)
            {
                for (var j = 0; j < width - 1; j++)
                {
                    var lowerLeft = j + i * width;
                    var lowerRight = (j + 1) + i * width;
                    var topLeft = j + (i + 1) * width;
                    var topRight = (j + 1) + (i + 1) * width;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)lowerLeft;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)lowerRight;
                }
            }

            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalColorTexture.VertexDeclaration,
                _vertices.Length,
                BufferUsage.WriteOnly);
            _vertexBuffer.SetData(_vertices);

            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length,
                BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);

        }

        public void SetHeight(float[] values)
        {
            
            
            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    var index = j + _width * i;
                    var value = values[index];
                    //var vertex0 = new VertexPositionColor(new Vector3(x + 0, y + 0, z + 0), color);
                    _vertices[index].Position = new Vector3(j, value, i);
                    _vertices[index].Color = Color.Bisque;
                    //vertices[index].TextureCoordinate.X = j;
                    //vertices[index].TextureCoordinate.Y = -i;
                }
            }
            
            _vertexBuffer.SetData(_vertices);
            
        }

        public void Draw(Effect effect, Matrix world, Matrix view, Matrix proj)
        {
            
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            
            var l_world = world;
            var l_view = view;
            var l_proj = proj;

            var n_scale = Matrix.CreateScale(1f);

            var n_rotation =
                Matrix.CreateRotationX(0) *
                Matrix.CreateRotationY(0) *
                Matrix.CreateRotationZ(0);

            var n_translation = Matrix.CreateTranslation(Position);

            l_world *= n_scale * n_rotation * n_translation;
            
            effect.Parameters["World"]?.SetValue(l_world);
            effect.Parameters["View"]?.SetValue(l_view);
            effect.Parameters["Projection"]?.SetValue(l_proj);
            
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indexBuffer.IndexCount / 3);
            }
        }
        
    }
}