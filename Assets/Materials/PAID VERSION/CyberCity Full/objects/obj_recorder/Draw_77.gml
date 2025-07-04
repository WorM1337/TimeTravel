/// @description Rec

if (keyboard_check_pressed(ord("G")))
{
	gifRecord = !gifRecord;
	
	if (gifRecord)
	{
		gif = gif_open(480,320);
	}
	else
	{
		gif_save(gif, "Rec.gif");	
	}
	
}

if (gifRecord)
{
	gif_add_surface(gif,application_surface,2,0,0,2);	
}