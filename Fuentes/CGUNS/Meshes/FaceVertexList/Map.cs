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

namespace CGUNS.Meshes.FaceVertexList
{
    /// <summary>
    /// Clase utilizada para implementar un mapeo
    /// </summary>
    public partial class Map
    {/// <summary>
    /// Clase interna clave a utilizada para el mapeo
    /// </summary>
        internal class clave
        {
            private int valor;
            private String key;
            private String path;
            /// <summary>
            /// Constructor para la clave
            /// </summary>
            /// <param name="v"> valor de clave</param>
            /// <param name="p"> valor a almacenar con la clave</param>
            /// <param name="k"> clave </param>
            public clave(int v, String p,String k)
            {
                key = k;
                valor = v;
                path = p;
            }
            /// <summary>
            /// Devuelve el valor de la clave almacenado
            /// </summary>
            /// <returns></returns>
            public int getValor()
            {
                return valor;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public String getPath()
            {
                return path;
            }
            /// <summary>
            /// funcion para chequear si la clave pasada por parámetro es la misma
            /// </summary>
            /// <param name="k"> String de la clave</param>
            /// <returns></returns>
            public bool iskey(String k)
            {
                return k.Equals(key);
            }

        }
        private List<clave> lista;
        /// <summary>
        /// funcion constructor del mapeo
        /// </summary>
        public Map()
        {
            lista = new List<clave>();
        }
        /// <summary>
        /// Funcion que devuelve un valor a partir de una clave dada
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int find(String key)
        {
            bool verd = false;
            int i = 0;
            while (!verd && i < lista.Count)
            {
                if (lista[i].iskey(key))
                    verd = true;
                else
                    i++;
            }
            if (i < lista.Count)
                return lista[i].getValor();
            else
                return -1;
        }
        /// <summary>
        /// funcion que devuelve un directorio a partir de una clave pasada
        /// </summary>
        /// <param name="key"> string de la clave</param>
        /// <returns></returns>
        public String findPath(String key)
        {
            bool verd = false;
            int i = 0;
            while (!verd && i < lista.Count)
            {
                if (lista[i].iskey(key))
                    verd = true;
                else
                    i++;
            }
            if (i < lista.Count)
                return lista[i].getPath();
            else
                return null;
        }
        /// <summary>
        /// funcion que setea el valor de una nueva entrada
        /// </summary>
        /// <param name="valor"> valor de la clave</param>
        /// <param name="path"> directorio de la clave</param>
        /// <param name="key"> clave</param>
        public void setClave(int valor, String path, String key)
        {
            lista.Add(new clave(valor,path ,key));
        }
        /// <summary>
        /// funcion que devuelve la cantidad de claves
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return lista.Count;
        }

    }

}
