using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeaCollector.HxObj
{
    public class MtlLoader
    {
        public static MtlFile Load(string path)
        {
            if (Directory.Exists(path))
            {
                throw new Exception("passed directory path");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("file could not be found");
            }

            var extension = Path.GetExtension(path);
            if (!extension.Equals(".mtl"))
            {
                throw new FileLoadException("file could not be loaded, wrong file type");
            }

            return LoadFromMtlFile(path);
        }

        private static MtlFile LoadFromMtlFile(string path)
        { 
            var result = new MtlFile();
            result.Materials = new Dictionary<string, MaterialData>();
            
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var split = line.Replace(".", ",").Split(" ");
                var type = split[0];

                if (type.Equals("newmtl"))
                {
                    result.Materials[split[1].Replace(",", ".")] = new MaterialData();
                }

                if (type.Equals("Ka"))
                {
                    var array = Array.ConvertAll(split[1..], float.Parse);
                    result.Materials.Last().Value.Ambient = array;
                }
                
                if (type.Equals("Kd"))
                {
                    var array = Array.ConvertAll(split[1..], float.Parse);
                    result.Materials.Last().Value.Diffuse = array;
                }
                
                if (type.Equals("Ks"))
                {
                    var array = Array.ConvertAll(split[1..], float.Parse);
                    result.Materials.Last().Value.Specular = array;
                }
                
                if (type.Equals("map_Kd"))
                {
                    result.Materials.Last().Value.DiffuseMap = split[1].Replace(",", ".");
                }
                
            }
            return result;
        }
    }
}