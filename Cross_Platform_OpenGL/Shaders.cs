using System;
using System.Collections.Generic;
using System.Text;

namespace Cross_Platform_OpenGL
{
    class Shaders
    {
        public static string GetVertexShader()
        {
            return @"
#version 100

attribute vec4 position;
attribute vec3 normal;

varying lowp vec4 colorVarying;

uniform mat4 modelViewProjectionMatrix;
uniform mat3 normalMatrix;

void main()
{
    vec3 eyeNormal = normalize(normalMatrix * normal);
    vec3 lightPosition = vec3(0.0, 0.0, 1.0);
    vec4 diffuseColor = vec4(1.0, 0.4, 0.4, 1.0);
    
    float nDotVP = max(0.0, dot(eyeNormal, normalize(lightPosition)));
                 
    colorVarying = diffuseColor * nDotVP;
    
    gl_Position = modelViewProjectionMatrix * position;
}
            ";
        }
        public static string GetFragmentShader()
        {
            return @"
#version 100

varying lowp vec4 colorVarying;

void main()
{
    gl_FragColor = colorVarying;
}
";
        }
    }
}
