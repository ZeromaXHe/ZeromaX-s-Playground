# Unity 中的 GPU 光线追踪：第 1 部分

https://www.gamedeveloper.com/programming/gpu-ray-tracing-in-unity-part-1

在本分步教程中学习如何从头开始创建基本的 GPU 光线跟踪器。在 Unity 中使用 Compute Shaders，您将能够渲染具有完美反射、硬阴影和超采样的球体，以实现抗锯齿。

[David Kuri](https://www.gamedeveloper.com/author/david-kuri), Blogger

May 4, 2018

21 Min Read

以下文章最初发布在这里：http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/

对于射线追踪来说，这确实是一个激动人心的时刻。最新进展，如[人工智能加速去噪](http://research.nvidia.com/publication/interactive-reconstruction-monte-carlo-image-sequences-using-recurrent-denoising)、微软宣布 [DirectX 12 中的原生支持](https://blogs.msdn.microsoft.com/directx/2018/03/19/announcing-microsoft-directx-raytracing/)以及彼得·雪莉在*按需付费*（*pay what you want*）的基础上[发布他的书](https://twitter.com/Peter_shirley/status/985561344555417600)，使光线追踪看起来终于有机会当庭（at court）被接受。现在谈论革命的开始可能还为时过早，但围绕这一主题学习和积累知识肯定是一个好主意。

在本文中，我们将使用 Unity 中的计算着色器从头开始编写一个非常简单的光线跟踪器。我们将使用的语言是 C# 用于脚本，HLSL 用于着色器。按照以下步骤操作，您将得到这样的渲染：

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/blt0b22585e69477b45/650e87da3e7aacd3026407f9/gpi-rt-teaser-sqr.png/?width=700&auto=webp&quality=80&disable=upscale)

## 射线追踪理论

首先，我想快速回顾一下基本的射线追踪理论。如果你熟悉，请跳过前面。

让我们想想照片在现实世界中是如何出现的——高度简化，但为了渲染的目的，这应该很好。这一切都始于发射光子的光源。光子沿直线飞行，直到它撞击表面，在那里它被反射或折射，并在减去表面吸收的一些能量后继续其旅程。最终，一些光子会撞击相机的图像传感器，从而产生最终的图像。光线追踪基本上模拟了这些步骤，以创建逼真的图像。

在实际应用中，光源发出的光子中只有一小部分会击中相机。因此，应用[亥姆霍兹互易原理](https://en.wikipedia.org/wiki/Helmholtz_reciprocity)，计算通常是相反的：光线不是从光源发射光子，而是从相机发射到场景中，反射或折射，最终击中光源。

我们将要构建的光线跟踪器是基于[特纳-惠特德（Turner Whitted）1980 年的一篇论文](https://dl.acm.org/citation.cfm?id=358882)。我们将能够模拟硬阴影和完美反射。它还将作为折射、漫反射全局照明、光泽反射和柔和阴影等更高级效果的基础。

## 基本设置

让我们从创建一个新的 Unity 项目开始。创建一个 C# 脚本 `RayTracingMaster.cs` 和一个计算着色器 `RayTracingShader.compute`。用一些基本代码填充 C# 脚本：

```c#
using UnityEngine;

public class RayTracingMaster : MonoBehaviour
{
    public ComputeShader RayTracingShader;

    private RenderTexture _target;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Render(destination);
    }

    private void Render(RenderTexture destination)
    {
        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        RayTracingShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Blit the result texture to the screen
        Graphics.Blit(_target, destination);
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            // Release render texture if we already have one
            if (_target != null)
                _target.Release();

            // Get a render target for Ray Tracing
            _target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
```

每当相机完成渲染时，Unity 都会自动调用 `OnRenderImage` 函数。要渲染，我们首先创建一个适当尺寸的渲染目标，并将其告知计算着色器。0 是计算着色器内核函数的索引——我们只有一个。

接下来，我们*分派*着色器。这意味着我们告诉 GPU 忙于执行着色器代码的多个线程组。每个线程组由着色器本身中设置的多个线程组成。线程组的大小和数量最多可以在三个维度上指定，这使得将计算着色器应用于任一维度的问题变得容易。在我们的例子中，我们希望为渲染目标的每个像素生成一个线程。Unity 计算着色器模板中定义的默认线程组大小为 `[numthreads(8,8,1)]`，因此我们将坚持这一点，每 8×8 像素生成一个线程组。最后，我们使用 `Graphics.Blit` 将结果写入屏幕。

让我们试试。将 `RayTracingMaster` 组件添加到场景的摄影机中（这对调用 `OnRenderImage` 很重要），指定计算着色器并进入播放模式。您应该看到 Unity 的计算着色器模板以美丽的三角形分形形式输出。

## 摄像头

现在我们可以在屏幕上显示东西了，让我们生成一些相机光线。由于 Unity 为我们提供了一个功能齐全的相机，我们将只使用计算出的矩阵来实现这一点。首先在着色器上设置矩阵。将以下行添加到脚本 `RayTracingMaster.cs` 中：

```c#
private Camera _camera;

private void Awake()
{
    _camera = GetComponent();
}

private void SetShaderParameters()
{
    RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
    RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
}
```

渲染前从 `OnRenderImage` 调用 `SetShaderParameters`。

在着色器中，我们定义了矩阵、射线结构和构造函数。请注意，在 HLSL 中，与 C# 不同，函数或变量声明需要在使用*前*出现。对于每个屏幕像素的中心，我们计算光线的原点和方向，并将后者作为颜色输出。以下是完整的着色器：

```glsl
#pragma kernel CSMain

RWTexture2D Result;
float4x4 _CameraToWorld;
float4x4 _CameraInverseProjection;

struct Ray
{
    float3 origin;
    float3 direction;
};

Ray CreateRay(float3 origin, float3 direction)
{
    Ray ray;
    ray.origin = origin;
    ray.direction = direction;
    return ray;
}

Ray CreateCameraRay(float2 uv)
{
    // Transform the camera origin to world space
    float3 origin = mul(_CameraToWorld, float4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;
    
    // Invert the perspective projection of the view-space position
    float3 direction = mul(_CameraInverseProjection, float4(uv, 0.0f, 1.0f)).xyz;
    // Transform the direction from camera to world space and normalize
    direction = mul(_CameraToWorld, float4(direction, 0.0f)).xyz;
    direction = normalize(direction);

    return CreateRay(origin, direction);
}

[numthreads(8,8,1)]
void CSMain (uint3 id : SV_DispatchThreadID)
{
    // Get the dimensions of the RenderTexture
    uint width, height;
    Result.GetDimensions(width, height);

    // Transform pixel to [-1,1] range
    float2 uv = float2((id.xy + float2(0.5f, 0.5f)) / float2(width, height) * 2.0f - 1.0f);

    // Get a ray for the UVs
    Ray ray = CreateCameraRay(uv);

    // Write some colors
    Result[id.xy] = float4(ray.direction * 0.5f + 0.5f, 1.0f);
}
```

尝试在检查器中旋转摄像头。你应该看到“多彩的天空”也有相应的表现。

现在，让我们用一个实际的天空盒替换这些颜色。我在我的例子中使用了 HDRI Haven 的 [Cape Hill](https://hdrihaven.com/hdri/?c=outdoor&h=cape_hill)，但你当然可以使用你喜欢的任何一个。下载并放入 Unity。在导入设置中，如果下载的分辨率高于 2048，请记住提高最大分辨率。现在，将 `public Texture SkyboxTexture` 添加到脚本中，在检查器中指定纹理，并通过将此行添加到 `SetShaderParameters` 函数中在着色器上进行设置：

```c#
RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
```

在着色器中，定义纹理和相应的采样器，以及我们将在一分钟内使用的 π 常数：

```glsl
Texture2D _SkyboxTexture;
SamplerState sampler_SkyboxTexture;
static const float PI = 3.14159265f;
```

现在，我们将对天空盒进行采样，而不是将方向写为颜色。为此，我们将[笛卡尔方向向量转换为球坐标](https://en.wikipedia.org/wiki/Spherical_coordinate_system#Coordinate_system_conversions)，并将其映射到纹理坐标。将 `CSMain` 的最后一位替换为：

```glsl
// Sample the skybox and write it
float theta = acos(ray.direction.y) / -PI;
float phi = atan2(ray.direction.x, -ray.direction.z) / -PI * 0.5f;
Result[id.xy] = _SkyboxTexture.SampleLevel(sampler_SkyboxTexture, float2(phi, theta), 0);
```

## 追踪

到现在为止，一直都还不错。现在我们开始实际追踪我们的光线。从数学上讲，我们将计算光线和场景几何体之间的交点，并存储命中参数（沿光线的位置、法线和距离）。如果我们的光线击中多个物体，我们将选择最接近的一个。让我们在着色器中定义结构体 RayHit：

```glsl
struct RayHit
{
    float3 position;
    float distance;
    float3 normal;
};

RayHit CreateRayHit()
{
    RayHit hit;
    hit.position = float3(0.0f, 0.0f, 0.0f);
    hit.distance = 1.#INF;
    hit.normal = float3(0.0f, 0.0f, 0.0f);
    return hit;
}
```

通常，场景由许多三角形组成，但我们将从简单的开始：相交一个无限的地平面和几个球体！

## 地面

在 y=0 处将直线与无限平面相交非常简单。不过，我们只接受正射线方向的命中，并拒绝任何不比潜在的先前命中更接近的命中。

默认情况下，HLSL 中的参数是按值传递的，而不是按引用传递的，因此我们只能处理副本，而不能将更改传播到调用函数。我们使用 `inout` 限定符传递 `RayHit bestHit`，以便能够修改原始结构。以下是着色器代码：

```glsl
void IntersectGroundPlane(Ray ray, inout RayHit bestHit)
{
    // Calculate distance along the ray where the ground plane is intersected
    float t = -ray.origin.y / ray.direction.y;
    if (t > 0 && t < bestHit.distance)
    {
        bestHit.distance = t;
        bestHit.position = ray.origin + t * ray.direction;
        bestHit.normal = float3(0.0f, 1.0f, 0.0f);
    }
}
```

要使用它，让我们添加一个框架 `Trace` 函数（我们将在一分钟内扩展它）：

```glsl
RayHit Trace(Ray ray)
{
    RayHit bestHit = CreateRayHit();
    IntersectGroundPlane(ray, bestHit);
    return bestHit;
}
```

此外，我们需要一个基本的着色功能。同样，我们传递带有 inout 的 Ray——稍后我们将在讨论反射时对其进行修改。出于调试目的，如果几何体被击中，我们将返回法线，否则将返回 skybox 采样代码：

```glsl
float3 Shade(inout Ray ray, RayHit hit)
{
    if (hit.distance < 1.#INF)
    {
        // Return the normal
        return hit.normal * 0.5f + 0.5f;
    }
    else
    {
        // Sample the skybox and write it
        float theta = acos(ray.direction.y) / -PI;
        float phi = atan2(ray.direction.x, -ray.direction.z) / -PI * 0.5f;
        return _SkyboxTexture.SampleLevel(sampler_SkyboxTexture, float2(phi, theta), 0).xyz;
    }
}
```

我们将在 `CSMain` 中使用这两个函数。如果您还没有删除 skybox 采样代码，请删除该代码，并添加以下行来跟踪光线并为命中对象着色：

```glsl
// Trace and shade
RayHit hit = Trace(ray);
float3 result = Shade(ray, hit);
Result[id.xy] = float4(result, 1);
```

## 球体

平面不是世界上最令人兴奋的东西，所以让我们马上添加一个球体。线球相交的数学可以在[维基百科](https://en.wikipedia.org/wiki/Line%E2%80%93sphere_intersection)上找到。这一次可能有两个射线击中候选点：入口点 p1-p2 和出口点 p1+p2。我们将首先检查入口点，只有在另一个出口点无效的情况下才使用出口点。在我们的例子中，球体被定义为由位置（xyz）和半径（w）组成的 `float4`。代码如下：

```glsl
void IntersectSphere(Ray ray, inout RayHit bestHit, float4 sphere)
{
    // Calculate distance along the ray where the sphere is intersected
    float3 d = ray.origin - sphere.xyz;
    float p1 = -dot(ray.direction, d);
    float p2sqr = p1 * p1 - dot(d, d) + sphere.w * sphere.w;
    if (p2sqr < 0)
        return;
    float p2 = sqrt(p2sqr);
    float t = p1 - p2 > 0 ? p1 - p2 : p1 + p2;
    if (t > 0 && t < bestHit.distance)
    {
        bestHit.distance = t;
        bestHit.position = ray.origin + t * ray.direction;
        bestHit.normal = normalize(bestHit.position - sphere.xyz);
    }
}
```

要添加球体，只需从 `Trace` 调用此函数，例如：

```glsl
// Add a floating unit sphere
IntersectSphere(ray, bestHit, float4(0, 3.0f, 0, 1.0f));
```

## 抗锯齿

目前的方法有一个问题：我们只测试每个像素的中心，所以你可以在结果中看到令人讨厌的锯齿效应（可怕的“锯齿”）。为了避免这种情况，我们将在每个像素上追踪多条光线，而不是一条。每条光线在像素区域内都有一个随机偏移。为了保持可接受的帧速率，我们正在进行渐进采样，这意味着我们将在每帧每像素跟踪一条光线，并在相机没有移动的情况下随时间对结果进行平均。每次相机移动（或任何其他参数，如视场、场景几何或场景照明发生变化），我们都需要重新开始。

让我们创建一个非常简单的图像效果着色器，用于将多个结果相加。将着色器命名为 `AddShader`，确保第一行为 `Shader "Hidden/AddShader"`。在 `Cull Off ZWrite Off ZTest Always` 后，添加 `Blend SrcAlpha OneMinusSrcAlpha` 以启用 alpha 混合。接下来，用以下行替换默认的 frag 函数：

```glsl
float _Sample;

float4 frag (v2f i) : SV_Target
{
    return float4(tex2D(_MainTex, i.uv).rgb, 1.0f / (_Sample + 1.0f));
}
```

此着色器现在将只绘制不透明度为 1 的第一个样本，下一个样本的不透明度为 1/2，然后是 1/3，以此类推，平均所有贡献相等的样本。

在脚本中，我们仍然需要计算样本并使用新创建的图像效果着色器：

```c#
private uint _currentSample = 0;
private Material _addMaterial;
```

当在 `InitRenderTexture` 中重建渲染目标时，您还应该重置 `_currentSamples = 0`，并添加一个检测相机变换更改的 `Update` 函数：

```c#
private void Update()
{
    if (transform.hasChanged)
    {
        _currentSample = 0;
        transform.hasChanged = false;
    }
}
```

要使用我们的自定义着色器，我们需要初始化一个材质，告诉它当前的示例，并在 `Render` 函数中将其用于 blitting 到屏幕：

```c#
// Blit the result texture to the screen
if (_addMaterial == null)
    _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
_addMaterial.SetFloat("_Sample", _currentSample);
Graphics.Blit(_target, destination, _addMaterial);
_currentSample++;
```

所以我们正在进行渐进采样，但我们仍然始终使用像素中心。在计算着色器中，定义一个 `float2 _PixelOffset`，并在 `CSMain` 中使用它，而不是硬 `float2(0.5f, 0.5f)` 偏移。回到脚本中，通过将此行添加到 `SetShaderParameters` 来创建随机偏移：

```c#
RayTracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));
```

如果你移动相机，你应该会看到图像仍然显示混叠，但如果你静止几帧，混叠会很快消失。以下是我们所做的好事的比较：

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/blt02e5720826f33f12/650e879d3663f053f0ece52e/gpu-rt-no-aa.png/?width=700&auto=webp&quality=80&disable=upscale)

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/bltf4fee65fcf88bb6e/650e87a64ba72b4bdbf96e61/gpu-rt-aa.png/?width=700&auto=webp&quality=80&disable=upscale)

## 反射

现在，我们的光线跟踪器的基础工作已经完成，所以我们可以开始处理那些真正将光线跟踪与其他渲染技术区分开来的奇特事情。完美的反射是我们清单上的第一项。这个想法很简单：每当我们撞击表面时，我们都会根据你可能在学校里记得的反射定律（入射角=反射角）反射光线，降低其能量，并重复直到我们撞击天空、耗尽能量或达到固定的最大反弹量。

在着色器中，向光线添加 `float3 energy`，并在 `CreateRay` 函数中将其初始化为 `ray.energy = float3(1.0f, 1.0f, 1.0f)`。光线在所有颜色通道上以全吞吐量开始，并将随着每次反射而减少。

现在，我们将执行最多 8 条轨迹（原始光线加上 7 次反弹），并将 `Shade` 函数调用的结果相加，但乘以光线的能量。例如，想象一条光线被反射一次，损失了 3/4 的能量。现在它继续行进并撞击天空，因此我们只将天空撞击能量的 1/4 传递给像素。像这样调整 `CSMain`，替换之前的 `Trace` 和 `Shade` 调用：

```glsl
// Trace and shade
float3 result = float3(0, 0, 0);
for (int i = 0; i < 8; i++)
{
    RayHit hit = Trace(ray);
    result += ray.energy * Shade(ray, hit);

    if (!any(ray.energy))
        break;
}
```

我们的 `Shade` 函数现在还负责更新能量和生成反射光线，所以这就是 `inout` 变得重要的地方。为了更新能量，我们使用曲面的镜面反射颜色执行元素乘法。例如，金的镜面反射率大致为 `float3(1.0f, 0.78f, 0.34f)`，因此它会反射 100% 的红光、78% 的绿光，但只有 34% 的蓝光，使反射呈现出明显的金色。注意不要用这些值中的任何一个超过 1，因为你会无缘无故地创造能量。此外，反射率通常比你想象的要低。请参阅 Naty Hoffman 的[《着色的物理和数学》](https://blog.selfshadow.com/publications/s2015-shading-course/hoffman/s2015_pbs_physics_math_slides.pdf)中的幻灯片 64，了解一些值。

HLSL 有一个内置功能，可以使用给定的法线反射光线，这很棒。由于浮点不准确，反射光线可能会被其反射的表面阻挡。为了防止这种自遮挡，我们将沿法线方向稍微偏移位置。这是新的 `Shade` 函数：

```glsl
float3 Shade(inout Ray ray, RayHit hit)
{
    if (hit.distance < 1.#INF)
    {
        float3 specular = float3(0.6f, 0.6f, 0.6f);

        // Reflect the ray and multiply energy with specular reflection
        ray.origin = hit.position + hit.normal * 0.001f;
        ray.direction = reflect(ray.direction, hit.normal);
        ray.energy *= specular;

        // Return nothing
        return float3(0.0f, 0.0f, 0.0f);
    }
    else
    {
        // Erase the ray's energy - the sky doesn't reflect anything
        ray.energy = 0.0f;

        // Sample the skybox and write it
        float theta = acos(ray.direction.y) / -PI;
        float phi = atan2(ray.direction.x, -ray.direction.z) / -PI * 0.5f;
        return _SkyboxTexture.SampleLevel(sampler_SkyboxTexture, float2(phi, theta), 0).xyz;
    }
}
```

您可能希望通过将其乘以大于 1 的因子来稍微增加天空盒的强度。现在，使用您的 `Trace` 函数。把一些球体放在一个循环中，你会得到这样的结果：

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/bltf6572e9b326618d9/650e879dbb57c67b292286f2/gpu-rt-mirror.png/?width=700&auto=webp&quality=80&disable=upscale)

## 方向光

因此，我们可以追踪镜面反射，这使我们能够渲染光滑的金属表面，但对于非金属，我们还需要一件事：漫反射。简而言之，金属只会反射被其镜面颜色着色的入射光，而非金属则允许光折射到表面，散射并沿随机方向离开，使其被其反照率颜色着色。在通常假设的理想[朗伯（Lambertian）曲面](https://en.wikipedia.org/wiki/Lambert%27s_cosine_law)的情况下，概率与所述方向与曲面法线之间的角度的余弦成正比。关于这个话题的更深入的讨论可以在[这里](https://computergraphics.stackexchange.com/questions/1513/how-physically-based-is-the-diffuse-and-specular-distinction)找到。

要开始使用漫反射照明，让我们将 `public Light DirectionalLight` 添加到我们的 `RayTracingMaster` 中，并指定场景的平行光。您可能还希望在 `Update` 函数中检测灯光的变换变化，就像我们已经为相机的变换所做的那样。现在将以下行添加到 `SetShaderParameters` 函数中：

```c#
Vector3 l = DirectionalLight.transform.forward;
RayTracingShader.SetVector("_DirectionalLight", new Vector4(l.x, l.y, l.z, DirectionalLight.intensity));
```

回到着色器中，定义 `float4 _DirectionalLight`。在 `Shade` 函数中，定义镜面反射颜色正下方的反照率颜色：

```glsl
float3 albedo = float3(0.8f, 0.8f, 0.8f);
```

用简单的漫反射着色替换之前的黑色返回：

```glsl
// Return a diffuse-shaded color
return saturate(dot(hit.normal, _DirectionalLight.xyz) * -1) * _DirectionalLight.w * albedo;
```

记住，点积定义为 `dot(a, b) = length(a) * length(b) * cos(theta)`。由于我们的两个向量（法线和光方向）都是单位长度，因此点积正是我们想要的：角度的余弦。光线和光线指向相反的方向，因此对于正面照明，点积返回 `-1` 而不是 `1`。我们需要翻转标志来弥补这一点。最后，我们将该值饱和（即将其钳制在 [0,1] 范围内）以防止负能量。

为了使平行光投射阴影，我们将追踪阴影光线。它从所讨论的表面位置开始（同样具有非常小的位移以避免自阴影），并指向光的来源方向。如果有什么东西挡住了通往无限的路，我们就不会使用任何漫射光。在 diffuse ` return` 语句上方添加以下行：

```glsl
// Shadow test ray
bool shadow = false;
Ray shadowRay = CreateRay(hit.position + hit.normal * 0.001f, -1 * _DirectionalLight.xyz);
RayHit shadowHit = Trace(shadowRay);
if (shadowHit.distance != 1.#INF)
{
    return float3(0.0f, 0.0f, 0.0f);
}
```

现在我们可以追踪一些带有硬阴影的光滑塑料球体！将镜面反射设置为0.04，反照率设置为0.8，得到以下图像：

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/bltb8388683a58f5b35/650e87c019d3133f45c61160/gpu-rt-diff-gloss.png/?width=700&auto=webp&quality=80&disable=upscale)

## 场景和材质

随着今天的高潮，让我们创造一些更复杂、更丰富多彩的场景！我们将用 C# 定义场景，而不是在着色器中硬编码所有内容，以获得更大的灵活性。

首先，我们将扩展着色器中的 `RayHit` 结构。我们将为每个对象定义材质属性并将其存储在 `RayHit` 中，而不是在 `Shade` 函数中全局定义材质属性。将 `float3 albedo` 和 `float3 specular` 添加到结构体中，并在 `CreateRayHit` 中将它们初始化为 `float3(0.0f, 0.0f, 0.07f)`。还要调整 `Shade` 函数，以使用 hit 中的这些值，而不是硬编码的值。

要建立对 CPU 和 GPU 上球体的共同理解，请在着色器和 C# 脚本中定义一个结构体 `Sphere`。在着色器方面，它看起来像这样：

```glsl
struct Sphere
{
    float3 position;
    float radius;
    float3 albedo;
    float3 specular;
};
```

在 C# 脚本中镜像此结构。

在着色器中，我们需要使 `IntersectSphere` 函数与我们的自定义结构而不是 `float4` 一起工作。这很容易做到：

```glsl
void IntersectSphere(Ray ray, inout RayHit bestHit, Sphere sphere)
{
    // Calculate distance along the ray where the sphere is intersected
    float3 d = ray.origin - sphere.position;
    float p1 = -dot(ray.direction, d);
    float p2sqr = p1 * p1 - dot(d, d) + sphere.radius * sphere.radius;
    if (p2sqr < 0)
        return;
    float p2 = sqrt(p2sqr);
    float t = p1 - p2 > 0 ? p1 - p2 : p1 + p2;
    if (t > 0 && t < bestHit.distance)
    {
        bestHit.distance = t;
        bestHit.position = ray.origin + t * ray.direction;
        bestHit.normal = normalize(bestHit.position - sphere.position);
        bestHit.albedo = sphere.albedo;
        bestHit.specular = sphere.specular;
    }
}
```

还可以在 `IntersectGroundPlane` 功能中设置 `bestHit.albedo` 和 `bestHit.specular` 来调整其材质。

接下来，定义 `StructuredBuffer _Spheres`。这是 CPU 将存储构成场景的所有球体的地方。从 `Trace` 函数中删除所有硬编码的球体，并添加以下行：

```glsl
// Trace spheres
uint numSpheres, stride;
_Spheres.GetDimensions(numSpheres, stride);
for (uint i = 0; i < numSpheres; i++)
    IntersectSphere(ray, bestHit, _Spheres[i]);
```

现在，我们将用一些生命来填充场景。回到 C#，让我们添加一些公共参数来控制球体的放置和实际的计算缓冲区：

```c#
public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
public uint SpheresMax = 100;
public float SpherePlacementRadius = 100.0f;
private ComputeBuffer _sphereBuffer;
```

在 `OnEnable` 中设置场景，并在 `OnDisable` 中释放缓冲区。这样，每次启用组件时都会生成一个随机场景。`SetUpScene` 函数将尝试将球体定位在特定半径内，并拒绝那些与现有球体相交的球体。一半的球体是金属的（黑色反照率，彩色镜面反射），另一半是非金属的（彩色反照率、4% 镜面反射）：

```c#
private void OnEnable()
{
    _currentSample = 0;
    SetUpScene();
}

private void OnDisable()
{
    if (_sphereBuffer != null)
        _sphereBuffer.Release();
}

private void SetUpScene()
{
    List spheres = new List();

    // Add a number of random spheres
    for (int i = 0; i < SpheresMax; i++)
    {
        Sphere sphere = new Sphere();

        // Radius and radius
        sphere.radius = SphereRadius.x + Random.value * (SphereRadius.y - SphereRadius.x);
        Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
        sphere.position = new Vector3(randomPos.x, sphere.radius, randomPos.y);

        // Reject spheres that are intersecting others
        foreach (Sphere other in spheres)
        {
            float minDist = sphere.radius + other.radius;
            if (Vector3.SqrMagnitude(sphere.position - other.position) < minDist * minDist)
                goto SkipSphere;
        }

        // Albedo and specular color
        Color color = Random.ColorHSV();
        bool metal = Random.value < 0.5f;
        sphere.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
        sphere.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;

        // Add the sphere to the list
        spheres.Add(sphere);

    SkipSphere:
        continue;
    }

    // Assign to compute buffer
    _sphereBuffer = new ComputeBuffer(spheres.Count, 40);
    _sphereBuffer.SetData(spheres);
}
```

新 ComputeBuffer 中的魔法数 40（sphere.Count，40）是缓冲区的步长，即内存中一个球体的字节大小。要计算它，请计算 Sphere 结构中的浮点数，并将其乘以浮点数的字节大小（4 个字节）。最后，在 `SetShaderParameters` 函数中设置着色器上的缓冲区：

```c#
RayTracingShader.SetBuffer(0, "_Spheres", _sphereBuffer);
```

## 结果

恭喜你，你做到了！现在，您有了一个可用的 GPU 驱动的 Whitted 光线跟踪器，能够渲染一个平面和许多具有镜面反射、简单漫反射照明和硬阴影的球体。完整的源代码可以在 [Bitbucket](https://bitbucket.org/Daerst/gpu-ray-tracing-in-unity/commits/tag/Tutorial_Pt1) 上找到。玩弄球体放置参数，欣赏美丽的景色：

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/blt663e6457d839e088/650eb3ee65a9a1cb8efd5e92/gpu-rt-result-dark.png/?width=700&auto=webp&quality=80&disable=upscale)

![img](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/bltfdc163ec09ce388d/650eb4135ec1bbc8c9e18b8b/gpu-rt-result-close.png/?width=700&auto=webp&quality=80&disable=upscale)

## 接下来是什么？

我们今天取得了相当大的成就，但仍有很多工作要做：漫反射全局照明、光泽反射、柔和阴影、带折射的非不透明材质，以及显然使用三角形网格而不是球体。在下一篇文章中，我们将把 Whitted 射线跟踪器扩展为路径跟踪器，以克服许多提到的现象。

感谢您抽出时间阅读本文！

# Unity 中的 GPU 光线追踪：第 2 部分

https://blog.csdn.net/wodownload2/article/details/103897548

原文章链接：http://blog.three-eyed-games.com/2018/05/12/gpu-path-tracing-in-unity-part-2/

> “没有什么比模糊概念的清晰图像更糟糕的了。”—— Ansel Adams

在本系列的第一部分中，我们创建了一个 Whitted 光线跟踪器，能够跟踪完美的反射和硬阴影。缺少的是模糊效果：漫反射、光泽反射和柔和阴影。

基于我们已有的代码，我们将迭代求解我在 1986 年制定的 James Kajiya 的渲染方程，并将我们的渲染器转换为能够捕获上述效果的路径跟踪器。同样，我们将使用C#编写脚本，使用 HLSL 编写着色器。代码托管在 Bitbucket 上。https://bitbucket.org/Daerst/gpu-ray-tracing-in-unity/src/Tutorial_Pt2/

这篇文章比上一篇文章更加数学化，但不要害怕。我会尽力解释每一个公式。这些公式可以帮助您了解发生了什么以及我们的渲染器为什么工作，所以我建议您尝试理解它们，如果有任何不清楚的地方，请在评论部分提问。

下图是使用 HDRI Haven 的涂鸦庇护所渲染的。本文中的其他图像是使用 Kiara 9 Dusk 渲染的。

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/2d28134051564f4505083672cc715a9f.png)

## 渲染方程

从形式上讲，照片级真实感渲染器的任务是求解渲染方程，可以写成如下：

$$L(x, \vec\omega_o)=L_e(x,\vec\omega_o)+\int_\Omega f_r(x,\vec\omega_i,\vec\omega_o)(\vec\omega_i \cdot \vec n)L(x,\vec\omega_i)d \vec\omega_i$$

让我们把它分解一下。最终，我们想确定屏幕上像素的亮度。渲染方程给出了从 x 点（光线的撞击点）沿 $\vec\omega_o$ 方向（光线来自的方向）发出的光量 $L(x, \vec\omega_o)$。表面本身可能是一个光源，向我们的方向发射光 $L(x, \vec\omega_o)$。大多数表面不会，所以它们只反射来自外部的光。这就是积分的作用。直观地说，它会聚集来自法线周围半球 $\Omega$ 中每个可能方向的光（所以现在我们只考虑从上方到达表面的光，而不是从下方到达的光，因为半透明材料需要从下方到达）。

第一部分 fr 称为双向反射分布函数（BRDF）。此功能直观地描述了我们处理的材料类型——金属或电介质，亮或暗，有光泽或无光泽。BRDF 定义了来自 $\vec\omega_i$ 的光在 $\vec\omega_o$ 方向上反射的比例。在实践中，这是使用红、绿和蓝光量的三分量向量来处理的，每个分量在 [0,1] 范围内。

第二部分 $\vec\omega_i \cdot \vec n$ 等价于 $\cos\theta$，其中 $\theta$ 是入射光和表面法线 $\vec n$ 之间的角度。想象一束平行光迎面照射到表面上。现在想象同一束光以平角照射到表面。光线将扩散到更大的区域，但这也意味着该区域中的每个点都比以前更暗。余弦解释了这一点。

最后，使用相同的方程递归地确定来自 $\vec\omega_i$ 的实际光。因此，点 x 处的光取决于来自上半球所有可能方向的入射光。在从点 x 开始的每个方向上都有另一个点 x'，其亮度再次取决于该点上半球所有可能方向的入射光。冲洗并重复。

这是一个在无穷多个半球积分域上的无穷递归积分方程。我们无法直接求解这个方程，但有一个相当简单的解。

1记住这一点！我们将谈论很多余弦，当我们谈论余弦时，我们指的是点积。由于 $a \cdot b =|a||b|\cos \theta$，并且我们处理的是方向（单位长度矢量），因此在计算机图形学中，点积在大多数情况下都是余弦。

## 蒙特卡洛来救援

蒙特卡罗积分是一种数值积分技术，它允许我们使用有限数量的随机样本来估计任何积分。此外，蒙特卡洛保证收敛到正确的解——你取的样本越多越好。以下是一般形式：

$$F_N \approx \frac{1}{N}\sum\limits^N_{n=0}\frac{f(x_n)}{p(x_n)}$$

因此，函数 $f(x_n)$ 的积分可以通过在积分域上对随机样本进行平均来估计。每个样本除以它被选择的概率 $p(x_n)$。这样，经常选择的样本的权重将小于很少选择的样本。

在半球上均匀采样（每个方向都有相同的被选中的概率），样本的概率是恒定的：$p(\omega) = 1/2\pi$（因为2π是单位半球的表面积）。如果你把这一切结合在一起，你就会得到：

$$L(x, \vec\omega_o)\approx L_e(x,\vec\omega_o)+\frac{1}{N}\sum\limits^N_{n=0}2\pi f_r(x,\vec\omega_i,\vec\omega_o)(\vec\omega_i \cdot \vec n)L(x,\vec\omega_i)$$

发射 $L_e(x,\vec\omega_o)$ 只是 `Shade` 函数的返回值。1/N 是我们的 `AddShader` 中已经发生的事情。当我们反射光线并进一步追踪它时，就会发生与 $L(x,\vec\omega_i)$ 的乘法。我们的使命是用一些生命来填充等式的绿色部分。

## 先决条件

在我们开始冒险之前，让我们先处理一些事情：样本积累、确定性场景和着色器随机性。

### 积累（Accumulation）

出于某种原因，**Unity 不会在 `OnRenderImage` 中给我 HDR 纹理作为目标**。对我来说，格式是 `R8G8B8A8_Typeless`，因此精度很快就会太低，无法将多个样本相加。为了克服这个问题，让我们将 `private RenderTexture converged` 添加到我们的 C# 脚本中。这将是我们的缓冲区，在将结果显示在屏幕上之前，以高精度累积结果。初始化/释放此纹理与 `InitRenderTexture` 函数中的 `_target` 完全相同。在 `Render` 函数中，将 blit 加倍：

```c#
Graphics.Blit(_target, _converged, _addMaterial);
Graphics.Blit(_converged, destination);
```

### 确定性场景

当您对渲染进行更改时，将其与之前的结果进行比较以判断效果会有所帮助。目前，每次重新启动播放模式或重新编译脚本时，我们都会看到一个新的随机场景。为了克服这个问题，在 C# 脚本中添加一个 `public int SphereSeed`，并在 `SetUpScene` 的开头添加以下行：

```c#
Random.InitState(SphereSeed);
```

现在，您可以手动设置场景的种子。输入任意数字并禁用/重新启用 `RayTracingMaster` 组件，直到找到您喜欢的场景。

用于示例图像的设置是：球体种子 1223832719、球体半径 [5,30]、球体最大值 10000、球体放置半径 100。

### 着色器随机性

在开始任何随机（stochastic）采样之前，我们需要着色器中的随机性。我正在使用我在网上某个地方找到的经典单行，https://stackoverflow.com/questions/12964279/whats-the-origin-of-this-glsl-rand-one-liner 为方便起见进行了修改：

```glsl
float2 _Pixel;
float _Seed;
float rand()
{
    float result = frac(sin(_Seed / 100.0f * dot(_Pixel, float2(12.9898f, 78.233f))) * 43758.5453f);
    _Seed += 1.0f;
    return result;
}
```

在 CSMain 中直接将 `_Pixel` 初始化为 `_Pixel = id.xy`，因此每个像素将使用不同的随机值 `_Seed` 在 `SetShaderParameters`函数中从 C# 初始化。

```c#
RayTracingShader.SetFloat("_Seed", Random.value);
```

我们在这里生成的随机数的质量是不确定的。值得研究和测试此功能，分析参数的影响并将其与其他方法进行比较。目前，我们只会使用它，并希望最好的结果。

## 半球采样

首先，我们需要在半球上均匀分布的随机方向。对于整个领域，Cory Simon 在本文中详细描述了这一非平凡的挑战。http://corysimon.github.io/articles/uniformdistn-on-sphere/

它很容易适应半球。以下是着色器代码：

```glsl
float3 SampleHemisphere(float3 normal)
{
    // Uniformly sample hemisphere direction
    float cosTheta = rand();
    float sinTheta = sqrt(max(0.0f, 1.0f - cosTheta * cosTheta));
    float phi = 2 * PI * rand();
    float3 tangentSpaceDir = float3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
    // Transform direction to world space
    return mul(tangentSpaceDir, GetTangentSpace(normal));
}
```

方向是为以正 Z 为中心的半球生成的，因此我们需要将其转换为以我们需要的任何法线为中心。为此，我们生成一个切线和二项式（两个与法线正交且彼此正交的向量）。我们首先选择一个辅助向量来生成切线。为此，我们取正 X，只有当法线（几乎）与 X 轴重合时，才会回落到正 Z。然后，我们可以使用叉积来生成切线，并随后生成二项式。

```glsl
float3x3 GetTangentSpace(float3 normal)
{
    // Choose a helper vector for the cross product
    float3 helper = float3(1, 0, 0);
    if (abs(normal.x) > 0.99f)
        helper = float3(0, 0, 1);
    // Generate vectors
    float3 tangent = normalize(cross(normal, helper));
    float3 binormal = normalize(cross(normal, tangent));
    return float3x3(tangent, binormal, normal);
}
```

## 兰伯特漫射

现在我们有了统一的随机方向，我们可以开始实现第一个 BRDF。Lambert BRDF 是漫反射最常用的方法，它非常简单：$f_r(x, \vec\omega_i, \vec\omega_o)$，其中 $k_d$ 是表面的反照率。让我们将其插入到蒙特卡洛渲染方程中（我现在将省略发射项），看看会发生什么：

$$L(x,\vec\omega_o) \approx \frac{1}{N}\sum\limits^N_{n=0}2k_d(\vec\omega_i\cdot\vec n)L(x, \vec\omega_i)$$

让我们马上把它放在着色器中。在 `Shade` 函数中，将 `if (hit.distance<1.#INF)` 子句中的代码替换为以下行：

```glsl
// Diffuse shading
ray.origin = hit.position + hit.normal * 0.001f;
ray.direction = SampleHemisphere(hit.normal);
ray.energy *= 2 * hit.albedo * sdot(hit.normal, ray.direction);
return 0.0f;
```

使用我们的均匀半球采样函数确定反射光线的新方向。射线的能量与上述方程的相关部分相乘。由于表面不发射任何光（而只反射它直接或间接从天空接收到的光），我们在这里返回 0。请记住，我们的 `AddShader` 为我们平均了样本，所以我们不需要关心 $\frac{1}{N}\sum$。CSMain 函数已经包含了与 $L(x,\vec\omega_i)$（下一个反射光线）的乘法，所以我们没什么可做的。

sdot 函数是我为自己定义的一个简单实用程序。它只是返回点积的结果，加上一个可选因子，然后被箝位为[0,1]：

```glsl
float sdot(float3 x, float3 y, float f = 1.0f)
{
    return saturate(dot(x, y) * f);
}
```

让我们回顾一下到目前为止代码的功能。CSMain 生成我们的主相机光线并调用 `Shade`。如果一个表面被击中，此函数将依次生成一条新的光线（在法线周围的半球中均匀随机），并将材料的BRDF和余弦因子计入光线的能量。如果天空被击中，我们将对 HDRI（我们唯一的光源）进行采样，并返回光线，该光线与光线的能量相乘（即从相机开始的所有先前撞击的乘积）。这是一个与收敛结果混合的单个样本。最后，每个样本的贡献为 1/N。
是时候试试了。由于金属没有漫反射，现在让我们在 C# 脚本的 `SetUpScene` 函数中禁用它们（仍在此处调用 `Random.value` 以保持场景确定性）：
进入播放模式，看看最初嘈杂的图像是如何清除并收敛到这样的良好渲染的：

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/28d98388ada1dde77d3b8f35bde7b977.png)

## Phong 镜面

对于几行代码来说还不错（还有一些仔细的数学计算——我看到你们正在慢慢成为朋友）。我们来加点香料吧通过添加 Phong BRDF 来解决镜面反射问题。最初的 Phong 配方也有一些问题（不是互惠的（reciprocal），不是节能的），但幸运的是，其他人解决了这个问题。修改后的 Phong BRDF 看起来像这样，其中 $\vec\omega_r$ 是完美反射的光方向，α 是控制粗糙度的 Phong 指数：

$$f_r(x, \vec\omega_i,\vec\omega_o) = k_s\frac{\alpha+2}{2\pi}(\vec\omega_r\cdot\vec\omega_o)^\alpha$$

这是一个小的二维图，显示了 α=15 的 Phong BRDF 在 45° 角入射光线下的样子。单击右下角可自行更改 α 值。

https://www.desmos.com/calculator/h7m3esuf28

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/692ef045d780eb942ae2462ea278b05a.png)

将其代入我们的蒙特卡洛渲染方程：

$$L(x,\vec\omega_o)\approx\frac{1}{N}\sum\limits^N_{n=0}k_s(\alpha+2)(\vec\omega_r\cdot\vec\omega_o)^\alpha(\vec\omega_i\cdot\vec n)L(x, \vec\omega_i)$$

最后，将此添加到我们已经拥有的 Lambert BRDF 中：

$$L(x,\vec\omega_o)\approx\frac{1}{N}\sum\limits^N_{n=0}[2k_d + k_s(\alpha+2)(\vec\omega_r\cdot\vec\omega_o)^\alpha](\vec\omega_i\cdot\vec n)L(x, \vec\omega_i)$$

在这里，它与 Lambert 漫反射一起出现在代码中：

```glsl
// Phong shading
ray.origin = hit.position + hit.normal * 0.001f;
float3 reflected = reflect(ray.direction, hit.normal);
ray.direction = SampleHemisphere(hit.normal);
float3 diffuse = 2 * min(1.0f - hit.specular, hit.albedo);
float alpha = 15.0f;
float3 specular = hit.specular * (alpha + 2) * pow(sdot(ray.direction, reflected), alpha);
ray.energy *= (diffuse + specular) * sdot(hit.normal, ray.direction);
return 0.0f;
```

请注意，我们用稍微不同但等效的点积（反射 $\vec\omega_o$ 而不是 $\vec\omega_i$）代替了点积。现在，在 SetUpScene 功能中重新启用金属材质，并对其进行拍摄。

尝试不同的 α 值，你会注意到一个问题：较低的指数已经需要很长时间才能收敛，而对于较高的指数，噪声尤其顽固。即使经过几分钟的等待，结果也远非美好，这对于如此简单的场景来说是不可接受的。α=15 和 α=300，有 8192 个样本，看起来像这样：

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/f8d2a4461635351387a1e429b54303c9.png)

