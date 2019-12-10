// VERTEX SHADER. Simple

#version 330


uniform mat3 MatrixNormal;
uniform mat4 projMat;
uniform mat4 mvMat;
uniform vec3 LPosEye;
//uniform vec3 CamPos;

in vec3 vPos;
in vec3 vNorm;


out vec3 Le;
out vec3 Ve;
out vec3 fNorm;
out vec3 fPos;
out vec3 fLuz;
//out vec3 camPosEye;

void main(){
	//posicion en coordenadas del ojo
	vec3 vPosEye = vec3(mvMat * vec4(vPos,1.0));
	//fPosEye = vPosEye;
	fPos = vPosEye;
	fLuz = LPosEye;
	//camPosEye = vec3(mvMat * vec4(CamPos,1.0));

	fNorm = normalize(MatrixNormal * vNorm);
	Le = normalize(vec3(LPosEye - vPosEye));
	Ve = normalize(-vPosEye);

	gl_Position = projMat * mvMat * vec4(vPos, 1.0);
}
