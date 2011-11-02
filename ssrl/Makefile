OSTYPE := $(shell uname)
libraw_r_OUT = ../LibRaw/lib/r
target_bin = ../bin/Release

ifneq (,$(findstring MINGW32, $(OSTYPE)))	# OS type is MinGW 32
	# In Windows we need to link to libws2_32
	LIBS = -lws2_32 -lm
else										# any other case assumed as a POSIX system
	LIBS = -lm
endif

COMMON_CPPFLAGS = -fopenmp -I"$(PWD)/include"

libraw_a_CPPFLAGS = -DLIBRAW_NOTHREADS $(COMMON_CPPFLAGS)

ifneq (,$(findstring MINGW32, $(OSTYPE)))   # OS type is MinGW 32
	# In Windows there is no pthread library
	libraw_r_CPPFLAGS = $(COMMON_CPPFLAGS)
else										# any other case assumed as a POSIX system
	libraw_r_CPPFLAGS = -pthread $(COMMON_CPPFLAGS)
endif

ifneq (,$(findstring MINGW32, $(OSTYPE)))   # OS type is MinGW 32
	# In Windows shared library is a dll
	libssrl_shared = ssrl.dll
else										# any other case assumed as a POSIX system
	libssrl_shared = libssrl.so
endif

all: $(target_bin)/$(libssrl_shared)

$(target_bin):
	mkdir -p $(target_bin)

$(target_bin)/$(libssrl_shared): $(target_bin)
	g++ -static -shared ssrl.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_r_OUT) -lraw $(LIBS)  