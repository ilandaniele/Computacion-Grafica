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
using IrrKlang;

namespace Labo0.CGUNS.Meshes.FaceVertexList
{
    class Auto : MeshObject
    {   
        /// <summary>
        /// Clase Auto: en esta clase modelamos el comportamiento de cada auto en base a la fisica y su sonido.
        /// </summary>
        ISoundEngine soundEngine;
        ISound sonidoColision;
        ISound sonidoAcelerar;
        ISound sonidoMilico;
        String[] SonidosColision =
        {
           "files/Sonido/colision1.mp3", "files/Sonido/colision2.mp3", "files/Sonido/colision3.mp3", "files/Sonido/colision4.mp3", "files/Sonido/grito.mp3"
        };
        /// <summary>
        /// Constructor de la clase Auto
        /// </summary>
        /// <param name="file">Este es el archivo OBJ del auto</param>
        /// <param name="mtl">Este es el archivo mtl del OBJ</param>
        /// <param name="posicionInicial">Esta es la posición inicial del auto en la escena</param>
        /// <param name="texturado"></param>
        public Auto(String file, String mtl, Vector3 posicionInicial, bool texturado) 
            : base(file, mtl, posicionInicial, texturado)
        {
            //Matrix4 bajar = Matrix4.CreateRotationX(-(float)Math.PI / 2);
            Matrix4 escala = Matrix4.CreateScale(1f, 1f, 1f);
            modelMat = escala;
            tAnt = DateTime.Now.TimeOfDay.TotalMilliseconds;
            //TODO esto no es asi.... debo setearselo a la fisica
            posAnt= posSig = posicionInicial;
            mass = 1500;
            soundEngine = new ISoundEngine();
        }

       


        //Parámetros para fisica
        private Vector3 posAnt;
        private Vector3 posSig;
        private Vector3 deltaPos;
        private Vector3 orientacionAnt;
        private double tAnt;
        private double tSig;

        private float vida = 1.0f;

        /// <summary>
        /// Devuelve la posicion actual del objeto dinamico
        /// </summary>
        /// <returns> Posicion en el espacio del mundo</returns>
        public Vector3 getPositionActual()
        {
            return posAnt;
        }

        private double timePast = DateTime.Now.TimeOfDay.TotalMilliseconds;

        /// <summary>
        /// Le resta la cantidad pasada por parametro a la vida al Auto
        /// </summary>
        /// <param name="dam"> cantidad de daño</param>
        public void setDamage(float dam)
        {   
            if(DateTime.Now.TimeOfDay.TotalMilliseconds - timePast > 1000)
            {
                timePast = DateTime.Now.TimeOfDay.TotalMilliseconds;
                vida -= dam;
                Console.WriteLine(principal + " " + vida);
            }
            if(vida<=0 && !principal)
            {
                sonidoMilico.Stop();
            }
            
        }

        private float pi = (float)(2 * Math.PI);

        private Matrix4 correccion = Matrix4.CreateTranslation(0, -0.065f, 0);
        Matrix4 mvMatrix;

        /// <summary>
        /// retorna la matriz de modelado actual del objeto 
        /// </summary>
        /// <returns> Matriz de modelado</returns>
        public Matrix4 getModelMatrix()
        {
            return mvMatrix;
        }

        /// <summary>
        /// setea la matriz de modelado creada por la fisica y le aplica una correccion a la distancia a la superficie
        /// </summary>
        /// <param name="mod"> Matriz de modelado externa</param>
        public override void setModelMatFisica(Matrix4 mod)
        {
            modelMatFisica = mod;
            mvMatrix = Matrix4.Identity;
            mvMatrix = Matrix4.Mult(modelMatFisica, mvMatrix);

            //para corregir la textura
            mvMatrix = Matrix4.Mult(correccion, mvMatrix);

        }

