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
    class Senial : MeshObject
    {
        protected float grados = 0;
        public Senial(String file, String mtl, Vector3 posicionInicial, bool texturado) : base( file, mtl, posicionInicial, texturado)
        {
            modelMat = Matrix4.CreateScale(1);
        }

        private int index = 0;
        private Vector3[] posicionesSeniales = {
new Vector3(-75.75191f, -7.384323f, 91.95035f)
,new Vector3(-103.289f, -3.629794f, 61.88545f)
,new Vector3(-97.44355f, 10.55352f, -60.15113f)};

        private float[] rotaciones = {
            (float)Math.PI/2, 0, 0 };
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

            setPosition(posicionesSeniales[index]);
            //Cuando esten las rotaciones
            setRotation(rotaciones[index]);

            Vector4 Ka = new Vector4(0.5f, 0.5f, 0.5f, 1);
            Matrix4 mvMatrix = Matrix4.Identity;
            
            sProgram.SetUniformValue("viewMatrix", viewMatrix);

            sProgram.SetUniformValue("k", 0.5f);
            sProgram.SetUniformValue("ka", Ka);
            sProgram.SetUniformValue("flagLuz", 0); //para habilitar o desabilitar la iluminación
            sProgram.SetUniformValue("flagTextura", 1); //para habilitar o desabilitar la textura
                                                        //sProgram.SetUniformValue("flagRelieve", 1); //para habilitar o desabilitar la grafica

            Matrix4 traslacion = Matrix4.CreateTranslation(position);
            Matrix4 escala = Matrix4.Mult(Matrix4.CreateScale(0.1f, 0.1f, 0.1f), traslacion);
            Matrix4 rotacion =  Matrix4.Mult(Matrix4.CreateRotationY(grados),escala);
           
            sProgram.SetUniformValue("modelMat", rotacion);
            //sProgram.SetUniformValue("gSampler", 0);

            Matrix3 MatNorm = new Matrix3(rotacion);
            MatNorm = Matrix3.Transpose(Matrix3.Invert(MatNorm));

            sProgram.SetUniformValue("MatrixNormal", MatNorm);
            base.Dibujar(sProgram, textUnit, unit, textUnitNormal,unitNormal);

            index = (index + 1) % cantSeniales();

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
        public int cantSeniales()
        {
            return posicionesSeniales.Length;
        }

      


    }
}
