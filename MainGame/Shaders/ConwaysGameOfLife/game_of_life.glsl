#[compute]
#version 450

const int gridWidth = 1024;
const vec4 aliveColor = vec4(1.0, 1.0, 1.0, 1.0);
const vec4 deadColor = vec4(0.0, 0.0, 0.0, 1.0);

layout (local_size_x = 32, local_size_y = 32, local_size_z = 1) in;

layout (set = 0, binding = 0, r8) restrict uniform readonly image2D inputImage;
layout (set = 0, binding = 1, r8) restrict uniform writeonly image2D outputImage;

bool isCellAlive(int x, int y) {
    vec4 pixel = imageLoad(inputImage, ivec2(x, y));
    return pixel.r > 0.5;
}

int getLiveNeighbors(int x, int y) {
    int count = 0;
    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
            if (i == 0 && j == 0) continue;
            int nx = x + i;
            int ny = y + j;
            if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridWidth) {
                vec4 pixel = imageLoad(inputImage, ivec2(nx, ny));
                count += int(isCellAlive(nx, ny));
            }
        }
    }
    return count;
}

void main() {
    ivec2 pos = ivec2(gl_GlobalInvocationID.xy);
    if (pos.x >= gridWidth || pos.y >= gridWidth) return;
    int liveNeighbors = getLiveNeighbors(pos.x, pos.y);
    bool isAlive = isCellAlive(pos.x, pos.y);
    bool nextState = isAlive;
    if (isAlive && liveNeighbors < 2 || liveNeighbors > 3) {
        nextState = false;
    } else if (!isAlive && liveNeighbors == 3) {
        nextState = true;
    }
    vec4 newColor = nextState ? aliveColor: deadColor;
    imageStore(outputImage, pos, newColor);
}