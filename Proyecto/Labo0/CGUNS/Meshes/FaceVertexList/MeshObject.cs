using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK; //La matematica
using OpenTK.Graphics.OpenGL;
using gl = OpenTK.Graphics.OpenGL.GL;
using CGUNS.Shaders;
using CGUNS.Cameras;
using CGUNS.Meshes;
using CGUNS.Parsers;
using System.Drawing.Imaging;
using BulletSharp;

namespace CGUNS.Meshes.FaceVertexList
{

    public class MeshObject
    {
        /// <summary>
        /// Clase genérica para la implementación de los objetos.
        /// </summary>
        /// <param name="file"> ubicacion y nombre del archivo obj del objeto</param>
        /// <param name="mtl"> ubicacion y nombre del archivo mtl del objeto</param>
        /// <param name="posicionInicial"> posicion inicial en la escena del objeto </param>
        /// <param name="texturado"></param>
        public MeshObject(String file, String mtl, Vector3 posicionInicial, bool texturado)
        {
            if(mtl != null)
                objeto = CGUNS.Parsers.ObjFileParser.parseFile(file);
            else
                objeto = CGUNS.Parsers.ObjFileParser.parseFileSinMTL(file);

            position = posicionInicial;
            tieneTextura = texturado;
            CGUNS.Parsers.ObjFileParser.setLists(this);
            if (mtl != null)
                CGUNS.Parsers.MtlFileParser.parseFile(objeto, mtl);
        }

        private List<Vector3> vertexList;

        //matrices
        protected Matrix4 modelMat;
        protected Matrix4 modelMatFisica;
        protected Matrix4 viewMatrix;


        //vectores
        protected Vector3 position;


        protected bool tieneTextura;

        protected List<CGUNS.Meshes.FaceVertexList.FVLMesh> objeto;

        private Map mapeo = new Map();

        protected float radio = 0.1f;
        /// <summary>
        /// Funcion que se encarga de inicializar los objetos
        /// </summary>
        /// <param name="sProgram"> shader program</param>
        /// <param name="sProgramN"> shader program</param>
        /// <param name="repeat"></param>
        public void Build(ShaderProgram sProgram, ShaderProgram sProgramN, int repeat)
        {

            for (int i = 0; i < objeto.Count; i++)
            {
                FVLMesh mesh = objeto[i];
                mesh.Build(sProgram, sProgramN, repeat);

                //Console.WriteLine("BUILD: " + mesh.getName() + " texture: " + mesh.getTexturePath());
                if (mesh.getName() != null)
                {
                    int text = mapeo.find(mesh.getName());

                    if (text == -1)
                    {
                        if (mesh.getTexturePath() != null)
                        {
                            text = CargarTextura(mesh.getTexturePath());
                            mapeo.setClave(text, mesh.getTexturePath(), mesh.getName());
                        }
                    }

                    mesh.setTexture(text);

                    if (mesh.getNormalPath() != null)
                    {
                        text = CargarTextura(mesh.getNormalPath());
                        mesh.setNormalIndex(text);
                    }


                }
                    //Console.WriteLine("objeto rodante:"+i+" "+ mesh.getName());

            }
            Console.WriteLine("finalizando: cantidad de texturas cargadas:" + mapeo.Count());

        }

        

        /// <summary>
        /// Funcion utilizada para setear la posicion inicial del objeto
        /// </summary>
        /// <param name="pos"> vector con la posicion del objeto</param>
        public void setPosition(Vector3 pos)
        {
            position = pos;
        }
        /// <summary>
        /// funcion que nos sirve para indicar el idice de textura a utilizar
        /// </summary>
        /// <param name="text"> indice de textura a utilizar</param>
        public void setTextura(int text)
        {
            for (int i = 0; i < objeto.Count; i++)
            {
                objeto[i].setTexture(text);
            }
            tieneTextura = true;


        }

