OSTYPE := $(shell uname)
libraw_r_OUT = ../LibRaw/lib/r
target_bin = ../bin/Release

COMMON_CPPFLAGS = -fopenmp -I"$(PWD)/include" -O4

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

all: $(target_bin)/$(libssrl_shared)

$(target_bin):
	mkdir -p $(target_bin)

$(target_bin)/$(libssrl_shared): $(target_bin)
	g++ -shared ssrl.cpp -o $@ $(COMMON_CPPFLAGS) $(libraw_r_OUT)/libraw.a $(LIBS)  
