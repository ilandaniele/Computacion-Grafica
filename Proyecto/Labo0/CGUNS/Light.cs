using System;
using System.Collections.Generic;
using System.Text;
using OpenTK; //La matematica


namespace CGUNS
{/// <summary>
/// Esta clase fue provista por la cátedra
/// </summary>
    class Light
    {
        Vector4 position;
        Vector4 Ia;
        Vector4 Id;
        Vector4 Is;
        float coneAngle;
        Vector3 coneDirection;
        int enabled;

        /// <summary>
        /// Setea la posición de la luz y la devuelve
        /// </summary>
        public Vector4 Position
        {
            get
            {
                return position;
            }
            set
            {
                this.position = value;
            }
        }
        /// <summary>
        /// Setea el valor ambiente y lo devuelve
        /// </summary>
        public Vector4 Iambient
        {
            get
            {
                return Ia;
            }
            set
            {
                this.Ia = value;
            }
        }
        /// <summary>
        /// Setea el valor difuso y lo devuelve
        /// </summary>
        public Vector4 Idiffuse
        {
            get
            {
                return Id;
            }
            set
            {
                this.Id = value;
            }
        }
        /// <summary>
        /// Setea el valor especular y lo devuelve
        /// </summary>
        public Vector4 Ispecular
        {
            get
            {
                return Is;
            }
            set
            {
                this.Is = value;
            }
        }
        /// <summary>
        /// Setea la dirección del cono y lo devuelve
        /// </summary>
        public Vector3 ConeDirection
        {
            get
            {
                return coneDirection;
            }
            set
            {
                this.coneDirection = value;
            }
        }
        /// <summary>
        /// Nos devuelve el angulo del cono y lo setea
        /// </summary>
        public float ConeAngle
        {
            get
            {
                return coneAngle;
            }
            set
            {
                this.coneAngle = value;
            }
        }
        /// <summary>
        /// Función que setea el valor de habilitación y lo devuelve
        /// </summary>
        public int Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                this.enabled = value;
            }
        }
        /// <summary>
        /// Función que permite apagar o prender una luz
        /// </summary>
        public void Toggle()
        {
            if (Enabled == 0)
            {
                Enabled = 1;
            }
            else
            {
                Enabled = 0;
            }
        }
    }
}
