using System;
using System.Collections.Generic;
using OpenTK;
using System.Text;
using CGUNS.Shaders;
using OpenTK.Graphics.OpenGL;
using gl = OpenTK.Graphics.OpenGL.GL;


namespace CGUNS.Meshes.FaceVertexList {
  public class FVLMesh {
    private List<FVLFace> faceList;
    private List<Vector3> vertexList;
    private List<Vector2> texCordList;
    private List<Vector3> vertexNormalList;

        private int[] indices;  //Los indices para formar las caras.
        private int[] indicesNorm;  //Los indices para formar las caras.
        private int[] indicesText;
        private Vector3[] normales;
        private Vector3[] posiciones;
        private Vector2[] texturas;
        private Vector4 KD = new Vector4(1, 0, 0, 1);
        private Vector4 KS = new Vector4(1, 1, 1, 1);
        private Vector4 KA = new Vector4(0.1f, 0.1f, 0.1f, 1);

        private String name;
        private String texturePath = null;
        private String path = null;
        private int texture;

        private bool tieneTextura = false;

        public FVLMesh() {
          faceList = new List<FVLFace>();
          vertexList = new List<Vector3>();
          vertexNormalList = new List<Vector3>();
          texCordList = new List<Vector2>();
            
        }

        public int[] getIndices()
        {
            return indices;
        }

        public void setVertexList(List<Vector3> list)
        {
            vertexList = list;
        }

        public void setNormalList(List<Vector3> list)
        {
            vertexNormalList = list;
        }

        public void setTextCoodList(List<Vector2> list)
        {
            texCordList = list;
        }


        public int getTexture()
        {
            return texture;
        }

        public void setTexture(int text)
        {
            texture = text;
        }

        public float getRadio()
        {
            return radio;
        }

        public Vector3 getCentroX()
        {
            return centroX;
        }

        public Vector3 getCentroY()
        {
            return centroY;
        }

        public Vector4 getKD()
        {
            return KD;
        }

        public void setKD(Vector4 kd)
        {
            KD = kd;
        }

        public void setPath(string fileName)
        {
           path = fileName;
        }

        public String getTexturePath()
        {
            return texturePath;
        }

        public void setTexturePath(String textPath)
        {
            texturePath = textPath;

        }

        private float radio = 0.1f;

        protected void encontrarRadio(Vector3 centro)
        {

            Vector3 lejano = posiciones[0];
            float mayorY = lejano[0];

            for (int i = 0; i < posiciones.Length; i++)
            {
                if (posiciones[i].Y > mayorY)
                    lejano = posiciones[i];
            }

            radio = Math.Max((new Vector3(lejano - centro)).LengthSquared, radio);

        }

        internal void setNormalIndex(int text)
        {
            normalIndex = text;
        }

        //busca el centro de rotacion de la rueda
        protected Vector3 encontrarCentroX()
        {
            float menorY = posiciones[0].Y,
                mayorY = posiciones[0].Y,
                menorZ = posiciones[0].Z,
                mayorZ = posiciones[0].Z;

            for (int i = 0; i < posiciones.Length; i++)
            {
                if (posiciones[i].Y > mayorY)
                    mayorY = posiciones[i].Y;
                if (posiciones[i].Y < menorY)
                    menorY = posiciones[i].Y;
                if (posiciones[i].Z > mayorZ)
                    mayorZ = posiciones[i].Z;
                if (posiciones[i].Z < menorZ)
                    menorZ = posiciones[i].Z;
            }


            float centroZ = (menorZ + mayorZ) / 2;
            float centroY = (menorY + mayorY) / 2;
            //TODO guardar arreglo de esto en el build
            Vector3 ret = new Vector3(0, centroY, centroZ);
            //Console.WriteLine("menorX: " + menorX + " mayorX " + mayorX + " menorZ "+menorZ+" mayorZ "+mayorZ+" medioX "+centroX+" medioZ "+centroZ);
            //con rotacion sobre Y solo translado roto y translado devuelta
            //Matrix4 aux = Matrix4.Mult(Matrix4.CreateTranslation(-centroX, 0, -centroZ), Matrix4.CreateRotationY(teta));
            //Matrix4 ret =  Matrix4.Mult(aux,Matrix4.CreateTranslation(centroX, 0, centroZ));
            return ret;
        }

