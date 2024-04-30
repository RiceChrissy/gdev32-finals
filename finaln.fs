#version 330 core

in vec3 shaderPosition;
in mat3 shaderTBN;
in vec2 shaderTexCoord;
in vec3 shaderLightPosition;
in vec3 shaderLightPosition2;
in vec3 shaderLightPosition3;
in vec3 shaderLightPosition4;
in vec3 shaderLightPosition5;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
out vec4 fragmentColor;

in vec3 directionalNormal;

//New Variables for Exercise 2
in vec3 shaderLightDirection;
uniform float spotLightRadius;
uniform float deg;                  //phi
uniform float spotLightOuterRadius;
uniform float outerDeg;             //gamma
uniform bool attenuationIsOn;



//Source: https://learnopengl.com/Lighting/Light-casters
// vec3 calculateSunLight(vec3 normal, vec3 viewDir, vec3 lAmbient, vec3 lDiffuse, vec3 lSpecular)
// {
//     vec3 direction = normalize(viewDir);
//     vec3 lightDir = normalize(-direction);
//     // diffuse shading
//     float diff = max(dot(normal, lightDir), 0.0);
//     // specular shading
//     vec3 reflectDir = reflect(-lightDir, normal);
//     // float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
//     // combine results
//     vec3 ambient = lAmbient * vec3(texture(diffuseMap, shaderTexCoord));
//     vec3 diffuse = lDiffuse * diff * vec3(texture(diffuseMap, shaderTexCoord));
//     // vec3 specular = lightSpecular * spec * vec3(texture(material.specular, shaderTexCoord));
//     vec3 specular = lSpecular * vec3(texture(diffuseMap, shaderTexCoord));
//     return (ambient + diffuse + specular);
// }

// vec4 calculateSpotLight(vec3 shaderLightPos)
// {
//     // Define spotlight properties
//     vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
//     float ambientIntensity = 0.05f;
//     float specularIntensity = 0.5f;            // specular (better implementation: look this up from a specular map!)
//     float specularPower = 4.0f;

//     // Look up the normal from the normal map and reorient it with the TBN matrix
//     vec3 textureNormal = vec3(texture(normalMap, shaderTexCoord));
//     textureNormal = normalize(textureNormal * 2.0f - 1.0f);
//     vec3 normalDir = normalize(shaderTBN * textureNormal);

//     // Calculate ambient
//     vec3 lightAmbient = lightColor * ambientIntensity;
//     vec3 sunLightAmbient = lightColor * ambientIntensity;

//     // Calculate the direction from the light to the fragment
//     vec3 lightDir = normalize(shaderLightPosition - shaderPosition);
//     vec3 sunLightDir = normalize(vec3(-0.2f, -1.0f, -0.3f));

//     // Calculate the angle between the light direction and the fragment's direction
//     //Source: https://learnopengl.com/Lighting/Light-casters
//     float theta     = dot(lightDir, normalize(-shaderLightDirection));
//     float epsilon   = spotLightRadius - spotLightOuterRadius;
//     float intensity = clamp((theta - spotLightOuterRadius) / epsilon, 0.0, 1.5);

//     //Calculate Attenuation
//     float distance    = length(shaderLightPosition - shaderPosition);
//     float attenuation = 1.0 / (1.0f + (0.045f * distance) + (0.0075f * distance * distance));
//                             //constant  +   linear  +   quadratic

//     // Calculate diffuse
//     vec3 lightDiffuse = max(dot(normalDir, -lightDir), 0.0f) * lightColor;
//     vec3 sunLightDiffuse = max(dot(normalDir, -sunLightDir), 0.0f) * lightColor;

//     // Calculate specular
//     vec3 viewDir = normalize(-shaderPosition);
//     vec3 reflectDir = reflect(-lightDir, normalDir);
//     vec3 lightSpecular = pow(max(dot(reflectDir, viewDir)* intensity, 0), specularPower) * lightColor * specularIntensity;
//     vec3 sunLightSpecular = pow(max(dot(reflectDir, viewDir), 0), specularPower) * lightColor * specularIntensity;

//     // Check if the fragment is within the spotlight cone
//     vec4 result;
//     if (intensity > 0)
//     {
//         // Compute final fragment color
//         if (attenuationIsOn == true){
//             lightDiffuse *= attenuation;
//             lightSpecular *= attenuation;
//             return result = vec4((lightAmbient + lightDiffuse + lightSpecular), 1.0f);
//         }
//         else
//             return result = vec4((lightAmbient + lightDiffuse + lightSpecular), 1.0f);
//     }
//     else
//     {
//         // Discard if fragment is outside the spotlight cone. This was used to make the spotlight more noticeable
//         // zeroing-out diffuse, ambient, and specular is actually better, exclude ambient so it's not pitch black
//         // lightDiffuse = lightSpecular = vec3(0.0f, 0.0f, 0.0f);
//         // lightAmbient = lightAmbient/5;  //Ambient outside the spotlight is dimmer than the ambient inside the spotlight
//         // vec3 test = sunLight(directionalNormal, sunLightDir, lightAmbient, sunLightDiffuse, sunLightSpecular);
//         // test += vec3((lightAmbient + lightDiffuse + lightSpecular));
//         return result = vec4(sunLightAmbient+sunLightDiffuse+sunLightSpecular, 1.0f);
//     }
// }

