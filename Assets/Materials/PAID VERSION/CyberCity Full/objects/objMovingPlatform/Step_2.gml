
// MOVEMENT


// if there is a solid below, reverse
if place_meeting(x, y+vsp, obj_elevator_stop)
{
    vsp = 0;
}

// move
y += vsp;