        private String normalPath;
        private int normalIndex;


        public void setNormalPath(string v)
        {
            normalPath = v;
        }
        public String getNormalPath()
        {
            return normalPath;
        }


        protected Vector3 encontrarCentroY()
        {
            float menorX = posiciones[0].X,
                mayorX = posiciones[0].X,
                menorZ = posiciones[0].Z,
                mayorZ = posiciones[0].Z;

            for (int i = 0; i < posiciones.Length; i++)
            {
                if (posiciones[i].X > mayorX)
                    mayorX = posiciones[i].X;
                if (posiciones[i].X < menorX)
                    menorX = posiciones[i].X;
                if (posiciones[i].Z > mayorZ)
                    mayorZ = posiciones[i].Z;
                if (posiciones[i].Z < menorZ)
                    menorZ = posiciones[i].Z;
            }


            float centroZ = (menorZ + mayorZ) / 2;
            float centroX = (menorX + mayorX) / 2;
            //TODO guardar arreglo de esto en el build
            Vector3 ret = new Vector3(centroX, 0, centroZ);
            //Console.WriteLine("menorX: " + menorX + " mayorX " + mayorX + " menorZ "+menorZ+" mayorZ "+mayorZ+" medioX "+centroX+" medioZ "+centroZ);
            //con rotacion sobre Y solo translado roto y translado devuelta
            //Matrix4 aux = Matrix4.Mult(Matrix4.CreateTranslation(-centroX, 0, -centroZ), Matrix4.CreateRotationY(teta));
            //Matrix4 ret =  Matrix4.Mult(aux,Matrix4.CreateTranslation(centroX, 0, centroZ));
            return ret;
        }

        internal int getNormal()
        {
            return normalIndex;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String n)
        {
            name = n;
        }

        public List<Vector3> VertexList {
      get { return vertexList; }
    }

    public List<FVLFace> FaceList {
      get { return faceList; }
    }

    public List<Vector3> VertexNormalList
    {
        get { return vertexNormalList; }
    }

        

        public List<Vector2> TexCordList
    {
        get { return texCordList; }
    }

    public int VertexCount
    {
        get { return vertexList.Count; }
    }
    public int VertexNormalCount
    {
        get { return vertexNormalList.Count; }
    }

        public void setKS(Vector4 ks)
        {
            KS = ks;
        }

        public Vector4 getKS()
        {
            return KS;
        }

        public int FaceCount {
        get { return faceList.Count; }
    }

    public int AddVertex(Vector3 vertex) {      
      vertexList.Add(vertex);
      return vertexList.Count - 1;
    }

    public int AddVertexNormal(Vector3 normal)
    {
        vertexNormalList.Add(normal);
        return vertexNormalList.Count - 1;
    }

    public int AddTexCord(Vector2 texCord)
    {
        texCordList.Add(texCord);
        return texCordList.Count - 1;
    }

    public int AddFace(FVLFace face) {
      faceList.Add(face);
      return faceList.Count - 1;
    }

    public void PrintLists() {
      String sender = "FVLMesh.printLists: ";
      FVLFace face;
      List<int> faceVertexes;
      log(sender, "Vertex List has {0} items.", vertexList.Count);
      for (int i = 0; i < vertexList.Count; i++) {
        log("", "V[{0}] = ({1}, {2}, {3})", i, vertexList[i].X, vertexList[i].Y, vertexList[i].Z);
      }
      int cantFaces = faceList.Count;
      log(sender, "Face List has {0} items.", cantFaces);
      for (int i = 0; i < cantFaces; i++) {
        face = faceList[i];
        faceVertexes = face.VertexIndexes;
        String format = "F[{0}] = ";
        for (int j = 0; j < faceList[i].VertexCount; j++) {
          format = format + " V[" + faceVertexes[j] + "],";
        }
        log("", format, i);
      }
      log(sender, "End!");
    }

        internal void ignoreTextCoord()
        {
            ignoreTC = true; 
        }

        private void log(String sender, String format, params Object[] args) {
          Console.Out.WriteLine(sender + format, args);
        }

        private Vector3 centroX = new Vector3(0);
        private Vector3 centroY = new Vector3(0);

