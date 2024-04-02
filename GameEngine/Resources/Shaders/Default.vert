#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;
layout (location = 2) in mat4 aModel;

uniform mat4 uViewProjectionMatrix;

out vec4 color;

void main()
{
    gl_Position = uViewProjectionMatrix * aModel * vec4(aPosition, 1.0);
    color = aColor;
}