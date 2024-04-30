#version 330 core

layout (location = 0) in vec3 vertexPosition;
layout (location = 1) in vec3 vertexNormal;
layout (location = 2) in vec3 vertexTangent;
layout (location = 3) in vec2 vertexTexCoord;
uniform mat4 projectionTransform;
uniform mat4 viewTransform;
uniform mat4 modelTransform;
uniform vec3 lightPosition;
uniform vec3 lightPos2;
uniform vec3 lightPos3;
uniform vec3 lightPos4;
uniform vec3 lightPos5;

out vec3 shaderPosition;
out mat3 shaderTBN;
out vec2 shaderTexCoord;
out vec3 shaderLightPosition;
out vec3 shaderLightPosition2;
out vec3 shaderLightPosition3;
out vec3 shaderLightPosition4;
out vec3 shaderLightPosition5;
out vec3 directionalNormal;

uniform vec3 spotLightDirection;
out vec3 shaderLightDirection;



void main()
{
    // combine the model and view transforms to get the camera space transform
    mat4 modelViewTransform = viewTransform * modelTransform;

    // compute the vertex's attributes in camera space
    shaderPosition = vec3(modelViewTransform * vec4(vertexPosition, 1.0f));
    shaderTexCoord = vertexTexCoord;

    // compute the normal transform as the transpose of the inverse of the camera transform,
    // then compute a TBN matrix using this transform
    mat3 normalTransform = mat3(transpose(inverse(modelViewTransform)));
    directionalNormal = normalize(normalTransform * vertexNormal);
    vec3 tangent = normalize(normalTransform * vertexTangent);
    vec3 bitangent = cross(directionalNormal, tangent);
    shaderTBN = mat3(tangent, bitangent, directionalNormal);

    // also compute the light position in camera space
    // (we want all lighting calculations to be done in camera space to avoid losing precision)
    shaderLightPosition = vec3(viewTransform * vec4(lightPosition, 1.0f));
    shaderLightDirection = vec3(normalTransform * spotLightDirection);

    // we still need OpenGL to compute the final vertex position in projection space
    // to correctly determine where the fragments of the triangle actually go on the screen
    gl_Position = projectionTransform * vec4(shaderPosition, 1.0f);
    
    /// light 2
    shaderLightPosition2 = vec3(viewTransform * vec4(lightPos2, 1.0f));

    /// light 3
    shaderLightPosition3 = vec3(viewTransform * vec4(lightPos3, 1.0f));

    /// light 4
    shaderLightPosition4 = vec3(viewTransform * vec4(lightPos4, 1.0f));

    /// light 5
    shaderLightPosition5 = vec3(viewTransform * vec4(lightPos5, 1.0f));

    ///////////////////////////////////////////////////////////////////////////
    // also compute this fragment position from the light's point of view
    ///////////////////////////////////////////////////////////////////////////
}
