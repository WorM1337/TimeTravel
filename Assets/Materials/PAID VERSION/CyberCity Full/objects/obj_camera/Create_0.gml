/// @description INIT

// VARS
camInd = view_get_camera(0); //camera id
camTarget_default = objPlayer; //camera target object
///camFocus_object = obj_camfocus; //camera focus object
camSmoothSpeed_default = 0.15; //camera smooth speed
camSmooth_default = true; //camera smooth


// DEFAULT [NOT IMPORTANT]
alarm[0] = 1;
var _cxy = camera_center_target(camInd, camTarget_default);
camTarget = camTarget_default;
camSmoothSpeed = camSmoothSpeed_default;
camSmooth = camSmooth_default;
shake_magnitude = 0;
shake_length = 0;
shake_remain = 0;
shake_x = 0;
shake_y = 0;
camTX = _cxy[0];
camTY = _cxy[1];
camX = 0;
camY = 0;
camW = 0;
camH = 0;
camXr = 0;
camYr = 0;