vec4 calculateSpotLight(vec3 shaderLightPos)
{
    // Define spotlight properties
    vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
    float ambientIntensity = 0.05f;
    float specularIntensity = 0.5f;            // specular (better implementation: look this up from a specular map!)
    float specularPower = 4.0f;

    // Look up the normal from the normal map and reorient it with the TBN matrix
    vec3 textureNormal = vec3(texture(normalMap, shaderTexCoord));
    textureNormal = normalize(textureNormal * 2.0f - 1.0f);
    vec3 normalDir = normalize(shaderTBN * textureNormal);

    // Calculate ambient
    vec3 lightAmbient = lightColor * ambientIntensity;

    // Calculate the direction from the light to the fragment
    vec3 lightDir = normalize(shaderLightPos - shaderPosition);

    // Calculate the angle between the light direction and the fragment's direction
    //Source: https://learnopengl.com/Lighting/Light-casters
    float theta     = dot(lightDir, normalize(-shaderLightDirection));
    float epsilon   = spotLightRadius - spotLightOuterRadius;
    float intensity = clamp((theta - spotLightOuterRadius) / epsilon, 0.0, 1.5);

    //Calculate Attenuation
    float distance    = length(shaderLightPos - shaderPosition);
    float attenuation = 1.0 / (1.0f + (0.045f * distance) + (0.0075f * distance * distance));
                            //constant  +   linear  +   quadratic

    // Calculate diffuse
    vec3 lightDiffuse = max(dot(normalDir, -lightDir), 0.0f) * lightColor;
    // Calculate specular
    vec3 viewDir = normalize(-shaderPosition);
    vec3 reflectDir = reflect(-lightDir, normalDir);
    vec3 lightSpecular = pow(max(dot(reflectDir, viewDir)* intensity, 0), specularPower) * lightColor * specularIntensity;

    // Check if the fragment is within the spotlight cone
    vec4 result;
    lightDiffuse *= intensity;
    lightAmbient *= intensity;
    lightSpecular *= intensity;
    // Compute final fragment color
    if (attenuationIsOn == true)
        return result = vec4((lightAmbient + lightDiffuse + lightSpecular) * attenuation, 1.0f);
    else
        return result = vec4((lightAmbient + lightDiffuse + lightSpecular), 1.0f);
}

vec4 calculateSunLight(vec3 sunLightDir, vec3 lAmbient, vec3 lDiffuse, vec3 lSpecular)
{
    vec3 lightDir = normalize(-sunLightDir);
    // combine results
    vec3 ambient = lAmbient;
    vec3 diffuse = lDiffuse;
    vec3 specular = lSpecular;
    return vec4((ambient + diffuse + specular), 1.0f);
}

void main()
{
    vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
    float ambientIntensity = 0.05f;
    float specularIntensity = 0.5f;            // specular (better implementation: look this up from a specular map!)
    float specularPower = 4.0f;
    // Look up the normal from the normal map and reorient it with the TBN matrix
    vec3 textureNormal = vec3(texture(normalMap, shaderTexCoord));
    textureNormal = normalize(textureNormal * 2.0f - 1.0f);
    vec3 normalDir = normalize(shaderTBN * textureNormal);

    vec3 lightDir = normalize(shaderLightPosition - shaderPosition);
    vec3 sunLightDir = normalize(vec3(-0.2f, -1.0f, -0.3f));
    vec3 viewDir = normalize(-shaderPosition);
    
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normalDir);

    vec3 sunLightAmbient = lightColor * ambientIntensity;
    vec3 sunLightDiffuse = max(dot(normalDir, -sunLightDir), 0.0f) * vec3(0.5f, 0.5f, 0.5f);
    vec3 sunLightSpecular = pow(max(dot(reflectDir, viewDir), 0), specularPower) * lightColor;

    vec4 e = vec4((sunLightAmbient + sunLightDiffuse + sunLightSpecular), 1.0f);
    e+= calculateSpotLight(shaderLightPosition);
    e+= calculateSpotLight(shaderLightPosition2);
    e+= calculateSpotLight(shaderLightPosition3);
    e+= calculateSpotLight(shaderLightPosition4);
    e+= calculateSpotLight(shaderLightPosition5);

    fragmentColor = e * texture(diffuseMap, shaderTexCoord);
}