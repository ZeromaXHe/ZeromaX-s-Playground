shader_type spatial;
render_mode skip_vertex_transform, unshaded, blend_add;

#define REVERSE_Z

uniform float lod = 0.0;
uniform float stretch_to_aspect = 0.5;

uniform int ghosts = 4;
uniform float ghost_dispersal = 0.5;
uniform float halo_width = 0.25;
uniform float distort = 0.25;

uniform float bloom_scale = 10.0;
uniform float bloom_bias = 0.95;

uniform float streak_strength = 1.0;

uniform int distortion_quality : hint_range(0, 2);

uniform sampler2D lens_color;
uniform sampler2D lens_dirt;
uniform sampler2D starburst;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

void vertex() {
#ifdef REVERSE_Z
	// https://godotengine.org/article/introducing-reverse-z/
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
#else
	POSITION = vec4(VERTEX, 1.0);
#endif
}

float weight(vec2 pos) {
	float w = length(vec2(0.5) - pos) / length(vec2(0.5));
	return pow(1.0 - w, 5.0);
}

vec3 bloomtex(sampler2D tex, vec2 texcoord) {
	return max(vec3(0.0), textureLod(tex, texcoord, lod).rgb - bloom_bias) * bloom_scale;
}

vec3 textureDistorted(sampler2D tex, vec2 texcoord, vec2 direction, float distortion) {
	vec3 color = vec3(0);
	float divisor = 1.0;
	color.r += bloomtex(tex, texcoord - direction * distortion).r;
	color.g += bloomtex(tex, texcoord).g;
	color.b += bloomtex(tex, texcoord + direction * distortion).b;

	if(distortion_quality == 1) {
		color.rg += bloomtex(tex, texcoord - direction * distortion * 0.5).rg * vec2(1.0, 0.5);
		color.gb += bloomtex(tex, texcoord + direction * distortion * 0.5).gb * vec2(0.5, 1.0);
		divisor = 2.0;
	} else if(distortion_quality == 2) {
		color.rg += bloomtex(tex, texcoord - direction * distortion * 0.667).rg * vec2(1.0, 0.333);
		color.rg += bloomtex(tex, texcoord - direction * distortion * 0.333).rg * vec2(1.0, 0.667);
		color.gb += bloomtex(tex, texcoord + direction * distortion * 0.333).gb * vec2(0.667, 1.0);
		color.gb += bloomtex(tex, texcoord + direction * distortion * 0.667).gb * vec2(0.333, 1.0);
		divisor = 3.0;
	}

	return color / divisor;
}

void fragment() {
	vec2 texcoord = 1.0 - SCREEN_UV;
	vec2 ghostVec = (vec2(0.5) - texcoord) * ghost_dispersal;

	float pixelSizeX = 1.0 / float(textureSize(SCREEN_TEXTURE, 0).x);

	float distortion = pixelSizeX * distort;
	vec2 direction = normalize(ghostVec);

	// Ghosts
	vec3 result = vec3(0.0);
	for(int i = 0; i < ghosts; ++i) {
		vec2 offset = fract(texcoord + ghostVec * float(i));
		result += textureDistorted(SCREEN_TEXTURE, offset, direction, distortion * weight(offset)) * weight(offset);
	}
	result *= texture(lens_color, vec2(length(vec2(0.5) - texcoord) / length(vec2(0.5)), 0)).rgb;

	// Halo
	vec2 aspect = vec2(1.0, mix(1.0, VIEWPORT_SIZE.x / VIEWPORT_SIZE.y, stretch_to_aspect));
	vec2 haloVec = normalize(ghostVec / aspect) * aspect * halo_width;
	result += textureDistorted(SCREEN_TEXTURE, texcoord + haloVec, direction, distortion) * weight(fract(texcoord + haloVec));

	// Starburst
	vec2 centerVec = SCREEN_UV - vec2(0.5);
	float d = length(centerVec);
	float radial = acos(centerVec.x / d);

	float starOffset = dot(INV_VIEW_MATRIX[2].xyz, vec3(1.0)) * 10.0;
	float star =
			texture(starburst, vec2(radial + starOffset)).r
		  * texture(starburst, vec2(radial + starOffset * 0.5)).r;
	star = clamp(star + (1.0 - smoothstep(0.0, 0.3, d)), 0.0, 1.0);
	result *= mix(0.5, star, streak_strength);

	ALBEDO = result * mix(texture(lens_dirt, texcoord).rgb, vec3(0.5), 0.4);
	//ALBEDO = texture(SCREEN_TEXTURE, SCREEN_UV).rgb * 2.;

	//uncomment to debug bright point extraction
	//ALBEDO = bloomtex(SCREEN_TEXTURE, SCREEN_UV).rgb;
}