        /// <summary>
        /// Sobreescribe a la funcion dibujar de la clase MeshObject
        /// </summary>
        /// <param name="sProgram"></param>
        /// <param name="textUnit"> Unidad de textura para el texturado</param>
        /// <param name="unit"> numero de la unidad de textura</param>
        /// <param name="textUnitNormal">Unidad de textura para el mapa de normales</param>
        /// <param name="unitNormal">numero de la unidad de textura para mapa de noemales</param>
        /// 

        public override void Dibujar(ShaderProgram sProgram, TextureUnit textUnit, int unit, TextureUnit textUnitNormal, int unitNormal)
        {
            sProgram.SetUniformValue("n1", 1.0f);//coheficiente del aire
            sProgram.SetUniformValue("n2", 2.0f);//coheficiente del material
            sProgram.SetUniformValue("k", 9.0f);//metalicidad
            sProgram.SetUniformValue("m", 0.05f);//tamaño del brillo

            sProgram.SetUniformValue("flagLuz", 1); //para habilitar o desabilitar la luz

            sProgram.SetUniformValue("flagRelieve", 0); //para habilitar o desabilitar el mapa de normales
            
          
            sProgram.SetUniformValue("viewMatrix", viewMatrix);


            //seteo la matriz de transformacion de las normales
            Matrix3 MatNorm = new Matrix3(mvMatrix);
            MatNorm = Matrix3.Transpose(Matrix3.Invert(MatNorm));



            Vector4 Kd = new Vector4(1, 0, 0, 1);
            Vector4 Ka = new Vector4(0.1f, 0.1f, 0.1f, 1);
            Vector4 Ke = new Vector4(1, 1, 1, 1);

            //seteo los Ka Ke y Kd
            sProgram.SetUniformValue("ka", Ka);
            //sProgram.SetUniformValue("kd", Kd);
            sProgram.SetUniformValue("ke", Ke);
            sProgram.SetUniformValue("flagRelieve", 0);


            //Este for sirve para dibujar las Meshes podemos modificar cada una de ellas si sabemos cual es cual o le damos a la clase FVLMesh los atributos
            for (int i = 0; i < objeto.Count; i++)
            {
                FVLMesh mesh = objeto[i];
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

                //verifico si la mesh es una rueda
                if (esRuedaCharguer(mesh) || esRuedaPolicia(mesh))
                {

                    Vector3 centro = mesh.getCentroX();
                    float radio = mesh.getRadio();
                    float dot = Vector3.Dot(direccion(), velocidad);


                    //para que dot quede entre -1 y 1
                    while (dot > 1)
                    {
                        dot -= 2;
                    }
                    while (dot < -1)
                    {
                        dot += 2;
                    }

                    float coef = (float)Math.Acos(dot);

                    coef = coef * (deltaPos.LengthSquared) / radio;
                    

                    if (dirZ != 0)
                        coef = coef * -dirZ;

                    theta = theta + coef;


                    //para que theta quede entre 0 y pi(que en realidad es 2pi)
                    while (theta > pi)
                    {
                        theta -= pi;
                    }
                    while (theta < 0)
                    {
                        theta += pi;
                    }


                    Matrix4 rot = crearRotacionX(centro);


                    if (esRuedaDelanteraCharguer(mesh, i))
                    {
                        centro = mesh.getCentroY();
                        //alfa += 0.002f;
                        rot = crearRotacionY(centro, rot);

                    }
                    rot = Matrix4.Mult(rot, mvMatrix);

                    sProgram.SetUniformValue("modelMat", rot);
                    //sProgram.SetUniformValue("mvMatInvert", Matrix4.Invert(mvMatrix));
                    //guardamos la matriz de model view


                    //seteo la matriz de transformacion de las normales
                    Matrix3 MatNormRot = new Matrix3(rot);
                    MatNormRot = Matrix3.Transpose(Matrix3.Invert(MatNormRot));

                    sProgram.SetUniformValue("MatrixNormal", MatNormRot);
                }
                else
                {
                    sProgram.SetUniformValue("modelMat", mvMatrix);
                    sProgram.SetUniformValue("MatrixNormal", MatNorm);
                }
               
                mesh.Dibujar(sProgram);
                
                //base.Dibujar(sProgram, textUnit, unit);
            }

        }

