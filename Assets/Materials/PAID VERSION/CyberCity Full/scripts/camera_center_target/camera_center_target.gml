
function camera_center_target(camera, target_object)
{
	/// @func camera_center_target(camera, target_object)
	/// @arg camera
	/// @arg target_object
	
	var _xx = target_object.x - camera_get_view_width(camera)/2;
	var _yy = target_object.y - camera_get_view_height(camera)/2;
	camera_set_view_pos(camera, _xx, _yy);
	
	return [_xx, _yy];
}