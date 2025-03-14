#version 330 core

struct Material {
    sampler2D textureDiff;
    sampler2D textureNormal;
    vec3 specular;
    float shininess; 
};

struct Light {
    //vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;

uniform vec3 lightPos;
uniform vec3 viewPos;


//uniform sampler2D textureDiff;
//uniform sampler2D textureNormal;

out vec4 FragColor;

//in vec3 Normal;
//in vec3 fragPos;
in vec2 texCoord;
in vec3 tangentLightPos;
in vec3 tangentViewPos;
in vec3 tangentFragPos;
in vec3 normal;
//in mat3 TBN;

//uniform vec3 tangentLightPos;
//uniform vec3 tangentViewPos;



void main()
{
    vec3 norm = texture(material.textureNormal, texCoord).rgb;
    norm = normalize(norm * 2.0 - 1.0);
    //norm = normal;

    //ambient
    vec3 ambient = light.ambient * vec3(texture(material.textureDiff, texCoord));

    //diffuse 
    vec3 lightDir = normalize(tangentLightPos - tangentFragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * (diff * vec3(texture(material.textureDiff, texCoord))); 

    //specular
    vec3 viewDir = normalize(tangentViewPos - tangentFragPos);
    //vec3 reflectDir = reflect(-lightDir, norm);
    vec3 halfDir = normalize(viewDir + lightDir);
    // float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    float spec = pow(max(dot(halfDir, norm), 0.0), material.shininess);
    vec3 specular = light.specular * (spec * material.specular);


    vec3 result = ambient + diffuse + specular;
    //FragColor = vec4(diff +result-result, 1.0);
    FragColor = vec4(result, 1.0);

//    vec3 norm_raw = texture(material.textureNormal, texCoord).rgb;
//    vec3 norm = normalize(norm_raw * 2.0 - 1.0);
//    norm = normalize(norm * TBN);
//    //vec3 norm = normal;
//
//    //ambient
//    vec3 ambient = light.ambient * vec3(texture(material.textureDiff, texCoord));
//
//    //diffuse 
//    vec3 lightDir = normalize(lightPos - fragPos);
//    float diff = max(dot(norm, lightDir), 0.0);
//    vec3 diffuse = light.diffuse * (diff * vec3(texture(material.textureDiff, texCoord))); 
//
//    //specular
//    vec3 viewDir = normalize(viewPos - fragPos);
//    //vec3 reflectDir = reflect(-lightDir, norm);
//    vec3 halfDir = normalize(viewDir + lightDir);
//    // float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
//    float spec = pow(max(dot(halfDir, norm), 0.0), material.shininess);
//    vec3 specular = light.specular * (spec * material.specular);
//
//
//    vec3 result = ambient + diffuse + specular;
//    FragColor = vec4(norm_raw*0.5+0.5+result-result, 1.0);
//
}