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
    class QuatCamera : Camera
    {
        private const float DEG2RAD = (float)(Math.PI / 180.0); //Para pasar de grados a radianes
        
        private Quaternion cameraRot;

        private bool rotacion = true;

        public QuatCamera()
        {
            //Por ahora la matriz de proyeccion queda fija. :)
            float fovy = 50 * DEG2RAD; //50 grados de angulo.
            float zNear = 0.1f; //Plano Near
            float zFar = 500f;  //Plano Far
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspectRadio, zNear, zFar);
            x = y = 0;
            //Posicion inicial de la camara.
            radius = 2.2f;
            cameraPos = new Vector3(0, 0, radius);
            cameraRot = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), 0);
        }
        
        /// <summary>
        /// Retorna la Matriz de Vista que representa esta camara.
        /// </summary>
        /// <returns></returns>
        public override Matrix4 getViewMatrix()
        {
            //Construimos la matriz y la devolvemos.

            //cameraPos = new Vector3(x, y, radius);
            Matrix4 ret;
            if (rotacion)
                //fps
                ret = Matrix4.Mult(Matrix4.CreateTranslation(cameraPos), Matrix4.CreateFromQuaternion(cameraRot));
            else
                ret = Matrix4.Mult(Matrix4.CreateFromQuaternion(cameraRot), Matrix4.CreateTranslation(-cameraPos));


            return ret;

        }

        public Matrix4 getCameraRot()
        {
            Matrix4 rot = Matrix4.CreateFromQuaternion(cameraRot);

            return rot;
            //Matrix4.Mult(traslation, rot);
        }
        
        /// <summary>
        /// Retorna la posicion de la camara FPS
        /// </summary>
        /// <returns></returns>

        public override Vector3 getPosition()
        {
            //revisar bien
            return - cameraPos;
        }

        private float deltaTheta = 0.1f;
        private float deltaPhi = 0.1f;

        //Funciones de movimiento y rotacion
        
        //ROTACION

        /// <summary>
        /// Su funcion es rotar la cama respecto del eje X en la camara FPS
        /// </summary>
        private void Arriba()
        {
            Quaternion tmpQuat = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), -(rotX)*deltaPhi);
            cameraRot = tmpQuat * cameraRot;
            cameraRot.Normalize();
        }

        /// <summary>
        /// Su funcion es rotar la cama respecto del eje Z en la camara FPS
        /// </summary>
        private void GirarDerecha()
        {
            Quaternion tmpQuat = Quaternion.FromAxisAngle(new Vector3(0, 0, 1), -(rotZ)*deltaPhi);
            cameraRot = tmpQuat * cameraRot;
            cameraRot.Normalize();
        }

        /// <summary>
        /// Su funcion es rotar la cama respecto del eje Y en la camara FPS
        /// </summary>
        private void Izquierda()
        {
            Quaternion tmpQuat = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -(rotY)*deltaTheta);
            cameraRot = tmpQuat * cameraRot;
            cameraRot.Normalize();
        }
        
        //MOVIMIENTO
        /// <summary>
        /// Su funcion es el desplazamiento hacia adelante en la camara FPS
        /// </summary>
        private void Acercar()
        {
            Matrix4 aux = Matrix4.CreateFromQuaternion(cameraRot);
            Vector3 axis = new Vector3(aux.M13, aux.M23, aux.M33);
            cameraPos -= new Vector3(axis * distance) * z;
        }

        /// <summary>
        /// Su funcion es desplazarse de costado en la camara FPS
        /// </summary>
        private void DesplazarIzquierda()
        {
            float delta = 0.1f;
            Matrix4 aux = Matrix4.CreateFromQuaternion(cameraRot);
            Vector3 axis = new Vector3(aux.M11, aux.M21, aux.M31);
            cameraPos += new Vector3(axis * delta) * x;
        }
        /// <summary>
        /// Su funcion es desplazarse hacia arriba y hacia abajo en la camara FPS
        /// </summary>
        private void DesplazarArriba()
        {
            float delta = 0.1f;
            Matrix4 aux = Matrix4.CreateFromQuaternion(cameraRot);
            Vector3 axis = new Vector3(aux.M12, aux.M22, aux.M32);
            cameraPos -= new Vector3(axis * delta) * y;
        }

        /// <summary>
        /// Extiende a la funcion move() de la clase Camera
        /// Su funcion es llamar a las funciones de movimiento dependiendo de los atributos seteados
        /// </summary>

        public override void move()
        {
            if (rotZ != 0)
            {
                GirarDerecha();
            }
            if(rotX != 0)
            {
                Arriba();
            }
            if(rotY != 0)
            {
                Izquierda();
            }
            if (x != 0)
            {
                DesplazarIzquierda();
            }
            if (y != 0)
            {
                DesplazarArriba();
            }
            if (z != 0)
            {
                Acercar();
            }
        }
        
        //sacado de https://www.gamedev.net/topic/303090-quaternion-camera-c-edition/
    }
}
