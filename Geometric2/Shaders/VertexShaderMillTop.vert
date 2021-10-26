#version 330
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec2 heightCoords1;
layout (location = 4) in vec2 heightCoords2;
layout (location = 5) in vec2 heightCoords3;

uniform sampler2D heightMap;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoords;

vec3 normalGenerator(vec3 a, vec3 b, vec3 c);
void main()
{
    //gl_Position = mvp * vec4(a_Position, 1.0);



    float xDiff = 1/10.0f;
    float yDiff = 1/10.0f;
    vec3 normal = aNormal;
    //if(normal.x < -1.0f)
        vec2 tHeightTexPos;
         
        tHeightTexPos = vec2(aTexCoords.x - xDiff, aTexCoords.y);
        float hL = texture(heightMap, tHeightTexPos).x;

        tHeightTexPos = vec2(aTexCoords.x + xDiff, aTexCoords.y);
        float hR = texture(heightMap, tHeightTexPos).x;

        tHeightTexPos = vec2(aTexCoords.x, aTexCoords.y - yDiff);
        float hD = texture(heightMap, tHeightTexPos).x;

        tHeightTexPos = vec2(aTexCoords.x, aTexCoords.y + yDiff);
        float hU = texture(heightMap, tHeightTexPos).x;

        vec3 v1 = vec3(xDiff,hL,0.0f)-vec3(-xDiff,hR,0.0f);
        vec3 v2 = vec3(0.0f,hU,yDiff)-vec3(0.0f,hD,-yDiff);
        
        normal = normalize(cross(v1,v2));
        normal.y=-normal.y;
        normal.z=-normal.z;
        //normal = vec3(0.0f,1.0f,0.0f);

        float height = texture(heightMap, aTexCoords).x;
    vec3 pos = vec3(a_Position.x, height, a_Position.z);
    //vec3 pos = vec3(a_Position.x, a_Position.y, a_Position.z);
    FragPos = vec3(vec4(pos, 1.0) * model);

//    vec3 a = vec3(texture(heightMap, heightCoords1)).x;
//    vec3 b = vec3(texture(heightMap, heightCoords2)).x;
//    vec3 c = vec3(texture(heightMap, heightCoords3)).x;

    //Normal = normalGenerator(a, b, c);
    Normal = aNormal;
    //Normal = normal;
    TexCoords = aTexCoords;

    gl_Position = vec4(pos, 1.0) * model * view * projection;
    //gl_Position = projection * view * model * vec4(a_Position, 1.0);
}

vec3 normalGenerator(vec3 a, vec3 b, vec3 c)
{
    vec3 A = b - a;
    vec3 B = c - a;
    float Nx = A.y * B.z - A.z * B.y;
    float Ny = A.z * B.x - A.x * B.z;
    float Nz = A.x * B.y - A.y * B.x;
    return vec3(Nx, Ny, Nz);
}
