#region platform collision
	scrCheckPlatformCollision(width);
#endregion

#region Gravity
	scrCheckGravity(width);

	if (vspeed > 0) {
		scrCheckGroundCollision(width);
	} else if (vspeed < 0) {
		scrCheckCeilingCollision(width, height);
	}
#endregion

#region change sprite
	scrUpdateSprite();
#endregion