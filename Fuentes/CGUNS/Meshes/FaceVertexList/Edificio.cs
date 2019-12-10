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
/// Clase utilizada para poder crear los objetos de tipo edificio en la escena, la cual extiende a la clase mesh object.
/// </summary>
    class Edificio : MeshObject
    {
        protected float grados = 0;
        protected float scale = 0.1f;
        /// <summary>
        /// Constructor para esta clase
        /// </summary>
        /// <param name="file">es el archivo OBJ para esta clase</param>
        /// <param name="mtl">corresponde al archivo mtl para esta clase</param>
        /// <param name="posicionInicial">corresponde a la posicion inicial en la que ubicaremos al objeto en la escena</param>
        /// <param name="texturado"></param>
        public Edificio(String file, String mtl, Vector3 posicionInicial, bool texturado) : base( file, mtl, posicionInicial, texturado)
        {
            modelMat = Matrix4.CreateScale(1f);
        }

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
            Vector4 Ka = new Vector4(0.2f, 0.2f, 0.2f, 1);
            Matrix4 mvMatrix = Matrix4.Identity;

            mvMatrix = Matrix4.Mult(modelMat, mvMatrix);
            sProgram.SetUniformValue("viewMatrix", viewMatrix);
            sProgram.SetUniformValue("modelMat", mvMatrix);

            //sProgram.SetUniformValue("k", 0.0f);

            sProgram.SetUniformValue("ka", Ka);

            sProgram.SetUniformValue("sigma", 0.1f);


            //sProgram.SetUniformValue("gSampler", 3);
            sProgram.SetUniformValue("flagLuz", 0); //para habilitar o desabilitar la grafica
            sProgram.SetUniformValue("flagTextura", 1); //para habilitar o desabilitar la grafica
                                                        //sProgram.SetUniformValue("flagRelieve", 1); //para habilitar o desabilitar la grafica

            //hasta aca
            Matrix4 T0 = Matrix4.CreateTranslation(position);
            //Matrix4 escala = Matrix4.Mult(Matrix4.CreateScale(scale), T0);
            Matrix4 rotacion = Matrix4.Mult(Matrix4.CreateRotationY(grados), T0);


            sProgram.SetUniformValue("viewMatrix", viewMatrix);
            sProgram.SetUniformValue("modelMat", rotacion);
            //sProgram.SetUniformValue("gSampler", 0);

            Matrix3 MatNorm = new Matrix3(rotacion);
            MatNorm = Matrix3.Transpose(Matrix3.Invert(MatNorm));

            sProgram.SetUniformValue("MatrixNormal", MatNorm);
            base.Dibujar(sProgram, textUnit, unit, textUnitNormal, unitNormal);
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
        /// Utilizado para pasar un valor a partir del cual se aplicara una transformacion de escalamiento
        /// </summary>
        /// <param name="angulo"> valor de cantidad de escalamiento</param>
        public void setScale(float escala)
        {
            scale = escala;
        }
        
    }
}
