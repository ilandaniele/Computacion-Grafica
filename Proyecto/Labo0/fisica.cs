using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK; //La matematica

using BulletSharp;
using BulletSharp.SoftBody;
using CGUNS.Cameras;
using CGUNS.Meshes.FaceVertexList;

namespace Labo0
{
    /// <summary>
    /// clase utilizara para modelar la fisica de los objetos
    /// </summary>
    public class fisica
    {

        private BroadphaseInterface broadphase;
        private DefaultCollisionConfiguration collisionConfiguration;
        private CollisionDispatcher dispatcher;
        SequentialImpulseConstraintSolver solver;
        private DiscreteDynamicsWorld dynamicsWorld;
        private RigidBody Rplano;
        private SoftBody S;

        private RigidBody fallRigidBody;
        private Random R;
        private int cantidad;
        private List<Matrix4> listaM = new List<Matrix4>();
        private List<RigidBody> listaRB = new List<RigidBody>();
        private RayResultCallback call;
        /// <summary>
        /// Setea y retorna la lista de matrices de los objetos
        /// </summary>
        public List<Matrix4> ListaM
        {
            get{return listaM;}
            set{ listaM = value;}
        }
        /// <summary>
        /// Devuelve la lista de objetos rigidos de colision
        /// </summary>
        public List<RigidBody> ListaRB
        {
            get { return listaRB;}
            set{ listaRB = value;}
        }
        /// <summary>
        /// devuelve la cantidad de objetos almacenados
        /// </summary>
        /// <returns></returns>
        public int getCantidad()
        {
            return cantidad;
        }
        /// <summary>
        /// setea la cantidad de objetos
        /// </summary>
        /// <param name="c"></param>
        public void setCantidad(int c)
        {
            cantidad = c;
        }
        /// <summary>
        /// Setea y retorna el mundo dinámico
        /// </summary>
        public DiscreteDynamicsWorld DynamicsWorld
        {
            get
            {
                return dynamicsWorld;
            }

            set
            {
                dynamicsWorld = value;
            }
        }
        /// <summary>
        /// constructor de la clase fisica
        /// </summary>
        public fisica()
        {
            inicializarMundo();
            R = new Random(17);
        }
        /// <summary>
        /// funcion encargada de inicializar el mundo con efectos fisicos
        /// </summary>
        private void inicializarMundo()
        {
            cantidad = 0;
            
            broadphase = new DbvtBroadphase(); //orden n^2
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            //solver = new SequentialImpulseConstraintSolver();
            //mundo
            DynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConfiguration);
            DynamicsWorld.Gravity = new Vector3(0, -10, 0);
            //se agregan los colliders
            //plano estatico 
            CollisionShape groundShape = new StaticPlaneShape(new Vector3(0, 1, 0), 0);
            
            //motion state hace referencia a la matriz con la cual se creará el cuerpo rigido
            DefaultMotionState myMotionState = new DefaultMotionState(Matrix4.CreateTranslation(0, 0, 0));
            
            //el primer parametro es la masa, y el último es la inercia inicial con la cual se creará
            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(0, myMotionState, groundShape, new Vector3(0, 0, 0));
            //se crea el plano como cuerpo rigido
            Rplano = new RigidBody(rbInfo);
            //finalmente se agrega el plano al mundo
            //DynamicsWorld.AddRigidBody(Rplano);
            
        }
        
        
        /// <summary>
        /// funcion encargada de setear la matriz del objeto correspondiente al indice en la posicion y rotacion indicadas
        /// </summary>
        /// <param name="index">indice del objeto</param>
        /// <param name="position">posicion</param>
        /// <param name="rotacion">rotacion</param>
        public void reiniciar(int index, Vector3 position, float rotacion)
        {
            RigidBody a = ListaRB[index];
            a.ClearForces();
            a.LinearVelocity = new Vector3(0);
            a.WorldTransform = Matrix4.Mult(Matrix4.CreateRotationY(rotacion),Matrix4.CreateTranslation(position));

        }

        /// <summary>
        /// funcion que corrige las velocidades angulares
        /// </summary>
        /// <param name="index">indice a utilizar en una lista</param>
        public void corregir(int index)
        {
            RigidBody a = ListaRB[index];
            a.LinearVelocity = Vector3.Transform(new Vector3(0,0,1),listaM[index]);
        }

        /// <summary>
        /// A partir de un mesh object genera un objeto de colision y lo setea en el mundo
        /// </summary>
        /// <param name="obj">objeto a agregar</param>
        /// <param name="posicion"> posicion inicial</param>
        /// <param name="mass"> masa</param>
        /// <param name="terreno"> si es terreno o es otro tipo de objeto</param>
        /// <returns></returns>
            
