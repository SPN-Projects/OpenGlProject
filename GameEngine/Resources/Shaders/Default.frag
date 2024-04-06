#version 460 core
#define MAX_TEXTURES 32

in vec4 color;
in vec2 texCoord;

uniform sampler2D uTexture;

out vec4 FragColor;

void main()
{
    FragColor = color * texture(uTexture, texCoord);
}