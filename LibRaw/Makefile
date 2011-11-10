OSTYPE := $(shell uname)

COMMON_CPPFLAGS = -fopenmp -I"$(PWD)" -O4 -static-libgcc -static-libstdc++

libraw_a_CPPFLAGS = -DLIBRAW_NOTHREADS $(COMMON_CPPFLAGS)

libraw_a_OUT = lib/a
libraw_r_OUT = lib/r

objects_a_OUT = obj/a
objects_r_OUT = obj/r

samples_a_OUT = bin/a
samples_r_OUT = bin/r

SOURCES = internal/dcraw_common.cpp \
          internal/dcraw_fileio.cpp \
          internal/demosaic_packs.cpp \
          src/libraw_cxx.cpp \
          src/libraw_datastream.cpp \
          src/libraw_c_api.cpp

libraw_a_OBJECTS = $(objects_a_OUT)/dcraw_common.o \
                   $(objects_a_OUT)/dcraw_fileio.o \
                   $(objects_a_OUT)/demosaic_packs.o \
                   $(objects_a_OUT)/libraw_cxx.o \
                   $(objects_a_OUT)/libraw_datastream.o \
                   $(objects_a_OUT)/libraw_c_api.o

libraw_r_OBJECTS = $(objects_r_OUT)/dcraw_common.o \
                   $(objects_r_OUT)/dcraw_fileio.o \
                   $(objects_r_OUT)/demosaic_packs.o \
                   $(objects_r_OUT)/libraw_cxx.o \
                   $(objects_r_OUT)/libraw_datastream.o \
                   $(objects_r_OUT)/libraw_c_api.o

ifneq (,$(findstring MINGW, $(OSTYPE)))			# OS type is MinGW 32
	# In Windows we need to link to libws2_32
	LIBS = -lws2_32 -lm
	# In Windows there is no pthread library
	libraw_r_CPPFLAGS = $(COMMON_CPPFLAGS)
	# In Windows shared library is a dll
	libraw_a_shared = raw.dll
	libraw_r_shared = raw.dll
else												# any other case assumed as a POSIX system
	LIBS = -lm
	libraw_r_CPPFLAGS = -pthread $(COMMON_CPPFLAGS)
	libraw_a_shared = libraw.so
	libraw_r_shared = libraw.so
endif



all: library-a library-r samples-a samples-r


# ===== Clean =====

clean: clean-a clean-r

clean-a:
	rm -f $(libraw_a_OUT)
	rm -rf $(objects_a_OUT)
	rm -rf $(samples_a_OUT)

clean-r:
	rm -rf $(libraw_r_OUT)
	rm -rf $(objects_r_OUT)
	rm -rf $(samples_r_OUT)

# ===== Library =====

# --- The thread-unsafe version ---

library-a: $(libraw_a_OUT)/libraw.a $(libraw_a_OUT)/$(libraw_a_shared)

$(libraw_a_OUT):
	mkdir -p $@

$(objects_a_OUT):
	mkdir -p $@
	
$(libraw_a_OUT)/$(libraw_a_shared): $(libraw_a_OBJECTS) $(libraw_a_OUT)
	g++ -shared -o $@ $(libraw_a_OBJECTS) $(LIBS)

$(libraw_a_OUT)/libraw.a: $(libraw_a_OBJECTS) $(libraw_a_OUT)
	ar crv $@ $(libraw_a_OBJECTS)
	ranlib $(libraw_a_OUT)/libraw.a
	          
$(objects_a_OUT)/dcraw_common.o: internal/dcraw_common.cpp $(objects_a_OUT)
	g++ -c $(libraw_a_CPPFLAGS) internal/dcraw_common.cpp -o $@

$(objects_a_OUT)/dcraw_fileio.o: internal/dcraw_fileio.cpp $(objects_a_OUT)
	g++ -c $(libraw_a_CPPFLAGS) internal/dcraw_fileio.cpp -o $@

$(objects_a_OUT)/demosaic_packs.o: internal/demosaic_packs.cpp $(objects_a_OUT)
	g++ -c $(libraw_a_CPPFLAGS) internal/demosaic_packs.cpp -o $@

