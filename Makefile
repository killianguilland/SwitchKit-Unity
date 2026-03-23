dynamic_macos:
	clang++ -dynamiclib -undefined suppress -Iinclude src/imu.cpp src/switch_controller.cpp src/rumble.cpp src/joycon_report.cpp src/SwitchKit_UnityWrapper.cpp -I /opt/homebrew/opt/hidapi/include -L/opt/homebrew/opt/hidapi/lib -lhidapi -std=c++11 -o switchkit.dylib

dynamic_test_macos: dynamic_macos main.cpp
	clang++ -o joycon_test main.cpp libjoycon.dylib -Iinclude /opt/homebrew/opt/hidapi/lib/libhidapi.dylib -I /opt/homebrew/opt/hidapi/include -std=c++11

static_macos:
	clang++ -c -Iinclude src/imu.cpp src/switch_controller.cpp src/rumble.cpp src/joycon_report.cpp -I /opt/homebrew/opt/hidapi/include -std=c++11
	clang++ -r imu.o joycon_report.o rumble.o switch_controller.o -o switchkit.o

static_test_macos: static_macos
	clang++ -o joycon_test main.cpp switchkit.o -Iinclude /opt/homebrew/opt/hidapi/lib/libhidapi.dylib -I /opt/homebrew/opt/hidapi/include -std=c++11