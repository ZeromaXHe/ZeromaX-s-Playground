@tool
extends Node

@export_range(1, 100) var divisions := 10
@export_range(1., 100.) var radius := 10.
@export var noise: FastNoiseLite

# 用于存放立方体扩大形成的球体网格的 MeshInstance3D 的父节点
@onready var mesh_ins_container: Node3D = %MeshInsContainer
# 用于放 2D TileMap 的 SubViewport，作为映射到球面上的地图
@onready var map_texture_viewport: SubViewport = %MapTextureViewport


func _ready() -> void:
	for child in mesh_ins_container.get_children():
		child.queue_free()
	var mesh = generate_mesh(divisions, radius)
	var mesh_ins = MeshInstance3D.new()
	mesh_ins.mesh = mesh
	var material = StandardMaterial3D.new()
	material.albedo_texture = map_texture_viewport.get_texture()
	mesh_ins.material_override = material
	mesh_ins_container.add_child(mesh_ins)
	if not Engine.is_editor_hint():
		mesh_ins.create_trimesh_collision()


func generate_mesh(divisions: int, radius: float) -> Mesh:
	var surface_tool = SurfaceTool.new()
	surface_tool.begin(Mesh.PRIMITIVE_TRIANGLES)
	surface_tool.set_smooth_group(-1)
	var quads: Array[Vector3] = [
		# 上
		Vector3(-1, 1, -1), # 上左前
		Vector3(1, 1, -1), # 上右前
		Vector3(-1, 1, 1), # 上左后
		Vector3(1, 1, 1), # 上右后
		# 下
		Vector3(-1, -1, 1), # 下左后
		Vector3(1, -1, 1), # 下右后
		Vector3(-1, -1, -1), # 下左前
		Vector3(1, -1, -1), # 下右前
		# 左
		Vector3(-1, 1, -1), # 上左前
		Vector3(-1, 1, 1), # 上左后
		Vector3(-1, -1, -1), # 下左前
		Vector3(-1, -1, 1), # 下左后
		# 右
		Vector3(1, 1, 1), # 上右后
		Vector3(1, 1, -1), # 上右前
		Vector3(1, -1, 1), # 下右后
		Vector3(1, -1, -1), # 下右前
		# 前
		Vector3(1, 1, -1), # 上右前
		Vector3(-1, 1, -1), # 上左前
		Vector3(1, -1, -1), # 下右前
		Vector3(-1, -1, -1), # 下左前
		# 后
		Vector3(-1, 1, 1), # 上左后
		Vector3(1, 1, 1), # 上右后
		Vector3(-1, -1, 1), # 下左后
		Vector3(1, -1, 1), # 下右后
	]
	for quad_i in quads.size() / 4:
		var v00 = quads[quad_i * 4]
		var v10 = quads[quad_i * 4 + 1]
		var v01 = quads[quad_i * 4 + 2]
		var v11 = quads[quad_i * 4 + 3]
		# 初始化网格（顶点和 UV）
		var tile_grid = TileGrid.new(surface_tool, quad_i * 4 * divisions * divisions,
			divisions, radius, noise, v00, v10, v01, v11)
		# 初始化面
		for i in divisions:
			for j in divisions:
				var tile = tile_grid.get_tile(i, j)
				# 初始化平面 - 左上三角面
				surface_tool.add_index(tile.i00)
				surface_tool.add_index(tile.i10)
				surface_tool.add_index(tile.i01)
				# 初始化平面 - 右下三角面
				surface_tool.add_index(tile.i11)
				surface_tool.add_index(tile.i01)
				surface_tool.add_index(tile.i10)
				# 初始化左侧立面
				if i > 0:
					var left_tile = tile_grid.get_tile(i - 1, j)
					if tile.height > left_tile.height:
						surface_tool.add_index(tile.i00)
						surface_tool.add_index(tile.i01)
						surface_tool.add_index(left_tile.i10)
						surface_tool.add_index(left_tile.i11)
						surface_tool.add_index(left_tile.i10)
						surface_tool.add_index(tile.i01)
					else:
						surface_tool.add_index(left_tile.i11)
						surface_tool.add_index(left_tile.i10)
						surface_tool.add_index(tile.i00)
						surface_tool.add_index(tile.i00)
						surface_tool.add_index(tile.i01)
						surface_tool.add_index(left_tile.i11)
				# 初始化上侧立面
				if j > 0:
					var up_tile = tile_grid.get_tile(i, j - 1)
					if tile.height > up_tile.height:
						surface_tool.add_index(tile.i10)
						surface_tool.add_index(tile.i00)
						surface_tool.add_index(up_tile.i01)
						surface_tool.add_index(up_tile.i01)
						surface_tool.add_index(up_tile.i11)
						surface_tool.add_index(tile.i10)
					else:
						surface_tool.add_index(up_tile.i01)
						surface_tool.add_index(up_tile.i11)
						surface_tool.add_index(tile.i10)
						surface_tool.add_index(tile.i10)
						surface_tool.add_index(tile.i00)
						surface_tool.add_index(up_tile.i01)
	surface_tool.generate_normals()
	return surface_tool.commit()


