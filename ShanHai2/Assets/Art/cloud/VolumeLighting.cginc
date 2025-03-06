#define PI 3.14159265359

float HGPhase(float3 inDir,float3 outDir,float g)
{
    float Costheta = dot(normalize(inDir),normalize(outDir));
    float resultParent = 4 * PI *pow(1 + pow(g,2) - 2 * g*Costheta,1.5);
    return (1 - pow(g,2))/resultParent;
    //return 1;
}



