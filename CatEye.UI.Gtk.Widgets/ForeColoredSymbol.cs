using System;
using System.Collections.Generic;
using Gtk;

namespace CatEye.UI.Gtk.Widgets
{
	public class ForeColoredSymbol : Bin
	{
		private Gdk.Pixbuf mSymbol = null;
		private Dictionary<StateType, Gdk.Pixbuf> mSymbolStates;
		
		public event EventHandler<EventArgs> SymbolChanged;

		public Gdk.Pixbuf Symbol
		{
			get { return mSymbol; }
			set
			{
				if (mSymbol != null) mSymbol.Dispose();
				mSymbol = (Gdk.Pixbuf)value.Clone();
				SetSizeRequest(mSymbol.Width, mSymbol.Height);
			}
		}
		
		protected override void OnStyleSet (Style previous_style)
		{
			RecreateSymbolStateImages();
		}
		
		protected virtual void OnSymbolChanged()
		{
			if (SymbolChanged != null) 
				SymbolChanged(this, EventArgs.Empty);
			RecreateSymbolStateImages();
		}
		
		protected unsafe void RecreateSymbolStateImages()
		{
			foreach (StateType st in Enum.GetValues(typeof(StateType)))
			{
				if (mSymbolStates.ContainsKey(st))
				{
					mSymbolStates[st].Dispose();
					mSymbolStates.Remove(st);
				}
				
				if (mSymbol != null)
				{
					Gdk.Color fore_color = this.Style.Foreground(st);
					Gdk.Pixbuf buf = (Gdk.Pixbuf)mSymbol.Clone();
					int h = buf.Height;
					int w = buf.Width;
					int stride = buf.Rowstride;
					
					int chan = buf.NChannels;
					
					byte *cur_row = (byte *)buf.Pixels;
					for (int j = 0; j < h; j++)
					{
						byte *cur_pixel = cur_row;
						for (int i = 0; i < w; i++)
						{
							byte r = (byte)(fore_color.Red / 256);
							byte g = (byte)(fore_color.Green / 256);
							byte b = (byte)(fore_color.Blue / 256);
							byte a = cur_pixel[3];
							
							cur_pixel[0] = r;
							cur_pixel[1] = g;
							cur_pixel[2] = b;
							cur_pixel[3] = a;
							
							cur_pixel += chan;
						}
						cur_row += stride;
					}
					mSymbolStates.Add(st, buf);
				}
			}
		}
		
		public ForeColoredSymbol ()
		{
			this.AddEvents((int)Gdk.EventMask.ExposureMask);
			mSymbolStates = new Dictionary<StateType, Gdk.Pixbuf>();
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			
			//base.OnExposeEvent (evnt);
			
			using (Gdk.GC gc = new Gdk.GC(evnt.Window))
			{
				if (mSymbolStates.ContainsKey(this.State))
				{
					int w, h;
					
					evnt.Window.GetSize(out w, out h);
					Gdk.Pixbuf pb = mSymbolStates[this.State];
					evnt.Window.DrawPixbuf(gc, pb, 0, 0,
						this.Allocation.X + this.Allocation.Width / 2 - pb.Width / 2, 
						this.Allocation.Y + this.Allocation.Height / 2 - pb.Height / 2, 
						pb.Width, pb.Height, 
						Gdk.RgbDither.None, 0, 0);
				}
			}
			return true;
		}
		
		public override void Dispose ()
		{
			if (mSymbol != null) mSymbol.Dispose();
			foreach (StateType st in Enum.GetValues(typeof(StateType)))
			{
				if (mSymbolStates.ContainsKey(st))
				{
					mSymbolStates[st].Dispose();
					mSymbolStates.Remove(st);
				}
			}
			base.Dispose ();
		}
	}
}