# 地块顶点索引信息，将四个点按 2D UV 坐标区分
class Tile:
	var i00: int = -1 # 左上角
	var i01: int = -1 # 左下角
	var i10: int = -1 # 右上角
	var i11: int = -1 # 右下角
	var height: float = randf_range(0.95, 1.05)


class TileGrid:
	var arr: Array[Tile] = []
	var divisions: int
	
	func _init(surface_tool: SurfaceTool, first_idx: int, div: int, radius: float, noise: FastNoiseLite,
			v00: Vector3, v10: Vector3, v01: Vector3, v11: Vector3):
		divisions = div
		var count = div + 1 # 每条边划分出的顶点个数（会是细分面数 + 1）
		var idx = first_idx
		arr.resize(div * div)
		for i in divisions: # i 对应列数
			var wi = i / float(div)
			var wi_1 = (i + 1) / float(div)
			var col_v00 = v00.lerp(v10, wi)
			var col_v10 = v00.lerp(v10, wi_1)
			var col_v01 = v01.lerp(v11, wi)
			var col_v11 = v01.lerp(v11, wi_1)
			for j in divisions: # j 对应行数
				var wj = j / float(div)
				var wj_1 = (j + 1) / float(div)
				var tile = Tile.new()
				var uv00 = Vector2(wi, wj)
				var uv10 = Vector2(wi_1, wj)
				var uv01 = Vector2(wi, wj_1)
				var uv11 = Vector2(wi_1, wj_1)
				var t_v00 = col_v00.lerp(col_v01, wj)
				var t_v10 = col_v10.lerp(col_v11, wj)
				var t_v01 = col_v00.lerp(col_v01, wj_1)
				var t_v11 = col_v10.lerp(col_v11, wj_1)
				var perlin = noise.get_noise_3dv((t_v00 + t_v01 + t_v10 + t_v11) / 4. * radius * radius)
				tile.height = 1. + 0.3 * perlin
				# 左上角 (0, 0)
				surface_tool.set_uv(uv00)
				surface_tool.add_vertex(_project_cube_vertex_to_sphere(t_v00, radius) * tile.height)
				tile.i00 = idx
				# 右上角 (1, 0)
				surface_tool.set_uv(uv10)
				surface_tool.add_vertex(_project_cube_vertex_to_sphere(t_v10, radius) * tile.height)
				tile.i10 = idx + 1
				# 左下角 (0, 1)
				surface_tool.set_uv(uv01)
				surface_tool.add_vertex(_project_cube_vertex_to_sphere(t_v01, radius) * tile.height)
				tile.i01 = idx + 2
				# 右下角 (1, 1)
				surface_tool.set_uv(uv11)
				surface_tool.add_vertex(_project_cube_vertex_to_sphere(t_v11, radius) * tile.height)
				tile.i11 = idx + 3
				idx += 4
				set_tile(i, j, tile)
	
	func _project_cube_vertex_to_sphere(p: Vector3, radius: float) -> Vector3:
		var x2 = p.x * p.x
		var y2 = p.y * p.y
		var z2 = p.z * p.z
		var x = p.x * sqrt(1. - (y2 + z2) / 2. + (y2 * z2) / 3.)
		var y = p.y * sqrt(1. - (z2 + x2) / 2. + (z2 * x2) / 3.)
		var z = p.z * sqrt(1. - (x2 + y2) / 2. + (x2 * y2) / 3.)
		return Vector3(x, y, z).normalized() * radius
	
	func get_tile(x: int, y: int) -> Tile:
		return arr[y * divisions + x]
	
	func set_tile(x: int, y: int, tile: Tile) -> void:
		arr[y * divisions + x] = tile
