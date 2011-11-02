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

typedef bool ExtractingProgressReporter(float progress);

extern "C"
{
	DllDef ExtractedRawImage ExtractRawImageFromFile(char* filename, bool divide_by_2, ExtractingProgressReporter* callback);
	DllDef void FreeExtractedRawImage(ExtractedRawImage img);
}


#endif /* SSRL_H_ */
