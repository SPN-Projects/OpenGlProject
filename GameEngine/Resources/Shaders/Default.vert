#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in mat4 aModel;
layout (location = 5) in vec4 aColor;

uniform mat4 uViewProjectionMatrix;

out vec4 color;

void main()
{
    // Hide the vertex if the alpha is 0, this doent prevent the vertex from being rendered and the fragment shader from being executed
    if(aColor.a == 0.0)
	{
		gl_Position = vec4(0.0, 0.0, 0.0, 0.0);
        color = vec4(0.0, 0.0, 0.0, 0.0);
        return;
	}

    gl_Position = uViewProjectionMatrix * aModel * vec4(aPosition, 1.0);    
    color = aColor;
}