shader_type canvas_item;

uniform sampler2D unlitTexture; // 二进制 0 的材质
uniform sampler2D litTexture; // 二进制 1 的材质
uniform sampler2D binaryDataTexture; // 二进制数据 2D 材质
uniform int gridWidth; // 单元格数量，水平和垂直方向

const int cellSize = 8; // 单元格像素大小
const float fadeDistance = 32.0; // 开始渐隐的边界距离

void fragment() {
	vec2 totalGridSize = vec2(float(gridWidth) * float(cellSize));
	vec2 scaledUV = UV * float(gridWidth);
	ivec2 cellIndex = ivec2(floor(scaledUV));
	vec2 cellUV = fract(scaledUV);
	bool isInsideCell = all(greaterThanEqual(cellUV, vec2(0.0)))
		&& all(lessThan(cellUV, vec2(1.0)));
	vec2 binaryDataUV = vec2(
		float(cellIndex.x) / float(gridWidth - 1),
		float(cellIndex.y) / float(gridWidth - 1));
	float binaryValue = texture(binaryDataTexture, binaryDataUV).r;
	bool isWhite = binaryValue > 0.5;
	float distanceX = min(scaledUV.x, float(gridWidth) - scaledUV.x)
		* float(cellSize);
	float distanceY = min(scaledUV.y, float(gridWidth) - scaledUV.y)
		* float(cellSize);
	float distanceToBorder = min(distanceX, distanceY);
	vec4 color = texture(unlitTexture, cellUV);
	if (isWhite && isInsideCell) {
		color = texture(litTexture, cellUV);
	}
	float alphaFactor = log(distanceToBorder * 1.0) / log(fadeDistance * 1.0);
	color.a *= alphaFactor;
	COLOR = color;
}
