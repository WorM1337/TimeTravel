
// START [Transition Out]
if (tr_type == 0)
{
	transition_time[0] -= 0.03;
	if (transition_time[0] <= 0)
	{
		event_user(0); //do actions
		transition_time[0] = 0;
	}
	
	shader_set(sh_transition);
	shader_set_uniform_f(shd_uni_Transitions_time, transition_time[0]/transition_length[0]);
	draw_sprite_stretched(tr_sprite, tr_animation, 0, 0 , 640, 360);
	shader_reset();
}


// END [Transition In]
if (tr_type == 1)
{
	transition_time[1] += 0.02;
	if (transition_time[1] >= 1)
	{
		event_user(0); //do actions
		transition_time[1] = 1;
	}
	
	shader_set(sh_transition);
	shader_set_uniform_f(shd_uni_Transitions_time, transition_time[1]/transition_length[1]);
	draw_sprite_stretched(tr_sprite, tr_animation, 0, 0 , 640, 360);
	shader_reset();
}