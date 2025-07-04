/// @description PARALLAX BG

// BACKGROUND PARALLAX MOVEMENT
if instance_exists(obj_par_camera)
{
	// higher division number, more slow
	if (room == Room1 || Room4)
	{
		layer_x("BG_Parallax_4",obj_par_camera.camX * 0.9);
		layer_x("BG_Parallax_3",obj_par_camera.camX * 0.8);
		layer_x("BG_Parallax_2",obj_par_camera.camX * 0.7);
		layer_x("BG_Parallax_1",obj_par_camera.camX * 0.6);
		layer_x("BG_Parallax_clouds",obj_par_camera.camX * 1.5 + current_time*0.005);
		layer_x("BG_Parallax_0",obj_par_camera.camX * 1.1);
	}
}