        public void Build(ShaderProgram sProgram, ShaderProgram sProgramN, int repeat)
        {
            
            CarasToIndices();
            
            if (texCordList.Count == 0 || ignoreTC)
                calcularTexturas(repeat);//tamaño del bitmap
            
            CrearVBOs();
            //CrearVBONormales();

            CrearVAO(sProgram);

            //CrearVAONormales(sProgramN);
            if (posiciones.Length > 0)
            {
                centroX = encontrarCentroX();
                centroY = encontrarCentroY();
                encontrarRadio(centroX);
            }

            CrearShadowVAO(sProgram);

        }

        private int h_ShadowVAO;
        private int h_VBO; //Handle del Vertex Buffer Object (posiciones de los vertices)
        private int n_VBO;

        private int n_VAO;
        private int n_EBO;
        private int h_EBO; //Handle del Elements Buffer Object (indices)
        private int h_VAO; //Handle del Vertex Array Object (Configuracion de los dos anteriores)
        private int t_VBO;
        private bool ignoreTC = false;

        private void CrearVBOs()
        {
            BufferTarget bufferType; //Tipo de buffer (Array: datos, Element: indices)
            IntPtr size;             //Tamanio (EN BYTES!) del buffer.
                                     //Hint para que OpenGl almacene el buffer en el lugar mas adecuado.
                                     //Por ahora, usamos siempre StaticDraw (buffer solo para dibujado, que no se modificara)
            BufferUsageHint hint = BufferUsageHint.StaticDraw;

            //VBO con el atributo "posicion" de los vertices.
            bufferType = BufferTarget.ArrayBuffer;

            size = new IntPtr(VertexList.Count * Vector3.SizeInBytes);
            
            //Vector3[] vertices = VertexList.ToArray();
            
            size = new IntPtr(posiciones.Length * Vector3.SizeInBytes);
            
            //copio todo esto y cambio h_VBO por n_VBO
            h_VBO = gl.GenBuffer();  //Le pido un Id de buffer a OpenGL
            gl.BindBuffer(bufferType, h_VBO); //Lo selecciono como buffer de Datos actual.

            gl.BufferData<Vector3>(bufferType, size, posiciones, hint); //Lo lleno con la info.
            gl.BindBuffer(bufferType, 0); // Lo deselecciono (0: ninguno)
            
            n_VBO = gl.GenBuffer();
            gl.BindBuffer(bufferType, n_VBO);

            gl.BufferData<Vector3>(bufferType, size, normales, hint); //Lo lleno con la info.
            gl.BindBuffer(bufferType, 0); // Lo deselecciono (0: ninguno)
            
            t_VBO = gl.GenBuffer();
            gl.BindBuffer(bufferType, t_VBO);
            size = new IntPtr(texturas.Length * Vector2.SizeInBytes);//cambia el size

            gl.BufferData<Vector2>(bufferType, size, texturas, hint); //Lo lleno con la info.
            gl.BindBuffer(bufferType, 0); // Lo deselecciono (0: ninguno)
            /**/
            //VBO con otros atributos de los vertices (color, normal, textura, etc).
            //Se pueden hacer en distintos VBOs o en el mismo.

            //EBO, buffer con los indices.
            bufferType = BufferTarget.ElementArrayBuffer;
            //size = new IntPtr(faceList.Count * sizeof(int));
            h_EBO = gl.GenBuffer();
            //para dibujas pociciones 
            
            int[] nuevoIndice = indicesANormales(normales.Length);
            size = new IntPtr(nuevoIndice.Length * sizeof(int));
            gl.BindBuffer(bufferType, h_EBO); //Lo selecciono como buffer de elementos actual.
            gl.BufferData<int>(bufferType, size, nuevoIndice, hint);
            gl.BindBuffer(bufferType, 0);
            
        }


