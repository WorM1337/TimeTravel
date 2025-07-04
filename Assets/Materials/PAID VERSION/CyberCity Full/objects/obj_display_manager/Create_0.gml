
#region Resolution Setup

/*------------------------------------------------------
You probably don't need to touch it, unless you want to.
------------------------------------------------------*/

// init
window_set_fullscreen(false); // fullscreen
global.ideal_width = 640;
global.ideal_height = 360; //global.ideal_height = (display_get_height()/4);

// aspect ratio and zoom
aspect_ratio = display_get_width() / display_get_height();
game_zoom = 2.3;

// calculate new ideal width.
global.ideal_width = round(global.ideal_height * aspect_ratio);
//global.ideal_height = round(global.ideal_width/aspect_ratio);

// round values
global.ideal_width = round(global.ideal_width);
global.ideal_height = round(global.ideal_height);

// check to make sure our ideal width and height isn't an odd number
if(global.ideal_width & 1) global.ideal_width -= 1;
if(global.ideal_height & 1) global.ideal_height -= 1;

// setup all the view ports
global.Main_Camera = camera_create_view(0, 0, global.ideal_width, global.ideal_height, 0, noone, 0, 0, 0, 0);
camera_set_view_size(global.Main_Camera, global.ideal_width, global.ideal_height);

for (var i=0; i<=999; i+=1)
{
	if (room_exists(i))
	{
		room_set_camera(i, 0, global.Main_Camera);
		room_set_view_enabled(i, true);
		room_set_viewport(i, 0, true, 0, 0, global.ideal_width*game_zoom, global.ideal_height*game_zoom);
	}
}

// resize everything
surface_resize(application_surface, global.ideal_width, global.ideal_height);
display_set_gui_size(global.ideal_width, global.ideal_height);
window_set_size(global.ideal_width*game_zoom, global.ideal_height*game_zoom);
#endregion

alarm[0] = 1; // center window