        /// <summary>
        /// funcion que nos sirve para poder setear la view matrix
        /// </summary>
        /// <param name="v"> View Matrix</param>
        public void setViewMatrix(Matrix4 v)
        {
            viewMatrix = v;
        }
        /// <summary>
        /// Funcion que setea la matriz de modelado fisica
        /// </summary>
        /// <param name="mod"> Matriz de modelado a setear</param>
        public virtual void setModelMatFisica(Matrix4 mod)
        {
            modelMatFisica = mod;
        }
        /// <summary>
        /// Funcion generica encargada de cargar las texturas para los objetos
        /// </summary>
        /// <param name="imagenTex"> ruta del archivo de textura a cargar</param>
        /// <returns></returns>
        private int CargarTextura(String imagenTex)
        {
            int texId = GL.GenTexture();//genera identificador para el buffer de textura
            try
            {
                GL.BindTexture(TextureTarget.Texture2D, texId);//pepara buffer de textura


                Bitmap bitmap = new Bitmap(Image.FromFile(imagenTex));//genera un mapa de bits

                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                 ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);


                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
                }


                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("Cargar Textura No File: "+imagenTex);
            }
            return texId;

        }
        protected int mass = 0;
        /// <summary>
        /// funcion que devuelve el atributo mass
        /// </summary>
        /// <returns></returns>
        public int getMass()
        {
            return mass;
        }
        /// <summary>
        ///  funcion para dibujar y aplicar todas las transformaciones correspondientes
        /// </summary>
        /// <param name="sProgram"></param>
        /// <param name="textUnit"> Unidad de textura para el texturado</param>
        /// <param name="unit"> numero de la unidad de textura</param>
        /// <param name="textUnitNormal">Unidad de textura para el mapa de normales</param>
        /// <param name="unitNormal">numero de la unidad de textura para mapa de noemales</param>
        /// 
        public virtual void Dibujar(ShaderProgram sProgram, TextureUnit textUnit, int unit, TextureUnit textUnitNormal, int unitNormal)
        {

            for (int i = 0; i < objeto.Count; i++)
            {
                FVLMesh mesh = objeto[i];

                //textura o KD
                if (mesh.getTexturePath() != null || tieneTextura)
                {

                    GL.ActiveTexture(textUnit);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.getTexture());
                    sProgram.SetUniformValue("gSampler", unit);
                    sProgram.SetUniformValue("flagTextura", 1);

                }
                else
                {
                    sProgram.SetUniformValue("flagTextura", 0);
                    sProgram.SetUniformValue("kd", mesh.getKD());

                }

                //para el mapa de normales

                if (mesh.getNormalPath() != null)
                {

                    GL.ActiveTexture(textUnitNormal);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.getNormal());
                    sProgram.SetUniformValue("gSamplerNormalMap", unitNormal);
                    sProgram.SetUniformValue("flagRelieve", 1);

                }
                else
                {
                    sProgram.SetUniformValue("flagRelieve", 0);

                }

                //KS

                sProgram.SetUniformValue("ke", mesh.getKS());



                mesh.Dibujar(sProgram);
            }



            //Console.WriteLine("Finalizando de dibujar " + objeto.Count + " objetos");

        }

        /// <summary>
        /// Funcion encargada de dibujar el mapa de sombras para ser utilizada para debuggear
        /// </summary>
        /// <param name="sProgram"> shader program correspondiente</param>
        public virtual void DibujarShadows(ShaderProgram sProgram)
        {

            for (int i = 0; i < objeto.Count; i++)
            {
                FVLMesh mesh = objeto[i];

                mesh.DibujarShadows(sProgram);
           }
        }

        /// <summary>
        /// Funcion a redefinir en cada subclase
        /// </summary>
        /// <param name="fisica"></param>
        public virtual void move(Labo0.fisica fisica) { }

        /// <summary>
        /// funcion que setea la lista de vertices del objeto
        /// </summary>
        /// <param name="list">lista de vectores</param>
        public void setVertexList(List<Vector3> list)
        {
            vertexList = list;
            for (int i = 0; i < objeto.Count; i++)
            {
                objeto[i].setVertexList(list); 
            }
        }
        /// <summary>
        /// funcion que setea la lista de normales del objeto
        /// </summary>
        /// <param name="list"> lista de normales</param>
        public void setNormalList(List<Vector3> list)
        {
            for (int i = 0; i < objeto.Count; i++)
            {
                objeto[i].setNormalList(list);
            }
        }
        /// <summary>
        /// funcion que setea la lista de coordenadas de textura al objeto
        /// </summary>
        /// <param name="list">lista de coordenadas de textura</param>
        public void setTextCoodList(List<Vector2> list)
        {
            for (int i = 0; i < objeto.Count; i++)
            {
                objeto[i].setTextCoodList(list);
            }
        }
        protected int index;
        /// <summary>
        /// funcion que setea el parametro index a un valor pasado por parametro
        /// </summary>
        /// <param name="i">valor entero a setear como indice</param>
        public void setIndex(int i)
        {
            index = i;
        }
        /// <summary>
        /// funcion que devuelve el indice almacenado
        /// </summary>
        /// <returns></returns>
        internal int getIndex()
        {
            return index;
        }
        /// <summary>
        /// Funcion que devuelve la lista de mesh del objeto
        /// </summary>
        /// <returns></returns>
        public List<CGUNS.Meshes.FaceVertexList.FVLMesh> getObjeto()
        {
            return objeto;
        }

        
        /// <summary>
        /// devuelve la posicion actual del objeto en la escena
        /// </summary>
        /// <returns></returns>
        public Vector3 getPosition()
        {
            return position;
        }
        /// <summary>
        /// funcion utilizada para la fisica que setea el cuerpo de colisiones
        /// </summary>
        /// <param name="shape"> el cuerpo de colisiones a utilizar</param>
        public void setColisionBody(TriangleMesh shape)
        {
            //siempre y cuando posiciones sea multiplo de 3
            for (int i = 0; i < objeto.Count; i++)
            {
                Vector3[] posiciones = objeto[i].getPosiciones();
                for(int j = 0; j< posiciones.Length; j += 3)
                {
                    shape.AddTriangle(posiciones[j], posiciones[j+1], posiciones[j+2]);
                }

            }
        }
        /// <summary>
        /// funcion que devuelve la lista de vertices del objeto
        /// </summary>
        /// <returns></returns>
        public List<Vector3> getVertexList()
        {
            return vertexList;
        }


        


    }



}
