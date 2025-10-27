shader_type canvas_item;

//noise texture
uniform sampler2D noise_texture: repeat_enable, filter_nearest;
//fog density
uniform float density: hint_range(0.0, 1.0) = 0.25;
//fog speed
uniform vec2 speed = vec2(0.02, 0.01);

void fragment() {
	// Called for every pixel the material is visible on.

	//make the fog slowly move
	vec2 uv = UV + speed * TIME;
	// Sample the noise texture
	float noise = texture(noise_texture, uv).r;
	//conver noise from (0.0, 1.0) range to the (-1.0, 1.0) range
	float fog = noise * 2.0 - 1.0;
	// Apply the fog effect
	//COLOR.a *= fog;
	COLOR.a = noise;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
