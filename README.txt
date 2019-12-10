Archivo README.txt: deber� ser escrito pensando en un usuario nuevo del programa, y
contendr� lo siguiente:
- Una lista de los archivos fuente que integran el proyecto, y breve descripci�n de su
contenido, a menos que sea obvio.
- Instrucciones para instalar y ejecutar el programa.
- Instrucciones para compilar (qu� librer�as hay que linkear, etc.).
- Breve gu�a de usuario (funcionalidad, interacciones, limitaciones).

Archivos fuente:
-AudioEngine.cs: Archivo fuente de la librer�a CSCore, que contiene la implementaci�n a utilizar
para poder reproducir canciones durante la ejecuci�n del juego
-fisica.cs: Archivo fuente utilizado para la implementaci�n del comportamiento de la fisica
-MainWindows.cs: Archivo fuente principal, donde se entrelaza todo lo implementado
-Program.cs
-ViewporQuad.cs: Archivo fuente que contiene las directivas para la implementaci�n del viewport
-Shaders en la carpeta /files/shaders, encargados del procesamiento.
-Meshes: Auto, Cielo, Edificio, Palmera, PosteLuz, Senial, Tierra,MeshObject contienen el c�digo
correspondiente para agregar objetos de este tipo en la escena
-Meshes: FVLMesh y FVLFace utilizados para el tratamiento de los meshes anteriores nombrados
-Meshes: Map utilizado para la implementacion de alguno de los meshes
-MtlFileParser y ObjFileParser: rescatan a partir de archivos .obj y .mtl las caracter�sticas
de cada objeto de la escena, como por ejemplo sus v�rtices, caras, normales, coordenadas de textura
y materiales.
-Exceptions, Shader y ShaderProgram
-Camera, QuatCamera, SphericalCamera, son archivos que contienen las implementaciones
de los distintos tipos de c�maras

Archivos .dll de librer�as:
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

Gu�a de usuario:
El usuario se puede mover por la escena siguiendo el coche mediante W (avanzar), A (girar hacia izquierda),
D (girar hacia derecha) y S (retroceder). 
Se puede cambiar de c�mara mediante la tecla V, se dispone de tres camaras del coche
Tambien si se desea se puede utilizar otras dos camaras para moverse libremente mediante la tecla T
En caso de alguna falla o bug, donde el auto no se mueva, se puede reiniciar la posicion del auto
mediante presionar la tecla R.
El espacio con el que se puede interactuar esta limitado al mapa, cualquier desplazamiento fuera de este
genera una ca�da que har� que se reinicie la posici�n del coche
Tambi�n se dispone de una radio presente en el m�vil con el cual mediante la tecla O se pueden variar las 
canciones a reproducir
En cuanto a las c�maras, mediante la tecla T presionada por primera vez, se cambia a la c�mara esf�rica, 
dicha c�mara girar� siguiendo un radio, la cual se podr� alejar con la tecla Q y acercar con la tecla E, 
y adem�s desplazarse hacia arriba y abajo con las teclas w y A, e ir a izquierda y derecha mediante A y D
Luego con la segunda presi�n de la tecla T, se puede utilizar la c�mara FPS mediante la cual nos podremos mover 
libremente sobre el mapa, mediante W y S para ir adelante y hacia atr�s, A y D hac�a los costados
para girar la camara se utilizan las teclas Q y E para rotar sobre el eje Y, H y K para rotar sobre el eje Z
y finalmente U y J para rotar sobre el eje X