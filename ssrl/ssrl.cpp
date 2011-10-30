#include <libraw.h>
#include <math.h>

#include "ssrl.h"

int internal_callback(void *d, enum LibRaw_progress p, int iteration, int expected)
{
	ExtractingProgressReporter* callback = (ExtractingProgressReporter*)d;

	if ((*callback)((float)((log2(p) + (float)iteration / expected) / log2(LIBRAW_PROGRESS_STRETCH)) ))
    	return 0;	// Continue
    else
    	return 1;	// Cancel

}

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
/*	if ( LIBRAW_SUCCESS != (ret = RawProcessor.dcraw_ppm_tiff_writer(out_filename)))
	{
		fprintf(stderr,"Cannot write %s: %s\n", out_filename, libraw_strerror(ret));
	}*/

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

void FreeExtractedRawImage(ExtractedRawImage img)
{
	LibRaw::dcraw_clear_mem(img.libraw_image);
}
