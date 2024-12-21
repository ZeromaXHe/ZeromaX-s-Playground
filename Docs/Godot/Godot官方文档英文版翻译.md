# 渲染

## 合成器（The Compositor）

https://docs.godotengine.org/en/stable/tutorials/rendering/compositor.html#the-compositor

合成器是 Godot 4 中的一项新功能，它允许在渲染[视口](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport)内容时控制渲染管道。

它可以在 [WorldEnvironment](https://docs.godotengine.org/en/stable/classes/class_worldenvironment.html#class-worldenvironment) 节点上配置，应用于所有视口，也可以在 [Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d) 上配置，仅应用于使用该相机的视口。

[Compositor](https://docs.godotengine.org/en/stable/classes/class_compositor.html#class-compositor)资源用于配置合成器。要开始，只需在相应的节点上创建一个新的合成器：

![../../_images/new_compositor.webp](https://docs.godotengine.org/en/stable/_images/new_compositor.webp)

> **注：**
>
> 合成器目前是仅由 Mobile 和 Forward+ 渲染器支持的功能。

### 合成器效果

合成器效果允许您在各个阶段将其他逻辑插入到渲染管道中。这是一个高级功能，需要对渲染管道有深入的了解才能发挥其最大的优势。

由于合成器效果的核心逻辑是从渲染管道调用的，因此需要注意的是，该逻辑将在进行渲染的线程内运行。需要小心确保我们不会遇到线程问题。

为了说明如何使用合成器效果，我们将创建一个简单的后处理效果，允许您编写自己的着色器代码，并通过计算着色器应用此全屏。您可以在[此处](https://github.com/godotengine/godot-demo-projects/tree/master/compute/post_shader)找到已完成的演示项目。

我们首先创建一个名为 `post_process_shader.gd` 的新脚本。我们将把它变成一个工具脚本，这样我们就可以在编辑器中看到合成器效果。我们需要从 [CompositorEffect](https://docs.godotengine.org/en/stable/classes/class_compositoreffect.html#class-compositoreffect) 扩展我们的节点。我们还必须给脚本一个类名。

```gdscript
@tool
extends CompositorEffect
class_name PostProcessShader
```

接下来，我们将为着色器模板代码定义一个常数。这是使我们的计算着色器工作的样板代码。

```gdscript
const template_shader : String = "#version 450

// Invocations in the (x, y, z) dimension
layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(rgba16f, set = 0, binding = 0) uniform image2D color_image;

// Our push constant
layout(push_constant, std430) uniform Params {
	vec2 raster_size;
	vec2 reserved;
} params;

// The code we want to execute in each invocation
void main() {
	ivec2 uv = ivec2(gl_GlobalInvocationID.xy);
	ivec2 size = ivec2(params.raster_size);

	if (uv.x >= size.x || uv.y >= size.y) {
		return;
	}

	vec4 color = imageLoad(color_image, uv);

	#COMPUTE_CODE

	imageStore(color_image, uv, color);
}"
```

有关计算着色器如何工作的更多信息，请查看[使用计算着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/compute_shaders.html#doc-compute-shaders)。

这里重要的一点是，对于屏幕上的每个像素，我们的 `main` 函数都会被执行，在这个函数中，我们加载像素的当前颜色值，执行用户代码，并将修改后的颜色写回彩色图像。

`#COMPUTE_CODE` 被我们的用户代码替换。

为了设置我们的用户代码，我们需要一个导出变量。我们还将定义一些我们将使用的脚本变量：

```gdscript
@export_multiline var shader_code : String = "":
	set(value):
		mutex.lock()
		shader_code = value
		shader_is_dirty = true
		mutex.unlock()

var rd : RenderingDevice
var shader : RID
var pipeline : RID

var mutex : Mutex = Mutex.new()
var shader_is_dirty : bool = true
```

请注意我们的代码中使用了 [Mutex](https://docs.godotengine.org/en/stable/classes/class_mutex.html#class-mutex)。我们的大部分实现都是从渲染引擎调用的，因此在我们的渲染线程中运行。

我们需要确保我们设置了新的着色器代码，并将着色器代码标记为脏，而渲染线程不会同时访问这些数据。

接下来，我们初始化我们的效果。

```gdscript
# Called when this resource is constructed.
func _init():
	effect_callback_type = EFFECT_CALLBACK_TYPE_POST_TRANSPARENT
	rd = RenderingServer.get_rendering_device()
```

这里的主要内容是设置 `effect_callback_type`，它告诉渲染引擎在渲染管道的哪个阶段调用我们的代码。

> **注：**
>
> 目前，我们只能访问 3D 渲染管道的各个阶段！

我们还可以参考我们的渲染设备，这将非常方便。

我们还需要在自己之后进行清理，为此，我们对 `NOTIFICATION_PREDELETE` 通知做出反应：

```gdscript
# System notifications, we want to react on the notification that
# alerts us we are about to be destroyed.
func _notification(what):
	if what == NOTIFICATION_PREDELETE:
		if shader.is_valid():
			# Freeing our shader will also free any dependents such as the pipeline!
			rd.free_rid(shader)
```

请注意，即使我们在渲染线程内创建了着色器，我们也不会在这里使用互斥体（mutex）。我们渲染服务器上的方法是线程安全的，`free_rid` 将推迟清理着色器，直到当前正在渲染的任何帧完成。

还要注意，我们并没有释放我们的管道。渲染设备执行依赖跟踪，由于管道依赖于着色器，因此当着色器被破坏时，它将自动释放。

从这一点开始，我们的代码将在渲染线程上运行。

我们的下一步是一个辅助函数，如果用户代码发生更改，它将重新编译着色器。

```gdscript
# Check if our shader has changed and needs to be recompiled.
func _check_shader() -> bool:
	if not rd:
		return false

	var new_shader_code : String = ""

	# Check if our shader is dirty.
	mutex.lock()
	if shader_is_dirty:
		new_shader_code = shader_code
		shader_is_dirty = false
	mutex.unlock()

	# We don't have a (new) shader?
	if new_shader_code.is_empty():
		return pipeline.is_valid()

	# Apply template.
	new_shader_code = template_shader.replace("#COMPUTE_CODE", new_shader_code);

	# Out with the old.
	if shader.is_valid():
		rd.free_rid(shader)
		shader = RID()
		pipeline = RID()

	# In with the new.
	var shader_source : RDShaderSource = RDShaderSource.new()
	shader_source.language = RenderingDevice.SHADER_LANGUAGE_GLSL
	shader_source.source_compute = new_shader_code
	var shader_spirv : RDShaderSPIRV = rd.shader_compile_spirv_from_source(shader_source)

	if shader_spirv.compile_error_compute != "":
		push_error(shader_spirv.compile_error_compute)
		push_error("In: " + new_shader_code)
		return false

	shader = rd.shader_create_from_spirv(shader_spirv)
	if not shader.is_valid():
		return false

	pipeline = rd.compute_pipeline_create(shader)
	return pipeline.is_valid()
```

在这个方法的顶部，我们再次使用互斥体来保护对用户着色器代码的访问，以及我们的脏标志。如果用户着色器代码脏了，我们会制作用户着色器代码的本地副本。

如果我们没有新的代码片段，如果我们已经有一个有效的管道，我们将返回 true。

如果我们确实有一个新的代码片段，我们会将其嵌入到模板代码中，然后进行编译。

> **警告：**
>
> 这里显示的代码在运行时编译了我们的新代码。这对于原型制作非常有用，因为我们可以立即看到更改后的着色器的效果。
>
> 这将阻止预编译和缓存此着色器，这在某些平台（如控制台）上可能是一个问题。请注意，演示项目附带了一个替代示例，其中 `glsl` 文件包含整个计算着色器，并使用了该着色器。Godot 能够使用这种方法预编译和缓存着色器。

最后，我们需要实现我们的效果回调，渲染引擎将在渲染的正确阶段调用它。

```gdscript
# Called by the rendering thread every frame.
func _render_callback(p_effect_callback_type, p_render_data):
	if rd and p_effect_callback_type == EFFECT_CALLBACK_TYPE_POST_TRANSPARENT and _check_shader():
		# Get our render scene buffers object, this gives us access to our render buffers.
		# Note that implementation differs per renderer hence the need for the cast.
		var render_scene_buffers : RenderSceneBuffersRD = p_render_data.get_render_scene_buffers()
		if render_scene_buffers:
			# Get our render size, this is the 3D render resolution!
			var size = render_scene_buffers.get_internal_size()
			if size.x == 0 and size.y == 0:
				return

			# We can use a compute shader here
			var x_groups = (size.x - 1) / 8 + 1
			var y_groups = (size.y - 1) / 8 + 1
			var z_groups = 1

			# Push constant
			var push_constant : PackedFloat32Array = PackedFloat32Array()
			push_constant.push_back(size.x)
			push_constant.push_back(size.y)
			push_constant.push_back(0.0)
			push_constant.push_back(0.0)

			# Loop through views just in case we're doing stereo rendering. No extra cost if this is mono.
			var view_count = render_scene_buffers.get_view_count()
			for view in range(view_count):
				# Get the RID for our color image, we will be reading from and writing to it.
				var input_image = render_scene_buffers.get_color_layer(view)

				# Create a uniform set, this will be cached, the cache will be cleared if our viewports configuration is changed.
				var uniform : RDUniform = RDUniform.new()
				uniform.uniform_type = RenderingDevice.UNIFORM_TYPE_IMAGE
				uniform.binding = 0
				uniform.add_id(input_image)
				var uniform_set = UniformSetCacheRD.get_cache(shader, 0, [ uniform ])

				# Run our compute shader.
				var compute_list := rd.compute_list_begin()
				rd.compute_list_bind_compute_pipeline(compute_list, pipeline)
				rd.compute_list_bind_uniform_set(compute_list, uniform_set, 0)
				rd.compute_list_set_push_constant(compute_list, push_constant.to_byte_array(), push_constant.size() * 4)
				rd.compute_list_dispatch(compute_list, x_groups, y_groups, z_groups)
				rd.compute_list_end()
```

在该方法开始时，我们检查是否有渲染设备，我们的回调类型是否正确，并检查我们是否有着色器。

> **注：**
>
> 对效果类型的检查只是一种安全机制。我们在 `_init` 函数中设置了这一点，但是用户可以在 UI 中更改它。

我们的 `p_render_data` 参数允许我们访问一个对象，该对象包含我们当前正在渲染的帧特定的数据。我们目前只对渲染场景缓冲区感兴趣，这些缓冲区为我们提供了对渲染引擎使用的所有内部缓冲区的访问。请注意，我们将其强制转换为 [RenderSceneBuffersRD](https://docs.godotengine.org/en/stable/classes/class_renderscenebuffersrd.html#class-renderscenebuffersrd)，以将整个 API 公开给此数据。

接下来，我们获得 `internal size`，即 3D 渲染缓冲区在升级之前的分辨率（如果适用），升级发生在我们的后处理运行之后。

根据我们的内部大小，我们计算我们的组大小，在模板着色器中查看我们的局部大小。

我们还填充推送常数，以便着色器知道我们的大小。Godot 在这里**还**不支持结构，所以我们使用 `PackedFloat32Array` 来存储这些数据。请注意，我们必须用 16 字节对齐来填充这个数组。换句话说，数组的长度需要是 4 的倍数。

现在我们循环浏览我们的视图，以防我们使用适用于立体渲染（XR）的多视图渲染。在大多数情况下，我们只有一种观点。

> **注：**
>
> 在这里使用多视图进行后处理没有性能优势，像这样单独处理视图仍然可以使 GPU 在有利的情况下使用并行性。

接下来，我们获取此视图的颜色缓冲区。这是渲染 3D 场景的缓冲区。

然后，我们准备一个 uniform 的集合，以便将颜色缓冲区传递给着色器。

请注意我们的 [UniformSetCacheRD](https://docs.godotengine.org/en/stable/classes/class_uniformsetcacherd.html#class-uniformsetcacherd) 缓存的使用，它确保我们可以在每一帧中检查我们的 uniform 集。由于我们的颜色缓冲区可以逐帧变化，并且当缓冲区被释放时，我们的统一缓存会自动清理统一集，这是确保我们不会泄漏内存或使用过时集的安全方法。

最后，我们通过绑定流水线、绑定 uniform 集、推送我们的推送常量数据和调用我们组的调度来构建计算列表。

完成合成器效果后，我们现在需要将其添加到合成器中。

在合成器上，我们展开合成器效果属性，然后按添加元素。

现在我们可以添加合成器效果：

![../../_images/add_compositor_effect.webp](https://docs.godotengine.org/en/stable/_images/add_compositor_effect.webp)

选择 `PostProcessShader` 后，我们需要设置用户着色器代码：

```gdscript
float gray = color.r * 0.2125 + color.g * 0.7154 + color.b * 0.0721;
color.rgb = vec3(gray);
```

完成所有这些后，我们的输出是灰度的。

![../../_images/post_process_shader.webp](https://docs.godotengine.org/en/stable/_images/post_process_shader.webp)

> **注：**
>
> 有关后期效果的更高级示例，请查看 Bastiaan Olij 创建的[基于径向模糊的天空光线](https://github.com/BastiaanOlij/RERadialSunRays)示例项目。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论**



[SephReed](https://github.com/SephReed) [Nov 22, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/261#discussioncomment-11344519)

以下是一个指向 github 讨论的链接，其中包含各种后处理方法和信息：
[godotengine/godot#99491](https://github.com/godotengine/godot/issues/99491)



# 着色器

## 使用计算着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/compute_shaders.html#using-compute-shaders

本教程将引导您完成创建最小计算着色器的过程。但首先，我们来了解一下计算着色器的背景以及它们如何与 Godot 配合使用。

> **注：**
>
> 本教程假设您通常熟悉着色器。如果您是着色器新手，请在继续本教程之前阅读[着色器简介](https://docs.godotengine.org/en/stable/tutorials/shaders/introduction_to_shaders.html#doc-introduction-to-shaders)和[您的第一个着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/index.html#toc-your-first-shader)。

计算着色器是一种特殊类型的着色器程序，面向通用编程。换句话说，它们比顶点着色器和片段着色器更灵活，因为它们没有固定的目的（即变换顶点或将颜色写入图像）。与片段着色器和顶点着色器不同，计算着色器在幕后几乎没有什么作用。你编写的代码是 GPU 运行的代码，其他很少。这可以使它们成为将繁重计算卸载到 GPU 的非常有用的工具。

现在，让我们开始创建一个简短的计算着色器。

首先，在您选择的**外部**文本编辑器中，在项目文件夹中创建一个名为 `compute_example.glsl` 的新文件。在 Godot 中编写计算着色器时，可以直接在 GLSL 中编写。Godot 着色器语言基于 GLSL。如果您熟悉 Godot 中的普通着色器，下面的语法看起来会有点熟悉。

> **注：**
>
> 计算着色器只能从基于 RenderingDevice 的渲染器（Forward+ 或 Mobile 渲染器）中使用。要按照本教程进行操作，请确保您使用的是 Forward+ 或 Mobile 渲染器。其设置位于编辑器的右上角。

请注意，移动设备上的计算着色器支持通常很差（由于驱动程序错误），即使它们在技术上得到了支持。

让我们来看看这个计算着色器代码：

```glsl
#[compute]
#version 450

// Invocations in the (x, y, z) dimension
layout(local_size_x = 2, local_size_y = 1, local_size_z = 1) in;

// A binding to the buffer we create in our script
layout(set = 0, binding = 0, std430) restrict buffer MyDataBuffer {
    float data[];
}
my_data_buffer;

// The code we want to execute in each invocation
void main() {
    // gl_GlobalInvocationID.x uniquely identifies this invocation across all work groups
    my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}
```

此代码获取一个浮点数数组，将每个元素乘以 2，并将结果存储回缓冲区数组中。现在让我们逐行看一下。

```glsl
#[compute]
#version 450
```

这两行传达了两件事：

1. 以下代码是一个计算着色器。这是编辑器正确导入着色器文件所需的 Godot 特定提示。
2. 代码使用 GLSL 版本 450。

您不应该为自定义计算着色器更改这两行。

```glsl
// Invocations in the (x, y, z) dimension
layout(local_size_x = 2, local_size_y = 1, local_size_z = 1) in;
```

接下来，我们传达每个工作组中要使用的调用数量。调用是在同一工作组中运行的着色器实例。当我们从 CPU 启动计算着色器时，我们告诉它要运行多少个工作组。工作组彼此并行运行。在运行一个工作组时，您无法访问另一工作组中的信息。但是，同一工作组中的调用可以对其他调用进行一些有限的访问。

将工作组和调用视为一个巨大的嵌套 `for` 循环。

```c#
for (int x = 0; x < workgroup_size_x; x++) {
  for (int y = 0; y < workgroup_size_y; y++) {
     for (int z = 0; z < workgroup_size_z; z++) {
        // Each workgroup runs independently and in parallel.
        for (int local_x = 0; local_x < invocation_size_x; local_x++) {
           for (int local_y = 0; local_y < invocation_size_y; local_y++) {
              for (int local_z = 0; local_z < invocation_size_z; local_z++) {
                 // Compute shader runs here.
              }
           }
        }
     }
  }
}
```

工作组和调用是一个高级主题。现在，请记住，我们将为每个工作组运行两次调用。

```c#
// A binding to the buffer we create in our script
layout(set = 0, binding = 0, std430) restrict buffer MyDataBuffer {
    float data[];
}
my_data_buffer;
```

在这里，我们提供了有关计算着色器可以访问的内存的信息。`layout` 属性允许我们告诉着色器在哪里查找缓冲区，我们稍后需要从 CPU 端匹配这些 `set` 和 `binding` 位置。

`restrict` 关键字告诉着色器只能从此着色器中的一个位置访问此缓冲区。换句话说，我们不会将此缓冲区绑定到另一个 `set` 或 `binding` 索引中。这很重要，因为它允许着色器编译器优化着色器代码。尽可能使用限制。

这是一个*无大小*的缓冲区，这意味着它可以是任何大小。因此，我们需要小心，不要从大于缓冲区大小的索引中读取。

```glsl
// The code we want to execute in each invocation
void main() {
    // gl_GlobalInvocationID.x uniquely identifies this invocation across all work groups
    my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}
```

最后，我们编写 `main` 函数，这是所有逻辑发生的地方。我们使用内置的 `gl_GlobalInvocationID` 变量访问存储缓冲区中的位置。`gl_GlobalInvocationID` 为当前调用提供全局唯一ID。

要继续，请将上述代码写入新创建的 `compute_example.glsl` 文件。

### 创建本地渲染设备

要与计算着色器交互并执行计算着色器，我们需要一个脚本。用您选择的语言创建一个新脚本，并将其附加到场景中的任何节点。

现在要执行着色器，我们需要一个本地 [RenderingDevice](https://docs.godotengine.org/en/stable/classes/class_renderingdevice.html#class-renderingdevice)，可以使用 [RenderingServer](https://docs.godotengine.org/en/stable/classes/class_renderingserver.html#class-renderingserver) 创建：

```gdscript
# Create a local rendering device.
var rd := RenderingServer.create_local_rendering_device()
```

```c#
// Create a local rendering device.
var rd = RenderingServer.CreateLocalRenderingDevice();
```

之后，我们可以加载新创建的着色器文件 `compute_example.glsl`，并使用以下命令创建它的预编译版本：

```gdscript
# Load GLSL shader
var shader_file := load("res://compute_example.glsl")
var shader_spirv: RDShaderSPIRV = shader_file.get_spirv()
var shader := rd.shader_create_from_spirv(shader_spirv)
```

```c#
// Load GLSL shader
var shaderFile = GD.Load<RDShaderFile>("res://compute_example.glsl");
var shaderBytecode = shaderFile.GetSpirV();
var shader = rd.ShaderCreateFromSpirV(shaderBytecode);
```

> **警告：**
>
> 无法使用 [RenderDoc](https://renderdoc.org/) 等工具调试本地 RenderingDevices。

### 提供输入数据

您可能还记得，我们想将一个输入数组传递给着色器，将每个元素乘以 2，得到结果。

我们需要创建一个缓冲区，将值传递给计算着色器。我们正在处理一个浮点数数组，因此我们将在本例中使用存储缓冲区。存储缓冲区采用字节数组，允许 CPU 与 GPU 之间传输数据。

因此，让我们初始化一个浮点数数组并创建一个存储缓冲区：

```gdscript
# Prepare our data. We use floats in the shader, so we need 32 bit.
var input := PackedFloat32Array([1, 2, 3, 4, 5, 6, 7, 8, 9, 10])
var input_bytes := input.to_byte_array()

# Create a storage buffer that can hold our float values.
# Each float has 4 bytes (32 bit) so 10 x 4 = 40 bytes
var buffer := rd.storage_buffer_create(input_bytes.size(), input_bytes)
```

```c#
// Prepare our data. We use floats in the shader, so we need 32 bit.
var input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var inputBytes = new byte[input.Length * sizeof(float)];
Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);

// Create a storage buffer that can hold our float values.
// Each float has 4 bytes (32 bit) so 10 x 4 = 40 bytes
var buffer = rd.StorageBufferCreate((uint)inputBytes.Length, inputBytes);
```

缓冲区就绪后，我们需要告诉渲染设备使用此缓冲区。为此，我们需要创建一个 uniform（就像在普通着色器中一样），并将其分配给一个 uniform 集，稍后我们可以将其传递给着色器。

```gdscript
# Create a uniform to assign the buffer to the rendering device
var uniform := RDUniform.new()
uniform.uniform_type = RenderingDevice.UNIFORM_TYPE_STORAGE_BUFFER
uniform.binding = 0 # this needs to match the "binding" in our shader file
uniform.add_id(buffer)
var uniform_set := rd.uniform_set_create([uniform], shader, 0) # the last parameter (the 0) needs to match the "set" in our shader file
```

```c#
// Create a uniform to assign the buffer to the rendering device
var uniform = new RDUniform
{
    UniformType = RenderingDevice.UniformType.StorageBuffer,
    Binding = 0
};
uniform.AddId(buffer);
var uniformSet = rd.UniformSetCreate(new Array<RDUniform> { uniform }, shader, 0);
```

### 定义计算管道

下一步是创建一组 GPU 可以执行的指令。为此，我们需要一个管道和一个计算列表。

我们需要做的计算结果的步骤是：

1. 创建新管道。
2. 开始列出 GPU 要执行的指令。
3. 将我们的计算列表绑定到我们的管道
4. 将我们的缓冲 uniform 绑定到我们的管道上
5. 指定要使用的工作组数量
6. 结束指令列表

```gdscript
# Create a compute pipeline
var pipeline := rd.compute_pipeline_create(shader)
var compute_list := rd.compute_list_begin()
rd.compute_list_bind_compute_pipeline(compute_list, pipeline)
rd.compute_list_bind_uniform_set(compute_list, uniform_set, 0)
rd.compute_list_dispatch(compute_list, 5, 1, 1)
rd.compute_list_end()
```

```c#
// Create a compute pipeline
var pipeline = rd.ComputePipelineCreate(shader);
var computeList = rd.ComputeListBegin();
rd.ComputeListBindComputePipeline(computeList, pipeline);
rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
rd.ComputeListDispatch(computeList, xGroups: 5, yGroups: 1, zGroups: 1);
rd.ComputeListEnd();
```

请注意，我们将计算着色器在 X 轴上分配 5 个工作组，在其他轴上分配一个工作组。由于我们在 X 轴上有 2 个本地调用（在着色器中指定），因此总共将启动 10 个计算着色器调用。如果读取或写入缓冲区范围之外的索引，则可能会访问着色器控制之外的内存或其他变量的部分，这可能会导致某些硬件出现问题。

### 执行计算着色器

在这一切之后，我们几乎完成了，但我们仍然需要执行我们的管道。到目前为止，我们只记录了我们希望 GPU 做什么；我们实际上还没有运行着色器程序。

要执行我们的计算着色器，我们需要将管道提交给 GPU 并等待执行完成：

```gdscript
# Submit to GPU and wait for sync
rd.submit()
rd.sync()
```

```c#
// Submit to GPU and wait for sync
rd.Submit();
rd.Sync();
```

理想情况下，您不会立即调用 `sync()` 来同步 RenderingDevice，因为这会导致 CPU 等待 GPU 完成工作。在我们的示例中，我们立即同步，因为我们希望我们的数据可以立即读取。一般来说，您希望在同步之前*至少*等待 2 或 3 帧，以便 GPU 能够与 CPU 并行运行。

> **警告：**
>
> 长时间的计算可能会导致 Windows 图形驱动程序“崩溃”，因为 TDR 是由 Windows 触发的。这是一种在经过一定时间后重新初始化图形驱动程序的机制，而图形驱动程序没有任何活动（通常为 5 到 10 秒）。

根据计算着色器执行的持续时间，您可能需要将其拆分为多个分派，以减少每个分派所需的时间并降低触发 TDR 的机会。鉴于 TDR 是时间相关的，与更快的 GPU 相比，运行给定计算着色器时，较慢的 GPU 可能更容易发生 TDR。

### 检索结果

您可能已经注意到，在示例着色器中，我们修改了存储缓冲区的内容。换句话说，着色器从我们的数组中读取数据，并再次将数据存储在同一个数组中，这样我们的结果就已经存在了。让我们检索数据并将结果打印到控制台。

```gdscript
# Read back the data from the buffer
var output_bytes := rd.buffer_get_data(buffer)
var output := output_bytes.to_float32_array()
print("Input: ", input)
print("Output: ", output)
```

```c#
// Read back the data from the buffers
var outputBytes = rd.BufferGetData(buffer);
var output = new float[input.Length];
Buffer.BlockCopy(outputBytes, 0, output, 0, outputBytes.Length);
GD.Print("Input: ", string.Join(", ", input));
GD.Print("Output: ", string.Join(", ", output));
```

有了这个，你就有了开始使用计算着色器所需的一切。

> **另请参见：**
>
> 演示项目存储库包含一个 [Compute Shader Heightmap 演示](https://github.com/godotengine/godot-demo-projects/tree/master/misc/compute_shader_heightmap)。该项目分别在 CPU 和 GPU 上执行高度贴图图像生成，这使您可以比较如何以两种不同的方式实现类似的算法（在大多数情况下，GPU 实现更快）。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**3 个评论**

[bitmammoth](https://github.com/bitmammoth) [Mar 19, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/17#discussioncomment-8835990)：

对于 c#，您可能需要指定 GodotCollections.Array 命名空间类型，以避免非泛型 system.collections 种类。



[Aman-Anas](https://github.com/Aman-Anas) [Jun 16, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/17#discussioncomment-9784442)

希望这些信息对在 C# 中使用大型数组的计算着色器的人有所帮助。我在网上找不到太多清晰的文档，尤其是 Godot 中的 C#。

虽然您仍然不能（在撰写本文时）将 `BufferGetData()` 调用到预先存在的已分配数组中（它是在内部分配的），但 C# 中有一些有用的方法可以避免在计算着色器工作流的其他部分进行内存分配和复制。这有助于提高性能和内存使用率。

替代使用可以在字节数组和浮点数组（或您自己的数据结构）之间进行转换的 `Buffer.BlockCopy()` 办法，可以考虑使用 `Span`s 和 `MemoryMarshal`。这里有一些例子。

> 小心这个力量！确保你的内存布局正确。BlockCopy 在大多数用例中都很快，只有在需要时才使用，例如您有大型数组。

从“提供输入数据”步骤，我们可以重新解释浮点数组。

```c#
var input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var inputBytes = MemoryMarshal.AsBytes(input.AsSpan());
```

或者，将数据存储为字节字段，并在添加数据之前对其进行强制转换（适用于某些用例）

```c#
var elements = 5;
var inputBytes = new byte[elements * sizeof(float)];
var input = MemoryMarshal.Cast<byte, float>(inputBytes);
input[0] = 1.0f; // Now inputBytes has its data changed!
```

另一个有用的技巧是将 `MemoryMarshal` 与结构体一起使用。这可以帮助清理 GLSL 代码并改进工作流程。

```c#
using System.Runtime.InteropServices;

// Pack= is how to do the alignment. 1 is the tightest,
// but make sure to check GLSL rules to make it match
// depending on your use case.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CoolData
{
    //  public fields may be bad practice, but using for simplicity
    public int wheels; 
    public float position;
    public float hoursOfSleep;
}
// as byte array (single struct)
byte[] coolDataBytes = new byte[Marshal.SizeOf<CoolData>()];
ref var myData = ref MemoryMarshal.AsRef<CoolData>(coolDataBytes);
myData.wheels = 4;
// You can also use MemoryMarshal.Cast() if you had many structs in an array.
// (For example, many structs generated on the GPU and returned with rd.BufferGetData())
```



[bkorkmaz](https://github.com/bkorkmaz) [Aug 21, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/17#discussioncomment-10407065)

着色器的一个非常简单的解释 ：）

```glsl
#[compute]
#version 450

// Compute Shader 可以处理三维数据。 
// 这基本上是一种用于 1D、2D 和 3D 数据结构的高效设计。
// 在我们的示例中，由于我们正在处理一个 1D 数组，因此我们只使用 X 轴。
//////////////////////////////////////////////////////////////////////////////////////////////// 
// 定义我们将在 GPU 上使用的轴。这里，只有 X 轴。X 轴上有 2 个线程。
layout(local_size_x = 2, local_size_y = 1, local_size_z = 1) in;

// 将我们的数据连接到 GPU。
// set = 数据类型
// binding = GPU 绑定点
// std430 = GPU 内存结构标准。
// restrict buffer MyDataBuffer = 出于优化目的，限制访问的缓冲区。
layout(set = 0, binding = 0, std430) restrict buffer MyDataBuffer {
    float data[];
}
my_data_buffer;

void main() {
    // 将数组的每个元素乘以 2，并将结果写回同一元素的位置。
    my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}
```

## 屏幕读取着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/screen-reading_shaders.html#screen-reading-shaders

### 引言

通常希望制作一个着色器，从它正在写入的同一屏幕读取。由于内部硬件的限制，OpenGL 或 DirectX 等 3D API 使这一点变得非常困难。GPU 非常并行，因此读写会导致各种缓存和一致性问题。因此，即使是最先进的硬件也无法正确支持这一点。

解决方法是将屏幕或屏幕的一部分复制到后缓冲区，然后在绘图时从中读取。Godot 提供了一些工具，使这一过程变得容易。

### 屏幕纹理

Godot [着色语言](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language)有一种特殊的纹理来访问屏幕上已经渲染的内容。在声明 `sampler2D` uniform 时，通过指定一个提示来使用它：`hint_screen_texture`。可以使用特殊的内置可变 `SCREEN_UV` 来获得当前片段相对于屏幕的 UV。因此，这个 canvas_item 片段着色器会产生一个不可见的对象，因为它只显示后面的内容：

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
    COLOR = textureLod(screen_texture, SCREEN_UV, 0.0);
}
```

这里使用 `textureLod` 是因为我们只想从底部 mipmap 读取。如果你想从模糊版本的纹理中读取，你可以将第三个参数增加到 `textureLod`，并将提示 `filter_nearest` 更改为 `filter_nearest_mipmap`（或启用 mipmaps 的任何其他过滤器）。如果使用带有 mipmaps 的过滤器，Godot 将自动为您计算模糊纹理。

> **警告：**
>
> 如果过滤模式未更改为名称中包含 `mipmap` 的过滤模式，则 LOD 参数大于 `0.0` 的 `textureLod` 将具有与 `0.0` LOD 参数相同的外观。

### 屏幕纹理示例

屏幕纹理可以用于许多事情。有一个专门的*屏幕空间着色器*演示，您可以下载观看和学习。一个例子是一个简单的着色器来调整亮度、对比度和饱和度：

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

uniform float brightness = 1.0;
uniform float contrast = 1.0;
uniform float saturation = 1.0;

void fragment() {
    vec3 c = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;

    c.rgb = mix(vec3(0.0), c.rgb, brightness);
    c.rgb = mix(vec3(0.5), c.rgb, contrast);
    c.rgb = mix(vec3(dot(vec3(1.0), c.rgb) * 0.33333), c.rgb, saturation);

    COLOR.rgb = c;
}
```

### 幕后故事

虽然这看起来很神奇，但事实并非如此。在 2D 中，当在即将绘制的节点中首次发现 `hint_screen_texture` 时，Godot 会将全屏复制到后缓冲区。在着色器中使用它的后续节点将不会为它们复制屏幕，因为这最终会导致效率低下。在 3D 中，屏幕是在不透明几何体通过之后但在透明几何体通过之前复制的，因此透明对象不会被捕获在屏幕纹理中。

因此，在 2D 中，如果使用 `hint_screen_texture` 的着色器重叠，第二个着色器将不会使用第一个着色器的结果，从而产生意想不到的视觉效果：

![../../_images/texscreen_demo1.png](https://docs.godotengine.org/en/stable/_images/texscreen_demo1.png)

在上图中，第二个球体（右上角)使用与下面第一个球体相同的屏幕纹理源，因此第一个球体“消失”或不可见。

在二维中，这可以通过 [BackBufferCopy](https://docs.godotengine.org/en/stable/classes/class_backbuffercopy.html#class-backbuffercopy) 节点进行纠正，该节点可以在两个球体之间实例化。BackBufferCopy 可以通过指定屏幕区域或整个屏幕来工作：

![../../_images/texscreen_bbc.png](https://docs.godotengine.org/en/stable/_images/texscreen_bbc.png)

通过正确的后缓冲区复制，两个球体可以正确混合：

![../../_images/texscreen_demo2.png](https://docs.godotengine.org/en/stable/_images/texscreen_demo2.png)

> **警告：**
>
> 在 3D 中，使用 `hint_screen_texture` 的材质本身被认为是透明的，不会出现在其他材质的最终屏幕纹理中。如果您计划实例化一个使用具有 `hint_screen_texture` 材质的场景，则需要使用 BackBufferCopy 节点。

在 3D 中，解决这个特定问题的灵活性较小，因为屏幕纹理只被捕获一次。在 3D 中使用屏幕纹理时要小心，因为它不会捕获透明对象，并且可能会使用屏幕纹理捕获对象前面的一些不透明对象。

通过在与对象相同的位置创建一个带有相机的[视口](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport)，然后使用[视口](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport)的纹理而不是屏幕纹理，可以在 3D 中再现后缓冲区逻辑。

### 后缓冲逻辑

因此，为了更清楚起见，以下是 Godot 中 2D 中的后缓冲区复制逻辑的工作原理：

- 如果一个节点使用 `hint_screen_texture`，则在绘制该节点之前，整个屏幕都会被复制到后缓冲区。这只是第一次；后续节点不会触发此操作。
- 如果在上述情况之前处理了 BackBufferCopy 节点（即使未使用 `hint_screen_texture`），则不会发生上述情况中描述的行为。换句话说，只有在节点中首次使用 `hint_screen_texture` 并且之前没有按树顺序找到 BackBufferCopy 节点（未禁用）的情况下，才会自动复制整个屏幕。
- BackBufferCopy 可以复制整个屏幕或一个区域。如果仅设置为一个区域（而不是整个屏幕），并且着色器使用的像素不在复制的区域中，则读取的结果是未定义的（很可能是前几帧的垃圾）。换句话说，可以使用 BackBufferCopy 复制屏幕的一个区域，然后从不同的区域读取屏幕纹理。避免这种行为！

### 深度纹理

对于 3D 着色器，还可以访问屏幕深度缓冲区。为此，使用 `hint_depth_texture` 提示。这种纹理不是线性的；必须使用逆投影矩阵对其进行转换。

以下代码检索正在绘制的像素下方的 3D 位置：

```glsl
uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_nearest;

void fragment() {
    float depth = textureLod(depth_texture, SCREEN_UV, 0.0).r;
    vec4 upos = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, depth, 1.0);
    vec3 pixel_position = upos.xyz / upos.w;
}
```

### 法线-粗糙度纹理

> **注：**
>
> 法线-粗糙度纹理仅在 Forward+ 渲染方法中受支持，不支持移动或兼容性。

同样，法线-粗糙度纹理可用于读取在深度预渲染中渲染的对象的法线和粗糙度。法线存储在 `.xyz` 通道中（映射到 0-1 范围），而粗糙度存储在 `.w` 通道中。

```glsl
uniform sampler2D normal_roughness_texture : hint_normal_roughness_texture, repeat_disable, filter_nearest;

void fragment() {
    float screen_roughness = texture(normal_roughness_texture, SCREEN_UV).w;
    vec3 screen_normal = texture(normal_roughness_texture, SCREEN_UV).xyz;
    screen_normal = screen_normal * 2.0 - 1.0;
```

### 重新定义屏幕纹理

屏幕纹理提示（`hint_screen_texture`、`hint_depth_texture` 和 `hint_normal_roughness_texture`）可以与多个 uniform 一起使用。例如，您可能希望使用不同的重复标志或过滤器标志多次读取纹理。

以下示例显示了一个着色器，该着色器使用线性过滤读取屏幕空间法线，但使用最近邻过滤读取屏幕区域粗糙度。

```glsl
uniform sampler2D normal_roughness_texture : hint_normal_roughness_texture, repeat_disable, filter_nearest;
uniform sampler2D normal_roughness_texture2 : hint_normal_roughness_texture, repeat_enable, filter_linear;

void fragment() {
    float screen_roughness = texture(normal_roughness_texture, SCREEN_UV).w;
    vec3 screen_normal = texture(normal_roughness_texture2, SCREEN_UV).xyz;
    screen_normal = screen_normal * 2.0 - 1.0;
```

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**3 个评论** 1 个回复



[DeveCout](https://github.com/DeveCout) [Sep 19, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-10692291)

我找不到测试镜头效果的测试项目。缺少链接还是怎么了？

​	[Calinou](https://github.com/Calinou) [Sep 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-10706855) Member

​	不幸的是，我认为这个测试项目没有在任何地方发布。此时，它必须从头开始重新创建。



[thompsop1sou](https://github.com/thompsop1sou) [Nov 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-11318374)

本文档中提到了 3D 透明对象的一个重大限制：

在 3D 中，屏幕是在不透明几何体通过之后但在透明几何体通过之前复制的，因此透明对象不会被捕获在屏幕纹理中。

这不仅会影响屏幕纹理，还会影响深度和法线-粗糙度纹理。

有几种方法可以解决这个问题，这样你就可以对透明对象进行后处理效果：

- 如果只使用屏幕纹理（不是深度或法线-粗糙度），请使用 2D 着色器。当您在 2D 着色器中访问屏幕纹理时，将显示透明对象。（我认为这是因为 3D 渲染已经全部完成。）这不适用于深度或法线-粗糙度纹理，因为这些纹理在 2D 着色器中不可用。
- 使用[合成器效果](https://docs.godotengine.org/en/stable/tutorials/rendering/compositor.html)。这是一个较低级别的解决方案，需要编写 GLSL 并像在计算着色器中一样传递数据。请注意，您需要将透明对象设置为“始终深度绘制”，以便它们显示在深度缓冲区中。目前，还没有办法让透明物体在法线-粗糙度纹理中显示出来。
- 您可以使用具有多个相机和视口的设置来手动捕获和传递纹理。设置起来有点复杂，但它最终会给你比合成器效果更多的控制权。您可以访问颜色、深度、法线或粗糙度数据，也可以传递自己的自定义数据。我在这里有一个示例项目：https://github.com/thompsop1sou/custom-screen-buffers



[SephReed](https://github.com/SephReed) [Nov 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-11319986)

有人能举一个使用屏幕空间着色器的 3d 场景的最小可行示例吗？它可能只是一个灰度着色器，但它会有很大的帮助。



## 使用 SubViewport 作为纹理

https://docs.godotengine.org/en/stable/tutorials/shaders/using_viewport_as_texture.html#using-a-subviewport-as-a-texture

### 引言

本教程将向您介绍如何将 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 用作可以应用于 3D 对象的纹理。为了做到这一点，它将引导您完成制作如下程序行星的过程：

![../../_images/planet_example.png](https://docs.godotengine.org/en/stable/_images/planet_example.png)

> **注：**
>
> 本教程不介绍如何编写像这个星球这样的动态大气。

本教程假设您熟悉如何设置基本场景，包括：[Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d)、[光源](https://docs.godotengine.org/en/stable/classes/class_omnilight3d.html#class-omnilight3d)、具有[基本网格（Primitive Mesh）](https://docs.godotengine.org/en/stable/classes/class_primitivemesh.html#class-primitivemesh)的 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d)，以及将 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d) 应用于网格。重点将放在使用 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 动态创建可以应用于网格的纹理上。

在本教程中，我们将介绍以下主题：

- 如何将 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 用作渲染纹理
- 使用等矩形映射将纹理映射到球体
- 程序行星的片段着色器技术
- 从[视口纹理](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)设置粗糙度贴图

### 设置场景

创建一个新场景，并添加以下节点，如下所示。

![../../_images/viewport_texture_node_tree.webp](https://docs.godotengine.org/en/stable/_images/viewport_texture_node_tree.webp)

进入 MeshInstance3D，将网格设置为 SphereMesh

### 设置 SubViewport

单击 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 节点并将其大小设置为 `(1024, 512)`。[SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 实际上可以是任何大小，只要宽度是高度的两倍。宽度需要是高度的两倍，这样图像才能准确地映射到球体上，因为我们将使用等矩形投影，但稍后会详细介绍。

接下来禁用 3D。我们将使用 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect) 渲染曲面，因此我们也不需要 3D。

![../../_images/planet_new_viewport.webp](https://docs.godotengine.org/en/stable/_images/planet_new_viewport.webp)

选择 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect)，然后在检查器中将锚点预设为 `Full Rect`。这将确保 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect) 占据整个 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport)。

![../../_images/planet_new_colorrect.webp](https://docs.godotengine.org/en/stable/_images/planet_new_colorrect.webp)

接下来，我们将[着色器材质](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial)添加到 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect) 中（ColorRect > CanvasItem > Material > Material > `New ShaderMaterial`)。

> **注：**
>
> 本教程建议您对着色有基本的了解。但是，即使您是着色器的新手，也会提供所有代码，因此您应该可以轻松理解。

单击着色器材质的下拉菜单按钮，然后单击/编辑。从这里转到“着色器”>“`新建着色器`”。给它起个名字，然后单击“创建”。在检查器中单击着色器以打开着色器编辑器。删除默认代码并添加以下内容：

```glsl
shader_type canvas_item;

void fragment() {
    COLOR = vec4(UV.x, UV.y, 0.5, 1.0);
}
```

保存着色器代码，您将在检查器中看到上面的代码渲染了一个类似下面的渐变。

![../../_images/planet_gradient.png](https://docs.godotengine.org/en/stable/_images/planet_gradient.png)

现在我们有了渲染的 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 的基础知识，我们有了一个可以应用于球体的独特图像。

### 应用纹理

现在进入 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d) 并向其中添加一个 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d)。不需要特殊的[着色器材质](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial)（尽管这对于更高级的效果来说是个好主意，比如上面例子中的氛围）。

MeshInstance3D > GeometryInstance > Geometry > Material Override > `New StandardMaterial3D`

然后单击 StandardMaterial3D 的下拉菜单，然后单击“编辑”

转到“资源”部分，选中“`本地到场景`”框。然后，转到“反照率”部分，单击“纹理”属性旁边的以添加反照率纹理。在这里，我们将应用我们制作的纹理。选择“新建视口纹理”

![../../_images/planet_new_viewport_texture.webp](https://docs.godotengine.org/en/stable/_images/planet_new_viewport_texture.webp)

单击您刚刚在检查器中创建的 ViewportTexture，然后单击“分配”。然后，从弹出的菜单中，选择我们之前渲染的 Viewport。

![../../_images/planet_pick_viewport_texture.webp](https://docs.godotengine.org/en/stable/_images/planet_pick_viewport_texture.webp)

现在，您的球体应该使用我们渲染到视口的颜色着色。

![../../_images/planet_seam.webp](https://docs.godotengine.org/en/stable/_images/planet_seam.webp)

注意到纹理环绕处形成的丑陋接缝了吗？这是因为我们根据 UV 坐标选取颜色，而 UV 坐标不会环绕纹理。这是二维地图投影中的一个经典问题。游戏开发人员通常有一个二维地图，他们想投影到球体上，但当它环绕时，它会有很大的接缝。对于这个问题，有一个优雅的解决方法，我们将在下一节中加以说明。

### 制作星球纹理

所以现在，当我们渲染到 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 时，它会神奇地出现在球体上。但是，我们的纹理坐标产生了一个丑陋的接缝。那么，我们如何得到一个以一种很好的方式围绕球体的坐标范围呢？一种解决方案是使用一个在纹理域上重复的函数。`sin` 和 `cos` 是两个这样的函数。让我们将它们应用于纹理，看看会发生什么。用以下内容替换着色器中的现有颜色代码：

```glsl
COLOR.xyz = vec3(sin(UV.x * 3.14159 * 4.0) * cos(UV.y * 3.14159 * 4.0) * 0.5 + 0.5);
```

![../../_images/planet_sincos.webp](https://docs.godotengine.org/en/stable/_images/planet_sincos.webp)

还不错。如果你环顾四周，你可以看到接缝现在已经消失了，但在它的位置上，我们挤压了极地。这种挤压是由于 Godot 在其 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d) 中将纹理映射到球体的方式造成的。它使用一种称为等矩形投影的投影技术，将球面图转换到二维平面上。

> **注：**
>
> 如果你对这项技术感兴趣，我们将把球坐标转换为笛卡尔坐标。球坐标表示球体的经度和纬度，而笛卡尔坐标实际上是从球体中心到该点的向量。

对于每个像素，我们将计算其在球体上的 3D 位置。由此，我们将使用 3D 噪声来确定颜色值。通过计算三维噪声，我们解决了极点处的夹紧问题。要理解原因，请想象一下在球体表面而不是二维平面上计算的噪声。当你在球体的整个表面进行计算时，你永远不会碰到边缘，因此你永远不会在极点上创建接缝或夹点。以下代码将 `UVs` 转换为笛卡尔坐标。

```glsl
float theta = UV.y * 3.14159;
float phi = UV.x * 3.14159 * 2.0;
vec3 unit = vec3(0.0, 0.0, 0.0);

unit.x = sin(phi) * sin(theta);
unit.y = cos(theta) * -1.0;
unit.z = cos(phi) * sin(theta);
unit = normalize(unit);
```

如果我们使用 `unit` 作为输出 `COLOR` 值，我们得到：

![../../_images/planet_normals.webp](https://docs.godotengine.org/en/stable/_images/planet_normals.webp)

现在我们可以计算球体表面的 3D 位置，我们可以使用 3D 噪波来制作行星。我们将直接从 [Shadertoy](https://www.shadertoy.com/view/Xsl3Dl) 中使用此噪声函数：

```glsl
vec3 hash(vec3 p) {
    p = vec3(dot(p, vec3(127.1, 311.7, 74.7)),
             dot(p, vec3(269.5, 183.3, 246.1)),
             dot(p, vec3(113.5, 271.9, 124.6)));

    return -1.0 + 2.0 * fract(sin(p) * 43758.5453123);
}

float noise(vec3 p) {
  vec3 i = floor(p);
  vec3 f = fract(p);
  vec3 u = f * f * (3.0 - 2.0 * f);

  return mix(mix(mix(dot(hash(i + vec3(0.0, 0.0, 0.0)), f - vec3(0.0, 0.0, 0.0)),
                     dot(hash(i + vec3(1.0, 0.0, 0.0)), f - vec3(1.0, 0.0, 0.0)), u.x),
                 mix(dot(hash(i + vec3(0.0, 1.0, 0.0)), f - vec3(0.0, 1.0, 0.0)),
                     dot(hash(i + vec3(1.0, 1.0, 0.0)), f - vec3(1.0, 1.0, 0.0)), u.x), u.y),
             mix(mix(dot(hash(i + vec3(0.0, 0.0, 1.0)), f - vec3(0.0, 0.0, 1.0)),
                     dot(hash(i + vec3(1.0, 0.0, 1.0)), f - vec3(1.0, 0.0, 1.0)), u.x),
                 mix(dot(hash(i + vec3(0.0, 1.0, 1.0)), f - vec3(0.0, 1.0, 1.0)),
                     dot(hash(i + vec3(1.0, 1.0, 1.0)), f - vec3(1.0, 1.0, 1.0)), u.x), u.y), u.z );
}
```

> **注：**
>
> 这一切都归功于作者 Inigo Quilez。它是在 `MIT` 的许可下出版的。

现在要使用 `noise`，请在 `fragment` 函数中添加以下内容：

```glsl
float n = noise(unit * 5.0);
COLOR.xyz = vec3(n * 0.5 + 0.5);
```

![../../_images/planet_noise.webp](https://docs.godotengine.org/en/stable/_images/planet_noise.webp)

> **注：**
>
> 为了突出显示纹理，我们将材质设置为无阴影。

现在你可以看到，噪音确实无缝地环绕着球体。虽然这看起来一点也不像你被承诺的星球。那么，让我们转向更丰富多彩的东西。

### 为星球着色

现在，让星球变彩色。虽然有很多方法可以做到这一点，但就目前而言，我们将坚持水和陆地之间的渐变。

为了在 GLSL 中制作渐变，我们使用了 `mix` 函数。`mix` 需要两个值进行插值，第三个参数选择在它们之间插值多少；本质上，它将这两种价值观混合在一起。在其他 API 中，此函数通常称为 `lerp`。然而，`lerp` 通常用于将两个浮点数混合在一起；`mix` 可以取任何值，无论是浮点数还是向量类型。

```glsl
COLOR.xyz = mix(vec3(0.05, 0.3, 0.5), vec3(0.9, 0.4, 0.1), n * 0.5 + 0.5);
```

第一种颜色是蓝色，代表海洋。第二种颜色是一种红色（因为所有外星行星都需要红色地形）。最后，它们通过 `n * 0.5 + 0.5` 混合在一起。`n` 在 `-1` 和 `1` 之间平滑变化。因此，我们将其映射到 `mix` 预期的 `0-1` 范围内。现在你可以看到颜色在蓝色和红色之间变化。

![../../_images/planet_noise_color.webp](https://docs.godotengine.org/en/stable/_images/planet_noise_color.webp)

这比我们想要的要模糊一点。行星通常在陆地和海洋之间有相对清晰的分隔。为了做到这一点，我们将把最后一项更改为 `smoothstep(-0.1, 0.0, n)`。因此，整行变成：

```glsl
COLOR.xyz = mix(vec3(0.05, 0.3, 0.5), vec3(0.9, 0.4, 0.1), smoothstep(-0.1, 0.0, n));
```

`smoothstep` 的作用是，如果第三个参数低于第一个参数，则返回 `0`，如果第二个参数大于第三个变量，则返回 `1`，如果第三数在第一个和第二个之间，则在 `0` 和 `1` 之间平滑混合。因此，在这一行中，当 `n` 小于 `-0.1` 时，`smoothstep` 返回 `0`，当 `n` 大于 `0` 时，它返回 `1`。

![../../_images/planet_noise_smooth.webp](https://docs.godotengine.org/en/stable/_images/planet_noise_smooth.webp)

还有一件事，让这里更像星球。土地不应该这么松软；让我们把边缘弄粗糙一点。着色器中经常使用的一种技巧是在不同频率下将不同级别的噪声叠加在一起，以制作带有噪声的粗糙地形。我们使用一层来制作大陆的整体块状结构。然后，另一层将边缘分解一点，然后是另一层，以此类推。我们要做的是用四行着色器代码而不是一行来计算 `n`。`n` 变为：

```glsl
float n = noise(unit * 5.0) * 0.5;
n += noise(unit * 10.0) * 0.25;
n += noise(unit * 20.0) * 0.125;
n += noise(unit * 40.0) * 0.0625;
```

现在星球看起来像：

![../../_images/planet_noise_fbm.webp](https://docs.godotengine.org/en/stable/_images/planet_noise_fbm.webp)

### 创造海洋

最后一件事让它看起来更像一颗行星。海洋和陆地反射光线的方式不同。所以我们希望海洋比陆地更明亮。我们可以通过将第四个值传递到输出 `COLOR` 的 `alpha` 通道并将其用作粗糙度贴图来实现这一点。

```glsl
COLOR.a = 0.3 + 0.7 * smoothstep(-0.1, 0.0, n);
```

这行对于水返回 `0.3`，对于陆地返回 `1.0`。这意味着土地将非常崎岖，而水将非常光滑。

然后，在材质的“金属”部分下，确保“`Metallic`”设置为 `0`，“`Specular`”设置为 `1`。原因是水很好地反射光线，但不是金属的。这些值在物理上并不准确，但对于这个演示来说已经足够了。

接下来，在“粗糙度”部分下，将粗糙度纹理设置为指向我们的行星纹理 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture) 的“[视口纹理](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)”。最后，将“`纹理通道`”设置为 `Alpha`。这指示渲染器将输出 `COLOR` 的 `alpha` 通道用作`粗糙度`值。

![../../_images/planet_ocean.webp](https://docs.godotengine.org/en/stable/_images/planet_ocean.webp)

你会注意到，除了地球不再反射天空之外，变化很小。这是因为，默认情况下，当使用 alpha 值渲染某物时，它会被绘制为背景上的透明对象。由于 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 的默认背景是不透明的，因此“[视口纹理](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)”的 `alpha` 通道为1，导致行星纹理的绘制颜色略微暗淡，各处的“`粗糙度`”值为 `1`。为了纠正这一点，我们进入 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 并启用“Transparent Bg”属性。由于我们现在正在渲染一个透明对象在另一个之上，我们希望启用`blend_premul_alpha`：

```glsl
render_mode blend_premul_alpha;
```

这会将颜色预先乘以 `alpha` 值，然后将它们正确地混合在一起。通常，当在另一种透明颜色上混合一种透明色时，即使背景的 `alpha` 为 `0`（就像在这种情况下一样），你最终也会遇到奇怪的颜色溢出问题。设置 `blend_premul_alpha` 可以解决这个问题。

现在，地球应该看起来像是在反射海洋上的光，而不是陆地上的光。在场景中围绕 [OmniLight3D](https://docs.godotengine.org/en/stable/classes/class_omnilight3d.html#class-omnilight3d) 移动，以便可以看到海洋反射的效果。

![../../_images/planet_ocean_reflect.webp](https://docs.godotengine.org/en/stable/_images/planet_ocean_reflect.webp)

这就是它。一个使用[子视口](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport)生成的程序行星。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论** 1 个回复



[rybern](https://github.com/rybern) [Nov 11, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/246#discussioncomment-11206823)

这是一个非常有用的教程，但我认为我可以看到两件事需要改进。记住，我是个初学者，所以我可能错了。

1. 您可以预先计算 3d 噪波纹理，并将其作为均匀纹理传递，而不是在着色器中计算。
2. 这可以通过空间着色器以相同的方式完成，但您根本不需要设置子视口。也许我错过了好处？

​	[tetrapod00](https://github.com/tetrapod00) [Nov 12, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/246#discussioncomment-11219617)

 	1. 是的！您可以预先计算噪声并将其传入。
 	2. 你说得对，复制这种特定效果不需要视口。它可以在单个空间着色器中完成。本教程是关于使用子视口作为纹理的，并且恰好使用了这个示例。

​	在某些时候，可以用一个使用 ViewportTexture 的实际*需求*示例重写此页面。如果你有任何想法，请随时在[文档仓库](https://github.com/godotengine/godot-docs/issues)中打开一个问题。



## 自定义后期处理

### 引言

Godot 提供了许多开箱即用的后处理效果，包括 Bloom、DOF 和 SSAO，这些效果在[环境和后处理](https://docs.godotengine.org/en/stable/tutorials/3d/environment_and_post_processing.html#doc-environment-and-post-processing)中进行了描述。但是，高级用例可能需要自定义效果。本文解释了如何编写自己的自定义效果。

实现自定义后处理着色器的最简单方法是使用 Godot 的内置功能从屏幕纹理读取。如果你对此不熟悉，你应该先阅读[屏幕读取着色器教程](https://docs.godotengine.org/en/stable/tutorials/shaders/screen-reading_shaders.html#doc-screen-reading-shaders)。

### 单通道后期处理

后处理效果是 Godot 渲染后应用于帧的着色器。要将着色器应用于帧，请创建 [CanvasLayer](https://docs.godotengine.org/en/stable/classes/class_canvaslayer.html#class-canvaslayer)，并为其赋予 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect)。为新创建的 `ColorRect` 指定一个新的 [ShaderMaterial](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial)，并将 `ColorRect` 的布局设置为“完全矩形”。

您的场景树看起来像这样：

![../../_images/post_tree1.png](https://docs.godotengine.org/en/stable/_images/post_tree1.png)

> **注：**
>
> 另一种更有效的方法是使用 [BackBufferCopy](https://docs.godotengine.org/en/stable/classes/class_backbuffercopy.html#class-backbuffercopy) 将屏幕的一个区域复制到缓冲区，并通过 `sampler2D` 使用 `hint_screen_texture` 在着色器脚本中访问它。

> **注：**
>
> 在撰写本文时，Godot 不支持同时渲染到多个缓冲区。后处理着色器将无法访问 Godot 未公开的其他渲染过程和缓冲区（如深度或法线/粗糙度）。您只能访问 Godot 作为采样器公开的渲染帧和缓冲区。

在这个演示中，我们将使用这种绵羊 [Sprite](https://docs.godotengine.org/en/stable/classes/class_sprite2d.html#class-sprite2d)。

![../../_images/post_example1.png](https://docs.godotengine.org/en/stable/_images/post_example1.png)

为 `ColorRect` 的`着色器材质`指定一个新的[着色器](https://docs.godotengine.org/en/stable/classes/class_shader.html#class-shader)。您可以使用 `hint_screen_texture` 和内置的 `SCREEN_UV` 制服，使用 `sampler2D` 访问帧的纹理和 UV。

将以下代码复制到着色器中。下面的代码是 [arlez80](https://bitbucket.org/arlez80/hex-mosaic/src/master/) 的十六进制像素化着色器，

```glsl
shader_type canvas_item;

uniform vec2 size = vec2(32.0, 28.0);
// If you intend to read from mipmaps with `textureLod()` LOD values greater than `0.0`,
// use `filter_nearest_mipmap` instead. This shader doesn't require it.
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
        vec2 norm_size = size * SCREEN_PIXEL_SIZE;
        bool half = mod(SCREEN_UV.y / 2.0, norm_size.y) / norm_size.y < 0.5;
        vec2 uv = SCREEN_UV + vec2(norm_size.x * 0.5 * float(half), 0.0);
        vec2 center_uv = floor(uv / norm_size) * norm_size;
        vec2 norm_uv = mod(uv, norm_size) / norm_size;
        center_uv += mix(vec2(0.0, 0.0),
                         mix(mix(vec2(norm_size.x, -norm_size.y),
                                 vec2(0.0, -norm_size.y),
                                 float(norm_uv.x < 0.5)),
                             mix(vec2(0.0, -norm_size.y),
                                 vec2(-norm_size.x, -norm_size.y),
                                 float(norm_uv.x < 0.5)),
                             float(half)),
                         float(norm_uv.y < 0.3333333) * float(norm_uv.y / 0.3333333 < (abs(norm_uv.x - 0.5) * 2.0)));

        COLOR = textureLod(screen_texture, center_uv, 0.0);
}
```

羊看起来像这样：

![../../_images/post_example2.png](https://docs.godotengine.org/en/stable/_images/post_example2.png)

### 多通道后期处理

一些后处理效果，如模糊，是资源密集型的。如果你把他们打散到多个通道中，你可以让他们运行得更快。在多通道材质中，每个通道都将前一通道的结果作为输入并对其进行处理。

要生成多通道后处理着色器，可以堆叠 `CanvasLayer` 和 `ColorRect` 节点。在上面的示例中，您使用 `CanvasLayer` 对象使用下面层上的帧渲染着色器。除了节点结构之外，步骤与单通道后处理着色器相同。

您的场景树看起来像这样：

![../../_images/post_tree2.png](https://docs.godotengine.org/en/stable/_images/post_tree2.png)

例如，您可以通过将以下代码附加到每个 `ColorRect` 节点来编写全屏高斯模糊效果。应用着色器的顺序取决于 `CanvasLayer` 在场景树中的位置，越高表示越快。对于这个模糊着色器，顺序并不重要。

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// Blurs the screen in the X-direction.
void fragment() {
    vec3 col = texture(screen_texture, SCREEN_UV).xyz * 0.16;
    col += texture(screen_texture, SCREEN_UV + vec2(SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(-SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(2.0 * SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(2.0 * -SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(3.0 * SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(3.0 * -SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(4.0 * SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.05;
    col += texture(screen_texture, SCREEN_UV + vec2(4.0 * -SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.05;
    COLOR.xyz = col;
}
```

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// Blurs the screen in the Y-direction.
void fragment() {
    vec3 col = texture(screen_texture, SCREEN_UV).xyz * 0.16;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, SCREEN_PIXEL_SIZE.y)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, -SCREEN_PIXEL_SIZE.y)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 2.0 * SCREEN_PIXEL_SIZE.y)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 2.0 * -SCREEN_PIXEL_SIZE.y)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 3.0 * SCREEN_PIXEL_SIZE.y)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 3.0 * -SCREEN_PIXEL_SIZE.y)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 4.0 * SCREEN_PIXEL_SIZE.y)).xyz * 0.05;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 4.0 * -SCREEN_PIXEL_SIZE.y)).xyz * 0.05;
    COLOR.xyz = col;
}
```

使用上述代码，您应该会得到如下全屏模糊效果。

![../../_images/post_example3.png](https://docs.godotengine.org/en/stable/_images/post_example3.png)

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论**



[SephReed](https://github.com/SephReed) [Nov 22, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/260#discussioncomment-11344504)

以下是对各种后处理方法及其优缺点的讨论：
[godotengine/godot#99491](https://github.com/godotengine/godot/issues/99491)