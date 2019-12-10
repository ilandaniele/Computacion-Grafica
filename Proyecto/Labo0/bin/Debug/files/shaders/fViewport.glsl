#version 330

layout(location = 0) out vec4 FragColor;

in vec2 fTextureCoordinates;

uniform sampler2D uShadowSampler;

void main()
{
	// Accedo a la tercer componente que tiene la información de profundidad.
	// En realidad OpenGL replica todas las componentes, entonces, todas tienen el mismo valor.
	float depth = texture(uShadowSampler, fTextureCoordinates).r;
	
	// Generamos un color en escala de grises con el valor de profundidad.
	FragColor  =  vec4(depth, depth, depth, 1.0);
}
