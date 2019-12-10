// VERTEX SHADER. Simple

#version 330


uniform mat4 projMat;
uniform mat4 viewMatrix;
uniform mat3 MatrixNormal;
uniform mat4 modelMat;

in vec3 vPos;
in vec3 vNorm;
in vec2 vText;

out vec3 fNorm;
out vec3 fPos;

out vec2 f_TexCoord;


uniform mat4 uLightBiasMatrix;
out vec4 fragPosLightSpace;

void main(){

	fPos = vPos;

	fNorm = vNorm;


	f_TexCoord = vText;

	fragPosLightSpace = uLightBiasMatrix * modelMat * vec4(vPos, 1.0);

	gl_Position = projMat * viewMatrix * modelMat * vec4(vPos, 1.0) ;
}
