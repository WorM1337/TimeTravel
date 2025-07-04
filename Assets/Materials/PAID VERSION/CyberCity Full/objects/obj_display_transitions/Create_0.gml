// VARS
tr_sprite = spr_transition_0; //transition sprite
tr_type = -1; //0 = start-out | 1 = end-in
tr_target_room = -1; //target room
tr_action = -1; //action to do when finish animation
tr_animation = 0; //what sprite subimage animation to use


// DEFAULT [NOT IMPORTANT]
shd_uni_Transitions_time = shader_get_uniform(sh_transition, "time");
transition_length[0] = 0.6; // seconds
transition_length[1] = 0.6;
transition_time[0] = 1;
transition_time[1] = 0; 