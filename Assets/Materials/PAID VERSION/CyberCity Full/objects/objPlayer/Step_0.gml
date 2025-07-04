#region Horizontal Movement
	var hor = keyboard_check(ord("D")) - keyboard_check(ord("A"));

	if (hor != 0) {
		scrMovement(sign(hor), width);
		action = "Walk";
	} else if (vspeed == 0) {
		action = "Idle";
	}
#endregion

#region Jump
	if (keyboard_check_pressed(vk_space)) {
		scrJump(width);
	}
#endregion