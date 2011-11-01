MINGW_LIBS = -lws2_32 -lm
LINUX_LIBS = -lm

LIBS = $(LINUX_LIBS)

COMMON_CPPFLAGS = -fopenmp -I"$(PWD)"

libraw_a_CPPFLAGS = -DLIBRAW_NOTHREADS $(COMMON_CPPFLAGS)
libraw_r_CPPFLAGS = -pthread $(COMMON_CPPFLAGS)

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

all: library-a library-r samples-a samples-r


clean:
	rm -f $(libraw_a_OUT)/* $(objects_a_OUT)/* \
	      $(libraw_r_OUT)/* $(objects_r_OUT)/* \
	      $(samples_a_OUT)/* $(samples_r_OUT)/*


# ===== Library =====



# --- The thread-unsafe version ---

library-a: $(libraw_a_OUT)/libraw.a $(libraw_a_OUT)/libraw.so

$(libraw_a_OUT)/libraw.so: $(libraw_a_OBJECTS)
	g++ -shared -o $@ $(libraw_a_OBJECTS) $(LIBS)

$(libraw_a_OUT)/libraw.a: $(libraw_a_OBJECTS)
	ar crv $@ $(libraw_a_OBJECTS)
	ranlib $(libraw_a_OUT)/libraw.a
	          
$(objects_a_OUT)/dcraw_common.o: internal/dcraw_common.cpp
	g++ -c $(libraw_a_CPPFLAGS) internal/dcraw_common.cpp -o $@

$(objects_a_OUT)/dcraw_fileio.o: internal/dcraw_fileio.cpp
	g++ -c $(libraw_a_CPPFLAGS) internal/dcraw_fileio.cpp -o $@

$(objects_a_OUT)/demosaic_packs.o: internal/demosaic_packs.cpp
	g++ -c $(libraw_a_CPPFLAGS) internal/demosaic_packs.cpp -o $@

$(objects_a_OUT)/libraw_cxx.o: src/libraw_cxx.cpp
	g++ -c $(libraw_a_CPPFLAGS) src/libraw_cxx.cpp -o $@

$(objects_a_OUT)/libraw_datastream.o: src/libraw_datastream.cpp
	g++ -c $(libraw_a_CPPFLAGS) src/libraw_datastream.cpp -o $@

$(objects_a_OUT)/libraw_c_api.o: src/libraw_c_api.cpp
	g++ -c $(libraw_a_CPPFLAGS) src/libraw_c_api.cpp -o $@

# --- The thread-safe version ---
    
library-r: $(libraw_r_OUT)/libraw.a $(libraw_r_OUT)/libraw.so

$(libraw_r_OUT)/libraw.so: $(libraw_r_OBJECTS)
	g++ -shared -o $@ $(libraw_r_OBJECTS) $(LIBS)

$(libraw_r_OUT)/libraw.a: $(libraw_r_OBJECTS)
	ar crv $@ $(libraw_r_OBJECTS)
	ranlib $(libraw_r_OUT)/libraw.a
       
$(objects_r_OUT)/dcraw_common.o: internal/dcraw_common.cpp
	g++ -c $(libraw_r_CPPFLAGS) internal/dcraw_common.cpp -o $@

$(objects_r_OUT)/dcraw_fileio.o: internal/dcraw_fileio.cpp
	g++ -c $(libraw_r_CPPFLAGS) internal/dcraw_fileio.cpp -o $@

$(objects_r_OUT)/demosaic_packs.o: internal/demosaic_packs.cpp
	g++ -c $(libraw_r_CPPFLAGS) internal/demosaic_packs.cpp -o $@

$(objects_r_OUT)/libraw_cxx.o: src/libraw_cxx.cpp
	g++ -c $(libraw_r_CPPFLAGS) src/libraw_cxx.cpp -o $@

$(objects_r_OUT)/libraw_datastream.o: src/libraw_datastream.cpp
	g++ -c $(libraw_r_CPPFLAGS) src/libraw_datastream.cpp -o $@

$(objects_r_OUT)/libraw_c_api.o: src/libraw_c_api.cpp
	g++ -c $(libraw_r_CPPFLAGS) src/libraw_c_api.cpp -o $@



# ===== Samples =====



# --- Samples linked with non thread-safe library ---

samples-a: $(samples_a_OUT)/dcraw_emu $(samples_a_OUT)/4channels

$(samples_a_OUT)/4channels: samples/4channels.cpp $(libraw_a_OUT)/libraw.a
	g++ -static samples/4channels.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_a_OUT) -lraw $(LIBS)

$(samples_a_OUT)/dcraw_emu: samples/dcraw_emu.cpp $(libraw_a_OUT)/libraw.a
	g++ -static samples/dcraw_emu.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_a_OUT) -lraw $(LIBS)
	
# --- Samples linked with thread-safe library ---

samples-r: $(samples_r_OUT)/dcraw_emu $(samples_r_OUT)/4channels

$(samples_r_OUT)/4channels: samples/4channels.cpp $(libraw_r_OUT)/libraw.a
	g++ -static samples/4channels.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_r_OUT) -lraw $(LIBS)

$(samples_r_OUT)/dcraw_emu: samples/dcraw_emu.cpp $(libraw_r_OUT)/libraw.a
	g++ -static samples/dcraw_emu.cpp -o $@ $(COMMON_CPPFLAGS) -L$(libraw_r_OUT) -lraw $(LIBS)
	