你可能会问：“为什么？我们以前有过如此完美的反射（α=∞）！”。问题是，我们正在生成均匀的样本，并根据 BRDF 对其进行加权。对于高 Phong 指数，除了那些非常接近完美反射的方向外，其他方向的 BRDF 值都很小，我们不太可能使用均匀样本随机选择它们。另一方面，如果我们真的击中了其中一个方向，BRDF 将非常大，可以补偿所有其他微小的样本。结果是一个非常高的方差。具有多个镜面反射的路径甚至更糟糕，导致您在上面的图像中看到的噪声。

## 更好的采样

为了使我们的路径跟踪器实用，我们需要改变范式。与其将宝贵的样本浪费在最终不重要的领域（因为它们将获得非常低的 BRDF 和/或余弦因子），不如生成重要的样本。

作为第一步，我们将得到我们的完美反射，然后看看我们如何推广这个想法。为此，我们将把着色逻辑分为漫反射和镜面反射。对于每个样本，我们将随机选择一个或另一个（取决于 $k_d$ 和 $k_s$ 的比率）。对于漫反射，我们将坚持均匀采样，但对于镜面反射，我们会明确地将光线反射到重要的单一方向。由于现在每种反射类型上花费的样本更少，我们需要相应地增加样本的贡献，最终得到相同的净额，如下所示：

