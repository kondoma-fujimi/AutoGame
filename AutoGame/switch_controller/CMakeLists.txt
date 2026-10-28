# == DO NOT EDIT THE FOLLOWING LINES for the Raspberry Pi Pico VS Code Extension to work ==
if(WIN32)
    set(USERHOME $ENV{USERPROFILE})
else()
    set(USERHOME $ENV{HOME})
endif()
set(sdkVersion 2.1.1)
set(toolchainVersion 14_2_Rel1)
set(picotoolVersion 2.1.1)
set(picoVscode ${USERHOME}/.pico-sdk/cmake/pico-vscode.cmake)
if (EXISTS ${picoVscode})
    include(${picoVscode})
endif()
# ====================================================================================
set(PICO_BOARD pico CACHE STRING "Board type")

cmake_minimum_required(VERSION 3.13)
include(pico_sdk_import.cmake)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include)
include_directories(${PICO_SDK_PATH}/lib/tinyusb/src)
include_directories(src)
project(poke_pico)

pico_sdk_init()

add_executable(poke_pico main.c 
src/usb_descriptors.c
src/MPG.c
src/tusb_driver.c)

# インクルードパスの追加（必要なら）
# target_include_directories(poke_pico PRIVATE include)

target_link_libraries(poke_pico 
pico_stdlib 
hardware_pio
tinyusb_device
tinyusb_board
)

pico_add_extra_outputs(poke_pico)
