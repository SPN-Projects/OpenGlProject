#version 460 core
#define MAX_TEXTURES 32

in vec4 color;
in vec2 texCoord;

out vec4 FragColor;

uniform sampler2D uTexture;

void main()
{
    FragColor = color * texture(uTexture, texCoord);
}