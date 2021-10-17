﻿#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;
out vec2 TexCoords;
out mat3 TBN;

void main()
{
    //gl_Position = mvp * vec4(a_Position, 1.0);
    FragPos = vec3(vec4(a_Position, 1.0) * model);
    TexCoords = aTexCoords;

    vec3 T = normalize(vec3(model * vec4(aTangent,   0.0)));
    vec3 B = normalize(vec3(model * vec4(aBitangent, 0.0)));
    vec3 N = normalize(vec3(model * vec4(aNormal,    0.0)));
    TBN = mat3(T, B, N);

    gl_Position = vec4(a_Position, 1.0) * model * view * projection;
    //gl_Position = projection * view * model * vec4(a_Position, 1.0);
}
