// FRAGMENT SHADER.
#version 330
#define MAX_LIGHTS 5


//multiples luces
struct Light {
	vec4 position;
	vec4 Ia;
	vec4 Id;
	vec4 Is;
	float coneAngle;
	vec3 coneDirection;
	int enabled;
};

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
uniform int numLights;
uniform Light allLights[MAX_LIGHTS];

//para mapa de normales
uniform mat3 MatrixNormal; //normalMatrix
uniform mat4 modelMat;

in vec3 fNorm;//fragNormal
in vec3 fPos;//fragPos


//modelMatrix

//para textura
in vec2 f_TexCoord;

//SOMBRAS
uniform sampler2D uShadowSampler;
uniform int luzPlayer;
// Posicion del fragmento en el espacio de la luz.
in vec4 fragPosLightSpace;

// Calcula la visibilidad del fragmento respecto la luz.
// Retorna 1 si es visible y 0 si no lo es.
float ShadowCalculation(vec4 fragPosLightSpace)
{
	vec3 lightDir = normalize( allLights[0].position.xyz  - fragPosLightSpace.xyz);
	float bias = max(0.0001 * (1.0 - dot(fNorm, lightDir)), 0.00001);
	
	float shadowDepth = textureProj(uShadowSampler, fragPosLightSpace).z;
	float fragDepth   = fragPosLightSpace.z;

	float shadow = 0.0;
	vec2 texelSize = 1.0/textureSize(uShadowSampler, 0);
	for (int x = -1; x <= 1; ++x){
		for (int y = -1; y <= 1; ++y){
			float pcfDepth = texture( uShadowSampler, fragPosLightSpace.xy + vec2(x, y) * texelSize).z;
			shadow += fragDepth - bias > pcfDepth ? 1.0f : 0.0f;
		}
	}
	shadow /= 9;
	// Si el fragmento esta fuera del alcance del shadow map entonces es visible.
	if (fragDepth > 1.0)
		shadow = 0.0;

	return shadow;
}


out vec4 fColor;


vec4 applyLight(Light light, vec3 surfacePos, vec3 N, vec3 V, vec4 Kd, vec4 Ka, vec4 Ke, int index) {
	float A= 0.3f;
	float B= 0.007f;
	float C= 0.00008f;
	
	float e = 2.718281828;
	float pi = 3.141592654;
	
	float attenuation = 1.0;
	vec3 L;
	float shadow = 0.0;

	if(index == luzPlayer)
			shadow =  ShadowCalculation(fragPosLightSpace);


	if (light.position.w == 0.0) { //Directional light
		L = normalize(light.position.xyz);
		attenuation = 1.0; //no attenuation for directional lights.
	} else { //Positional light (Spot or Point)
		L = normalize(light.position.xyz -surfacePos);
		//Cone restrictions
		vec3 coneDirection = normalize(light.coneDirection);
		vec3 rayDirection = -L;
		float lightToSurfaceAngle = degrees(acos(dot(rayDirection, coneDirection)));
		if (lightToSurfaceAngle <= light.coneAngle) { //Inside cone
			float distanceToLight = length(light.position.xyz - surfacePos);
			attenuation = 1.0 / ( A + B * distanceToLight + C * pow(distanceToLight, 2));
		} else {
			attenuation = 0.0;
		}
	}
	//AMBIENT
	vec4 ambient = light.Ia * Ka;
	//DIFUSSE
	float diffuseCoefficient = max(0.0, dot(N, L));
	vec4 diffuse = light.Id * Kd * diffuseCoefficient;

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
	vec4 specular = light.Is * Ke * specularCoefficient;
	return (ambient + attenuation * (diffuse + specular) * (1.0 - shadow)) * light.enabled ;
}



void main(){
	

	vec3 relieve = normalize(MatrixNormal*(2*(vec3(texture2D(gSamplerNormalMap, f_TexCoord)))));
	
	vec3 N = normalize(MatrixNormal * fNorm);

	if(flagRelieve == 1)
	{
		if(abs(N.y) > abs(N.x) && abs(N.y) > abs(N.z))
		{
			if((N.y) >= 0)
				N = normalize(vec3(relieve.x,relieve.y,relieve.z));
			else
				N = normalize(vec3(relieve.x,-relieve.y,relieve.z));		
		}
		else if(abs(N.z) > abs(N.x) && abs(N.z) > abs(N.y))
		{
			if((N.z) >= 0)
				N = normalize(vec3(relieve.x,relieve.z,relieve.y));
			else
				N = normalize(vec3(relieve.x,-relieve.z,relieve.y));
		
		}
		else if(abs(N.x) > abs(N.y) && abs(N.x) > abs(N.z))
		{
			if((N.x) >= 0)
				N = normalize(vec3(relieve.y,relieve.x,relieve.z));
			else
				N = normalize(vec3(relieve.y,-relieve.x,relieve.z));
		
		}

		//N = normalize(vec3(relieve.x,relieve.y,relieve.z));//preguntar porque debo sumar fNorm

	}
		

	
	//para textura
	
	vec4 kade = kd;
	if(flagTextura == 1)
		kade = (texture2D(gSampler, f_TexCoord));

	vec3 surfacePos = vec3(modelMat * vec4(fPos, 1));
	vec3 V = normalize(-surfacePos+cameraPosition);
	
	//color final
	vec4 color = vec4(0);
	for(int i = 0; i < numLights; ++i) {
		color += applyLight(allLights[i], surfacePos, N, V, kade, ka, ke, i);
	}	

    
    if(flagLuz == 0)
	      color = kade;


    fColor = color;

}
