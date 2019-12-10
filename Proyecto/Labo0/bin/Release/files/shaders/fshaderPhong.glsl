// FRAGMENT SHADER.
#version 330



uniform vec3 color_luz;

uniform vec3 ka;
uniform vec3 ke;
uniform vec3 kd;
uniform float n1; 
uniform float n2;
uniform float k;
uniform float m;
uniform float sigma;


//in vec3 camPosEye;
in vec3 fNorm;
in vec3 Ve;
in vec3 Le;
in vec3 fPos;
in vec3 fLuz;

out vec4 fColor;

void main(){
	float e = 2.718281828;
	float pi = 3.141592654;

	vec3 N = normalize(fNorm);

	//L es el vector entre el ojo y la luz?
	vec3 L = normalize(Le);

	vec3 R = reflect(L, N);
	vec3 V = normalize(Ve);
	//vec3 V = normalize(normalize(camPosEye)+Ve);
	
	vec3 H = normalize(L+V);


	//termino difuso
	float distancia = length(fPos - fLuz);
	float a= 0.1;
	float b= 0.001;
	float c= 0.00001;
	float atenuacion = min( 1.0 / (a+b*distancia + c*distancia*distancia),1.0);
	
	//difuso
	float difuso =  max( dot(L,N) , 0.0 ); 

	//float fiL = acos(dot(N,L));
	//float fiE = acos(dot(V,N));
	
	float titaL = acos(dot(N,L));
	float titaE = acos(dot(N,V));

	float alfaDif = max(titaL,titaE);
	float betaDif= min(titaL,titaE);
	
	float gammaDif = dot(V-N*dot(V,N) , L-N*dot(L,N));

	float A = 1 - 0.5*(sigma*sigma/(sigma*sigma+0.57));
	float B = 0.45*(sigma*sigma/(sigma*sigma+0.09));

	difuso = (1/pi)*max(0,cos(titaL))*(A+(B*max(0,gammaDif))*sin(alfaDif)*tan(betaDif));


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

	float especular =  F/pi * D*G/(dot(N,V)*dot(N,L));
	//p = F(tita)/pi * D*G/((N,V)(N,L))
	
	
	if(dot(L,N)<0)
		especular = 0;


	vec3 color = ka + atenuacion * color_luz * (kd*difuso + ke *especular);


	fColor = vec4(color,1.0);	
}
