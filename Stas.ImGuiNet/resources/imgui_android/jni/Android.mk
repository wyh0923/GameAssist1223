TARGET_PLATFORM := android-7
LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)
LOCAL_ARM_MODE := arm
LOCAL_MODULE := ImguiAndroidWrapper
LOCAL_SRC_FILES += ImguiAndroidWrapper.cpp imgui/imgui.cpp imgui/imgui_draw.cpp imgui/imgui_demo.cpp imgui_impl_android.cpp
LOCAL_ARM_MODE := arm
COMMON_CFLAGS := -Werror -DANDROID -DDISABLE_IMPORTGL \
-isystem $(SYSROOT)/usr/include/
ifeq ($(TARGET_ARCH),x86)
LOCAL_CFLAGS := $(COMMON_CFLAGS)
else
LOCAL_CFLAGS := -mfpu=vfp -mfloat-abi=softfp \
-fno-short-enums $(COMMON_CFLAGS)
endif
LOCAL_LDLIBS := -llog -lGLESv2 -Wl,-s
LOCAL_CPPFLAGS += -std=gnu++0x
include $(BUILD_SHARED_LIBRARY)