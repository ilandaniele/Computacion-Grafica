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
    class Palmera : MeshObject
    {
        protected float grados = 0;
        public Palmera(String file, String mtl, Vector3 posicionInicial, bool texturado) : base( file, mtl, posicionInicial, texturado)
        {
            modelMat = Matrix4.CreateScale(0.2f);
        }

        private int index = 0;
        private Vector3[] posicionesPalmera =
            {new Vector3(-58.00554f, -8.624352f, 86.76325f)
            ,new Vector3(-76.16628f, -5.215658f, 58.67889f)
            ,new Vector3(-77.53313f, 2.147285f, -8.936444f)
            ,new Vector3(-84.24205f, 7.028872f, -44.5687f)
            ,new Vector3(-73.26212f, 9.35125f, -79.87945f)
            ,new Vector3(17.14654f, 0.447199f, -93.75159f)
            ,new Vector3(54.35487f, -2.41066f, -86.49213f)
            ,new Vector3(50.70316f, -3.762312f, -65.15649f)
            ,new Vector3(37.4277f, -5.097899f, -29.99191f)
            ,new Vector3(47.04975f, -5.955256f, -20.4744f)
            ,new Vector3(143.4456f, -12.4403f, 52.87657f)
            ,new Vector3(139.7268f, -12.92527f, 85.56916f)
            ,new Vector3(63.23732f, -12.25695f, 128.0493f)
            ,new Vector3(-115.984f, -7.431495f, 93.38258f)
            ,new Vector3(-114.9955f, -4.901716f, 69.06427f)
            ,new Vector3(-99.21703f, -4.096097f, 55.12569f)
            ,new Vector3(-91.63875f, 1.368041f, 8.140349f)
            ,new Vector3(-109.8806f, 8.273103f, -34.41034f)
            ,new Vector3(-107.2168f, 10.03488f, -53.3362f)
            ,new Vector3(-85.97091f, 11.1493f, -92.58923f)
            ,new Vector3(-84.2367f, 12.09249f, -103.7728f)
            ,new Vector3(-36.18105f, 7.18579f, -118.8885f)
            ,new Vector3(18.89822f, 1.695182f, -125.0309f)
            ,new Vector3(79.56255f, -2.122424f, -124.5534f)
            ,new Vector3(89.97368f, -3.121812f, -112.4171f)
            ,new Vector3(87.48208f, -4.469173f, -96.87048f)
            ,new Vector3(88.1273f, -4.791121f, -69.61124f)
            ,new Vector3(82.38156f, -5.953857f, -45.16854f)
            ,new Vector3(70.80479f, -6.087326f, -34.85545f)
            ,new Vector3(128.6069f, -12.79087f, 97.89208f)
            ,new Vector3(100.8037f, -12.48619f, 117.2987f)
            ,new Vector3(88.16919f, -12.20671f, 114.7077f)
            ,new Vector3(34.06834f, 0.58756f, -124.2686f)
            ,new Vector3(64.1028f, -1.3495725f, -121.034f)
            ,new Vector3(72.33125f, -1.9931708f, -114.773f)
            ,new Vector3(96.84356f, -5.31836f, -68.56371f)
            ,new Vector3(95.73638f, -6.55286f, -43.76194f)
            ,new Vector3(97.91722f, -7.150412f, -33.84806f)
            ,new Vector3(141.8474f, -12.85744f, 79.08878f)
            ,new Vector3(137.56f, -12.89228f, 87.20827f)
            ,new Vector3(109.6116f, -12.53207f, 106.1983f)
            ,new Vector3(56.00508f, -12.14639f, 125.8179f)
            ,new Vector3(25.1157f, -11.78564f, 125.5649f)
            ,new Vector3(8.725653f, -11.41683f, 120.4096f)
            ,new Vector3(-6.508575f, -10.420182f, 115.4193f)
            ,new Vector3(-27.2178f, -10.37462f, 108.9538f)
            ,new Vector3(-44.32918f, -10.01208f, 108.4635f)
            ,new Vector3(-65.79777f, -9.561935f, 109.0614f)
            ,new Vector3(-90.77061f, -9.412727f, 112.4688f) };

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

            setPosition(posicionesPalmera[index]);
            //Cuando esten las rotaciones
            //setRotation(rotaciones[index]);

            Vector4 Ka = new Vector4(0.2f, 0.2f, 0.2f, 1);
            Matrix4 mvMatrix = Matrix4.Identity;
            
            sProgram.SetUniformValue("viewMatrix", viewMatrix);
            

            sProgram.SetUniformValue("k", 0.0f);
            sProgram.SetUniformValue("ka", Ka);
            sProgram.SetUniformValue("flagLuz", 1); //para habilitar o desabilitar la grafica
            sProgram.SetUniformValue("flagTextura", 1); //para habilitar o desabilitar la grafica
                                                        //sProgram.SetUniformValue("flagRelieve", 1); //para habilitar o desabilitar la grafica



            //hasta aca
            
            Matrix4 traslacion = Matrix4.CreateTranslation(position);
            Matrix4 escala = Matrix4.Mult(Matrix4.CreateScale(0.1f, 0.1f, 0.1f), traslacion);
            Matrix4 rotacion =  Matrix4.Mult(Matrix4.CreateRotationY(grados),escala);
           
            sProgram.SetUniformValue("modelMat", rotacion);
            //sProgram.SetUniformValue("gSampler", 0);

            Matrix3 MatNorm = new Matrix3(rotacion);
            MatNorm = Matrix3.Transpose(Matrix3.Invert(MatNorm));

            sProgram.SetUniformValue("MatrixNormal", MatNorm);
            base.Dibujar(sProgram, textUnit, unit, textUnitNormal,unitNormal);

            index = (index + 1) % cantPalmeras();

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
        public int cantPalmeras()
        {
            return posicionesPalmera.Length;
        }
    }
}
