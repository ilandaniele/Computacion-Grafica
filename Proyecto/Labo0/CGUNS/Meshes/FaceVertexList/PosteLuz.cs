using CGUNS.Meshes.FaceVertexList;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGUNS.Shaders;
using OpenTK.Graphics.OpenGL;

namespace Labo0.CGUNS.Meshes.FaceVertexList
{/// <summary>
/// Clase utilizada para poder crear los objetos de este tipo en la escena, la cual extiende a la clase mesh object.
/// </summary>
    class PosteLuz : MeshObject
    {
        protected float grados = 0;
        /// <summary>
        /// Constructor para esta clase
        /// </summary>
        /// <param name="file">es el archivo OBJ para esta clase</param>
        /// <param name="mtl">corresponde al archivo mtl para esta clase</param>
        /// <param name="posicionInicial">corresponde a la posicion inicial en la que ubicaremos al objeto en la escena</param>
        /// <param name="texturado"></param>
        public PosteLuz(String file, String mtl, Vector3 posicionInicial, bool texturado) : base( file, mtl, posicionInicial, texturado)
        {
            modelMat = Matrix4.CreateScale(1);
        }

        private int index = 0;
        private Vector3[] posicionesPoste = { new Vector3 (-69.01176f, -1.5347605f, 7.818247f) //0
                                                ,new Vector3 (-97.572f, -7.479682f, 85.86602f) //4
                                                ,new Vector3(-38.88878f, -9.362993f, 87.91214f) //6
                                                ,new Vector3 (6.846281f, -10.80279f, 99.08743f) //1
                                                ,new Vector3 (88.96018f, -12.14561f, 95.60033f) //2
                                                ,new Vector3 (126.2402f, -11.92012f, 50.01223f) //3
                                                ,new Vector3 (72.96187f, -7.661844f, -3.6334f) //5
                                                ,new Vector3 (39.53136f, -4.809922f, -37.55835f)  //7
                                                ,new Vector3 (62.90529f, -2.416607f, -92.87378f) //8
                                                ,new Vector3 (21.78622f, 0.759793f, -105.4716f)  //9
                                                ,new Vector3 (-62.85768f, 10.45207f, -105.8642f) }; //10

        private float[] rotaciones = {
            (float)Math.PI,//0
            (float)Math.PI,//4
            (float)-Math.PI / 2,//6
            (float)Math.PI * 3f / 2f,//1
            (float)-Math.PI / 2,//2
            0,//3
            (float)Math.PI / 2,//5
            0,//7
            0, (float)Math.PI / 2, (float)Math.PI / 2 };
        /// <summary>
        /// Sobreescribe a la funcion dibujar de la clase MeshObject y aplica todas las transformaciones correspondientes
        /// </summary>
        /// <param name="sProgram"></param>
        /// <param name="textUnit"> Unidad de textura para el texturado</param>
        /// <param name="unit"> numero de la unidad de textura</param>
        /// <param name="textUnitNormal">Unidad de textura para el mapa de normales</param>
        /// <param name="unitNormal">numero de la unidad de textura para mapa de noemales</param>
        /// 
        public override void Dibujar(ShaderProgram sProgram, TextureUnit textUnit, int unit, TextureUnit textUnitNormal, int unitNormal)
        {

            setPosition(posicionesPoste[index]);
            setRotation(rotaciones[index]);

            Vector4 Ka = new Vector4(0.2f, 0.2f, 0.2f, 1);
            Matrix4 mvMatrix = Matrix4.Identity;
            
            sProgram.SetUniformValue("viewMatrix", viewMatrix);

            sProgram.SetUniformValue("k", 0.0f);
            sProgram.SetUniformValue("ka", Ka);
            sProgram.SetUniformValue("flagLuz", 1); //para habilitar o desabilitar la grafica
            sProgram.SetUniformValue("flagTextura", 1); //para habilitar o desabilitar la grafica
     
            Matrix4 traslacion = Matrix4.CreateTranslation(position);
            Matrix4 escala = Matrix4.Mult(Matrix4.CreateScale(2f, 2f, 2f), traslacion);
            Matrix4 rotacion =  Matrix4.Mult(Matrix4.CreateRotationY(grados),escala);
           
            sProgram.SetUniformValue("modelMat", rotacion);
            

            Matrix3 MatNorm = new Matrix3(rotacion);
            MatNorm = Matrix3.Transpose(Matrix3.Invert(MatNorm));

            sProgram.SetUniformValue("MatrixNormal", MatNorm);
            base.Dibujar(sProgram, textUnit, unit, textUnitNormal,unitNormal);

            index = (index + 1) % cantPostes();

        }
        /// <summary>
        /// Utilizado para pasar un valor a partir del cual se aplicara una transformacion de rotacion
        /// </summary>
        /// <param name="angulo"> valor del angulo a rotar</param>
        public void setRotation(float angulo)
        {
            grados = angulo;
        }
        /// <summary>
        /// Funcion que nos devuelve la cantidad de objetos de esta instancia creados
        /// </summary>
        /// <returns></returns>
        public int cantPostes()
        {
            return posicionesPoste.Length;
        }

        /// <summary>
        /// devuelve las posiciones de los focos de los postes de luz, mediante un indice el cual devuelve la respectiva posicion para el respectivo poste con su indice
        /// </summary>
        /// <param name="indice"></param>
        /// <returns></returns>
        public Vector3 getPosicionFoco(int indice)
        {
            float angle = rotaciones[indice];
        Vector3 desplazamiento = new Vector3(6.666f * (float)Math.Cos(angle), 17.812f, -6.666f * (float)Math.Sin(angle));
            return desplazamiento + posicionesPoste[indice];
        }

        /// <summary>
        /// Funcion utilizada para obtener las 3 luces mas cercanas a la posicion actual del auto
        /// </summary>
        /// <param name="posicionAuto"> es la posicion actual del auto</param>
        /// <returns></returns>
        public Vector3[] LucesCercanas(Vector3 posicionAuto)
        {
            Vector3 menor = posicionesPoste[0];
            Vector3 auxiliar;
            Vector3 auxiliar2;
            int[] luces = new int[3];
            int indice = 0;
            for (int i = 0; i < posicionesPoste.Length; i++)
            {
                
                auxiliar = posicionesPoste[i] - posicionAuto;
                auxiliar2 = menor - posicionAuto;

                double aux1 = Math.Abs(Math.Sqrt(Math.Pow(auxiliar.X, 2) + Math.Pow(auxiliar.Y, 2) + Math.Pow(auxiliar.Z, 2)));
                double aux2 = Math.Abs(Math.Sqrt(Math.Pow(auxiliar2.X, 2) + Math.Pow(auxiliar2.Y, 2) + Math.Pow(auxiliar2.Z, 2)));

                if (aux1 < aux2)
                {
                    menor = posicionesPoste[i];
                    indice = i;
                }
                /**/
            }
            
            if (indice == posicionesPoste.Length - 1)
            {
                luces[0] = indice - 1;
                luces[1] = indice;
                luces[2] = 0;
            }
            else
            {
                if (indice == 0)
                {
                    luces[0] = posicionesPoste.Length - 1;
                    luces[1] = 0;
                    luces[2] = 1;
                }
                else
                {
                    luces[0] = indice - 1;
                    luces[1] = indice;
                    luces[2] = indice + 1;
                }
            }
            Vector3[] salida = new Vector3[3];
            salida[0] = getPosicionFoco(luces[0]);
            salida[1] = getPosicionFoco(luces[1]);
            salida[2] = getPosicionFoco(luces[2]);
            return salida;
        }


    }
}
