#version 460 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec4 aColor;
layout (location = 3) in vec3 aTranslation;
layout (location = 4) in vec3 aRotation;
layout (location = 5) in vec2 aScale;

//uniform mat4 uViewProjectionMatrix;

layout(std140, binding = 0) uniform CameraData
{
  vec4 uCameraPosition;
  mat4 uProjectionMatrix;
};


out vec4 color;
out vec2 texCoord;

mat4 createRotationXMatrix(float angle)
{
	float cosTheta = cos(angle);
	float sinTheta = sin(angle);

	return mat4(1.0, 0.0, 0.0, 0.0,
				0.0, cosTheta, -sinTheta, 0.0,
				0.0, sinTheta, cosTheta, 0.0,
				0.0, 0.0, 0.0, 1.0);
}

mat4 createRotationYMatrix(float angle)
{
	float cosTheta = cos(angle);
	float sinTheta = sin(angle);

	return mat4(cosTheta, 0.0, sinTheta, 0.0,
				0.0, 1.0, 0.0, 0.0,
				-sinTheta, 0.0, cosTheta, 0.0,
				0.0, 0.0, 0.0, 1.0);
}

mat4 createRotationZMatrix(float angle)
{
	float cosTheta = cos(angle);
	float sinTheta = sin(angle);

	return mat4(cosTheta, -sinTheta, 0.0, 0.0,
				sinTheta, cosTheta, 0.0, 0.0,
				0.0, 0.0, 1.0, 0.0,
				0.0, 0.0, 0.0, 1.0);
}

mat4 createTranslationMatrix(vec3 translation)
{
	return mat4(1.0, 0.0, 0.0, 0.0,
				0.0, 1.0, 0.0, 0.0,
				0.0, 0.0, 1.0, 0.0,
				translation.x, translation.y, translation.z, 1.0);
}

mat4 createScaleMatrix(vec3 scale)
{
	return mat4(scale.x, 0.0, 0.0, 0.0,
				0.0, scale.y, 0.0, 0.0,
				0.0, 0.0, scale.z, 0.0,
				0.0, 0.0, 0.0, 1.0);
}

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

	mat4 cameraModel = createTranslationMatrix(-uCameraPosition.xyz);
	mat4 aModel = createTranslationMatrix(aTranslation) * createRotationXMatrix(aRotation.x) * createRotationYMatrix(aRotation.y) * createRotationZMatrix(aRotation.z) * createScaleMatrix(vec3(aScale, 1.0));

    gl_Position = uProjectionMatrix * cameraModel * aModel * vec4(aPosition, 1.0);    
    color = aColor;
    texCoord = aTexCoord;
}