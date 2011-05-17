using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("CrotateStageOperation")]
	public class CrotateStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private string[] mPresetAspectRatioNames = new string[]
		{
			"Photo album",
			"Photo portrait",
			"Screen album",
			"Screen portrait"
		};

		private double[] mPresetAspectRatioValues = new double[]
		{
			3.0 / 2, // Photo album
			2.0 / 3, // Photo portrait
			4.0 / 3, // Screen album
			3.0 / 4  // Screen portrait
		};

		private double mAngle = 45;
		private Point mCenter = new Point(0.5, 0.5);
		private double mCropWidth = 1;
		private double mCropHeight = 1;
		
		private int mAspectRatioPreset;
		private bool mAspectRatioCustom;
		private double mAspectRatio = 1;	// Width divided by height
		private CrotateStageOperation.Mode mMode = CrotateStageOperation.Mode.Disproportional;
		
		public double[] PresetAspectRatioValues
		{
			get { return mPresetAspectRatioValues; }
		}
		
		public string[] PresetAspectRatioNames
		{
			get { return mPresetAspectRatioNames; }
		}		
		
		public double Angle
		{
			get { return mAngle; }
			set
			{
				mAngle = value;
				OnChanged();
			}
		}
		
		public Point Center 
		{
			get { return mCenter; }
			set
			{
				mCenter = value;
				OnChanged();
			}
		}
		
		public double CropWidth
		{
			get { return mCropWidth; }
			set
			{
				mCropWidth = value;
				OnChanged();
			}
		}
		
		public double CropHeight
		{
			get { return mCropHeight; }
			set
			{
				mCropHeight = value;
				OnChanged();
			}
		}

		public int AspectRatioPreset
		{
			get { return mAspectRatioPreset; }
			set
			{
				mAspectRatioPreset = value;
				OnChanged();
			}
		}

		public double AspectRatio
		{
			get { return mAspectRatio; }
			set
			{
				mAspectRatio = value;
				OnChanged();
			}
		}
		
		public bool AspectRatioCustom
		{
			get { return mAspectRatioCustom; }
			set
			{
				mAspectRatioCustom = value;
				OnChanged();
			}
		}
		
		public CrotateStageOperation.Mode Mode
		{
			get { return mMode; }
			set
			{
				mMode = value;
				OnChanged();
			}
		}

		public CrotateStageOperationParameters ()
		{
		}
		
		public override System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = base.SerializeToXML (xdoc);
			xn.Attributes.Append(xdoc.CreateAttribute("Angle")).Value = mAngle.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("CropWidth")).Value = mCropWidth.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("CropHeight")).Value = mCropHeight.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("AspectRatio")).Value = mAspectRatio.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("AspectRatioPreset")).Value = mAspectRatioPreset.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("AspectRatioCustom")).Value = mAspectRatioCustom.ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Mode")).Value = ((int)mMode).ToString();
			
			xn.AppendChild(mCenter.SerializeToXML(xdoc)).Attributes.Append(xdoc.CreateAttribute("Name")).Value = "Center";
			return xn;
		}
		
		public override void DeserializeFromXML (XmlNode node)
		{
			base.DeserializeFromXML (node);
			foreach (XmlNode xn in node.ChildNodes)
			{
				if (xn.Name == "Point" && 
					(xn.Attributes["Name"] != null) && 
					(xn.Attributes["Name"].Value == "Center")
				) 
					mCenter.DeserializeFromXML(xn);
			}

			double res = 0;
			if (node.Attributes["Angle"] != null)
			{
				if (double.TryParse(node.Attributes["Angle"].Value, NumberStyles.Float, nfi, out res))
				{
					mAngle = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Angle value");
			}
			if (node.Attributes["CropWidth"] != null)
			{
				if (double.TryParse(node.Attributes["CropWidth"].Value, NumberStyles.Float, nfi, out res))
				{
					mCropWidth = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse CropWidth value");
			}
			if (node.Attributes["CropHeight"] != null)
			{
				if (double.TryParse(node.Attributes["CropHeight"].Value, NumberStyles.Float, nfi, out res))
				{
					mCropHeight = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse CropHeight value");
			}
			if (node.Attributes["AspectRatio"] != null)
			{
				if (double.TryParse(node.Attributes["AspectRatio"].Value, NumberStyles.Float, nfi, out res))
				{
					mAspectRatio = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AspectRatio value");
			}

			int ires;
			if (node.Attributes["AspectRatioPreset"] != null)
			{
				if (int.TryParse(node.Attributes["AspectRatioPreset"].Value, out ires))				{
					mAspectRatioPreset = ires;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AspectRatioPreset value");
			}
			if (node.Attributes["Mode"] != null)
			{
				if (int.TryParse(node.Attributes["Mode"].Value, out ires))
				{
					mMode = (CrotateStageOperation.Mode)ires;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Mode value");
			}
			
			bool bres;
			if (node.Attributes["AspectRatioCustom"] != null)
			{
				if (bool.TryParse(node.Attributes["AspectRatioCustom"].Value, out bres))				{
					mAspectRatioCustom = bres;
				}
				else
					throw new IncorrectNodeValueException("Can't parse AspectRatioCustom value");
			}
			OnChanged();
		}
	}
}

