using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using CGUNS.Meshes.FaceVertexList;
using OpenTK;

namespace CGUNS.Parsers
{
    public class ObjFileParser
    {
        private const String VERTEX = "v";
        private const String FACE = "f";
        private const String NORMAL = "vn";
        private const String OBJ = "o";
        private const String OBJG = "g";

        private const String TEXCORD = "vt";
        private const String MTL = "usemtl";
        private const Char COMMENT = '#';
        private static Char[] SEPARATORS = { ' ', '/' };
        private static bool logEnabled = false;
        
        internal static void setLists(MeshObject mesh)
        {
            mesh.setVertexList(vertexList);
            mesh.setNormalList(vertexNormalList);
            mesh.setTextCoodList(texCordList);
        }

        internal static List<FVLMesh> parseFileSinMTL(string fileName)
        {
            List<FVLMesh> ListMesh = new List<FVLMesh>();

            vertexList = new List<Vector3>();
            vertexNormalList = new List<Vector3>();
            texCordList = new List<Vector2>();
            String line;
            String[] lineSplit;
            String sender = "ObjFileParser.parseFile: ";
            info(sender, "Opening file: {0}", fileName);
            StreamReader file = new StreamReader(fileName);
            info(sender, "OK. Reading all the lines of the file...");
            bool primeravez = true;
            line = file.ReadLine();
            FVLMesh mesh = new FVLMesh();
            String material = "";
            while (line != null)
            {
                line = line.Trim(); //Saco espacios en blanco.
                if ((line.Length != 0) && (!line[0].Equals(COMMENT))) //Si no es comentario
                {

                    lineSplit = line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                    if (lineSplit[0].Equals(OBJ) || lineSplit[0].Equals(OBJG))
                    {
                        // vertexCount += (mesh.VertexCount);
                        // normalCount += mesh.VertexNormalList.ToArray().Length;
                        // textCount += mesh.TexCordList.Count;
                        
                                mesh = new FVLMesh();
                                ListMesh.Add(mesh);
                                mesh.setPath(fileName);
                                //material = parseMtl(mesh, lineSplit);
                         
                    }
                    else if (lineSplit[0].Equals(VERTEX))
                    {
                        if (ListMesh.ToArray().Length == 0)
                        {
                            //mesh = new FVLMesh();

                            ListMesh.Add(mesh);
                        }
                        parseVertex(mesh, lineSplit);
                    }
                    else if (lineSplit[0].Equals(NORMAL))
                    {
                        parseNormal(mesh, lineSplit);
                    }

                    else if (lineSplit[0].Equals(FACE))
                    {
                        parseFace(mesh, line);//HERE!!
                    }

                    else if (lineSplit[0].Equals(TEXCORD))
                    {
                        parseTexCord(mesh, lineSplit);
                    }
                    else if (lineSplit[0].Equals(MTL))
                    {
                        //parseMtl(mesh, lineSplit);
                    }


                    else
                    {
                        log(sender, "Not supported instruction: {0}", lineSplit[0]);
                    }
                }
                line = file.ReadLine();
            }
            //Console.WriteLine("Parser Lenght " + ListMesh.ToArray().Length);
            file.Close();
            info(sender, "FINISHED!");



            return ListMesh;
        }

        private static List<Vector3> vertexList;
        private static List<Vector2> texCordList;
        private static List<Vector3> vertexNormalList;


        public static List<FVLMesh> parseFile(String fileName)
        {
            List<FVLMesh> ListMesh = new List<FVLMesh>();

            vertexList = new List<Vector3>();
            vertexNormalList = new List<Vector3>();
            texCordList = new List<Vector2>();
            String line;
            String[] lineSplit;
            String sender = "ObjFileParser.parseFile: ";
            info(sender, "Opening file: {0}", fileName);
            StreamReader file = new StreamReader(fileName);
            info(sender, "OK. Reading all the lines of the file...");
            bool primeravez = true;
            line = file.ReadLine(); 
            FVLMesh mesh = new FVLMesh();
            String material = "";
            while (line != null)
            {
                line = line.Trim(); //Saco espacios en blanco.
                if ((line.Length != 0) && (!line[0].Equals(COMMENT))) //Si no es comentario
                {

                    lineSplit = line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                    if ( lineSplit[0].Equals(MTL))
                    {
                       // vertexCount += (mesh.VertexCount);
                       // normalCount += mesh.VertexNormalList.ToArray().Length;
                       // textCount += mesh.TexCordList.Count;
                       //if(lineSplit[0].Equals(MTL))
                           /* if (primeravez)
                            {//el mesh ya existe y era un "g" o un "o"
                                primeravez = false;
                                material = parseMtl(mesh, lineSplit);
                            }
                            else/**/
                            {//es un usemtl

                                mesh = new FVLMesh();
                                ListMesh.Add(mesh);
                                mesh.setPath(fileName);
                                material = parseMtl(mesh, lineSplit);
                            }
                        /*else
                        {
                            primeravez = true;
                            mesh = new FVLMesh();
                            ListMesh.Add(mesh);
                            mesh.setPath(fileName);
                            mesh.setName(material);
                        }/**/

                        //excepciones
                        if (material.Equals("Body_paint4"))
                        {
                            mesh.setTexturePath("files/dragonfuego.jpg");
                            mesh.ignoreTextCoord();
                        }

                    }
                    else if (lineSplit[0].Equals(VERTEX))
                    {
                        if (ListMesh.ToArray().Length == 0)
                        {
                            //mesh = new FVLMesh();

                            ListMesh.Add(mesh);
                        }
                        parseVertex(mesh, lineSplit);
                    }
                    else if (lineSplit[0].Equals(NORMAL))
                    {
                        parseNormal(mesh, lineSplit);
                    }

                    else if (lineSplit[0].Equals(FACE))
                    {
                        parseFace(mesh, line);//HERE!!
                    }

                    else if (lineSplit[0].Equals(TEXCORD))
                    {
                        parseTexCord(mesh, lineSplit);
                    }
                    else if (lineSplit[0].Equals(MTL))
                    {
                        //parseMtl(mesh, lineSplit);
                    }


                    else
                    {
                        log(sender, "Not supported instruction: {0}", lineSplit[0]);
                    }
                }
                line = file.ReadLine();
            }
            //Console.WriteLine("Parser Lenght " + ListMesh.ToArray().Length);
            file.Close();
            info(sender, "FINISHED!");



            return ListMesh;
        }

