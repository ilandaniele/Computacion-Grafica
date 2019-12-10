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
using System.Threading;

using CGUNS.Meshes.FaceVertexList;
using CGUNS;
using Labo0.CGUNS.Meshes.FaceVertexList;
using CSCore.CoreAudioAPI;

namespace Labo0
{/// <summary>
/// Clase main sobre la cual correra el proyecto
/// </summary>
    public partial class MainWindow : Form
    {

        Sound reproductor = new Sound();

        public MainWindow()
        {
            fisica = new fisica();
            InitializeComponent();
            
        }

        private ShaderProgram sProgram; //Nuestro programa de shaders.
        private ShaderProgram sProgramON; //Nuestro programa de shaders.


        private MeshObject mesa;
        private CGUNS.Meshes.FaceVertexList.Auto player, auto2;

        private List<Auto> policias = new List<Auto>();

        private fisica fisica;
        private MeshObject avion;
        //La posicion de la luz
        private MeshObject luz;
        private PosteLuz posteLuz;
        //Skybox
        private CGUNS.Meshes.FaceVertexList.Cielo cielo;
        //el plano del piso
        private MeshObject tierra;
        private MeshObject techo;
        //palmeras
        private Palmera palmeras;
        //Seniales
        private Senial SenialLimiteVelocidad;

        //camara
        private Camera myCamera;

        private Rectangle viewport; //Viewport a utilizar (Porcion del glControl en donde voy a dibujar).
        
        //BORRAR
        private int tex2, tex3, tex4;
        
        //TODO MESA este flag lo pongo para hacer pruebas
        private bool flagPrueba = true;
        private int flagCamara = 0;
        private int cantTiposCamara = 3;
        private Light[] luces; //Mis multiples luces.

        private String[] musica = { "files/Sonido/Reproductor/animals.mp3",
                                    "files/Sonido/Reproductor/green.mp3",
                                    "files/Sonido/Reproductor/linkin.mp3",
                                    "files/Sonido/Reproductor/sad.mp3", };
        int indMusica = 0;
        
