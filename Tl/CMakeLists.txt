cmake_minimum_required(VERSION 3.8)
project(Tl)

set(CMAKE_CXX_STANDARD 11)

set(SOURCE_FILES main.cpp word.cpp word.h)
add_executable(Tl ${SOURCE_FILES})