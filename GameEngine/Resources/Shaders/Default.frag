#version 460 core
#define MAX_TEXTURES 32

in vec4 color;

out vec4 FragColor;

void main()
{
    FragColor = color;
}