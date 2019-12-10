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
using CGUNS.Meshes.FaceVertexList;

namespace Labo0.CGUNS.Meshes.FaceVertexList
{/// <summary>
/// Clase utilizada para poder crear el objeto cielo en la escena, la cuál extiende a la clase mesh object.
/// </summary>
    class Cielo : MeshObject
    {    /// <summary>
         /// Constructor para esta clase
         /// </summary>
         /// <param name="file">es el archivo OBJ para esta clase</param>
         /// <param name="mtl">corresponde al archivo mtl para esta clase</param>
         /// <param name="posicionInicial">corresponde a la posicion inicial en la que ubicaremos al objeto en la escena</param>
         /// <param name="texturado"></param>
        public Cielo():base("CGUNS/ModelosOBJ/Skybox/SkySphere.obj", "CGUNS/ModelosOBJ/Skybox/SkySphere.mtl", new Vector3(0, 0, 0), true)
        {
            Matrix4 escala = Matrix4.CreateScale(0.001f, 0.001f, 0.001f);
            escala = Matrix4.Mult(Matrix4.CreateRotationX(-(float)Math.PI ), escala);
            modelMat = escala;
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

            mvMatrix = Matrix4.Mult(  modelMat, mvMatrix);
            sProgram.SetUniformValue("viewMatrix", viewMatrix);
            sProgram.SetUniformValue("modelMat", mvMatrix);

            //sProgram.SetUniformValue("gSampler", 3);
            sProgram.SetUniformValue("flagLuz", 0); //para habilitar o desabilitar la grafica
            sProgram.SetUniformValue("flagTextura", 1); //para habilitar o desabilitar la grafica


            Matrix3 MatNorm = new Matrix3(modelMat);
            MatNorm = Matrix3.Transpose(Matrix3.Invert(MatNorm));
            base.Dibujar(sProgram, textUnit, unit, textUnitNormal, unitNormal);
        }
    }
}
