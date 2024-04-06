#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in mat4 aModel;
layout (location = 6) in vec4 aColor;

uniform mat4 uViewProjectionMatrix;

out vec4 color;
out vec2 texCoord;

void main()
{
    // Hide the vertex if the alpha is 0, this doent prevent the vertex from being rendered and the fragment shader from being executed
    if(aColor.a == 0.0)
	{
		gl_Position = vec4(0.0, 0.0, 0.0, 0.0);
        color = vec4(0.0, 0.0, 0.0, 0.0);
        texCoord = vec2(0.0, 0.0);
        return;
	}

    gl_Position = uViewProjectionMatrix * aModel * vec4(aPosition, 1.0);    
    color = vec4(aTexCoord, 0.0, 1.0);
    texCoord = aTexCoord;
}