// 原代码来源（MIT 协议）：
// https://www.shadertoy.com/view/Xf2XWt
// 从 Shadertoy 翻译参考文档：
// https://docs.godotengine.org/zh-cn/4.x/tutorials/shaders/converting_glsl_to_godot_shaders.html
shader_type canvas_item;

float msign(in float x) { return (x<0.0)?-1.0:1.0; }

float sdEllipse( vec2 p, in vec2 ab )
{
	p = abs( p ); 
    if( p.x>p.y ){ p=p.yx; ab=ab.yx; }
	
	float l = ab.y*ab.y - ab.x*ab.x;
	
    float m = ab.x*p.x/l; 
	float n = ab.y*p.y/l; 
	float m2 = m*m;
	float n2 = n*n;
	
    float c = (m2+n2-1.0)/3.0; 
	float c3 = c*c*c;

    float d = c3 + m2*n2;
    float q = d  + m2*n2;
    float g = m  + m *n2;

    float co;

    if( d<0.0 )
    {
        float h = acos(q/c3)/3.0;
        float s = cos(h) + 2.0;
        float t = sin(h) * sqrt(3.0);
        float rx = sqrt( m2-c*(s+t) );
        float ry = sqrt( m2-c*(s-t) );
        co = ry + sign(l)*rx + abs(g)/(rx*ry);
    }
    else
    {
        float h = 2.0*m*n*sqrt(d);
        float s = msign(q+h)*pow( abs(q+h), 1.0/3.0 );
        float t = msign(q-h)*pow( abs(q-h), 1.0/3.0 );
        float rx = -(s+t) - c*4.0 + 2.0*m2;
        float ry =  (s-t)*sqrt(3.0);
        float rm = sqrt( rx*rx + ry*ry );
        co = ry/sqrt(rm-rx) + 2.0*g/rm;
    }
    co = (co-m)/2.0;

    float si = sqrt( max(1.0-co*co,0.0) );
 
    vec2 r = ab * vec2(co,si);
	
    return length(r-p) * msign(p.y-r.y);
}

float hash13(vec3 p3)
{
    //  from DAVE HOSKINS
    p3 = fract(p3 * .1031);
    p3 += dot(p3, p3.zyx + 31.32);
    return fract((p3.x + p3.y) * p3.z);
}

#define LESS_TIME (TIME * 0.125f * 0.5f)

struct Ring
{
    vec2 center;
    vec2 radius;
    vec3 color1;
    vec3 color2;
    float time;
};

vec3 glowingRing(vec2 p, Ring ring)
{      
    float T = ring.time;
    vec2 rad = ring.radius + cos(T*0.5 + vec2(0.3,1.0));

    float d = sdEllipse( p + ring.center, rad );   
    vec3 col = ring.color1;

    col *= exp(-2.5*abs(d));
    col = mix( col, ring.color2, 1.0-smoothstep(0.0,0.005,abs(d)) );
    
    return col;
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 iResolution = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 p = (2.0* FRAGCOORD.xy - iResolution.xy)/iResolution.xy;
       
    vec3 c1 = vec3(189.0f, 151.0f, 100.0f) / 255.0f;
    vec3 c3 = vec3( 75.0f, 123.9f, 163.0f) / 255.0f;

    vec3 c2 = c1 + vec3(0.3f, 0.3f, 0.3f);
    vec3 c4 = c3 + vec3(0.3f, 0.3f, 0.3f);
    
    vec3 color1 = vec3(0.0f);
    vec3 color2 = vec3(0.0f);
    
    Ring ring;
    
    for (float i = 0.0f; i < 3.0f; i++)
    {
        float o = i * 0.1f;
        ring.center = vec2(-2.0f, cos(LESS_TIME*1.123)* -2.0f);
        ring.radius = vec2( 2.5f, 2.5f + cos(LESS_TIME));
        ring.color1 = c1;
        ring.color2 = c2;
        ring.time = o + LESS_TIME * 1.0f;
        color1 = max(color1, glowingRing(p, ring));
    }

    for (float i = 0.0f; i < 2.0f; i++)
    {
        float o = i * 0.1f;
        ring.center = vec2( sin(LESS_TIME)*2.0f, 2.0f);
        ring.radius = vec2( 2.5f+cos(LESS_TIME), 2.5f);
        ring.color1 = c3;
        ring.color2 = c4;
        ring.time = o + LESS_TIME * 1.131f + 17.0;
        color2 = max(color2, glowingRing(p, ring));
    }
    
    //Blend additively or with screen blending
    // COLOR = vec4(color1+color2 - color1*color2, 1.0);
    COLOR = vec4(mix(max(color1, color2), color1+color2, 0.75f), 1.0);
    
    float noise = (hash13(vec3(FRAGCOORD.x, FRAGCOORD.y, TIME)) - 0.5f) * 4.0f / 255.0f;
    COLOR += noise;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
