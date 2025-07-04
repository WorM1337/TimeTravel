function game_transition(target_room, tr_type, animation, action)
{
	/// @func game_transition(target_room, tr_type, animation, action)
	/// @param target_room
	/// @param tr_type
	/// @param animation
	/// @param action
	
	if !instance_exists(obj_display_transitions)
	{
		var L = instance_create_depth(0,0,-15000,obj_display_transitions);
		L.tr_target_room = target_room;
		L.tr_type = tr_type;
		L.tr_animation = animation;
		L.tr_action = action;
	}
}