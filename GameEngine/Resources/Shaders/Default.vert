#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform mat4 uViewProjectionMatrix;

out vec3 color;

void main()
{
    gl_Position = uViewProjectionMatrix * vec4(aPosition, 1.0);
    color = aColor;
}