#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 aTexCoords;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    //gl_Position = mvp * vec4(a_Position, 1.0);
    FragPos = vec3(vec4(a_Position, 1.0) * model);
    TexCoords = aTexCoords;
    gl_Position = vec4(a_Position, 1.0) * model * view * projection;
    //gl_Position = projection * view * model * vec4(a_Position, 1.0);
}