        /// <summary>
        /// Funcion encargada de inicializar todos los objetos a utilizar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl3_Load(object sender, EventArgs e)
        {
            
            logContextInfo(); //Mostramos info de contexto.
            
            sProgram = SetupShaders("vshaderPhong.glsl", "fshaderPhong.glsl"); //Creamos los shaders y el programa de shader
            sProgramON = SetupShaders("vshaderOrenNayar.glsl", "fshaderOrenNayar.glsl"); //Creamos los shaders y el programa de shader
            mShadowProgram = SetupShaders("vShadow.glsl", "fShadow.glsl");
            mShadowViewportProgram = SetupShaders("vViewport.glsl", "fViewport.glsl");
            //LUCES
            //esto hay que modificarlo luces = new Light[4];
            luces = new Light[4];

            crearLuces(4);
            
            player = new CGUNS.Meshes.FaceVertexList.Auto("CGUNS/ModelosOBJ/DODGE CHARGER/CHARGER69.obj", "CGUNS/ModelosOBJ/DODGE CHARGER/CHARGER69.mtl", new Vector3(0, -5, 0),false);
            //les paso null pero es un ShaderProgram
            player.Build(sProgram, null, 1); //Construyo los objetos OpenGL 
            player.setIndex(
                fisica.agregarCuboRigido(
                    player,
                    player.getPosition(),
                    player.getMass(), false
                )
                );

            //le digo que es el principal
            player.setPrincipal();


            //AUTOS DE POLICIA POR AHORA PRUEBO 5
            for (int i=0; i<1; i++)
            {
                //conviene ttener un arreglo de posiciones iniciales
                auto2 = new CGUNS.Meshes.FaceVertexList.Auto("CGUNS/ModelosOBJ/Mustang/mustang.obj", "CGUNS/ModelosOBJ/Mustang/mustang.mtl", new Vector3(4*i + 4, -i-1, 0), false);

                auto2.Build(sProgram, null, 1); //Construyo los objetos OpenGL 
                auto2.setIndex(
                    fisica.agregarCuboRigido(
                        auto2,
                        auto2.getPosition(),
                        auto2.getMass(), false
                    )
                    );

                auto2.setPlayer(player);

                policias.Add(auto2);
            }
              
            //le digo al jugador cuales son los policias
            player.setPolicias(policias);

            //TERRENO
            tierra = new Tierra("CGUNS/ModelosOBJ/Pista/pista.obj", "CGUNS/ModelosOBJ/Pista/pista.mtl", new Vector3(0, -5, 0), false);

            tierra.Build(sProgramON, null, 1);

            tierra.setIndex(
               fisica.agregarCuboRigido(
                   tierra,
                   tierra.getPosition(),
                   tierra.getMass(), true
               )
               );

            techo = new Tierra("CGUNS/ModelosOBJ/Pista/techo.obj", null, new Vector3(0, -5, 0), false);

            techo.Build(sProgram, null, 1);

            techo.setIndex(
               fisica.agregarCuboRigido(
                   techo,
                   techo.getPosition(),
                   techo.getMass(), true
               )
               );



            //CIUDAD
            ciudad = new Edificio("CGUNS/ModelosOBJ/Ciudad/ciudad.obj", "CGUNS/ModelosOBJ/Ciudad/ciudad.mtl", new Vector3(0, -5.0f, 0), false);
            ciudad.Build(sProgramON, null, 1);

            //PALMERAS
            palmeras = new Palmera("CGUNS/ModelosOBJ/Palmera/palmeras.obj", "CGUNS/ModelosOBJ/Palmera/palmeras.mtl", new Vector3(0, 0, 0), false);
            palmeras.Build(sProgram, null, 1);

            //Senial
            SenialLimiteVelocidad = new Senial("CGUNS/ModelosOBJ/Senial/SenialLimiteVelocidad.obj", "CGUNS/ModelosOBJ/Senial/SenialLimiteVelocidad.mtl", new Vector3(0, 0, 0), false);
            SenialLimiteVelocidad.Build(sProgram, null, 1);

            cielo = new CGUNS.Meshes.FaceVertexList.Cielo();
            cielo.Build(sProgram, null, 1);

            posteLuz = new PosteLuz("CGUNS/ModelosOBJ/PosteLuz/PosteLuz.obj", "CGUNS/ModelosOBJ/PosteLuz/PosteLuz.mtl", new Vector3(0, 0, 0), false);
            posteLuz.Build(sProgram, null, 1);
            
            //camara
            myCamera = new SphericalCamera();
            
            gl.ClearColor(Color.Gray); //Configuro el Color de borrado.
            gl.Enable(EnableCap.DepthTest);
            //gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //De cada poligono solo dibujo las lineas de contorno (wireframe).
            gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //SONIDO DE FONDO
            //indice aleatorio entre 0 y musica.Length - 1 
            indMusica = (new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds))).Next() % musica.Length;
            reproductor.Open(musica[indMusica]);
            reproductor.Volume = 5;
            //reproductor.Play();


            mShadowViewportQuad = new ViewportQuad();
            mShadowViewportQuad.Build(mShadowViewportProgram);
            CrearShadowTextures();

