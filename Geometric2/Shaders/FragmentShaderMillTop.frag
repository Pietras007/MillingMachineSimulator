#version 330

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float     shininess;
};


struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;
uniform vec3 viewPos;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

in mat3 TBN;

void main()
{
    //Ambient
    vec3 lightPos = light.position;//vec3(0,1,0);
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords)) * 0.1;

    // Diffuse 
    vec3 norm = normalize(TBN * vec3(0, 0,1));//normalize(texture(material.diffuse, TexCoords).rgb*2.0 - 1.0);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));

    // Specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));

    vec3 result = ambient + diffuse + specular;
    gl_FragColor = vec4(result, 1.0);
    //gl_FragColor = vec4(vec3(texture(material.diffuse, TexCoords)), 1.0);
}