        public int agregarCuboRigido(MeshObject obj, Vector3 posicion, int mass, bool terreno)
        {
            
            TriangleMesh objeto = new TriangleMesh();
            cantidad++;

            obj.setColisionBody(objeto);
            // para un cubo
            //CollisionShape shape = new BoxShape(colision);
            if (terreno)
            {
                Matrix4 M = Matrix4.CreateTranslation(posicion);
                ListaM.Add(M);
                DefaultMotionState fallMotionState2 = new DefaultMotionState(M);
                
                CollisionShape shape = new BvhTriangleMeshShape(objeto, true);
                

                Vector3 localInertia2 = shape.CalculateLocalInertia(0);


                RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(0.0f, fallMotionState2, shape, localInertia2);

               

                RigidBody tierra = new RigidBody(rbInfo);


                tierra.Friction = 0.3f;
                tierra.RollingFriction = 0;
                tierra.Restitution = 0.0f;
                
                ListaRB.Add(tierra);
                


                DynamicsWorld.AddRigidBody(tierra);
            }
            else
            {
                CollisionShape shape = new ConvexTriangleMeshShape(objeto, true);


                //TODO rotaciones
                Matrix4 M = Matrix4.CreateTranslation(posicion);
                ListaM.Add(M);
                DefaultMotionState fallMotionState2 = new DefaultMotionState(M);
                Vector3 localInertia2 = shape.CalculateLocalInertia(mass);
                RigidBodyConstructionInfo fallRigidBodyCI2 = new RigidBodyConstructionInfo(1, fallMotionState2, shape, localInertia2);
                fallRigidBody = new RigidBody(fallRigidBodyCI2);


                fallRigidBody.Friction = 0.7f; //El tanque tracciona, no se mueve si no se le indica.
                fallRigidBody.RollingFriction = 0;
                //fallRigidBody.ForceActivationState(ActivationState.DisableDeactivation); //Siempre activo!
                fallRigidBody.Restitution = 0f;

                //para que no se trabe
                fallRigidBody.ForceActivationState(ActivationState.DisableDeactivation);
                ListaRB.Add(fallRigidBody);

                DynamicsWorld.AddRigidBody(fallRigidBody);
            }
            return cantidad - 1;
        }
        /// <summary>
        /// Funcion que aplica un impulso en el punto position en el objeto correspondiente al indice
        /// </summary>
        /// <param name="fza"> vector de impulso</param>
        /// <param name="position">posicion en el objeto</param>
        /// <param name="index">indice del objeto</param>
        public void aplicarFuerza(Vector3 fza, Vector3 position, int index)
        {
            RigidBody R = ListaRB[index];
            position.Y = 0;
            R.ApplyImpulse(fza, position);
            if (fza.Y > 0)
            {

            }
            
            //R.LinearVelocity = new Vector3( 3f, 0, 0);
        }
        /// <summary>
        /// funcion utilizada para hacer girar el objeto correspondiente al indice
        /// </summary>
        /// <param name="vel">vector de velocidad</param>
        /// <param name="index">indice perteneciente al objeto</param>
        public void aplicarVelocidadAngular(Vector3 vel, int index)
        {
            RigidBody R = ListaRB[index];
            R.AngularVelocity += vel;
            //R.LinearVelocity = new Vector3( 3f, 0, 0);
        }
        /// <summary>
        /// funcion que corrige las velocidades angulares
        /// </summary>
        /// <param name="index">indice a utilizar en una lista</param>
        //Esta funcion frena la velocdad angular del vehiculo
        public void clearForces(int index)
        {
            RigidBody R = ListaRB[index];
           // Console.WriteLine("CLEAR antes" + R.AngularVelocity);

            R.AngularVelocity= R.AngularVelocity*0.9f;
            //Console.WriteLine("CLEAR " + R.AngularVelocity + nuevo);
        }

    
        /// <summary>
        /// funcion que devuelve verdadero si ambos objetos pasados por parametros correspondientes a los indices, estan en colision
        /// </summary>
        /// <param name="index">indice del chocado</param>
        /// <param name="colider">indice del chocador</param>
        /// <returns></returns>
        public bool colition(int index, int colider)
        {
            RigidBody R = ListaRB[index];
            RigidBody P = ListaRB[colider];

            R.UserObject = "auto";
            P.UserObject = "otro";
            
            int numManifolds = dynamicsWorld.Dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = dynamicsWorld.Dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject obA = contactManifold.Body0 as CollisionObject;
                CollisionObject obB = contactManifold.Body1 as CollisionObject;

                int numContacts = contactManifold.NumContacts;
                for (int j = 0; j < numContacts; j++)
                {
                    ManifoldPoint pt = contactManifold.GetContactPoint(j);
                    if (pt.Distance < 0.0f)
                    {
                        Vector3 ptA = pt.PositionWorldOnA;
                        Vector3 ptB = pt.PositionWorldOnB;
                        Vector3 normalOnB = pt.NormalWorldOnB;
                    }
                }
                if (obA.UserObject!=null && obB.UserObject != null)
                {
                    //Console.WriteLine((prueba++) +"YES:"+obA.UserObject.ToString()+" "+ obB.UserObject.ToString());
                    return true;
                }
            }
            return false;
        }

        
       

  
        /// <summary>
        /// funcion encargada de eliminar el mundo fisico
        /// </summary>
        public void dispose()
        {
            DynamicsWorld.Dispose();
        }
        

       
    }



}