            tiempo = DateTime.Now.TimeOfDay.TotalMilliseconds;

        }

        /// <summary>
        /// Funcion encargada de cargar la textura al objeto
        /// </summary>
        /// <param name="imagenTex"> ubicacion del archivo de la imagen de textura</param>
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


                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                
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

        private int mShadowTextureUnit = 0;

        private int fbo;
        private int depthTexture;

        private ShaderProgram mShadowProgram; //Nuestro programa de shaders.

        private Rectangle mShadowViewport; // Viewport para el shadow mapping

        private ShaderProgram mShadowViewportProgram; //Nuestro programa de shaders.


        private bool mShowLightsView = false;
        private ViewportQuad mShadowViewportQuad;

        Matrix4 biasMatrix = new Matrix4(0.5f, 0.0f, 0.0f, 0.0f,
                                                  0.0f, 0.5f, 0.0f, 0.0f,
                                                  0.0f, 0.0f, 0.5f, 0.0f,
                                                  0.5f, 0.5f, 0.5f, 1.0f);

        Vector3 lightUp = new Vector3(0, 1, 0);

        // --- PROJECTION MATRIX ---
        // La matrix de proyeccion es una matrix ortografica que abarca toda la escena.
        // Estos valores son seleccionados de forma que toda la escena visible es incluida.

        Matrix4 lightProjMatrix = Matrix4.CreateOrthographicOffCenter(
                                                                        -10,
                                                                         10,
                                                                        -10,
                                                                         10,
                                                                         1.0f,
                                                                         100.0f);

        /// <summary>
        /// Funcion utilizada para crear el frame buffer de profundidad
        /// </summary>
        private void CrearShadowTextures()
        {
            // Necesito generar un framebuffer y una textura 2D para almacenar el depth buffer.
            TextureTarget textureTarget = TextureTarget.Texture2D;
            FramebufferTarget framebufferTarget = FramebufferTarget.Framebuffer;

            mShadowViewport = new Rectangle(0, 0, 4096, 4096);

            // 1. Genero un framebuffer.
            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(framebufferTarget, fbo);


            // 2. Genero una textura para vincular al framebuffer.
            GL.ActiveTexture(TextureUnit.Texture0 + mShadowTextureUnit);
            depthTexture = GL.GenTexture();
            GL.BindTexture(textureTarget, depthTexture);
            GL.TexImage2D(
                textureTarget,
                0,
                PixelInternalFormat.DepthComponent, // Solo voy a utilizar el componente de profundidad.
                mShadowViewport.Width,
                mShadowViewport.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent,
                PixelType.Float,
                IntPtr.Zero);

            GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(textureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(textureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            // 3. Seteo que cuando salgo de los limites de la textura sampleo el color blanco.
            float[] borderColor = { 1.0f, 0.0f, 0.0f, 0.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            // Especifico que la textura se accede como "comparación", utilizando la función "less".
            // Por esto en el shader utilizamos el sampler sampler2dshadow, que accede a la textura y devuelve como resultado 1 o 0 correspondiente a una comparación.
            // (ver el shader para más detalles al respecto)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Less);

            // 4. Vinculo la textura al framebuffer.
            GL.FramebufferTexture2D(
                framebufferTarget,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                depthTexture,
                0);
            // Para que el framebuffer este completo debo indicar que no tendrá buffer de color.
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            // 5. IMPORTANTE: Chequeo si el framebuffer esta completo.
            if (GL.CheckFramebufferStatus(framebufferTarget) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new InvalidOperationException("El framebuffer no fue completamente creado.");
            }

            //TODO PROBANDO LINEAS
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            // Regreso al framebuffer por defecto.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        //Vector3 look = new Vector3(0, 0, 75);
        Vector3 look = new Vector3(-50, 0, 200);

        //Vector3 pos = new Vector3(0, 3.5f, -7);
        Vector3 pos = new Vector3(1.5f, 1f, -2);

        private Vector3[] cameraLook = { new Vector3(0, 0, 75), new Vector3(0, 0, -75), new Vector3(-50, 0, 200) , new Vector3(0, 0, 100), new Vector3(0, 0, 5)};
        
        private Vector3[] cameraPos= { new Vector3(0, 3.5f*0.2f, -7 * 0.2f), new Vector3(0, 3.5f * 0.2f, -1 * 0.2f), new Vector3(1.5f * 0.2f, 1f * 0.2f, -2 * 0.2f) , new Vector3(-0.11f * 0.2f, 2.85f * 0.2f, -8.0f * 0.2f), new Vector3(24.80178f * 0.2f, 14.17845f * 0.2f, 17.10958f * 0.2f) };

        private int maxCamera = 5;

        private int indexCamera = 0;

        private Matrix4 viewMatrix;
        private Matrix4 projMatrix;

        /// <summary>
        /// Funcion utilizada para setear los parametros a la hora del dibujado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl3_Paint(object sender, PaintEventArgs e)
        {
            //FISICA

            //fisica
            fisica.DynamicsWorld.StepSimulation(7);

            //Cosas de las matrices fisicas
            for (int i = 0; i < fisica.getCantidad(); i++)
            {

                fisica.ListaM[i] = fisica.ListaRB[i].MotionState.WorldTransform; //provee la matriz de modelado del cuerpo rigido

            }

            player.setModelMatFisica(fisica.ListaM[player.getIndex()]);

            for (int i = 0; i < policias.Count; i++)
            {
                auto2 = policias[i];
                auto2.setModelMatFisica(fisica.ListaM[auto2.getIndex()]);
            }

            //SOMBRAS

            // Calculo la matrix MVP desde el punto de vista de la luz.
            // Ojo con el up, debe estar mirando correctamente al target.
            Vector3[] posicionesFocos = posteLuz.LucesCercanas(player.getPositionActual());
            for (int i = 0; i < luces.Length; i++)
            {
                if (i > 0 && i < 4)
                {
                    luces[i].Position = new Vector4(posicionesFocos[i - 1], 1);
                }
            }

            Vector3 lightEye = luces[2].Position.Xyz;
            Vector3 lightTarget = player.getPositionActual(); //- new Vector3(0.2f,20,0.2f);
            // --- VIEW MATRIX ---
            Matrix4 lightViewMatrix = Matrix4.LookAt(lightEye, lightTarget, lightUp);

           // Console.WriteLine(lightTarget + " "+player.getPositionActual());
           

            // --- VIEW PROJECTION ---
            // La matrix de modelado es la identidad.
            Matrix4 lightSpaceMatrix = lightViewMatrix * lightProjMatrix;

            // --- RENDER ---
            // 1. Se renderiza la escena desde el punto de vista de la luz
            GenerarShadowMap(lightSpaceMatrix);
            

            
            //SE LLAMA A LA FUNCION  MOVE DE LA CAMARA (SOLO FUNCIONA CON QUATCAMERA Y CAMERA)
            myCamera.move();
            
            viewMatrix = myCamera.getViewMatrix();
            projMatrix = myCamera.getProjectionMatrix();
            
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); //Borramos el contenido del glControl.
            
            gl.Viewport(viewport); //Especificamos en que parte del glControl queremos dibujar.

            //para mantener la relacion de aspecto con el viewport
            myCamera.setAspectRatio(((float)viewport.Width)/((float)viewport.Height));

            try
            {
                sProgram.Activate();


                sProgram.SetUniformValue("luzPlayer",2);
                sProgram.SetUniformValue("projMat", projMatrix);
                
                sProgram.SetUniformValue("cameraPosition", myCamera.getPosition());

                //SOMBRAS
                Matrix4 lightBiasMatrix = lightSpaceMatrix * biasMatrix;

                //SIN BIAS
                sProgram.SetUniformValue("uLightBiasMatrix", lightBiasMatrix);
                sProgram.SetUniformValue("uShadowSampler", mShadowTextureUnit);



                //MULTIPLES LUCES
                
                
                cargarLuces(sProgram);
                
                player.move(fisica);

                
                //SE LE SETEA LA POSICION A LA CAMARA (SOLO SI ES SPHERICAL ESTO FUNCIONA)
                {
                    Vector3 look = cameraLook[indexCamera];
                    
                    Vector3 pos = cameraPos[indexCamera];
                    look = look - pos;
                    look = Vector3.Transform(look, player.getModelMatrix());
                    pos = Vector3.Transform(pos, player.getModelMatrix());
                    myCamera.setLookAt(look);
                    myCamera.setPosition(pos);
                    viewMatrix = myCamera.getViewMatrix();

                }
                    
                player.setViewMatrix(viewMatrix);
                player.Dibujar(sProgram, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);
                
                //auto2
                for (int i = 0; i < policias.Count; i++)
                {   
                    auto2 = policias[i];
                    if(!primeraVezPolicia)
                        auto2.move(fisica);
                    auto2.setViewMatrix(viewMatrix);
                    auto2.Dibujar(sProgram, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);
                }

                //POSTES DE LUZ
                posteLuz.setViewMatrix(myCamera.getViewMatrix());
                for (int i = 0; i < posteLuz.cantPostes(); i++)
                {
                    posteLuz.Dibujar(sProgram, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);
                }

                //PALMERAS
                palmeras.setViewMatrix(myCamera.getViewMatrix());
                for (int i = 0; i < palmeras.cantPalmeras(); i++)
                {
                    palmeras.Dibujar(sProgram, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);
                }

                
                // CIELO
                /**/
                cielo.setViewMatrix(viewMatrix);
                cielo.Dibujar(sProgram, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);

                //Seniales, tengo que hacer un for debido a que tengo mas de una
                for (int i = 0; i < SenialLimiteVelocidad.cantSeniales(); i++)
                {
                    SenialLimiteVelocidad.setViewMatrix(viewMatrix);
                    SenialLimiteVelocidad.Dibujar(sProgram, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);
                }

                sProgram.Deactivate(); //Desactivamos el programa de shader.

                //Esto dibuja con OREN NAYAR
                sProgramON.Activate();
                {
                    sProgramON.SetUniformValue("projMat", projMatrix);

                    sProgramON.SetUniformValue("cameraPosition", myCamera.getPosition());
                    sProgramON.SetUniformValue("projMat", projMatrix);

                    sProgramON.SetUniformValue("uShadowSampler", mShadowTextureUnit);
                    sProgramON.SetUniformValue("uLightBiasMatrix", lightBiasMatrix);

                    sProgramON.SetUniformValue("luzPlayer", 2);


                    cargarLuces(sProgramON);

                    viewMatrix = myCamera.getViewMatrix();

                    //TIERRA
                    tierra.setViewMatrix(viewMatrix);
                    tierra.setModelMatFisica(fisica.ListaM[tierra.getIndex()]);

                    tierra.Dibujar(sProgramON, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);

                    //CIUDAD
                    ciudad.setViewMatrix(viewMatrix);
                    ciudad.Dibujar(sProgramON, TextureUnit.Texture1, 1, TextureUnit.Texture2, 2);


                }
                sProgramON.Deactivate();


                if (mShowLightsView)
                {
                    // Para dibujar debeo cambiar la configuración de la textura con los valores de profundidad.
                    // Indico que no es más una textura con acceso de "comparación".
                    GL.ActiveTexture(TextureUnit.Texture0 + mShadowTextureUnit);
                    GL.BindTexture(TextureTarget.Texture2D, depthTexture);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);

                    // Dibujo el contenido de la textura de profundidad.
                    DibujarShadowMap();

                    // Vuelvo a configurar la textura con valores de profundidad para que sea accedida como comparación.
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Less);
                }

                glControl3.SwapBuffers(); 
                //Intercambiamos buffers frontal y trasero, para evitar flickering.

                if (primeraVez)
                {
                    primeraVez = false;
                    player.posicionEnPistaCercana(fisica,player.getPositionActual());
                   
                }

                if (primeraVezPolicia &&  DateTime.Now.TimeOfDay.TotalMilliseconds - tiempo > 15000)
                {
                    primeraVezPolicia = false;
                     for (int i = 0; i < policias.Count; i++)
                    {
                        policias[i].posicionEnPistaCercana(fisica, policias[i].getPositionActual());
                    } 
                }

            }
            catch (Exception ex )
            {
                Console.WriteLine("Error en dibujar "+ ex);
            }
        }

        private bool primeraVez = true;
        private bool primeraVezPolicia = true;
        double tiempo;
        /// <summary>
        /// funcion encargada de dibujar el viewport para el mapa de sombras
        /// </summary>
        //SOMBRAS DIBUJAR VIEWPORT
        private void DibujarShadowMap()
        {
            // --- SETEO EL ESTADO ---
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Rectangle viewport2 = new Rectangle(0, 0, viewport.Width / 4, viewport.Height / 4);
            GL.Viewport(viewport2);

            mShadowViewportProgram.Activate();

            Matrix4 projectionMatrix = Matrix4.CreateOrthographicOffCenter(
                viewport2.Left,
                viewport2.Right,
                viewport2.Top,
                viewport2.Bottom,
                0.0f,
                1.0f);

            Vector2 viewportSize = new Vector2(viewport2.Width, viewport2.Height);

            // --- SETEO UNIFORMS ---
            mShadowViewportProgram.SetUniformValue("uViewportOrthographic", projectionMatrix);
            mShadowViewportProgram.SetUniformValue("uViewportSize", viewportSize);
            mShadowViewportProgram.SetUniformValue("uShadowSampler", mShadowTextureUnit);

            // --- DIBUJO ---
            mShadowViewportQuad.Dibujar(mShadowViewportProgram);

            mShadowViewportProgram.Deactivate();
        }
        /// <summary>
        /// funcion encargada de generar el mapa de sombras, carga el frame buffer de la profundidad
        /// </summary>
        /// <param name="lightSpaceMatrix"> matriz de transformacion al espacio de la luz</param>
        //SOMBRAS GENERAR
        private void GenerarShadowMap(Matrix4 lightSpaceMatrix)
        {

            // --- SETEO EL ESTADO ---
            GL.Enable(EnableCap.DepthTest); // Habilito el uso del buffer de profundidad.
             GL.Viewport(mShadowViewport);

            // Para evitar el shadow acne almacenaremos el valor de profundidad con cierto offset.
            // De esta manera al comparar fragmentos, estará más marcada la diferencia de si debe ser sombreado o no.
            // Pueden probar comentar estas dos líneas para ver el efecto del shadow acne (noten que esta es una posible solución, existen otras, incluso mejores).
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1.0f, 1.0f);

            // Limpio el framebuffer el contenido de la pasada anterior.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 modelMatrix = Matrix4.Identity;

            mShadowProgram.Activate();

            // La matriz es uniforme a todos los objetos renderizados.
            mShadowProgram.SetUniformValue("uLightSpaceMatrix", lightSpaceMatrix);


            GL.Enable(EnableCap.CullFace); // Habilito que no se renderize alguna de las caras de cada triángulo. Falta indicar que cara no se renderiza.
            GL.CullFace(CullFaceMode.Front); // Indico que no se dibuje la cara de frente. Con esto evitamos en cierta medida la "pelea" que se genera entre fragmentos con similar valor de profundidad.

            //jugador
            mShadowProgram.SetUniformValue("uModelMatrix", player.getModelMatrix());
            player.DibujarShadows(mShadowProgram);


            //policias
            for (int i = 0; i < policias.Count; i++)
            {
                auto2 = policias[i];
                mShadowProgram.SetUniformValue("uModelMatrix", auto2.getModelMatrix());
                auto2.DibujarShadows(mShadowProgram);
            }

            //tierra
            //mShadowProgram.SetUniformValue("uModelMatrix", Matrix4.Identity);
            //tierra.DibujarShadows(mShadowProgram);

            GL.Disable(EnableCap.CullFace);


            //HASTA ACA SE DIBUJA

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            mShadowProgram.Deactivate();
            
            GL.Disable(EnableCap.PolygonOffsetFill);
        }



        private void glControl3_Resize(object sender, EventArgs e)
        {   
            //Actualizamos el viewport para que dibuje en el centro de la pantalla.
            Size size = glControl3.Size;

            viewport.X = 0;
            viewport.Y = 0;
            viewport.Width = size.Width;
            viewport.Height = size.Height;
            
            glControl3.Invalidate(); //Invalidamos el glControl para que se redibuje.(llama al metodo Paint)
        }
        /// <summary>
        /// funcion para setear los shaders a utilizar en el proyecto
        /// </summary>
        /// <param name="vShaderName"></param>
        /// <param name="fShaderName"></param>
        /// <returns></returns>
        private ShaderProgram SetupShaders(String vShaderName, String fShaderName)
        {
            //Lo hago con mis clases, que encapsulan el manejo de shaders.
            //1. Creamos los shaders, a partir de archivos.

            String vShaderFile = "files/shaders/"+ vShaderName;
            String fShaderFile = "files/shaders/"+ fShaderName;
            Shader vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            Shader fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            try
            {
                vShader.Compile();
                fShader.Compile();
            }
            catch(ShaderCompilationException ex)
            {
                Console.WriteLine(ex);
            }
            //3. Creamos el Programa de shader con ambos.
            ShaderProgram sProgram = new ShaderProgram();
            sProgram.AddShader(vShader);
            sProgram.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgram.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();

            return sProgram;
        }

        private void logContextInfo()
        {
            String version, renderer, shaderVer, vendor;//, extensions;
            version = gl.GetString(StringName.Version);
            renderer = gl.GetString(StringName.Renderer);
            shaderVer = gl.GetString(StringName.ShadingLanguageVersion);
            vendor = gl.GetString(StringName.Vendor);
            //extensions = gl.GetString(StringName.Extensions);
            log("========= CONTEXT INFORMATION =========");
            log("Renderer:       {0}", renderer);
            log("Vendor:         {0}", vendor);
            log("OpenGL version: {0}", version);
            log("GLSL version:   {0}", shaderVer);
            //log("Extensions:" + extensions);
            log("===== END OF CONTEXT INFORMATION =====");

        }
        private void log(String format, params Object[] args)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(format, args), "[CGUNS]");
        }

        //private int x= 1, y = 20, z=0;
        private Edificio ciudad;

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
            timer1.Dispose();
            fisica.dispose();
            reproductor.Stop();
            reproductor.Dispose();
            Console.WriteLine("Exit");
        }
        /// <summary>
        /// metodo encargado de crear las luces correspondientes a utilizar
        /// </summary>
        /// <param name="i"> cantidad de luces a crear</param>
        private void crearLuces(int i)
        {
            //LUCES
            luces[0] = new Light();
            luces[0].Position = new Vector4( 70f, 200f, -118f, 0.0f); //Directional light(hacia +y)
            luces[0].Iambient = new Vector4(0.3f, 0.3f, 0.3f, 1);
            luces[0].Idiffuse = new Vector4(1f, 0.856f, 0.66f, 1);
            luces[0].Ispecular = new Vector4(1f, 0.856f, 0.66f, 1);
            luces[0].ConeAngle = 12.0f; //NOT USED IN DIRECTIONAL
            luces[0].ConeDirection = new Vector3(0.0f, 1.0f, 0.0f);//NOT USED IN DIRECTIONAL
            luces[0].Enabled = 1;

            luces[1] = new Light();
            //luces[1].Position = new Vector4(x, y, z, 1.0f); //spot desde arriba
            luces[1].Iambient = new Vector4(0.0f, 0.0f, 0.0f, 1);
            luces[1].Idiffuse = new Vector4(1f, 1f, 1f, 1);
            luces[1].Ispecular = new Vector4(1f, 1f, 1f, 1);
            luces[1].ConeAngle = 30.0f;
            luces[1].ConeDirection = new Vector3(0, -1.0f, 0);
            luces[1].Enabled = 1;

            //luz del primer poste
            luces[2] = new Light();
            //luces[2].Position = new Vector4(3.0f, 10f, 0.0f, 1.0f); //spot desde arriba
            luces[2].Iambient = new Vector4(0.0f, 0.0f, 0.0f, 1);
            luces[2].Idiffuse = new Vector4(1f, 1f, 1f, 1);
            luces[2].Ispecular = new Vector4(1f, 1f, 1f, 1);
            luces[2].ConeAngle = 30.0f;
            luces[2].ConeDirection = new Vector3(0, -1.0f, 0);
            luces[2].Enabled = 1;

            luces[3] = new Light();
            //luces[3].Position = new Vector4(-3.0f, 10f, 0.0f, 1.0f); //spot desde arriba
            luces[3].Iambient = new Vector4(0.0f, 0.0f, 0.0f, 1);
            luces[3].Idiffuse = new Vector4(1f, 1f, 1f, 1);
            luces[3].Ispecular = new Vector4(1f, 1f, 1f, 1);
            luces[3].ConeAngle = 30.0f;
            luces[3].ConeDirection = new Vector3(0, -1.0f, 0);
            luces[3].Enabled = 1;

        }
        /// <summary>
        /// metodo utilizado para cargar las luces en los shaders
        /// </summary>
        /// <param name="sProgram"> shader program a utilizar</param>
        private void cargarLuces(ShaderProgram sProgram)
        {
            sProgram.SetUniformValue("numLights", luces.Length);

            for (int i = 0; i < luces.Length; i++)
            {
                

                sProgram.SetUniformValue("allLights[" + i + "].position", luces[i].Position);
                sProgram.SetUniformValue("allLights[" + i + "].Ia", luces[i].Iambient);
                sProgram.SetUniformValue("allLights[" + i + "].Id", luces[i].Idiffuse);
                sProgram.SetUniformValue("allLights[" + i + "].Is", luces[i].Ispecular);
                sProgram.SetUniformValue("allLights[" + i + "].coneAngle", luces[i].ConeAngle);
                sProgram.SetUniformValue("allLights[" + i + "].coneDirection", luces[i].ConeDirection);
                sProgram.SetUniformValue("allLights[" + i + "].enabled", luces[i].Enabled);
            }

            //ESTO es para borrar solo dibuja esferas 

        }

        /// <summary>
        /// handler cuando se libera la tecla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl3_KeyUp(object sender, KeyEventArgs e)
        {
            if (flagCamara == 0)
            {
                player.keyreleased(e);
            }
            else
            {
                myCamera.keyReleased(e);
            }

        }
        /// <summary>
        /// handler utilizado cuando se aprieta una tecla, aqui indicaremos que se hace con cada tecla apretada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.T)
            {
                flagCamara = (flagCamara + 1) % cantTiposCamara;
                if (flagCamara == 0)
                {
                    myCamera = new SphericalCamera();
                }
                if (flagCamara == 1)
                {
                    myCamera = new Camera();
                }
                if (flagCamara == 2)
                {
                    myCamera = new QuatCamera();
                }
            }
            if (e.KeyCode == Keys.V)
            {
                indexCamera = (indexCamera + 1) % maxCamera;
            }

            //SI ES LA CAMARA DEL AUTO SE MUEVE EL AUTO
            if (flagCamara == 0){
                player.keypressed(e, fisica);
            }
            else
            {
                myCamera.keypressed(e);
            }
            

            if (e.KeyCode == Keys.O)
            {
                reproductor.Stop();
                indMusica = (indMusica + 1) % musica.Length;
                reproductor.Open(musica[indMusica]);
                reproductor.Volume = 5;
                reproductor.Play();
            }
            
            //PARA OBTENER POSICIONES Y UBICAR OBJETOS
            
            if (e.KeyCode == Keys.P)
                Console.WriteLine(myCamera.getPosition());
            /**/
        }
        
        private void timer_tick(object sender, EventArgs e)
        {
            try
            {
                glControl3.Invalidate();
            }
            catch(System.NullReferenceException ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
