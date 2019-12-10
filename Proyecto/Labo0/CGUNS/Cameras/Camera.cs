using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using System.Windows.Forms;

namespace CGUNS.Cameras
{
    /// <summary>
    /// Representa una Camara en coordenadas esfericas.
    /// La camara apunta y orbita alrededor del origen de coordenadas (0,0,0).
    /// El vector "up" de la camara es esl eje "Y" (0,1,0).
    /// La posicion de la camara esta dada por 3 valores: Radio, Theta, Phi.
    /// </summary>
    class Camera
    {
        private const float DEG2RAD = (float)(Math.PI / 180.0); //Para pasar de grados a radianes

        protected Matrix4 projMatrix; //Matriz de Proyeccion.

        protected Vector3 cameraPos;

        protected int x = 0, y = 0, z = 0;
        protected int rotX = 0, rotY = 0, rotZ = 0;

        protected float radius;
        protected float theta;
        protected float phi;

        //Valores necesarios para calcular la Matriz de Vista.
        protected Vector3 eye = new Vector3(3.0f, 0.0f, 0.0f);
        protected Vector3 target = new Vector3(0, 0, 0);
        protected Vector3 up = Vector3.UnitY;
        protected Vector3 position = new Vector3(3.0f, 0.0f, 0.0f);
        protected float aspectRadio = 1; //Cuadrado 

        protected float fovy = 50 * DEG2RAD; //50 grados de angulo.
        protected float zNear = 0.1f; //Plano Near
        protected float zFar = 500f;  //Plano Far

        /// <summary>
        /// Setea el aspect ratio a la cammara
        /// </summary>
        /// <param name="rt"></param>
        public void setAspectRatio(float rt)
        {
            aspectRadio = rt;
        }
        /// <summary>
        /// devuelve el aspect ratio de la camara
        /// </summary>
        /// <returns></returns>
        public float getAspectRatio()
        {
            return aspectRadio;
        }
        /// <summary>
        /// Constructor de la clase camara
        /// </summary>
        public Camera()
        {
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspectRadio, zNear, zFar);

            //Posicion inicial de la camara.
            radius = 2f;
            theta = 45.0f;
            phi = 45.0f;
        }
        
        /// <summary>
        /// setea una posicion a la camara
        /// </summary>
        /// <param name="nuevo"></param>
        public virtual void setPosition(Vector3 nuevo)
        {
            //position = nuevo;
        }
        /// <summary>
        /// devuelve la posicion de la camara
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 getPosition()
        {
            return toCartesian();
        }

        public virtual void setLookAt(Vector3 nuevo)
        {
            //target = nuevo;
        }
        /// <summary>
        /// Retorna la Matriz de Projeccion que esta utilizando esta camara.
        /// </summary>
        /// <returns></returns>
        public Matrix4 getProjectionMatrix()
        {
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspectRadio, zNear, zFar);
            return projMatrix;
        }
        /// <summary>
        /// Retorna la Matriz de Vista que representa esta camara.
        /// </summary>
        /// <returns></returns>
        public virtual Matrix4 getViewMatrix()
        {
            //Pasamos de sistema esferico, a sistema cartesiano
            eye = toCartesian();
            //Construimos la matriz y la devolvemos.
            return Matrix4.LookAt(eye, target, up);
        }

        /// <summary>
        /// devuelve un vector3 convertido a coordenadas cartesianas
        /// </summary>
        /// <returns>Vector3 coordenadas</returns>

        protected Vector3 toCartesian()
        {
            Vector3 position = new Vector3();
            position.Y = (float)(radius * Math.Cos(phi * DEG2RAD));
            position.X = (float)(radius * Math.Sin(phi * DEG2RAD) * Math.Cos(theta * DEG2RAD));
            position.Z = (float)(radius * Math.Sin(phi * DEG2RAD) * Math.Sin(theta * DEG2RAD));
            return position;
        }

        protected float distance = 1f;
        private float deltaTheta = 0.2f;
        private float deltaPhi = 0.2f;
        /// <summary>
        /// acerca la camara
        /// </summary>
        private void Acercar()
        {
            if ((distance < radius))
            {
                radius = radius + (rotY)*distance;
            }
            else
            {
                radius = radius + distance;
            }
        }
        /// <summary>
        /// Rota la camara
        /// </summary>
        public void girarX()
        {
            phi = phi + (z)*deltaPhi;
            //PARA EVITAR GIMBAL LOCK
            if (phi < 10)
            {
                phi = 10;
            }
            if (phi > 170)
            {
                phi = 170;
            }
        }
        
        private void GirarY() {
            theta = theta + (x)*deltaTheta;
        }

        /// <summary>
        /// mueve la camara dependiendo de los parametros seteados por los key pressed
        /// </summary>
        
        public virtual void move()
        {
            if (rotY != 0)
            {
                Acercar();
            }
            if(z != 0)
            {
                girarX();
            }
            if(x != 0)
            {
                GirarY();
            }

        }

        /// <summary>
        /// Setea variables de estado dependiendo del KeyEventArgs pasado por parametro
        /// cuando se presiona una tecla
        /// </summary>
        /// <param name="e">KeyEventArgs de la tecla presionada</param>


        public void keypressed(KeyEventArgs e)
        {
            //MOVIMIENTO
            if (e.KeyCode == Keys.W)
            {
                z = -1;
            }
            if (e.KeyCode == Keys.S)
            {
                z = 1;
            }
            if (e.KeyCode == Keys.D)
            {
                x = -1;
            }

            if (e.KeyCode == Keys.A)
            {
                x = 1;
            }

            if (e.KeyCode == Keys.Space)
            {
                y = 1;
            }

            if (e.KeyCode == Keys.C)
            {
                y = -1;
            }

            //ROTACIONES
            if (e.KeyCode == Keys.U)
            {
                rotX = -1;
            }
            if (e.KeyCode == Keys.J)
            {
                   rotX = 1;
            }
            if (e.KeyCode == Keys.K)
            {
                rotZ = -1;
            }

            if (e.KeyCode == Keys.H)
            {
                rotZ = 1;
            }

            if (e.KeyCode == Keys.Q)
            {
                rotY = 1;
            }

            if (e.KeyCode == Keys.E)
            {
                rotY = -1;
            }
            
        }

        /// <summary>
        /// Setea variables de estado dependiendo del KeyEventArgs pasado por parametro
        /// cuando se suelta una tecla
        /// </summary>
        /// <param name="e">KeyEventArgs de la tecla soltada</param>

        public virtual void keyReleased(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                z = 0;
            }
            if (e.KeyCode == Keys.S)
            {
                z = 0;
            }
            if (e.KeyCode == Keys.D)
            {
                x = 0;
            }

            if (e.KeyCode == Keys.A)
            {
                x = 0;
            }

            if (e.KeyCode == Keys.Space)
            {
                y = 0;
            }

            if (e.KeyCode == Keys.C)
            {
                y = 0;
            }

            //ROTACIONES
            if (e.KeyCode == Keys.U)
            {
                rotX = 0;
            }
            if (e.KeyCode == Keys.J)
            {
                rotX = 0;
            }
            if (e.KeyCode == Keys.K)
            {
                rotZ = 0;
            }

            if (e.KeyCode == Keys.H)
            {
                rotZ = 0;
            }

            if (e.KeyCode == Keys.Q)
            {
                rotY = 0;
            }

            if (e.KeyCode == Keys.E)
            {
                rotY = 0;
            }

        }


        //sacado de https://www.gamedev.net/topic/303090-quaternion-camera-c-edition/
    }
}
