using System;
using System.Xml;
using System.Globalization;

namespace CatEye.Core
{
	[StageOperationID("CrotateStageOperation")]
	public class CrotateStageOperationParameters : StageOperationParameters
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private double mAngle = 45;
		private Point mCenter = new Point(0.5, 0.5);
		private double mCropWidth = 1;
		private double mCropHeight = 1;
		private double mAspectRatio = 2.0 / 3;	// Width divided by height
		private CrotateStageOperation.Mode mMode = CrotateStageOperation.Mode.Disproportional;
		private CrotateStageOperation.Measure mMeasure = CrotateStageOperation.Measure.Percents;
		
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
				mCropWidth = value;
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

		public CrotateStageOperation.Mode Mode
		{
			get { return mMode; }
			set
			{
				mMode = value;
				OnChanged();
			}
		}

		public CrotateStageOperation.Measure Measure
		{
			get { return mMeasure; }
			set
			{
				mMeasure = value;
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
			xn.Attributes.Append(xdoc.CreateAttribute("Mode")).Value = ((int)mMode).ToString();
			xn.Attributes.Append(xdoc.CreateAttribute("Measure")).Value = ((int)mMeasure).ToString();
			
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
			if (node.Attributes["Mode"] != null)
			{
				if (int.TryParse(node.Attributes["Mode"].Value, out ires))
				{
					mMode = (CrotateStageOperation.Mode)ires;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Mode value");
			}
			if (node.Attributes["Measure"] != null)
			{
				if (int.TryParse(node.Attributes["Measure"].Value, out ires))
				{
					mMeasure = (CrotateStageOperation.Measure)ires;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Measure value");
			}
			
			OnChanged();
		}
	}
}