```glsl
// Calculate chances of diffuse and specular reflection
hit.albedo = min(1.0f - hit.specular, hit.albedo);
float specChance = energy(hit.specular);
float diffChance = energy(hit.albedo);
float sum = specChance + diffChance;
specChance /= sum;
diffChance /= sum;
// Roulette-select the ray's path
float roulette = rand();
if (roulette < specChance)
{
    // Specular reflection
    ray.origin = hit.position + hit.normal * 0.001f;
    ray.direction = reflect(ray.direction, hit.normal);
    ray.energy *= (1.0f / specChance) * hit.specular * sdot(hit.normal, ray.direction);
}
else
{
    // Diffuse reflection
    ray.origin = hit.position + hit.normal * 0.001f;
    ray.direction = SampleHemisphere(hit.normal);
    ray.energy *= (1.0f / diffChance) * 2 * hit.albedo * sdot(hit.normal, ray.direction);
}
return 0.0f;
```

能量函数是一个对颜色通道进行平均的小助手：

```glsl
float energy(float3 color)
{
    return dot(color, 1.0f / 3.0f);
}
```

这是我们上次构建的 Whitted 光线跟踪器的美化变体，但现在具有真正的漫反射着色（读作“软阴影、环境遮挡、漫反射全局照明”）：

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/e2efe21c1c7f6fd6afaff4bed0c5ff7a.png)

