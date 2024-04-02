#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in mat4 aModel;
layout (location = 5) in vec4 aColor;

uniform mat4 uViewProjectionMatrix;

out vec4 color;

void main()
{
    gl_Position = uViewProjectionMatrix * aModel * vec4(aPosition, 1.0);
    color = aColor;
}