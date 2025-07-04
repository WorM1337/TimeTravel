/// @description LOGIC

// CAMERA MOVEMENT
// get camera view coordenates
camX = camera_get_view_x(camInd)
camY = camera_get_view_y(camInd)
camW = camera_get_view_width(camInd);
camH = camera_get_view_height(camInd);
camXr = camW/2;
camYr = camH/1.4;


// shake camera
if !(shake_remain == 0)
{
	shake_x = random_range(-shake_remain, shake_remain);
	shake_y = random_range(-shake_remain, shake_remain);
	shake_remain = max(0, shake_remain-((1/shake_length)*shake_magnitude));
}


// follow target
if instance_exists(camTarget)
{
	if (camSmooth)
	{
		camTX = lerp(camTX, camTarget.x - camXr, camSmoothSpeed);
		camTY = lerp(camTY, camTarget.y - camYr, camSmoothSpeed);
	}
	else
	{
		camTX = camTarget.x - camXr;
		camTY = camTarget.y - camYr;
	}
}



// clamp and move camera
camTX = clamp(camTX, 0, room_width - camW);
camTY = clamp(camTY, 0, room_height - camH);
camera_set_view_size(camInd, camW, camH);
camera_set_view_pos(camInd, camTX+shake_x, camTY+shake_y);