## 重要性抽样

让我们再次看看基本的蒙特卡洛公式：

$$F_N\approx\frac{1}{N}\sum\limits^N_{n=0}\frac{f(x_n)}{p(x_n)}$$

如您所见，我们将每个样本的贡献除以选择该特定样本的概率。到目前为止，我们在半球上使用了均匀采样，因此常数 $p(\omega)=1/2\pi$。正如我们之前看到的，这远非最佳，例如在 Phong BRDF 的情况下，它在一组非常狭窄的方向上很大。

想象一下，我们可以找到一个与被积函数完全匹配的概率分布：$p(x)=f(x)$。这就是会发生的事情：

$$F_N\approx\frac{1}{N}\sum\limits^N_{n=0}1$$

现在，没有样本的贡献很小。相反，这些样本将以较低的概率被选择。这将大大减少结果的方差，使渲染收敛更快。

在实践中，找到这样一个完美的分布是不现实的，因为被积函数的一些因素（在我们的例子中是 BRDF × 余弦 × 入射光）是未知的（最突出的是入射光），但已经根据 BRDF × 余弦甚至只有 BRDF 分布样本对我们有很大的好处。这被称为重要性抽样。

## 余弦采样

对于以下步骤，我们需要用余弦（幂）分布替换均匀样本分布。记住，我们不想将均匀样本与余弦相乘，降低它们的贡献，而是想按比例生成更少的样本。

