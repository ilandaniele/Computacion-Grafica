using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using CGUNS.Meshes.FaceVertexList;
using OpenTK;

namespace CGUNS.Parsers
{
    public class MtlFileParser
    {
        private const String MTL = "NEWMTL";
        private const String MAP_KD = "MAP_KD";
        private const String KD = "KD";
        private const String KS = "KS";
        private const String MAP_NORMAL = "MAP_NORMAL"; 

        private const Char COMMENT = '#';
        private static Char[] SEPARATORS = { ' ' };

        public static void parseFile(List<FVLMesh> objeto, String fileName)
        {
            String line;
            String[] lineSplit;
            StreamReader file = new StreamReader(fileName);
            int select = -1;
            line = file.ReadLine(); 
            FVLMesh mesh = new FVLMesh();
            Map mapeo = new Map();

            String texturePath = "";

            Char[] Sep = { '/' };
            String[] split = fileName.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split.Length - 1; i++)
            {
                texturePath += split[i] + "/";
            }
            String name = "";
            while (line != null)
            {
                line = line.Trim(); //Saco espacios en blanco.
                if ((line.Length != 0) && (!line[0].Equals(COMMENT))) //Si no es comentario
                {

                    lineSplit = line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                    if (lineSplit[0].ToUpper().Equals(MTL))
                    {
                        //select = parseMTL(objeto,lineSplit[1]);
                        name = lineSplit[1];

                    }

                    if (lineSplit[0].ToUpper().Equals(KD))
                    {
                        if (lineSplit.Length > 1)
                            parseKD(objeto, lineSplit, name);

                    }

                    if (lineSplit[0].ToUpper().Equals(KS))
                    {
                        if (lineSplit.Length > 1)
                            parseKS(objeto, lineSplit, name);

                    }

                    if (lineSplit[0].ToUpper().Equals(MAP_KD))
                    {
                        if( lineSplit.Length > 1 && !lineSplit[1].Equals(""))
                        {
                            parseMAP(objeto, lineSplit, texturePath,name);
                            
                            
                        }

                    }

                    if (lineSplit[0].ToUpper().Equals(MAP_NORMAL))
                    {
                        if (lineSplit.Length > 1)
                            parseMap_Normal(objeto, lineSplit, texturePath, name);

                    }

                }
                line = file.ReadLine();
            }
            //Console.WriteLine("Parser Lenght " + ListMesh.ToArray().Length);
            file.Close();

            
        }

        private static void parseMap_Normal(List<FVLMesh> mesh, String[] texture, String texturePath, String name)
        {
            String aux = "";
            int i;
            for (i = 1; i < texture.Length - 1; i++)
            {
                aux += texture[i] + " ";
            }
            aux += texture[i];
            for (i = 0; i < mesh.Count; i++)
            {
                if (name.Equals(mesh[i].getName()))
                {
                    mesh[i].setNormalPath(texturePath + aux);
                }
            }
        }

        private static void parseKD(List<FVLMesh> mesh, string[] args, String name)
        {
            Vector4 vertex = new Vector4();
            vertex.W = 1;

            switch (args.Length)
            {
                case 2:
                    vertex.X = float.Parse(args[1], System.Globalization.NumberStyles.Number);
                    vertex.Y = 0.0f;
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 3:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 4:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    //vertex.W = 1.0f;
                    break;
                case 5:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    vertex.W = float.Parse(args[4], System.Globalization.NumberStyles.Number);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < mesh.Count; i++)
            {
                if (name.Equals(mesh[i].getName()))
                {
                    mesh[i].setKD(vertex); ;
                }
            }

        }

        private static void parseKS(List<FVLMesh> mesh, string[] args, String name)
        {
            Vector4 vertex = new Vector4();
            vertex.W = 1;

            switch (args.Length)
            {
                case 2:
                    vertex.X = float.Parse(args[1], System.Globalization.NumberStyles.Number);
                    vertex.Y = 0.0f;
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 3:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 4:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    //vertex.W = 1.0f;
                    break;
                case 5:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    vertex.W = float.Parse(args[4], System.Globalization.NumberStyles.Number);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < mesh.Count; i++)
            {
                if (name.Equals(mesh[i].getName()))
                {
                    mesh[i].setKS(vertex); ;
                }
            }

        }

        private static void parseMAP(List<FVLMesh> mesh, String[] texture ,String texturePath, String name)
        {
            String aux = "";
            int i;
            for (i = 1; i<texture.Length-1; i++)
            {
                aux += texture[i] + " ";
            }
            aux += texture[i];
            for (i = 0; i < mesh.Count; i++)
            {
                if (name.Equals(mesh[i].getName()))
                {
                    mesh[i].setTexturePath(texturePath + aux);
                }
            }
            
        }

        private static int parseMTL(List<FVLMesh> objeto, string name)
        {
            bool encontre = false;
            int i = 0;
            while (!encontre && i<objeto.Count)
            {
                if (name.Equals(objeto[i].getName()) && objeto[i].getName()!=null)
                    encontre = true;
                else
                    i++;
            }
            return i;
        }
        
    }
}
