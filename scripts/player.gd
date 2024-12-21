extends CharacterBody2D


const SPEED: float = 300.0


func _physics_process(_delta: float) -> void:
	var directionX := Input.get_axis("move_left", "move_right")
	var directionY := Input.get_axis("move_up", "move_down")
	var actual_speed := SPEED
	
	if directionX and directionY:
		actual_speed /= 1.35
	
	if directionX:
		velocity.x = directionX * actual_speed
	else:
		velocity.x = move_toward(velocity.x, 0, actual_speed)
		
	if directionY:
		velocity.y = directionY * actual_speed
	else:
		velocity.y = move_toward(velocity.y, 0 , actual_speed)

	move_and_slide()


func _input(event):
	if event is InputEventMouseButton:
		var camera: Camera2D = get_node("%MainCamera")
		if event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed:
			camera.set_zoom(camera.get_zoom() - Vector2(0.1, 0.1))
		if event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed:
			camera.set_zoom(camera.get_zoom() + Vector2(0.1, 0.1))