        private void CrearVAO(ShaderProgram sProgram)
        {
            // Indice del atributo a utilizar. Este indice se puede obtener de tres maneras:
            // Supongamos que en nuestro shader tenemos un atributo: "in vec3 vPos";
            // 1. Dejar que OpenGL le asigne un indice cualquiera al atributo, y para consultarlo hacemos:
            //    attribIndex = gl.GetAttribLocation(programHandle, "vPos") DESPUES de haberlo linkeado.
            // 2. Nosotros le decimos que indice queremos que le asigne, utilizando:
            //    gl.BindAttribLocation(programHandle, desiredIndex, "vPos"); ANTES de linkearlo.
            // 3. Nosotros de decimos al preprocesador de shader que indice queremos que le asigne, utilizando
            //    layout(location = xx) in vec3 vPos;
            //    En el CODIGO FUENTE del shader (Solo para #version 330 o superior)      
            int attribIndex;
            int cantComponentes; //Cantidad de componentes de CADA dato.
            VertexAttribPointerType attribType; // Tipo de CADA una de las componentes del dato.
            int stride; //Cantidad de BYTES que hay que saltar para llegar al proximo dato. (0: Tightly Packed, uno a continuacion del otro)
            int offset; //Offset en BYTES del primer dato.
            BufferTarget bufferType; //Tipo de buffer.

            // 1. Creamos el VAO
            h_VAO = gl.GenVertexArray(); //Pedimos un identificador de VAO a OpenGL.
            gl.BindVertexArray(h_VAO);   //Lo seleccionamos para trabajar/configurar.

            //2. Configuramos el VBO de posiciones.
            attribIndex = sProgram.GetVertexAttribLocation("vPos"); //Yo lo saco de mi clase ProgramShader.
            cantComponentes = 3;   // 3 componentes (x, y, z)
            attribType = VertexAttribPointerType.Float; //Cada componente es un Float.
            stride = 0;  //Los datos estan uno a continuacion del otro.
            offset = 0;  //El primer dato esta al comienzo. (no hay offset).
            bufferType = BufferTarget.ArrayBuffer; //Buffer de Datos.

            gl.EnableVertexAttribArray(attribIndex); //Habilitamos el indice de atributo.
            gl.BindBuffer(bufferType, h_VBO); //Seleccionamos el buffer a utilizar.
            gl.VertexAttribPointer(attribIndex, cantComponentes, attribType, false, stride, offset);//Configuramos el layout (como estan organizados) los datos en el buffer.
            
            //VBO de normales
            
            attribIndex = sProgram.GetVertexAttribLocation("vNorm"); //Yo lo saco de mi clase ProgramShader.
            cantComponentes = 3;   // 3 componentes (x, y, z)
            attribType = VertexAttribPointerType.Float; //Cada componente es un Float.
            stride = 0;  //Los datos estan uno a continuacion del otro.
            offset = 0;  //El primer dato esta al comienzo. (no hay offset).
            bufferType = BufferTarget.ArrayBuffer; //Buffer de Datos.

            gl.EnableVertexAttribArray(attribIndex); //Habilitamos el indice de atributo.
            gl.BindBuffer(bufferType, n_VBO); //Seleccionamos el buffer a utilizar.
            gl.VertexAttribPointer(attribIndex, cantComponentes, attribType, false, stride, offset);//Configuramos el layout (como estan organizados) los datos en el buffer.
            
            //VBO de texturas
            
            attribIndex = sProgram.GetVertexAttribLocation("vText"); //Yo lo saco de mi clase ProgramShader.
                                                                        // attribIndex = 1;
            cantComponentes = 2;   // 3 componentes (s, t)
            attribType = VertexAttribPointerType.Float; //Cada componente es un Float.
            stride = 0;  //Los datos estan uno a continuacion del otro.
            offset = 0;  //El primer dato esta al comienzo. (no hay offset).
            bufferType = BufferTarget.ArrayBuffer; //Buffer de Datos.

            gl.EnableVertexAttribArray(attribIndex); //Habilitamos el indice de atributo.
            gl.BindBuffer(bufferType, t_VBO); //Seleccionamos el buffer a utilizar.
            gl.VertexAttribPointer(attribIndex, cantComponentes, attribType, false, stride, offset);//Configuramos el layout (como estan organizados) los datos en el buffer.
            /**/
            // 2.a.El bloque anterior se repite para cada atributo del vertice (color, normal, textura..)


            // 3. Configuramos el EBO a utilizar. (como son indices, no necesitan info de layout)
            bufferType = BufferTarget.ElementArrayBuffer;
           gl.BindBuffer(bufferType, h_EBO);

            // 4. Deseleccionamos el VAO.
            gl.BindVertexArray(0);
            //indices = null;
            //indicesNorm = null;
            //indicesText = null;
    }