        /// <summary>
        /// Setea una lista de Auto con los enemigos de la escena
        /// </summary>
        /// <param name="policias">  Lista de Auto </param>
        internal void setPolicias(List<Auto> policias)
        {
            this.policias = policias;
            
        }

        private float theta = 0;
        private float alfa = 0;
        
        protected Auto player;

        /// <summary>
        /// setea el Auto del jugador, se utiliza en general para conocer la ubicacion del mismo
        /// </summary>
        /// <param name="p"> parametro de tipo Auto que referencia al jugador</param>
        public void setPlayer(Auto p)
        {
            player = p;
        }

        private bool principal = false;
        /// <summary>
        /// Setea un flag para el comportamiento tipico del jugador que luego 
        /// se utiliza para diferenciar entre los enemigos y el jugador
        /// </summary>
        public void setPrincipal()
        {
            principal = true;
            vida = 400f;
            //cambio = 2.0f;
            //potenciaDoblar = 1.7f;
        }
        
        private int dirX = 0;
        private int dirZ = 0;
        private int dirZAnt = 0;
        private float potenciaDoblar = 0.5f;
        private Vector3 velocidad;

        /// <summary>
        /// retorna el vector velocidad acual del Auto
        /// </summary>
        /// <returns> Vector3 velocidad</returns>
        public Vector3 getVelocidad()
        {
            return velocidad;
        }
        
        /// <summary>
        /// Rota el auto hacia la izquierda
        /// </summary>
        /// <param name="fisica"></param>
        /// <param name="velocidad"></param>
        /// 
        private void getRotacionIzquierda(fisica fisica, Vector3 velocidad)
        {
            
            {
                fisica.aplicarVelocidadAngular(dirZAnt * Vector3.Transform(new Vector3(0, potenciaDoblar, 0), modelMatFisica.ClearTranslation()), index);
            }

            if(alfa < pi / 8)
            {
                alfa += 0.1f ;
            }

        }

        /// <summary>
        /// Rota el auto hacia la derecha
        /// </summary>
        /// <param name="fisica"></param>
        /// <param name="velocidad"></param>

        private void getRotacionDerecha(fisica fisica, Vector3 velocidad)
        {
            
            {
                fisica.aplicarVelocidadAngular(dirZAnt * Vector3.Transform(new Vector3(0, -potenciaDoblar, 0), modelMatFisica.ClearTranslation()), index);
            }

            if (alfa > - pi / 8)
            {
                alfa -= 0.1f;
            }

        }


        private float cambio = 0.7f;

        /// <summary>
        /// Retorna la direccion actual del auto
        /// </summary>
        /// <returns> Vector3 Direccion actual</returns>
        public Vector3 direccion()
        {
            return Vector3.Transform(new Vector3(0, 0, 1), modelMatFisica.ClearTranslation());
        }

        /// <summary>
        /// Retorna la direccion bitangente del auto
        /// </summary>
        /// <returns> Vector3 Direccion bitangente actual</returns>
        public Vector3 direccionBitangente()
        {
            return Vector3.Transform(new Vector3(1, 0, 0), modelMatFisica.ClearTranslation());
        }

        private Vector3 refZ = new Vector3(0, 0, 1);

        private bool primeravez = true;
        private bool flagT = true;
        double tiempo;

