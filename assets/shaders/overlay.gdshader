shader_type canvas_item;

uniform vec3 color;
uniform bool enabled = false;

void fragment() {
	vec4 initial_color = texture(TEXTURE, UV);
	if (enabled){
		COLOR = vec4(color.r, color.b, color.g, initial_color.a);
	}
}