        protected void CrearShadowVAO(ShaderProgram sProgram)
        {
            // Indice del atributo a utilizar. Este indice se puede obtener de tres maneras:
            // Supongamos que en nuestro shader tenemos un atributo: "in vec3 vPos";
            // 1. Dejar que OpenGL le asigne un indice cualquiera al atributo, y para consultarlo hacemos:
            //    attribIndex = GL.GetAttribLocation(programHandle, "vPos") DESPUES de haberlo linkeado.
            // 2. Nosotros le decimos que indice queremos que le asigne, utilizando:
            //    GL.BindAttribLocation(programHandle, desiredIndex, "vPos"); ANTES de linkearlo.
            // 3. Nosotros de decimos al preprocesador de shader que indice queremos que le asigne, utilizando
            //    layout(location = xx) in vec3 vPos;
            //    En el CODIGO FUENTE del shader (Solo para #version 330 o superior)      
            int attribIndex;
            int cantComponentes; //Cantidad de componentes de CADA dato.
            VertexAttribPointerType attribType; // Tipo de CADA una de las componentes del dato.
            int stride; //Cantidad de BYTES que hay que saltar para llegar al proximo dato. (0: Tightly Packed, uno a continuacion del otro)
            int offset; //Offset en BYTES del primer dato.
            BufferTarget bufferType; //Tipo de buffer.

            // 1. Creamos el VAO
            h_ShadowVAO = GL.GenVertexArray(); //Pedimos un identificador de VAO a OpenGL.
            GL.BindVertexArray(h_ShadowVAO);   //Lo seleccionamos para trabajar/configurar.

            //2. Configuramos el VBO de posiciones.
            attribIndex = 0; //sProgram.GetVertexAttribLocation("vPos"); //Yo lo saco de mi clase ProgramShader.
            cantComponentes = 3;   // 3 componentes (x, y, z)
            attribType = VertexAttribPointerType.Float; //Cada componente es un Float.
            stride = 0;  //Los datos estan uno a continuacion del otro.
            offset = 0;  //El primer dato esta al comienzo. (no hay offset).
            bufferType = BufferTarget.ArrayBuffer; //Buffer de Datos.

            GL.EnableVertexAttribArray(attribIndex); //Habilitamos el indice de atributo.
            GL.BindBuffer(bufferType, h_VBO); //Seleccionamos el buffer a utilizar.
            GL.VertexAttribPointer(attribIndex, cantComponentes, attribType, false, stride, offset);//Configuramos el layout (como estan organizados) los datos en el buffer.

            bufferType = BufferTarget.ElementArrayBuffer;
            GL.BindBuffer(bufferType, h_EBO);

            // 4. Deseleccionamos el VAO.
            GL.BindVertexArray(0);
        }

        public void Dibujar(ShaderProgram sProgram)
        {
            Dibujar_privado(sProgram, h_VAO);
            
        }

        public void DibujarShadows(ShaderProgram sProgram)
        {
            Dibujar_privado(sProgram, h_ShadowVAO);
        }

        public void Dibujar_privado(ShaderProgram sProgram, int h_VAO )
        {

            PrimitiveType primitive; //Tipo de Primitiva a utilizar (triangulos, strip, fan, quads, ..)
            int offset; // A partir de cual indice dibujamos?
            int count;  // Cuantos?
            DrawElementsType indexType; //Tipo de los indices.

            primitive = PrimitiveType.Triangles;  //Usamos trianglos.
            offset = 0;  // A partir del primer indice.
            count = indices.Length * 2; // Todos los indices.

            indexType = DrawElementsType.UnsignedInt; //Los indices son enteros sin signo.

            gl.BindVertexArray(h_VAO); //Seleccionamos el VAO a utilizar.
            gl.DrawElements(primitive, count, indexType, offset); //Dibujamos utilizando los indices del VAO.
            gl.BindVertexArray(0); //Deseleccionamos el VAO

            // DibujarNormales(sProgram);
        }

        private Vector3[] normalesDib()
        {
            //indices.Length
            Vector3[] toret = new Vector3[posiciones.Length * 2 ];
            int i;
            float delta = 0.2f;//cambiar esto para escalar normales
            for (i = 0; i < posiciones.Length; i++)
            {
                toret[i * 2] = posiciones[i];
                Vector3 aux = new Vector3((posiciones[i].X + normales[i].X/ delta) , (posiciones[i].Y + normales[i].Y/ delta) , (posiciones[i].Z + normales[i].Z/ delta) );
                
                toret[i * 2 + 1] = aux;
                
            }
            return toret;
        }

