using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace CGUNS.Cameras
{
    /// <summary>
    /// Representa una Camara en coordenadas esfericas.
    /// La camara apunta y orbita alrededor del origen de coordenadas (0,0,0).
    /// El vector "up" de la camara es esl eje "Y" (0,1,0).
    /// La posicion de la camara esta dada por 3 valores: Radio, Theta, Phi.
    /// </summary>
    class SphericalCamera : Camera
    {
        private const float DEG2RAD = (float)(Math.PI / 180.0); //Para pasar de grados a radianes
        
        public SphericalCamera()
        {
            //Por ahora la matriz de proyeccion queda fija. :)
            float fovy = 50 * DEG2RAD; //50 grados de angulo.
            float zNear = 0.1f; //Plano Near
            float zFar = 500f;  //Plano Far
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspectRadio, zNear, zFar);

            //Posicion inicial de la camara.
            radius = 1.2f;
            theta = -45.0f;
            phi = 45.0f;
        }

        /*
        public Vector3 getCameraPos()
        {
            return position;
        }/**/
        public override void setPosition(Vector3 nuevo)
        {
            position = nuevo;
        }
        
        public override void setLookAt(Vector3 nuevo)
        {
            target = nuevo;
        }
        
        public override Vector3 getPosition()
        {
            return position;
        }

        /**/
        /*
        /// <summary>
        /// Retorna la Matriz de Projeccion que esta utilizando esta camara.
        /// </summary>
        /// <returns></returns>
        public Matrix4 getProjectionMatrix()
        {
            return projMatrix;
        }
        /**/
        /// <summary>
        /// Retorna la Matriz de Vista que representa esta camara.
        /// </summary>
        /// <returns></returns>
        public override Matrix4 getViewMatrix()
        {
            //Pasamos de sistema esferico, a sistema cartesiano
            //eye = toCartesian();
            //Construimos la matriz y la devolvemos.
            eye = position;
            return Matrix4.LookAt(eye, target, up);
        }
        
        /*
        public void Acercar(float distance)
        {
            if ((distance > 0) && (distance < radius))
            {
                radius = radius - distance;
            }
            toCartesian();
        }

        public void Alejar(float distance)
        {
            if (distance > 0)
            {
                radius = radius + distance;
            }
            toCartesian();
        }

        private float deltaTheta = 10;
        private float deltaPhi = 10;

        public void Arriba()
        {
            phi = phi - deltaPhi;
            if (phi < 10)
            {
                phi = 10;
            }
            toCartesian();
        }

        public void Abajo()
        {
            phi = phi + deltaPhi;
            if (phi > 170)
            {
                phi = 170;
            }
            toCartesian();
        }

        public void Izquierda()
        {
            theta = theta + deltaTheta;
            toCartesian();
        }

        public void Derecha()
        {
            theta = theta - deltaTheta;
            toCartesian();
        }
        /**/
        private void log(String format, params Object[] args)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(format, args), "[Camera]");
        }

    }
}
