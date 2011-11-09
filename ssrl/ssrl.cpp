#include <libraw/libraw.h>
#include <math.h>
#include <stdio.h>

#include "ssrl.h"

// ** Private functions **

int internal_callback(void *d, enum LibRaw_progress p, int iteration, int expected)
{
	ExtractingProgressReporter* callback = (ExtractingProgressReporter*)d;

	if ((*callback)((float)((log2(p) + (float)iteration / expected) / log2(LIBRAW_PROGRESS_STRETCH)) ))
    	return 0;	// Continue
    else
    	return 1;	// Cancel

}

// ** Public functions **

ExtractedRawImage ExtractRawImageFromFile(char* filename, bool divide_by_2, ExtractingProgressReporter* callback)
{
	ExtractedRawImage res;
	res.data = 0;	// data = 0 means "error during processing"

	LibRaw RawProcessor;

	RawProcessor.set_progress_handler(internal_callback,(void*)callback);

	RawProcessor.imgdata.params.gamm[0] = RawProcessor.imgdata.params.gamm[1] =
	                                      RawProcessor.imgdata.params.no_auto_bright = 1;
	RawProcessor.imgdata.params.output_bps = 16;
	RawProcessor.imgdata.params.highlight  = 9;
	RawProcessor.imgdata.params.threshold  = (float)100;

	if (divide_by_2)
	{
		RawProcessor.imgdata.params.half_size         = 1;
		RawProcessor.imgdata.params.four_color_rgb    = 1;
	}

	//int ret = RawProcessor.open_buffer(buffer, size);
	int ret = RawProcessor.open_file(filename, 1024 * 1024 * 1024);
	if (ret != LIBRAW_SUCCESS)
	{
		fprintf(stderr,"Cannot open the data buffer: %s\n", libraw_strerror(ret));
		return res; // no recycle b/c open_file will recycle itself
	}
	if ((ret = RawProcessor.unpack()) != LIBRAW_SUCCESS)
	{
		fprintf(stderr,"Cannot unpack the data: %s\n", libraw_strerror(ret));
		return res;
	}
	if (LIBRAW_SUCCESS != (ret = RawProcessor.dcraw_process()))
	{
		fprintf(stderr,"Cannot do postprocessing on the data: %s\n", libraw_strerror(ret));
		if(LIBRAW_FATAL_ERROR(ret))
			return res;
	}

    libraw_processed_image_t *image = RawProcessor.dcraw_make_mem_image(&ret);
    if (image == 0)
    {
    	return res;
    }

    res.bitsPerChannel = image->bits;
    res.width = image->width;
    res.height = image->height;
    res.data = image->data;
    res.libraw_image = image;

	RawProcessor.recycle(); // just for show this call

	return res;
}

int ExtractDescriptionFromFile(char* filename, ExtractedDescription* res)
{
	res->data = 0;	// data = 0 means "error during processing"

	LibRaw RawProcessor;

	int ret = RawProcessor.open_file(filename, 1024 * 1024 * 1024);
	if (ret != LIBRAW_SUCCESS)
	{
		fprintf(stderr,"Cannot open the data buffer: %s\n", libraw_strerror(ret));
		return 1; // no recycle b/c open_file will recycle itself
	}
	if ((ret = RawProcessor.unpack_thumb()) != LIBRAW_SUCCESS)
	{
		fprintf(stderr,"Cannot unpack the thumbnail data: %s\n", libraw_strerror(ret));
		return 2;
	}
    libraw_processed_image_t *image = RawProcessor.dcraw_make_mem_thumb(&ret);
    if (image == 0)
    {
    	return 3;
    }

    ret = RawProcessor.dcraw_thumb_writer("test.jpeg");
    if (ret != 0)
    {
    	return 10 + ret;
    }

    res->data = image->data;
    res->data_size = image->data_size;
    res->libraw_image = image;
    res->is_jpeg = image->type == LIBRAW_IMAGE_JPEG;

    // Extracting the other data
    res->aperture = RawProcessor.imgdata.other.aperture;
    res->shutter = RawProcessor.imgdata.other.shutter;
    res->focal_len = RawProcessor.imgdata.other.focal_len;
    res->iso_speed = RawProcessor.imgdata.other.iso_speed;
    res->shot_order = RawProcessor.imgdata.other.shot_order;
    res->timestamp = RawProcessor.imgdata.other.timestamp;

    res->artist = new char[64];
    strcpy(res->artist, RawProcessor.imgdata.other.artist);
    res->desc = new char[512];
    strcpy(res->desc, RawProcessor.imgdata.other.desc);
    res->gpsdata = new unsigned[32];
    memcpy(res->gpsdata, RawProcessor.imgdata.other.gpsdata, 32 * sizeof(unsigned int));

	RawProcessor.recycle(); // just for show this call

	return 0;
}

void FreeExtractedRawImage(ExtractedRawImage img)
{
	LibRaw::dcraw_clear_mem(img.libraw_image);
}

void FreeExtractedDescription(ExtractedDescription img)
{
	LibRaw::dcraw_clear_mem(img.libraw_image);
	delete[] img.artist;
	delete[] img.desc;
	delete[] img.gpsdata;
}