$(objects_a_OUT)/libraw_cxx.o: src/libraw_cxx.cpp $(objects_a_OUT)
	g++ -c $(libraw_a_CPPFLAGS) src/libraw_cxx.cpp -o $@

$(objects_a_OUT)/libraw_datastream.o: src/libraw_datastream.cpp $(objects_a_OUT)
	g++ -c $(libraw_a_CPPFLAGS) src/libraw_datastream.cpp -o $@

$(objects_a_OUT)/libraw_c_api.o: src/libraw_c_api.cpp $(objects_a_OUT)
	g++ -c $(libraw_a_CPPFLAGS) src/libraw_c_api.cpp -o $@

# --- The thread-safe version ---
    
library-r: $(libraw_r_OUT)/libraw.a $(libraw_r_OUT)/$(libraw_r_shared)

$(libraw_r_OUT):
	mkdir -p $@

$(objects_r_OUT):
	mkdir -p $@

$(libraw_r_OUT)/$(libraw_r_shared): $(libraw_r_OBJECTS) $(libraw_r_OUT)
	g++ -shared -o $@ $(libraw_r_OBJECTS) $(LIBS)

$(libraw_r_OUT)/libraw.a: $(libraw_r_OBJECTS) $(libraw_r_OUT)
	ar crv $@ $(libraw_r_OBJECTS)
	ranlib $(libraw_r_OUT)/libraw.a
       
$(objects_r_OUT)/dcraw_common.o: internal/dcraw_common.cpp $(objects_r_OUT)
	g++ -c $(libraw_r_CPPFLAGS) internal/dcraw_common.cpp -o $@

$(objects_r_OUT)/dcraw_fileio.o: internal/dcraw_fileio.cpp $(objects_r_OUT)
	g++ -c $(libraw_r_CPPFLAGS) internal/dcraw_fileio.cpp -o $@

$(objects_r_OUT)/demosaic_packs.o: internal/demosaic_packs.cpp $(objects_r_OUT)
	g++ -c $(libraw_r_CPPFLAGS) internal/demosaic_packs.cpp -o $@

$(objects_r_OUT)/libraw_cxx.o: src/libraw_cxx.cpp $(objects_r_OUT)
	g++ -c $(libraw_r_CPPFLAGS) src/libraw_cxx.cpp -o $@

$(objects_r_OUT)/libraw_datastream.o: src/libraw_datastream.cpp $(objects_r_OUT)
	g++ -c $(libraw_r_CPPFLAGS) src/libraw_datastream.cpp -o $@

$(objects_r_OUT)/libraw_c_api.o: src/libraw_c_api.cpp $(objects_r_OUT)
	g++ -c $(libraw_r_CPPFLAGS) src/libraw_c_api.cpp -o $@



# ===== Samples =====



# --- Samples linked with non thread-safe library ---

samples-a: $(samples_a_OUT)/dcraw_emu $(samples_a_OUT)/4channels

$(samples_a_OUT):
	mkdir -p $@	

$(samples_a_OUT)/4channels: samples/4channels.cpp $(libraw_a_OUT)/libraw.a $(samples_a_OUT)
	g++ -static samples/4channels.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_a_OUT) -lraw $(LIBS)

$(samples_a_OUT)/dcraw_emu: samples/dcraw_emu.cpp $(libraw_a_OUT)/libraw.a $(samples_a_OUT)
	g++ -static samples/dcraw_emu.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_a_OUT) -lraw $(LIBS)
	
# --- Samples linked with thread-safe library ---

samples-r: $(samples_r_OUT)/dcraw_emu $(samples_r_OUT)/4channels

$(samples_r_OUT):
	mkdir -p $@	

$(samples_r_OUT)/4channels: samples/4channels.cpp $(libraw_r_OUT)/libraw.a $(samples_r_OUT)
	g++ -static samples/4channels.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_r_OUT) -lraw $(LIBS)

$(samples_r_OUT)/dcraw_emu: samples/dcraw_emu.cpp $(libraw_r_OUT)/libraw.a $(samples_r_OUT)
	g++ -static samples/dcraw_emu.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_r_OUT) -lraw $(LIBS)
	