Thomas Poulet 的这篇文章描述了如何做到这一点。我们将在 SampleHemisphere 函数中添加一个 alpha 参数，用于确定余弦采样的幂：0 表示均匀，1 表示余弦，或更高的 Phong 指数。在代码中：

```glsl
float3 SampleHemisphere(float3 normal, float alpha)
{
    // Sample the hemisphere, where alpha determines the kind of the sampling
    float cosTheta = pow(rand(), 1.0f / (alpha + 1.0f));
    float sinTheta = sqrt(1.0f - cosTheta * cosTheta);
    float phi = 2 * PI * rand();
    float3 tangentSpaceDir = float3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
    // Transform direction to world space
    return mul(tangentSpaceDir, GetTangentSpace(normal));
}
```

现在每个样本的概率是

$$p(\omega)=\frac{\alpha+1}{2\pi}(\vec\omega\cdot\vec n)^\alpha$$

它的美丽可能不会立即显现，但它将在一分钟内展开。

## 重要性抽样兰伯特

首先，我们将改进漫反射渲染。我们的均匀分布已经很好地适应了常数 Lambert BRDF，但通过包含余弦因子，我们可以做得更好。余弦采样的概率分布（其中 α=1）为 $\frac{(\vec\omega_i\cdot\vec n)}{\pi}$，这将我们的扩散蒙特卡罗公式简化为：

