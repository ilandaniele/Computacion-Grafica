// VERTEX SHADER. Simple

#version 330


uniform mat4 projMat;
uniform mat4 mvMat;

in vec3 vPos;
in vec3 vColor;
out vec4 fragColor;

void main(){
   fragColor = vec4(vColor,1.0);

   gl_Position = projMat * mvMat * vec4(vPos, 1.0);
}
