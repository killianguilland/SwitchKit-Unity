#include "switch_controller.h"
#include <hidapi/hidapi.h>

using namespace SwitchKit;

extern "C" {

    struct SK_Vector2 {
        double x;
        double y;
    };

    struct SK_Vector3 {
        double x;
        double y;
        double z;
    };

    struct SK_Color24 {
        uint8_t r;
        uint8_t g;
        uint8_t b;
    };

    // Initialization
    __attribute__((visibility("default"))) int SwitchKit_Init() {
        return hid_init();
    }

    __attribute__((visibility("default"))) int SwitchKit_Exit() {
        return hid_exit();
    }

    // Opens a device and returns a pointer to the SwitchController
    __attribute__((visibility("default"))) void* SwitchKit_OpenController(uint16_t vendor_id, uint16_t product_id) {
        hid_device* handle = hid_open(vendor_id, product_id, nullptr);
        if (handle) {
            return new SwitchController(handle);
        }
        return nullptr;
    }

    __attribute__((visibility("default"))) void SwitchKit_CloseController(void* handle) {
        if (handle) {
            SwitchController* ctrl = static_cast<SwitchController*>(handle);
            delete ctrl;
        }
    }

    __attribute__((visibility("default"))) void SwitchKit_Poll(void* handle) {
        if (handle) {
            static_cast<SwitchController*>(handle)->poll();
        }
    }

    __attribute__((visibility("default"))) void SwitchKit_SetInputReportMode(void* handle, int mode) {
        if (handle) {
            static_cast<SwitchController*>(handle)->set_input_report_mode(static_cast<InputReportMode>(mode));
        }
    }

    // IMU
    __attribute__((visibility("default"))) void SwitchKit_SetIMUEnabled(void* handle, bool enabled) {
        if (handle) {
            static_cast<SwitchController*>(handle)->set_imu_enabled(enabled);
        }
    }

    __attribute__((visibility("default"))) SK_Vector3 SwitchKit_GetGyro(void* handle) {
        if (handle) {
            Vector3 g = static_cast<SwitchController*>(handle)->get_gyro();
            return { g.x, g.y, g.z };
        }
        return { 0, 0, 0 };
    }

    __attribute__((visibility("default"))) SK_Vector3 SwitchKit_GetAccel(void* handle) {
        if (handle) {
            Vector3 a = static_cast<SwitchController*>(handle)->get_accel();
            return { a.x, a.y, a.z };
        }
        return { 0, 0, 0 };
    }

    // Buttons
    __attribute__((visibility("default"))) bool SwitchKit_GetButton(void* handle, int button) {
        if (handle) {
            return static_cast<SwitchController*>(handle)->get_button(static_cast<SwitchControllerReport::Button>(button));
        }
        return false;
    }

    __attribute__((visibility("default"))) bool SwitchKit_GetButtonPressedThisFrame(void* handle, int button) {
        if (handle) {
            return static_cast<SwitchController*>(handle)->get_button_pressed_this_frame(static_cast<SwitchControllerReport::Button>(button));
        }
        return false;
    }

    // Sticks
    __attribute__((visibility("default"))) SK_Vector2 SwitchKit_GetStick(void* handle, int stick) {
        if (handle) {
            Vector2 s = static_cast<SwitchController*>(handle)->get_stick(static_cast<SwitchController::Stick>(stick));
            return { s.x, s.y };
        }
        return { 0, 0 };
    }

    // Requests
    __attribute__((visibility("default"))) void SwitchKit_RequestDeviceInfo(void* handle) {
        if (handle) static_cast<SwitchController*>(handle)->request_device_info();
    }

    __attribute__((visibility("default"))) void SwitchKit_RequestStickCalibration(void* handle) {
        if (handle) static_cast<SwitchController*>(handle)->request_stick_calibration();
    }

    __attribute__((visibility("default"))) void SwitchKit_RequestIMUCalibration(void* handle) {
        if (handle) static_cast<SwitchController*>(handle)->request_imu_calibration();
    }

    __attribute__((visibility("default"))) void SwitchKit_RequestColorData(void* handle) {
        if (handle) static_cast<SwitchController*>(handle)->request_color_data();
    }

    // Properties
    __attribute__((visibility("default"))) int SwitchKit_GetBatteryLevel(void* handle) {
        if (handle) return static_cast<int>(static_cast<SwitchController*>(handle)->get_battery_level());
        return 0;
    }

    __attribute__((visibility("default"))) bool SwitchKit_GetBatteryCharging(void* handle) {
        if (handle) return static_cast<SwitchController*>(handle)->get_battery_charging();
        return false;
    }

    __attribute__((visibility("default"))) int SwitchKit_GetControllerType(void* handle) {
        if (handle) return static_cast<int>(static_cast<SwitchController*>(handle)->get_controller_type());
        return 0;
    }

    __attribute__((visibility("default"))) SK_Color24 SwitchKit_GetColor(void* handle, int role) {
        if (handle) {
            Color24 c = static_cast<SwitchController*>(handle)->get_color(static_cast<SwitchController::ColorRole>(role));
            return { c.r, c.g, c.b };
        }
        return { 0, 0, 0 };
    }

    // Ring-Con
    __attribute__((visibility("default"))) void SwitchKit_EnableRingCon(void* handle) {
        if (handle) static_cast<SwitchController*>(handle)->enable_ringcon();
    }

    __attribute__((visibility("default"))) void SwitchKit_DisableRingCon(void* handle) {
        if (handle) static_cast<SwitchController*>(handle)->disable_ringcon();
    }

    __attribute__((visibility("default"))) bool SwitchKit_GetRingConConnected(void* handle) {
        if (handle) return static_cast<SwitchController*>(handle)->get_ringcon_connected();
        return false;
    }

    __attribute__((visibility("default"))) double SwitchKit_GetRingConFlex(void* handle) {
        if (handle) return static_cast<SwitchController*>(handle)->get_ringcon_flex();
        return 0.0;
    }

    // Player Lights
    __attribute__((visibility("default"))) void SwitchKit_SetPlayerLights(void* handle, int p1, int p2, int p3, int p4) {
        if (handle) {
            static_cast<SwitchController*>(handle)->set_player_lights(
                static_cast<SwitchController::PlayerLight>(p1),
                static_cast<SwitchController::PlayerLight>(p2),
                static_cast<SwitchController::PlayerLight>(p3),
                static_cast<SwitchController::PlayerLight>(p4)
            );
        }
    }

    // Rumble
    __attribute__((visibility("default"))) void SwitchKit_Rumble(void* handle, double left_low_freq, double left_low_amp, double left_high_freq, double left_high_amp, double right_low_freq, double right_low_amp, double right_high_freq, double right_high_amp) {
        if (handle) {
            HDRumbleConfig config;
            config.left.low.frequency = left_low_freq;
            config.left.low.amplitude = left_low_amp;
            config.left.high.frequency = left_high_freq;
            config.left.high.amplitude = left_high_amp;
            config.right.low.frequency = right_low_freq;
            config.right.low.amplitude = right_low_amp;
            config.right.high.frequency = right_high_freq;
            config.right.high.amplitude = right_high_amp;
            static_cast<SwitchController*>(handle)->rumble(config);
        }
    }
}
