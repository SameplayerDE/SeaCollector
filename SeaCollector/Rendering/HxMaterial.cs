using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeaCollector.Rendering
{
    public class HxMaterial
    {
        public string BaseUri;
        public string NormalUri;
        public string EmissiveUri;
        public string AlphaMode;
        public float AlphaCutoff;
        public float RoughnessFactor;
        public float MetallicFactor;
        public bool DoubleSided = false;
        public Color BaseColorFactor = Color.White;
        public Color EmissiveFactor = Color.White;
        public List<string> Extensions = new List<string>();
        public SamplerState SamplerState;
    }
}