        private Vector3[] colorear(int lenght)
        {
            Vector3[] ret = new Vector3[lenght];
            for(int i = 0; i<lenght; i++)
            {
                if (i % 2 == 0)
                    ret[i] = new Vector3(0, 0, 1);
                else
                    ret[i] = new Vector3(1, 0, 0);
            }

            return ret;
        }

        private void CarasToIndices()
        {
            
            bool tieneText = texCordList.Count > 0 && !ignoreTC;
            
            int cantFaces = faceList.Count;

            int cant =  3;

            indices = new int[cantFaces * cant]; //OJO solo si  TODAS las caras son triágulos
            indicesNorm = new int[cantFaces * cant]; //OJO solo si  TODAS las caras son triágulos
            indicesText = new int[cantFaces * cant];
            int i = 0;
           
                for (int f = 0; f < cantFaces; f++)
            {
                FVLFace cara = faceList[f];
                int[] indicesCara = cara.IndicesDeCara();
                int[] indicesNormales = cara.IndicesDeNormales();
                int[] indicesTexturas = cara.IndicesDeTexturas();
                
                    for (int j = 0; j < 3; j++)
                    {
                        indices[i] = indicesCara[j];
                        indicesNorm[i] = indicesNormales[j];
                        if (tieneText && indicesTexturas.Length>0)
                        {
                            indicesText[i] = indicesTexturas[j];
                        }
                        i++;
                    }
                }
                
            VertexToIndices();
            
            NormalesToIndices();
           
            if (tieneText)
                TexturasToindices();
        }

        private void VertexToIndices()
        {
            posiciones = new Vector3[indices.Length];
            Vector3[] vertex = VertexList.ToArray();

            for (int i = 0; i < indices.Length; i++)
            {
                posiciones[i] = vertex[indices[i]];

            }
        }

        private void TexturasToindices()
        {

            texturas = new Vector2[indicesText.Length];
            Vector2[] text = texCordList.ToArray();

            for (int i = 0; i < indicesText.Length; i++)
            {
                texturas[i] = text[indicesText[i]];

            }
        }


        private void NormalesToIndices()
        {
            int cantFaces = faceList.Count;
            normales = new Vector3[indicesNorm.Length];
            Vector3[] normal = vertexNormalList.ToArray();

            for (int i = 0; i < indicesNorm.Length; i++)
            {
                normales[i] = normal[indicesNorm[i]];

            }



        }

        //funcion que devuelve un arreglo de indices 0,1,2,3,4...,cant-1
        private int[] indicesANormales(int cant)
        {
            int[] ret = new int[cant];

            for (int i = 0; i < cant; i++)
            {
                ret[i] = i;
            }

            return ret;

        }

        public void cargarTexturasRepetidas(int cant)
        {
            calcularTexturas(cant);
        }

        private void calcularTexturas(int cant)
        {
            float menorX = 0, mayorX = 0, menorZ = 0, mayorZ = 0;

            for(int i = 0; i < posiciones.Length; i++)
            {
                if (posiciones[i].X > mayorX)
                    mayorX = posiciones[i].X;
                if (posiciones[i].X < menorX)
                    menorX = posiciones[i].X;
                if (posiciones[i].Y > mayorZ)
                    mayorZ = posiciones[i].Z;
                if (posiciones[i].Y < menorZ)
                    menorZ = posiciones[i].Z;
            }

            float largoZ = (Math.Abs(menorZ) + Math.Abs(mayorZ)) ;
            float largoX = (Math.Abs(menorX) + Math.Abs(mayorX)) ;

            texturas = new Vector2[posiciones.Length];

            float x;
            float z;
            for (int i = 0; i < posiciones.Length; i++)
            {
                x = (posiciones[i].X + Math.Abs(menorX));
                
                z = (posiciones[i].Y + Math.Abs(menorZ));
               
                texturas[i] = new Vector2( x/largoX * cant, z/largoZ * cant);
            }
        }

        public Vector3[] getPosiciones()
        {
            return posiciones;
        }

    }
    



}