$$L(x,\vec\omega_o)\approx\frac{1}{N}\sum\limits^N_{n=0}k_d L(x,\vec\omega_i)$$

```glsl
// Diffuse reflection
ray.origin = hit.position + hit.normal * 0.001f;
ray.direction = SampleHemisphere(hit.normal, 1.0f);
ray.energy *= (1.0f / diffChance) * hit.albedo;
```

这将使我们的漫反射着色稍微加速。现在让我们来处理真正的罪魁祸首。

## 重要性抽样 Phong

对于 Phong BRDF，程序类似。这一次，我们得到了两个余弦的乘积：渲染方程中的正则余弦（如漫反射情况）乘以 BRDF 自己的幂余弦。我们只会照顾后者。

让我们把上面的概率分布插入到我们的 Phong 方程中。详细的推导可以在 Lafortune 和 Willems 中找到：使用改进的 Phong 反射模型进行基于物理的渲染（1994）：

$$L(x, \vec\omega_o)\approx\frac{1}{N} \sum\limits^N_{n=0}k_s\frac{\alpha+2}{\alpha + 1}(\vec\omega_i\cdot\vec n)L(x,\vec\omega_i)$$

```glsl
// Specular reflection
float alpha = 15.0f;
ray.origin = hit.position + hit.normal * 0.001f;
ray.direction = SampleHemisphere(reflect(ray.direction, hit.normal), alpha);
float f = (alpha + 2) / (alpha + 1);
ray.energy *= (1.0f / specChance) * hit.specular * sdot(hit.normal, ray.direction, f);
```

这些更改足以解决高 Phong 指数的任何问题，并使我们的渲染在更合理的时间内收敛。

## 材质

最后，让我们扩展场景生成，以便获得球体平滑度和发射的不同值！在 C# 中，向 `struct Sphere` 添加 `public float smoothness` 和 `public Vector3 emission`。由于我们更改了结构的大小，因此在创建 Compute Buffer时需要调整步幅（4 × 浮点数，还记得吗？）。使 SetUpScene 函数为平滑度和发射设置一些值。

回到着色器中，将这两个变量添加到 `struct Sphere` 和 `struct RayHit` 中，并在 `CreateRayHit` 中将其初始化。最后但并非最不重要的一点是，在 `IntersectGroundPlane`（硬编码，放入任何你想要的东西）和 `IntersectSphere`（从 Sphere 中获取值）中设置这两个值。

我想使用 Unity 标准着色器中的平滑度值，这与任意 Phong 指数不同。以下是一个相当有效的转换，用于 `Shade` 函数：

```glsl
float SmoothnessToPhongAlpha(float s)
{
    return pow(1000.0f, s * s);
}
```

```glsl
float alpha = SmoothnessToPhongAlpha(hit.smoothness);
```

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/55a779e9513307234b516836ea8aef97.png)

只需返回 `Shade` 中的值即可使用发射：

```glsl
return hit.emission;
```

## 结果

深呼吸，放松，等待你的图像清晰地进入舒缓的视线，如下所示：

