Archivo README.txt: deberá ser escrito pensando en un usuario nuevo del programa, y
contendrá lo siguiente:
- Una lista de los archivos fuente que integran el proyecto, y breve descripción de su
contenido, a menos que sea obvio.
- Instrucciones para instalar y ejecutar el programa.
- Instrucciones para compilar (qué librerías hay que linkear, etc.).
- Breve guía de usuario (funcionalidad, interacciones, limitaciones).

Archivos fuente:
-AudioEngine.cs: Archivo fuente de la librería CSCore, que contiene la implementación a utilizar
para poder reproducir canciones durante la ejecución del juego
-fisica.cs: Archivo fuente utilizado para la implementación del comportamiento de la fisica
-MainWindows.cs: Archivo fuente principal, donde se entrelaza todo lo implementado
-Program.cs
-ViewporQuad.cs: Archivo fuente que contiene las directivas para la implementación del viewport
-Shaders en la carpeta /files/shaders, encargados del procesamiento.
-Meshes: Auto, Cielo, Edificio, Palmera, PosteLuz, Senial, Tierra,MeshObject contienen el código
correspondiente para agregar objetos de este tipo en la escena
-Meshes: FVLMesh y FVLFace utilizados para el tratamiento de los meshes anteriores nombrados
-Meshes: Map utilizado para la implementacion de alguno de los meshes
-MtlFileParser y ObjFileParser: rescatan a partir de archivos .obj y .mtl las características
de cada objeto de la escena, como por ejemplo sus vértices, caras, normales, coordenadas de textura
y materiales.
-Exceptions, Shader y ShaderProgram
-Camera, QuatCamera, SphericalCamera, son archivos que contienen las implementaciones
de los distintos tipos de cámaras

Archivos .dll de librerías:
BulletSharp.dll
CSCore.dll
ikpMP3.dll
irrKlang.dll
irrKlang.NET4.dll

Archivo de libreria de OpenTK:
OpenTK.dll
OpenTK.GLControl.dll


Instrucciones para instalar y ejecutar el programa:
Tan solo hacer doble click sobre el ejecutable "Labo0.exe" de la carpeta "Ejecutable"

Instrucciones para compilar: 
Linkear librerias IrrKlang, CSCORE y BulletSharp.

Guía de usuario:
El usuario se puede mover por la escena siguiendo el coche mediante W (avanzar), A (girar hacia izquierda),
D (girar hacia derecha) y S (retroceder). 
Se puede cambiar de cámara mediante la tecla V, se dispone de tres camaras del coche
Tambien si se desea se puede utilizar otras dos camaras para moverse libremente mediante la tecla T
En caso de alguna falla o bug, donde el auto no se mueva, se puede reiniciar la posicion del auto
mediante presionar la tecla R.
El espacio con el que se puede interactuar esta limitado al mapa, cualquier desplazamiento fuera de este
genera una caída que hará que se reinicie la posición del coche
También se dispone de una radio presente en el móvil con el cual mediante la tecla O se pueden variar las 
canciones a reproducir
En cuanto a las cámaras, mediante la tecla T presionada por primera vez, se cambia a la cámara esférica, 
dicha cámara girará siguiendo un radio, la cual se podrá alejar con la tecla Q y acercar con la tecla E, 
y además desplazarse hacia arriba y abajo con las teclas w y A, e ir a izquierda y derecha mediante A y D
Luego con la segunda presión de la tecla T, se puede utilizar la cámara FPS mediante la cual nos podremos mover 
libremente sobre el mapa, mediante W y S para ir adelante y hacia atrás, A y D hacía los costados
para girar la camara se utilizan las teclas Q y E para rotar sobre el eje Y, H y K para rotar sobre el eje Z
y finalmente U y J para rotar sobre el eje X