        /// <summary>
        /// Esta función modela el comportamiento de los vehiculos dependiendo de las variables seteadas
        /// de forma externa
        /// </summary>
        /// <param name="fisica"> objeto que modela la fisica de la escena</param>
        public override void move(fisica fisica)
        {
            if(posAnt.LengthSquared> 60000 || posAnt.Y<-15)
            {
                posicionEnPistaCercana(fisica,posAnt);
            }

            if (vida > 0) { 
                Vector3 dir = direccion();
                

                tSig = DateTime.Now.TimeOfDay.TotalMilliseconds;
                if (primeravez)
                    primeravez = false;
                else
                    posSig = Vector3.Transform(new Vector3(0.01f, 1, 0.01f), modelMatFisica);

                float deltaT = (float)(tSig - tAnt);
                if (deltaT == 0)
                    deltaT = 0.00001f;
                velocidad = (posSig / deltaT - posAnt / deltaT);//fisica.ListaRB[index].LinearVelocity;//(posSig / deltaT - posAnt / deltaT);

                if (dirZ != 0)
                {
                    dirZAnt = -dirZ;
                    dir.Y = 0;
                    fisica.aplicarFuerza(dir * cambio * -dirZ, aplicarTransformacion(new Vector3(0, 0, 0)), index);
                }
                if (dirX != 0 && seMueve(velocidad))
                    if (dirX == -1)
                    {
                        getRotacionDerecha(fisica, velocidad);
                    }
                    else
                    {
                        getRotacionIzquierda(fisica, velocidad);
                    }
                else
                {
                    if (dirX == 0)
                    {
                        if (alfa > 0)
                            alfa -= 0.1f;
                        else if (alfa < 0)
                            alfa += 0.1f;
                        if (Math.Abs(alfa) < 0.1f)
                            alfa = 0;
                    }

                    /**/
                    //TODO aca tengo que hacer algo para la rotacion de las ruedas
            }

            float angulo;
            
            Vector3 rozamiento = new Vector3(-velocidad);
            rozamiento.Y = 0;
            rozamiento.Normalize();

            //esta velocidad es casi 0 pero en 0 da un bug por eso se tiene en cuenta
            //si la altura supera 1.5 entonces no hay rozamiento (el auto esta en el aire)
            if (velocidad.LengthSquared > 0.00001)
            {
                //TODO VER ESTO DEL ROZAMIENTO
                rozamiento = rozamiento * 0.15f;
                if (velocidad.LengthSquared > 0.001)
                {
                    //aplico fuerza de rozamiento a las 4 ruedas
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(0.5f, 0, 1.25f)), index);
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(-0.5f, 0, 1.25f)), index);
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(0.5f, 0, -1.25f)), index);
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(-0.5f, 0, -1.25f)), index);
                    
                }
                rozamiento = rozamiento * 0.9f;
                //cuando el auto esta en movimiento de costado se tiene que frenar antes
                Vector2 dirXZ = new Vector2(dir.X, dir.Z);
                Vector2 velXZ = new Vector2(velocidad.X, velocidad.Z);
                dirXZ.Normalize();
                velXZ.Normalize();
                angulo = (float)Math.Acos(Vector2.Dot(velXZ, dirXZ));
                if (angulo > 0.38f && angulo < Math.PI - 0.38f || angulo < -0.38f && angulo > Math.PI + 0.35f)
                {
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(0.5f, 0, 1.25f)), index);
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(-0.5f, 0, 1.25f)), index);
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(0.5f, 0, -1.25f)), index);
                    fisica.aplicarFuerza(rozamiento, aplicarTransformacion(new Vector3(-0.5f, 0, -1.25f)), index);
                    //i++;
                }

            }
            /**/
            //con esto freno el auto para que no rote exageradamente
            angulo = (float)Math.Acos(Vector3.Dot(orientacionAnt, dir));
            if (angulo > 0)
            {
                //es una correccion de la velocidad angular en Y
                fisica.clearForces(index);
            }

            orientacionAnt = dir;
            deltaPos = posSig - posAnt;
            posAnt = posSig;
            tAnt = tSig;

            if (!principal)
            {
                moverComportamiento(fisica, dir);
            }
            
            corregir(fisica, dir);
                if (principal)
                {
                    checkColition(fisica, policias);
                    if (dirZ == -1)
                    {
                        if (flagT == true)
                        {
                            tiempo = DateTime.Now.TimeOfDay.TotalMilliseconds;
                            flagT = false;
                        }
                        if (!soundEngine.IsCurrentlyPlaying("files/Sonido/aceleracionInicial.mp3") && !soundEngine.IsCurrentlyPlaying("files/Sonido/aceleracionFinal.mp3"))
                        {
                            if (DateTime.Now.TimeOfDay.TotalMilliseconds - tiempo > 17000d)
                            {
                                //sonidoAcelerar.Stop();
                                sonidoAcelerar = soundEngine.Play2D("files/Sonido/aceleracionFinal.mp3", true);

                            }
                            else
                            {
                                sonidoAcelerar = soundEngine.Play2D("files/Sonido/aceleracionInicial.mp3", false);
                            }
                        }
                    }
                    if (dirZAnt == 1 && dirZ == 0)
                    {
                        if (soundEngine.IsCurrentlyPlaying("files/Sonido/aceleracionInicial.mp3") || soundEngine.IsCurrentlyPlaying("files/Sonido/aceleracionFinal.mp3"))
                        {
                            sonidoAcelerar.Stop();
                            flagT = true;

                        }
                    }

                }  
            }
        }


        /// <summary>
        /// checkea la distancia al jugador y reinicia si es necesario
        /// </summary>
        /// <param name="fisica"> objeto que modela la fisica de la escena</param>
        private void checkearDistancia(fisica fisica)
        {
            if ((player.getPositionActual() - getPositionActual()).LengthSquared > 4000)
            {
                posicionEnPistaCercana(fisica,player.getPositionActual());
            }
        }


        /// <summary>
        /// Simula el comportamiento del auto enemigo
        /// </summary>
        /// <param name="fisica"> objeto que modela la fisica de la escena</param>
        /// <param name="dir"></param>
        private void moverComportamiento(fisica fisica, Vector3 dir)
        {
            checkearDistancia(fisica);
            Vector3 pos = player.getPositionActual();

            //el vector que va desde la posicin del jugador a la actual del auto
            pos = pos - posAnt;
            if (deltaPos.Y < 0.1) 
            {
                pos.Normalize();
                dir.Normalize();
                Vector3 reference = new Vector3(0, 1, 0);
                Vector3 giro = Vector3.Cross(dir, pos);
                giro.Normalize();
                reference.Normalize();
                float angulo = (float)Math.Acos(Vector3.Dot(reference, giro));

                float anguloGiro = (float)Math.Acos(Vector3.Dot(pos, dir));

                if (giro.Y > 0)
                {
                    if (anguloGiro > 0.25)
                    {

                        dirX = 1;
                    }

                    else dirX = 0;
                }

                else if (giro.Y < 0)
                {
                    if (anguloGiro > 0.25)
                    {
                        dirX = -1;

                    }

                    else dirX = 0;
                }
                
                //esto es para que haga marcha atras
                if (anguloGiro > 0 && anguloGiro < Math.PI - 0.5f
                    || anguloGiro > Math.PI + 0.5f && anguloGiro < Math.PI * 2 - 0.5f)
                    dirZ = -1;
                else
                {
                    dirZ = 1;
                    dirX = 1;
                }

            }
            if (!soundEngine.IsCurrentlyPlaying("files/Sonido/sirena.mp3"))
            {
                sonidoMilico = soundEngine.Play3D("files/Sonido/sirena.mp3", 0, 0, 0, true);
                //sonidoMilico.Position = new Vector3D(0, player.getPositionActual().Y, player.getPositionActual().Z);
                
            }
            pos = player.getPositionActual() - posAnt;
            float dis = (pos.Length > 5) ? pos.Length : 5;
            sonidoMilico.Volume = 5f / dis;
        }

        /// <summary>
        /// funcion que corrige el problema de la mala rotacion 
        /// </summary>
        /// <param name="fisica"> objeto que modela la fisica de la escena</param>
        /// <param name="dir"></param>

        private void corregir(fisica fisica, Vector3 dir)
        {
            float pot = 0.1f;

            Vector3 bit = direccionBitangente();

            Vector3 fza = new Vector3(0,-pot,0);

           
            fisica.aplicarFuerza(fza, aplicarTransformacion(new Vector3(0.5f, 0, 1.25f)), index);
            fisica.aplicarFuerza(fza, aplicarTransformacion(new Vector3(-0.5f, 0, 1.25f)), index);
            fisica.aplicarFuerza(fza, aplicarTransformacion(new Vector3(0.5f, 0, -1.25f)), index);
            fisica.aplicarFuerza(fza, aplicarTransformacion(new Vector3(-0.5f, 0, -1.25f)), index);
             
            
        }

        /// <summary>
        /// checkea la colision entre los autos y llama a setDamage si es necesario
        /// </summary>
        /// <param name="fisica"> objeto que modela la fisica de la escena</param>
        /// <param name="list"> lista de autos de policia</param>
        private void checkColition(fisica fisica, List<Auto> list)
        {
            
            for(int i =0; i < list.Count; i++)
            {
                Auto auto = list[i];
                bool aux = fisica.colition(index, auto.getIndex());
                
                if (aux)
                {
                    float damage = auto.getVelocidad().LengthSquared;
                    Vector3 dirAuto = new Vector3(auto.getVelocidad());
                    dirAuto.Normalize();
                    Vector3 posAuto = auto.getPositionActual();

                    Vector3 dir = new Vector3(velocidad);
                    dir.Normalize();
                    Vector3 pos = getPositionActual();


                    Vector2 dirAuto2D = new Vector2(dirAuto.X, dirAuto.Z);
                    Vector2 posAuto2D = new Vector2(posAuto.X, posAuto.Z);

                    Vector2 dir2D = new Vector2(dir.X, dir.Z);
                    Vector2 pos2D = new Vector2(pos.X, pos.Z);

                    Vector2 toAuto = posAuto2D - pos2D;

                    float angle = Vector2.Dot(toAuto, dir2D);
                    //lo choque
                    if (angle > 0 && velocidad.LengthSquared > 0)
                    {
                        auto.setDamage(velocidad.LengthSquared * 1000f);
                        //colision.Open("files/Sonido/metal.wav");
                        //colision.Play();
                    }

                    toAuto = -posAuto2D + pos2D;

                    angle = Vector2.Dot(toAuto, dirAuto2D);
                    //me choco
                    if (angle > 0 && dirAuto.LengthSquared > 0)
                    {
                        setDamage(dirAuto.LengthSquared*10);
                        //TODO HAY QUE ACOMODAR ESTO
                        //colision.Open("files/Sonido/metal.wav");
                        //colision.Play();
                    }
                    if (!soundEngine.IsCurrentlyPlaying("files/Sonido/colision1.mp3") && !soundEngine.IsCurrentlyPlaying("files/Sonido/colision2.mp3") && !soundEngine.IsCurrentlyPlaying("files/Sonido/colision3.mp3") && !soundEngine.IsCurrentlyPlaying("files/Sonido/colision4.mp3"))
                    {
                        int indice = (new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds))).Next() % SonidosColision.Length;
                        sonidoColision = soundEngine.Play2D(SonidosColision[indice], false);

                    }



                }
            }

           


        }

        /// <summary>
        /// checkea si el auto tiene velocidad
        /// 
        /// </summary>
        /// <param name="velocidad">Vector3 velocidad</param>
        /// <returns></returns>
        private bool seMueve(Vector3 velocidad)
        {
            return (Math.Sqrt(Math.Pow(velocidad.X,2) + Math.Pow(velocidad.Z,2)) > 0.002 );
        }

        private Vector3 aplicarTransformacion(Vector3 vec)
        {
            return Vector3.Transform(vec, modelMatFisica.ClearTranslation());
        }

        /// <summary>
        /// Setea variables de estado dependiendo del KeyEventArgs pasado por parametro
        /// cuando se presiona una tecla
        /// </summary>
        /// <param name="e">KeyEventArgs de la tecla presionada</param>
        /// <param name="fisica"> objeto que modela la fisica de la escena</param>

        public void keypressed(KeyEventArgs e, fisica fisica)
        {
            if (e.KeyCode == Keys.W)
            {
                dirZ = -1;
            }
            if (e.KeyCode == Keys.A)
            {
                dirX = 1;
            }
            if (e.KeyCode == Keys.D)
            {
                dirX = -1;
            }

            if (e.KeyCode == Keys.S)
            {
                dirZ = 1;
            }

            if (e.KeyCode == Keys.R)
            {
                posicionEnPistaCercana(fisica, posAnt);
            }
            
        }
        /// <summary>
        /// Setea variables de estado dependiendo del KeyEventArgs pasado por parametro
        /// cuando se suelta una tecla
        /// </summary>
        /// <param name="e">KeyEventArgs de la tecla soltada</param>

        public void keyreleased(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                dirZ = 0;
            }
            if (e.KeyCode == Keys.A)
            {
                dirX = 0;
            }
            if (e.KeyCode == Keys.D)
            {
                dirX = 0;
            }

            if (e.KeyCode == Keys.S)
            {
                dirZ = 0;
            }
        }



        /// <summary>
        /// retorna una matriz de rotacion con respecto a X
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        protected Matrix4 crearRotacionX(Vector3 rot)
        {
            Matrix4 aux = Matrix4.Mult(Matrix4.CreateTranslation(0, -rot.Y, -rot.Z), Matrix4.CreateRotationX(theta));
            Matrix4 ret = Matrix4.Mult(aux, Matrix4.CreateTranslation(0, rot.Y, rot.Z));
            return ret;
        }

        int j = 0;
        private List<Auto> policias;

        /// <summary>
        /// retorna una matriz de rotacion con respecto a Y y la matriz de rotacion pasada por parametro
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>

        protected Matrix4 crearRotacionY(Vector3 rot, Matrix4 rotacion)
        {
            Matrix4 aux = Matrix4.Mult(rotacion, Matrix4.CreateTranslation(-rot.X, 0, -rot.Z)); 
            aux = Matrix4.Mult(aux, Matrix4.CreateRotationY(alfa));
            aux = Matrix4.Mult(aux, Matrix4.CreateTranslation(rot.X, 0, rot.Z));
            //aux = Matrix4.Mult(aux, rotacion);
            return aux;
        }

        /// <summary>
        /// retorna true si la mesh pasada por parametro es una rueda del Charguer
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        protected bool esRuedaCharguer(FVLMesh mesh)
        {
            //solo para el charguer por ahora
            try
            {
                return mesh != null && mesh.getTexturePath() != null && ("CGUNS/ModelosOBJ/DODGE CHARGER/TIRE.png".Equals(mesh.getTexturePath()) ||
                             "CGUNS/ModelosOBJ/DODGE CHARGER/TIREBACK.png".Equals(mesh.getTexturePath()) ||
                             "CGUNS/ModelosOBJ/DODGE CHARGER/CHARGER69 RIM.png".Equals(mesh.getTexturePath()) ||
                             "CGUNS/ModelosOBJ/DODGE CHARGER/CHARGER69 RIM1.png".Equals(mesh.getTexturePath()));
            }
            catch (System.NullReferenceException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// retorna true si la mesh pasada por parametro es una rueda del Plicia
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>

        protected bool esRuedaPolicia(FVLMesh mesh)
        {
            //solo para el charguer por ahora

            try
            {
                return mesh != null && mesh.getTexturePath() != null && ("CGUNS/ModelosOBJ/Mustang/mustang_tyre.jpg".Equals(mesh.getTexturePath()));
            }
            catch (System.NullReferenceException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// retorna true si la mesh pasada por parametro es una rueda delantera del Charguer
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="index"></param>
        /// <returns></returns>

        protected bool esRuedaDelanteraCharguer(FVLMesh mesh, int index)
        {
            //solo para el charguer por ahora
            try
            {
                
                return mesh != null && mesh.getTexturePath() != null && (
                             ("CGUNS/ModelosOBJ/DODGE CHARGER/TIRE.png".Equals(mesh.getTexturePath()) && (index == 3 || index == 79)) ||
                             "CGUNS/ModelosOBJ/DODGE CHARGER/TIREBACK.png".Equals(mesh.getTexturePath()) ||
                             ("CGUNS/ModelosOBJ/DODGE CHARGER/CHARGER69 RIM.png".Equals(mesh.getTexturePath()) && (index == 5 || index == 81)) ||
                             ("CGUNS/ModelosOBJ/DODGE CHARGER/CHARGER69 RIM1.png".Equals(mesh.getTexturePath()) && (index == 4 || index == 80))
                             );
            }
            
            catch (System.NullReferenceException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            
        }

        private Vector3[] posicionesPista = { new Vector3(-101.2876f, -6.865585f, 95.01864f)
                                             ,new Vector3(-87.83908f, -2.307993f, 46.3694f)
                                             ,new Vector3(-81.72786f, 2.451682f, 6.12345f)
                                             ,new Vector3(-99.26801f, 9.638534f, -38.17747f)
                                             ,new Vector3(-84.45982f, 11.62421f, -87.01163f)
                                             ,new Vector3(0.1996851f, 4.208432f, -111.8112f)
                                             ,new Vector3(-30.65872f, 7.096386f, -101.2458f)
                                             ,new Vector3(65.34482f, -0.3993442f, -109.5369f)
                                             ,new Vector3(50.71632f, -3.164403f, -49.631f)
                                             ,new Vector3(77.67184f, -5.959321f, -11.52991f)
                                             ,new Vector3(132.0017f, -10.80501f, 48.23187f)
                                             ,new Vector3(75.22968f, -10.80098f, 108.2063f)
                                             ,new Vector3(-30.4989f, -8.496518f, 97.71658f)
                                             ,new Vector3(-71.27215f, -7.497179f, 96.87579f)};


        /// <summary>
        /// reinicia al auto en una posicion de la pista especifica
        /// </summary>
        /// <param name="fisica"></param>
        /// <param name="posAnt"></param>
        public void posicionEnPistaCercana(fisica fisica, Vector3 posAnt)
        {
            float rotacion = (float)Math.Acos(Vector3.Dot(Vector3.Normalize(direccion()), refZ));
            if (Vector3.Cross(direccion(), refZ).Y > 0)
            {
                rotacion = -rotacion;
            }
            Vector3 menor = new Vector3(1000.0f, 1000.0f, 1000.0f);
            Vector3 auxiliar = new Vector3(1, 1, 1);
            Vector3 auxiliar2 = new Vector3(1, 1, 1);
            for (int i = 0; i < posicionesPista.Length; i++)
            {
                auxiliar = posicionesPista[i] - posAnt;
                auxiliar2 = menor - posAnt;
                if (auxiliar.LengthSquared < auxiliar2.LengthSquared)
                {
                    menor = posicionesPista[i];
                }
            }

            position = menor;
            fisica.reiniciar(index, getPosition(), rotacion);
        }

        /*
        private float angle = 0;

        public Vector3 lucesPolicia()
        {
            Vector3 ConeDirection = Vector3.Transform(new Vector3(0, 0, 1), Matrix4.CreateRotationY(angle));

            angle += 0.1f;

            return ConeDirection;
        }

        private Vector3 desplLuz1 = new Vector3();
        private Vector3 desplLuz2 = new Vector3();

        public Vector3[] pocicionLuces()
        {
            Vector3[] ret = new Vector3[2];

            ret[0] = posSig + desplLuz1;
            ret[1] = posSig + desplLuz2;

            return ret;
        }
        /**/

    }
}