        public static void parseVertex(FVLMesh mesh, String[] args)
        {
            String sender = "ObjFileParser.parseVertex: ";
            Vector3 vertex = new Vector3();
            switch (args.Length)
            {
                case 2:
                    log(sender, "Vertex definition must be (x,y,z [,w]). Found only x.");
                    vertex.X = float.Parse(args[1], System.Globalization.NumberStyles.Number);
                    vertex.Y = 0.0f;
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 3:
                    log(sender, "Vertex definition must be (x,y,z [,w]). Found only x, y.");
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
                    log(sender, "Found (x, y, z, w). Discarding w component.");
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    //vertex.W = float.Parse(args[4], System.Globalization.NumberStyles.Number);
                    break;
                default:
                    break;
            }
            //mesh.AddVertex(vertex);
            vertexList.Add(vertex);
            //vertexCount++;
        }

        public static void parseNormal(FVLMesh mesh, String[] args)
        {
            Vector3 vertex = new Vector3();
            vertex.X = float.Parse(args[1], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            vertex.Y = float.Parse(args[2], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            vertex.Z = float.Parse(args[3], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            //mesh.AddVertexNormal(vertex);
            vertexNormalList.Add(vertex);

            //Console.WriteLine(vertex);
        }

        public static void parseTexCord(FVLMesh mesh, String[] args)
        {
            Vector2 texCord = new Vector2();
            texCord.X = float.Parse(args[1], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            texCord.Y = float.Parse(args[2], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            //mesh.AddTexCord(texCord);
            texCordList.Add(texCord);

        }

        public static String parseMtl(FVLMesh mesh, String[] args)
        {
            String name;
            name = args[1];
            
            mesh.setName(name);

            return name;
        }

        public static void parseFace(FVLMesh mesh, string line)
        {
            FVLFace face = new FVLFace();

            int i = 2; // componente 1 = f , comp 2 = ' '
            String vertex;
            String texCord;
            String normal;

            while (i < line.Length)
            {
                vertex = "";
                texCord = "";
                normal = "";
                while (i < line.Length && line[i] != ' ' && line[i] != '/')
                {
                    vertex = vertex + line[i];
                    i++;
                }

                if (i < line.Length && line[i] != ' ')
                {
                    i++;
                    if (line[i] != '/')
                    {
                        while (i < line.Length && line[i] != ' ' && line[i] != '/')
                        {
                            texCord = texCord + line[i];
                            i++;
                        }
                    }
                    i++;

                    if (i < line.Length && line[i] != '/')
                    {
                        while (i < line.Length && line[i] != ' ' && line[i] != '/')
                        {
                            normal = normal + line[i];
                            i++;
                        }
                    }
                }
                if (vertex == "")
                {
                    Console.WriteLine(line);
                }
                i++;
                face.AddVertex((int)(Int32.Parse(vertex, NumberStyles.Integer)-1 ));
                if (!normal.Equals(""))
                {
                    face.AddNormal((int)(Int32.Parse(normal, NumberStyles.Integer) -1 ));
                    // Console.WriteLine(normal);
                }
                if (!texCord.Equals(""))
                {
                    face.AddTexCord((int)(Int32.Parse(texCord, NumberStyles.Integer)-1 ));
                }
            }

            mesh.AddFace(face);
        }


        private static void log(String sender, String format, params Object[] args)
        {
            if (logEnabled)
            {
                System.Console.WriteLine(sender + format, args);
            }
        }
        private static void info(String sender, String format, params Object[] args)
        {
            System.Console.WriteLine(sender + format, args);
        }

    }
}
