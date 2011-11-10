#ifndef SSRL_H_
#define SSRL_H_

#ifdef WIN32
#define DllDef __declspec(dllexport)
#else
#define DllDef
#endif


using namespace std;

struct ExtractedRawImage {
	int width;
	int height;
	int bitsPerChannel;
	void* data;
	libraw_processed_image_t* libraw_image;
};

struct ExtractedDescription
{
	void* data;
	int data_size;	// in bytes
	bool is_jpeg;

	float       iso_speed;
    float       shutter;
    float       aperture;
    float       focal_len;
    time_t      timestamp;
    unsigned    shot_order;
    unsigned*   gpsdata;
    char*       desc;
    char*       artist;
    char*       camera_maker;
    char*       camera_model;
    int         flip;

	libraw_processed_image_t* libraw_image;
};

typedef bool ExtractingProgressReporter(float progress);

extern "C"
{
	DllDef ExtractedRawImage ExtractRawImageFromFile(char* filename, bool divide_by_2, ExtractingProgressReporter* callback);
	DllDef int ExtractDescriptionFromFile(char* filename, ExtractedDescription* res);
	DllDef void FreeExtractedRawImage(ExtractedRawImage img);
	DllDef void FreeExtractedDescription(ExtractedDescription img);
}


#endif /* SSRL_H_ */
