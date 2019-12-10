// FRAGMENT SHADER.
#version 330


//multiples luces
//struct Light {
uniform	vec4 position;
uniform	vec4 Ia;
uniform vec4 Id;
uniform	vec4 Is;
uniform	float coneAngle;
uniform	vec3 coneDirection;
uniform	int enabled;
//};

//para textura

uniform sampler2D gSampler; 

uniform sampler2D gSamplerNormalMap; 
/**/

uniform vec4 ka;
uniform vec4 ke;
uniform vec4 kd;

uniform float n1; 
uniform float n2;
uniform float k;
uniform float m;
uniform int flagTextura;
uniform int flagLuz;
uniform int flagRelieve;


//multiples luces
uniform vec3 cameraPosition;
//uniform int numLights;
//uniform Light allLights;

//para mapa de normales
uniform mat3 MatrixNormal; //normalMatrix
uniform mat4 modelMat;

in vec3 fNorm;//fragNormal
in vec3 fPos;//fragPos


;//modelMatrix

//para textura
in vec2 f_TexCoord;


out vec4 fColor;


vec4 applyLight(vec3 surfacePos, vec3 N, vec3 V, vec4 Kd, vec4 Ka, vec4 Ke) {
	float A= 0.3f;
	float B= 0.007f;
	float C= 0.00008f;
	
	float e = 2.718281828;
	float pi = 3.141592654;
	
	float attenuation = 1.0;
	vec3 L;
	if (position.w == 0.0) { //Directional light
		L = normalize(position.xyz);
		attenuation = 1.0; //no attenuation for directional lights.
	} else { //Positional light (Spot or Point)
		L = normalize(position.xyz -surfacePos);
		//Cone restrictions
		vec3 coneDirection = normalize(coneDirection);
		vec3 rayDirection = -L;
		float lightToSurfaceAngle = degrees(acos(dot(rayDirection, coneDirection)));
		if (lightToSurfaceAngle <= coneAngle) { //Inside cone
			float distanceToLight = length(position.xyz - surfacePos);
			attenuation = 1.0 / ( A + B * distanceToLight + C * pow(distanceToLight, 2));
		} else {
			attenuation = 0.0;
		}
	}
	//AMBIENT
	vec4 ambient = Ia * Ka;
	//DIFUSSE
	float diffuseCoefficient = max(0.0, dot(N, L));
	vec4 diffuse = Id * Kd * diffuseCoefficient;

	//SPECULAR
	float specularCoefficient = 0.0;

	vec3 incidenceVector = -L;
	vec3 H = normalize( L + V);

	{
			///especular
		//termino de Fresnel
		float n = n1/n2;
		float F0=((k*k)+pow((n-1),2))/((k*k)+pow((n+1),2));
		float tita = acos(dot(L,N));

		float F = F0 + (1 - F0)*pow((1-cos(tita)),5);

		//distribucion de las orientaciones de las microfacetas

		float beta = acos(dot(H,N));

		float D = pow(e,-pow(tan(beta),2)/(m*m))/(m*m*pow(cos(beta),4));

		//factor de atenuacion geometrica

		float Ge = 2 * dot(N,H) * dot(N,V) / dot(V,H);
		float Gs = 2 * dot(N,H) * dot(N,L) / dot(V,H);

		float G = min(1,min(Gs,Ge));

		specularCoefficient =  F/pi * D*G/(dot(N,V)*dot(N,L));
		
	}
	if (dot(N, L) < 0.0) {
		specularCoefficient = 0;
	}
	vec4 specular = Is * Ke * specularCoefficient;
	return ambient + attenuation * (diffuse + specular) * enabled;
}



void main(){
	

	vec3 relieve = normalize(MatrixNormal*(2*(vec3(texture2D(gSamplerNormalMap, f_TexCoord))-vec3(0.5,0.5,0.5))));
	
	vec3 N = normalize(MatrixNormal * fNorm);

	if(flagRelieve == 1)
		N = normalize(vec3(relieve.x,relieve.y,relieve.z)+fNorm);//preguntar porque debo sumar fNorm

	
	//para textura
	
	vec4 kade = kd;
	if(flagTextura == 1)
		kade = (texture2D(gSampler, f_TexCoord));

	vec3 surfacePos = vec3(modelMat * vec4(fPos, 1));
	vec3 V = normalize(-surfacePos+cameraPosition);
	
	//color final
	vec4 color = vec4(0);
	//for(int i = 0; i < numLights; ++i) {
	color += applyLight(surfacePos, N, V, kade, ka, ke);
	//}	

    

    //vec4 color = ka +  color_luz  * (kade*difuso + ke*especular);//atenuacion *

    if(flagLuz == 0)
	      color = kade;

    //if(DrawSkyBox == 1)
      //color = colorRef;


    fColor = color;

}
