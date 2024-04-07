#version 460 core
#define MAX_TEXTURES 32

in vec4 vColor;
in vec2 vTexCoord;
in flat int vTextureIndex;

uniform sampler2D uTextures[MAX_TEXTURES];

out vec4 FragColor;

void main()
{
    vec4 color = vColor * texture(uTextures[vTextureIndex], vTexCoord);
    if(color.a < 0.1)
        discard;

    FragColor = color;
}