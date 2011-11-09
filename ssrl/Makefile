OSTYPE := $(shell uname)
libraw_path = ../LibRaw/lib/a
target_bin_R = ../bin/Release
target_bin_D = ../bin/Debug

COMMON_CPPFLAGS = -fopenmp -I"$(PWD)/include"

R_FLAGS = -O4
D_FLAGS = -O0 -g3

libraw_a_CPPFLAGS = -DLIBRAW_NOTHREADS $(COMMON_CPPFLAGS)

ifneq (,$(findstring MINGW32, $(OSTYPE)))			# OS type is MinGW 32
	# In Windows we need to link to libws2_32
	LIBS = -lws2_32 -lm
	# In Windows there is no pthread library
	libraw_r_CPPFLAGS = $(COMMON_CPPFLAGS)
	# In Windows shared library is a dll
	libssrl_shared = ssrl.dll
else												# any other case assumed as a POSIX system
	LIBS = -lm
	libraw_r_CPPFLAGS = -pthread $(COMMON_CPPFLAGS)
	libssrl_shared = libssrl.so
endif

all: debug release

debug: $(target_bin_D)/$(libssrl_shared)

release: $(target_bin_R)/$(libssrl_shared)

$(target_bin_D):
	mkdir -p $(target_bin_D)

$(target_bin_R):
	mkdir -p $(target_bin_R)

$(target_bin_D)/$(libssrl_shared): $(target_bin_D)
	g++ -shared ssrl.cpp -o $@ $(COMMON_CPPFLAGS) $(D_FLAGS) $(libraw_path)/libraw.a $(LIBS)  

$(target_bin_R)/$(libssrl_shared): $(target_bin_R)
	g++ -shared ssrl.cpp -o $@ $(COMMON_CPPFLAGS) $(R_FLAGS) $(libraw_path)/libraw.a $(LIBS)  
	