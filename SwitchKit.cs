using System;
using System.Runtime.InteropServices;

namespace SwitchKit
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SK_Vector2
    {
        public double x;
        public double y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SK_Vector3
    {
        public double x;
        public double y;
        public double z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SK_Color24
    {
        public byte r;
        public byte g;
        public byte b;
    }

    public enum InputReportMode
    {
        MODE_ACTIVE_NFC_IR_POLLING = 0x00,
        MODE_ACTIVE_NFC_IR_POLLING_CONFIG = 0x01,
        MODE_ACTIVE_NFC_IR_POLLING_DATA_CONFIG = 0x02,
        MODE_ACTIVE_IR_CAMERA_POLLING = 0x03,
        MODE_MCU_UPDATE_STATE_REPORT_UNK = 0x23,
        MODE_STANDARD = 0x30,
        MODE_NFC_IR = 0x31,
        MODE_BASIC = 0x3F
    }

    public enum SwitchControllerType
    {
        CONTROLLER_L = 1,
        CONTROLLER_R = 2,
        CONTROLLER_PRO = 3
    }

    public enum PlayerLight
    {
        LIGHT_OFF = 0x00,
        LIGHT_ON = 0x01,
        LIGHT_FLASH = 0x10
    }

    public enum ColorRole
    {
        COLOR_BODY,
        COLOR_BUTTON,
        COLOR_LEFT_GRIP,
        COLOR_RIGHT_GRIP
    }

    public enum Stick
    {
        STICK_LEFT,
        STICK_RIGHT
    }

    public enum Button
    {
        BTN_A,
        BTN_B,
        BTN_X,
        BTN_Y,
        BTN_SR,
        BTN_SL,
        BTN_R,
        BTN_ZR,
        BTN_MINUS,
        BTN_PLUS,
        BTN_STICK_R,
        BTN_STICK_L,
        BTN_HOME,
        BTN_CAPTURE,
        BTN_CHARGING_GRIP,
        BTN_DOWN,
        BTN_UP,
        BTN_RIGHT,
        BTN_LEFT,
        BTN_L,
        BTN_ZL,
        BTN_RINGCON_FLEX
    }

    public enum BatteryLevel
    {
        BATTERY_EMPTY,
        BATTERY_CRITICAL,
        BATTERY_LOW,
        BATTERY_MEDIUM,
        BATTERY_FULL
    }

    public class SwitchController : IDisposable
    {
        private const string LibName = "switchkit";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SwitchKit_Init();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SwitchKit_Exit();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SwitchKit_OpenController(ushort vendor_id, ushort product_id);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_CloseController(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_Poll(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_SetInputReportMode(IntPtr handle, int mode);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_SetIMUEnabled(IntPtr handle, bool enabled);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SK_Vector3 SwitchKit_GetGyro(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SK_Vector3 SwitchKit_GetAccel(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SwitchKit_GetButton(IntPtr handle, int button);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SwitchKit_GetButtonPressedThisFrame(IntPtr handle, int button);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SK_Vector2 SwitchKit_GetStick(IntPtr handle, int stick);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_RequestDeviceInfo(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_RequestStickCalibration(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_RequestIMUCalibration(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_RequestColorData(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SwitchKit_GetBatteryLevel(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SwitchKit_GetBatteryCharging(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SwitchKit_GetControllerType(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SK_Color24 SwitchKit_GetColor(IntPtr handle, int role);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_EnableRingCon(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_DisableRingCon(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SwitchKit_GetRingConConnected(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern double SwitchKit_GetRingConFlex(IntPtr handle);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_SetPlayerLights(IntPtr handle, int p1, int p2, int p3, int p4);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SwitchKit_Rumble(IntPtr handle,
            double left_low_freq, double left_low_amp, double left_high_freq, double left_high_amp,
            double right_low_freq, double right_low_amp, double right_high_freq, double right_high_amp);


        private IntPtr handle = IntPtr.Zero;

        public static void Init()
        {
            SwitchKit_Init();
        }

        public static void Exit()
        {
            SwitchKit_Exit();
        }

        public SwitchController(ushort vendorId, ushort productId)
        {
            handle = SwitchKit_OpenController(vendorId, productId);
            if (handle == IntPtr.Zero)
            {
                throw new Exception("Could not open SwitchController. Please ensure the controller is paired and hidapi is working.");
            }
        }

        public void Poll()
        {
            if (handle != IntPtr.Zero) SwitchKit_Poll(handle);
        }

        public void SetInputReportMode(InputReportMode mode)
        {
            if (handle != IntPtr.Zero) SwitchKit_SetInputReportMode(handle, (int)mode);
        }

        public void SetIMUEnabled(bool enabled)
        {
            if (handle != IntPtr.Zero) SwitchKit_SetIMUEnabled(handle, enabled);
        }

        public SK_Vector3 GetGyro()
        {
            return handle != IntPtr.Zero ? SwitchKit_GetGyro(handle) : new SK_Vector3();
        }

        public SK_Vector3 GetAccel()
        {
            return handle != IntPtr.Zero ? SwitchKit_GetAccel(handle) : new SK_Vector3();
        }

        public bool GetButton(Button button)
        {
            return handle != IntPtr.Zero && SwitchKit_GetButton(handle, (int)button);
        }

        public bool GetButtonPressedThisFrame(Button button)
        {
            return handle != IntPtr.Zero && SwitchKit_GetButtonPressedThisFrame(handle, (int)button);
        }

        public SK_Vector2 GetStick(Stick stick)
        {
            return handle != IntPtr.Zero ? SwitchKit_GetStick(handle, (int)stick) : new SK_Vector2();
        }

        public void RequestDeviceInfo()
        {
            if (handle != IntPtr.Zero) SwitchKit_RequestDeviceInfo(handle);
        }

        public void RequestStickCalibration()
        {
            if (handle != IntPtr.Zero) SwitchKit_RequestStickCalibration(handle);
        }

        public void RequestIMUCalibration()
        {
            if (handle != IntPtr.Zero) SwitchKit_RequestIMUCalibration(handle);
        }

        public void RequestColorData()
        {
            if (handle != IntPtr.Zero) SwitchKit_RequestColorData(handle);
        }

        public BatteryLevel GetBatteryLevel()
        {
            return handle != IntPtr.Zero ? (BatteryLevel)SwitchKit_GetBatteryLevel(handle) : BatteryLevel.BATTERY_EMPTY;
        }

        public bool GetBatteryCharging()
        {
            return handle != IntPtr.Zero && SwitchKit_GetBatteryCharging(handle);
        }

        public SwitchControllerType GetControllerType()
        {
            return handle != IntPtr.Zero ? (SwitchControllerType)SwitchKit_GetControllerType(handle) : SwitchControllerType.CONTROLLER_PRO;
        }

        public SK_Color24 GetColor(ColorRole role)
        {
            return handle != IntPtr.Zero ? SwitchKit_GetColor(handle, (int)role) : new SK_Color24();
        }

        public void EnableRingCon()
        {
            if (handle != IntPtr.Zero) SwitchKit_EnableRingCon(handle);
        }

        public void DisableRingCon()
        {
            if (handle != IntPtr.Zero) SwitchKit_DisableRingCon(handle);
        }

        public bool GetRingConConnected()
        {
            return handle != IntPtr.Zero && SwitchKit_GetRingConConnected(handle);
        }

        public double GetRingConFlex()
        {
            return handle != IntPtr.Zero ? SwitchKit_GetRingConFlex(handle) : 0.0;
        }

        public void SetPlayerLights(PlayerLight p1, PlayerLight p2, PlayerLight p3, PlayerLight p4)
        {
            if (handle != IntPtr.Zero) SwitchKit_SetPlayerLights(handle, (int)p1, (int)p2, (int)p3, (int)p4);
        }

        public void Rumble(
            double left_low_freq, double left_low_amp, double left_high_freq, double left_high_amp,
            double right_low_freq, double right_low_amp, double right_high_freq, double right_high_amp)
        {
            if (handle != IntPtr.Zero)
            {
                SwitchKit_Rumble(handle, left_low_freq, left_low_amp, left_high_freq, left_high_amp,
                                 right_low_freq, right_low_amp, right_high_freq, right_high_amp);
            }
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                SwitchKit_CloseController(handle);
                handle = IntPtr.Zero;
            }
        }
    }
}