![在这里插入图片描述](https://i-blog.csdnimg.cn/blog_migrate/5d879e3ff19ddb2f94b326fa0584551a.png)

祝贺你！你已经穿过了这片充满数学的森林。您实现了一个能够进行漫反射和镜面着色的路径跟踪器，并学习了重要性采样，立即应用了该概念，使渲染在几分钟内收敛，而不是几小时或几天。

这篇文章在复杂性和结果质量方面都比上一篇文章有了很大的飞跃。完成数学运算需要时间，但值得一试，因为它将大大加深你对正在发生的事情的理解，并允许你在不破坏物理合理性的情况下扩展算法。

感谢您的关注！在第三部分中，我们将把采样和着色抛在脑后（目前……），回到文明世界，与默勒先生（Gentleman Möller）和特龙博先生（Trumbore）会面。他们对三角形有一两句话要说。



# Unity 中的  GPU 光线追踪：第 3 部分

https://www.gamedeveloper.com/programming/gpu-path-tracing-in-unity-part-3

我们基于 Unity Compute Shader 的光线跟踪器现在可以显示具有模糊效果的球体，如柔和阴影、光泽反射和漫反射 GI，但我们缺少了拼图中的一个重要部分：三角形。这是我们今天的工作。

[David Kuri](https://www.gamedeveloper.com/author/david-kuri), Blogger

March 18, 2019

13 Min Read

> **“算术！代数！几何！宏伟的三位一体！发光的三角形！谁不认识你，谁就没有理智！”——劳特雷阿蒙伯爵**

今天，我们要迈出一大步。我们正在超越迄今为止我们一直在追踪的纯球形结构和无限平面，引入三角形——现代计算机图形学的本质，整个虚拟词所包含的元素。如果你想从[上次中断的地方](http://blog.three-eyed-games.com/2018/05/12/gpu-path-tracing-in-unity-part-2/)继续，请使用[第 2 部分的代码](https://bitbucket.org/Daerst/gpu-ray-tracing-in-unity/src/Tutorial_Pt2/)。我们今天要做的事情的完成代码可以在[这里](https://bitbucket.org/Daerst/gpu-ray-tracing-in-unity/src/Tutorial_Pt3/)找到。我们走！

## 三角形

三角形的定义很简单：它只是一个由三个连接的顶点组成的列表，每个顶点都存储其位置，然后存储法线。从你的角度来看，顶点的缠绕顺序决定了你是看前面还是后面。传统上，逆时针缠绕顺序被认为是“前”。

首先，我们需要能够判断光线是否击中三角形，以及在哪里。[Tomas Akenine-Möller](https://twitter.com/inversepixel) 和 Ben Trumbore 于 1997 年提出了一种非常流行（但肯定不是唯一）的确定光线三角形交点的算法。您可以在[此处](http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/pubs/raytri_tam.pdf)阅读论文“快速、最小存储射线三角交点”中的所有详细信息。

论文中的代码可以很容易地移植到 HLSL 着色器代码中：

```glsl
static const float EPSILON = 1e-8;

bool IntersectTriangle_MT97(Ray ray, float3 vert0, float3 vert1, float3 vert2,
    inout float t, inout float u, inout float v)
{
    // find vectors for two edges sharing vert0
    float3 edge1 = vert1 - vert0;
    float3 edge2 = vert2 - vert0;

    // begin calculating determinant - also used to calculate U parameter
    float3 pvec = cross(ray.direction, edge2);

    // if determinant is near zero, ray lies in plane of triangle
    float det = dot(edge1, pvec);

    // use backface culling
    if (det < EPSILON)
        return false;
    float inv_det = 1.0f / det;

    // calculate distance from vert0 to ray origin
    float3 tvec = ray.origin - vert0;

    // calculate U parameter and test bounds
    u = dot(tvec, pvec) * inv_det;
    if (u < 0.0 || u > 1.0f)
        return false;

    // prepare to test V parameter
    float3 qvec = cross(tvec, edge1);

    // calculate V parameter and test bounds
    v = dot(ray.direction, qvec) * inv_det;
    if (v < 0.0 || u + v > 1.0f)
        return false;

    // calculate t, ray intersects triangle
    t = dot(edge2, qvec) * inv_det;

    return true;
}
```

要使用此功能，您需要一条光线和三角形的三个顶点。返回值告诉三角形是否被击中。在命中的情况下，会计算三个附加值：t 描述了沿光线到命中点的距离，u/v 是指定命中点在三角形上的位置的三个重心坐标中的两个（其中最后一个可以计算为 w = 1 - u - v）。如果你还不知道重心坐标，请阅读 [Scratchapixel](https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/barycentric-coordinates) 上的精彩解释。

闲话少说，让我们追踪一个硬编码的三角形！找到着色器的 Trace 函数并添加以下代码段：

```glsl
// Trace single triangle
float3 v0 = float3(-150, 0, -150);
float3 v1 = float3(150, 0, -150);
float3 v2 = float3(0, 150 * sqrt(2), -150);
float t, u, v;
if (IntersectTriangle_MT97(ray, v0, v1, v2, t, u, v))
{
    if (t > 0 && t < bestHit.distance)
    {
        bestHit.distance = t;
        bestHit.position = ray.origin + t * ray.direction;
        bestHit.normal = normalize(cross(v1 - v0, v2 - v0));
        bestHit.albedo = 0.00f;
        bestHit.specular = 0.65f * float3(1, 0.4f, 0.2f);
        bestHit.smoothness = 0.9f;
        bestHit.emission = 0.0f;
    }
}
```

如前所述，t 存储了沿射线的距离，我们可以直接使用它来计算撞击点。法线对于正确反射很重要，可以使用任何两条三角形边的叉积来获得。进入游戏模式，享受你的第一个自我追踪三角形：

![Bronze triangle](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/blt58491a58018ffcca/650e8841fc5c84e3a10d41e0/bronze-triangle.png/?width=700&auto=webp&quality=80&disable=upscale)

**练习**：尝试使用重心坐标而不是距离来计算位置。如果你做得对，光滑的三角形看起来和以前完全一样。

## 三角形网格

我们已经克服了第一个障碍，但追踪完整的三角形网格是另一回事。我们需要先学习一些关于网格的基本知识。如果你熟悉这一点，可以随意浏览下一段。

在计算机图形学中，网格由多个缓冲区定义，其中最重要的缓冲区是顶点和索引缓冲区。顶点缓冲区是一个 3D 向量列表，描述了每个顶点在对象空间中的位置（这意味着当你平移、旋转或缩放对象时，这些值不需要改变——它们会使用矩阵乘法动态地从对象空间转换到世界空间）。索引缓冲区是一个整数列表，这些整数是指向顶点缓冲区的索引。每三个指数组成一个三角形。例如，如果索引缓冲区为 [0,1,2,0,2,3]，则有两个三角形：第一个三角形由顶点缓冲区中的第一、第二和第三个顶点组成，而第二个三角形由第一、第三和第四个顶点组成。因此，索引缓冲区也定义了上述的卷绕顺序。除了顶点和索引缓冲区外，其他缓冲区还可以向每个顶点添加信息。最常见的附加缓冲区存储法线、纹理坐标（称为纹理坐标或简称为 UV）和顶点颜色。

## 使用 GameObjects

我们需要做的第一件事是真正了解应该成为光线跟踪过程一部分的游戏对象。简单的解决方案是执行 `FindObjectOfType()`，但我们会选择更灵活、更快的方法。让我们添加一个新组件 `RayTracingObject`：

```c#
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RayTracingObject : MonoBehaviour
{
    private void OnEnable()
    {
        RayTracingMaster.RegisterObject(this);
    }

    private void OnDisable()
    {
        RayTracingMaster.UnregisterObject(this);
    }
}
```

该组件被添加到我们想要在光线跟踪中使用的每个对象中，并负责将它们注册到 `RayTracingMaster` 中。在 master 中添加以下函数：

```c#
private static bool _meshObjectsNeedRebuilding = false;
private static List _rayTracingObjects = new List();

public static void RegisterObject(RayTracingObject obj)
{
    _rayTracingObjects.Add(obj);
    _meshObjectsNeedRebuilding = true;
}

public static void UnregisterObject(RayTracingObject obj)
{
    _rayTracingObjects.Remove(obj);
    _meshObjectsNeedRebuilding = true;
}
```

到目前为止，一切顺利——我们知道要追踪哪些对象。现在到了关键部分：我们即将从 Unity 的网格（矩阵、顶点和索引缓冲区，还记得吗？）收集所有数据，将它们放入我们自己的数据结构中，并上传到 GPU，以便着色器可以使用它们。让我们从 master 中 C# 端的数据结构和缓冲区定义开始：

```c#
struct MeshObject
{
    public Matrix4x4 localToWorldMatrix;
    public int indices_offset;
    public int indices_count;
}

private static List _meshObjects = new List();
private static List _vertices = new List();
private static List _indices = new List();
private ComputeBuffer _meshObjectBuffer;
private ComputeBuffer _vertexBuffer;
private ComputeBuffer _indexBuffer;
```

…让我们在着色器中做同样的事情。你现在已经习惯了，不是吗？

```glsl
struct MeshObject
{
    float4x4 localToWorldMatrix;
    int indices_offset;
    int indices_count;
};

StructuredBuffer _MeshObjects;
StructuredBuffer _Vertices;
StructuredBuffer _Indices;
```

我们的数据结构已经到位，所以我们现在可以用实际数据填充它们。我们将所有网格中的所有顶点收集到一个大列表中，并将所有索引收集到大 `List<int>` 中。虽然这对顶点来说不是问题，但我们需要调整索引，使其仍然指向大缓冲区中的正确顶点。例如，假设到目前为止，我们已经添加了价值 1000 个顶点的对象，现在我们正在添加一个简单的立方体网格。第一个三角形可能由索引 [0,1,2] 组成，但由于在开始添加立方体的顶点之前，缓冲区中已经有 1000 个顶点，因此我们需要移动索引，使其变为 [1000, 1001, 1002]。这是它在代码中的样子：

```c#
private void RebuildMeshObjectBuffers()
{
    if (!_meshObjectsNeedRebuilding)
    {
        return;
    }

    _meshObjectsNeedRebuilding = false;
    _currentSample = 0;

    // Clear all lists
    _meshObjects.Clear();
    _vertices.Clear();
    _indices.Clear();

    // Loop over all objects and gather their data
    foreach (RayTracingObject obj in _rayTracingObjects)
    {
        Mesh mesh = obj.GetComponent().sharedMesh;

        // Add vertex data
        int firstVertex = _vertices.Count;
        _vertices.AddRange(mesh.vertices);

        // Add index data - if the vertex buffer wasn't empty before, the
        // indices need to be offset
        int firstIndex = _indices.Count;
        var indices = mesh.GetIndices(0);
        _indices.AddRange(indices.Select(index => index + firstVertex));

        // Add the object itself
        _meshObjects.Add(new MeshObject()
        {
            localToWorldMatrix = obj.transform.localToWorldMatrix,
            indices_offset = firstIndex,
            indices_count = indices.Length
        });
    }

    CreateComputeBuffer(ref _meshObjectBuffer, _meshObjects, 72);
    CreateComputeBuffer(ref _vertexBuffer, _vertices, 12);
    CreateComputeBuffer(ref _indexBuffer, _indices, 4);
}
```

在 `OnRenderImage` 函数中调用 `RebuildMeshObjectBuffers`，不要忘记在 `OnDisable` 中释放新的缓冲区。以下是我在上面的代码中使用的两个辅助函数，使缓冲区处理更容易一些：

```c#
private static void CreateComputeBuffer(ref ComputeBuffer buffer, List data, int stride)
    where T : struct
{
    // Do we already have a compute buffer?
    if (buffer != null)
    {
        // If no data or buffer doesn't match the given criteria, release it
        if (data.Count == 0 || buffer.count != data.Count || buffer.stride != stride)
        {
            buffer.Release();
            buffer = null;
        }
    }

    if (data.Count != 0)
    {
        // If the buffer has been released or wasn't there to
        // begin with, create it
        if (buffer == null)
        {
            buffer = new ComputeBuffer(data.Count, stride);
        }

        // Set data on the buffer
        buffer.SetData(data);
    }
}

private void SetComputeBuffer(string name, ComputeBuffer buffer)
{
    if (buffer != null)
    {
        RayTracingShader.SetBuffer(0, name, buffer);
    }
}
```

太好了，我们有缓冲区，它们充满了所需的数据！现在我们只需要告诉着色器。在 `SetShaderParameters` 中，添加以下代码（并且，由于我们的新辅助函数，您也可以在处理球体缓冲区时缩短代码）：

```c#
SetComputeBuffer("_Spheres", _sphereBuffer);
SetComputeBuffer("_MeshObjects", _meshObjectBuffer);
SetComputeBuffer("_Vertices", _vertexBuffer);
SetComputeBuffer("_Indices", _indexBuffer);
```

呼。这很乏味，但看看我们刚刚做了什么：我们收集了网格的所有内部数据（矩阵、顶点和索引），把它们放在一个漂亮而简单的结构中，并将其发送到 GPU，GPU 现在正急切地等待着使用它。

## 追踪网格

我们不要让 GPU 等待。我们已经有了在着色器中跟踪单个三角形的代码，而网格实际上只是一堆三角形。这里唯一的新东西是，我们使用我们的矩阵，使用内在函数 `mul`（用于乘法）将顶点从对象空间转换到世界空间。矩阵包含对象的平移、旋转和缩放。它是 4×4，所以我们需要一个 4d 向量进行乘法。前三个分量（x，y，z）来自我们的顶点缓冲区。我们将第四个分量（w）设置为 1，因为我们正在处理一个点。如果这是一个方向，我们会在那里加一个 0 来忽略矩阵中的任何平移和缩放。困惑吗？请至少阅读[本教程](http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/)八遍。以下是着色器代码：

```glsl
void IntersectMeshObject(Ray ray, inout RayHit bestHit, MeshObject meshObject)
{
    uint offset = meshObject.indices_offset;
    uint count = offset + meshObject.indices_count;
    for (uint i = offset; i < count; i += 3)
    {
        float3 v0 = (mul(meshObject.localToWorldMatrix, float4(_Vertices[_Indices[i]], 1))).xyz;
        float3 v1 = (mul(meshObject.localToWorldMatrix, float4(_Vertices[_Indices[i + 1]], 1))).xyz;
        float3 v2 = (mul(meshObject.localToWorldMatrix, float4(_Vertices[_Indices[i + 2]], 1))).xyz;

        float t, u, v;
        if (IntersectTriangle_MT97(ray, v0, v1, v2, t, u, v))
        {
            if (t > 0 && t < bestHit.distance)
            {
                bestHit.distance = t;
                bestHit.position = ray.origin + t * ray.direction;
                bestHit.normal = normalize(cross(v1 - v0, v2 - v0));
                bestHit.albedo = 0.0f;
                bestHit.specular = 0.65f;
                bestHit.smoothness = 0.99f;
                bestHit.emission = 0.0f;
            }
        }
    }
}
```

我们离实际看到这一切只差一步了。让我们稍微重构一下 `Trace` 函数，并添加网格对象的跟踪：

```glsl
	RayHit Trace(Ray ray)
    {
        RayHit bestHit = CreateRayHit();
        uint count, stride, i;
        // Trace ground plane
        IntersectGroundPlane(ray, bestHit);
        // Trace spheres
        _Spheres.GetDimensions(count, stride);
        for (i = 0; i < count; i++)
        {
            IntersectSphere(ray, bestHit, _Spheres[i]);
        }
        // Trace mesh objects
        _MeshObjects.GetDimensions(count, stride);
        for (i = 0; i < count; i++)
        {
            IntersectMeshObject(ray, bestHit, _MeshObjects[i]);
        }
        return bestHit;
    }
```

## 结果

就是这样！让我们添加一些简单的网格（Unity 的 primitves 工作得很好），给它们一个 RayTracingObject 组件，然后观察奇迹的发生。**不要**使用任何详细的网格（超过几百个三角形）！我们的着色器缺少适当的优化，如果你走得太远，可能需要几秒钟甚至几分钟才能跟踪每个像素的一个样本。结果是你的 GPU 驱动程序将被系统杀死，Unity 可能会崩溃，你的机器需要重新启动。

![Flat primitives](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/blt5a1c576d91bdd0b0/650e88244965463fac4b5ff0/flat-primitives.png/?width=700&auto=webp&quality=80&disable=upscale)

请注意，我们的网格不是平滑的，而是平坦的着色。由于我们还没有将顶点的法线上传到缓冲区，我们需要使用叉积来单独获得每个三角形的法线，并且不能在三角形区域内插值。我们将在本系列教程的下一部分中处理这个问题。

为了好玩，我从 [Morgan McGuire 的档案](https://casual-effects.com/g3d/data10/index.html#mesh3)中下载了Stanford Bunny，并使用 [Blender](https://www.blender.org/)的抽取修改器将其缩减为 431 个三角形。您可以在着色器的 `IntersectMeshObject` 函数中使用灯光设置和硬编码材质。这是 [Grafitti 庇护所](https://hdrihaven.com/hdri/?h=graffiti_shelter)里的一只电介质兔子，它有着漂亮的柔和阴影和微妙的漫反射 GI：

![Bunny matte](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/bltc372ac312e3cd86f/650e882e285a8a474bf4bc56/bunny-matte.png/?width=700&auto=webp&quality=80&disable=upscale)

…在 [Cape Hill](https://hdrihaven.com/hdri/?h=cape_hill) 强烈的定向光下，一只金属兔子在地板上投下了一些迪斯科般的斑点：

![Bunny glossy](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/bltc798f25ce0dee3c3/650e882a4ba72b5912f96e67/bunny-reflecting.jpg/?width=700&auto=webp&quality=80&disable=upscale)

…还有两只小兔子躲在 [Kiara 9 Dusk](https://hdrihaven.com/hdri/?h=kiara_9_dusk) 蓝天下的一块大岩石 Suzanne 下面（我通过检查索引偏移量是否为 0，为第一个对象硬编码了一种替代材质）：

![Suzanne and Bunnies](https://eu-images.contentstack.com/v3/assets/blt740a130ae3c5d529/blt23b67bb1cccb4a86/650e88566b0b7138bf544e1c/suzanne-and-bunnies.png/?width=700&auto=webp&quality=80&disable=upscale)

## 接下来是什么？

第一次在自己的光线跟踪器中看到真正的网格真的很酷，不是吗？我们今天处理了相当多的数据，了解了 Möller-Trumbore 交叉口，并整合了所有内容，以便 Unity 的 GameObjects 可以立即使用。我们还看到了光线追踪的美丽一面：一旦你集成了一个新的交点，所有很酷的效果（软阴影、镜面反射和漫反射 GI 等）都会正常工作。

渲染光滑的兔子花了很长时间，我仍然需要对结果进行一些轻微的过滤，以去除最刺耳的噪音。为了克服这一点，场景通常被结构化为网格、kD 树或边界体积层次等空间结构，这大大加快了更大场景的渲染速度。

但首先要做的是：接下来我们应该做的是修复法线，这样我们的网格（即使是低多边形）看起来比现在更平滑。当对象移动时，矩阵会自动更新，与 Unity 材质的实际耦合，而不仅仅是一个硬编码的耦合，听起来也是一个好主意。我们将在本系列的下一部分中处理这个问题。

你已经走了这么远。谢谢，第四部分见！
