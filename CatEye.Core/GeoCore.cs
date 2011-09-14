using System;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

namespace CatEye.Core
{
	public class SelfCrossingException : Exception
	{
		public SelfCrossingException(string message) : base(message) { }
	}
	public class ZeroVectorsAngleException : Exception
	{
		public ZeroVectorsAngleException(string message) : base(message) { }
	}
	public class PolygonNotConvexException : Exception
	{
		public PolygonNotConvexException(string message) : base(message) { }
	}
	
	public class Point
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private double _X, _Y;
		public double X { get { return _X; } }
		public double Y { get { return _Y; } }
		public Point(double x, double y)
		{
			_X = x; _Y = y;
		}
		
		public static implicit operator Vector(Point p)
		{
			return new Vector(p._X, p._Y);
		}
		
		public static Point Rotate(Point src, double angle, Point center)
		{
			double x = src.X - center.X, y = src.Y - center.Y;
			
			return new Point(
				x * Math.Cos(angle) - y * Math.Sin(angle) + center.X,
				y * Math.Cos(angle) + x * Math.Sin(angle) + center.Y
			);
		} 
		public System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Point");
			xn.Attributes.Append(xdoc.CreateAttribute("X")).Value = _X.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Y")).Value = _Y.ToString(nfi);
			return xn;
		}
		
		public void DeserializeFromXML (XmlNode node)
		{
			if (node.Name != "Point")
				throw new IncorrectNodeException("Node should have name \"Point\"");
			
			double res = 0;
			if (node.Attributes["X"] != null)
			{
				if (double.TryParse(node.Attributes["X"].Value, NumberStyles.Float, nfi, out res))
				{
					_X = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse X value");
			}
			if (node.Attributes["Y"] != null)
			{
				if (double.TryParse(node.Attributes["Y"].Value, NumberStyles.Float, nfi, out res))
				{
					_Y = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse Y value");
			}
		}
		
		public static Point Move(Point p, Vector v)
		{
			return new Point(p._X + v.X, p._Y + v.Y);
		}
		
		public static double Distance(Point p, Point q)
		{
			return Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
		}
	}
	
	public class Segment
	{
		private Point _A, _B;
		public Point A { get { return _A; } }
		public Point B { get { return _B; } }
		
		private Dictionary<Segment, Point> _Crossings = new Dictionary<Segment, Point>();
		
		public Segment HasCrossing(Point crossing)
		{
			foreach (Segment s in _Crossings.Keys)
			{
				if (_Crossings[s] == crossing) return s;
			}
			return null;
		}
		
		public Segment(Point a, Point b)
		{
			_A = a; _B = b;
		}
		
		public static Point GetCrossing(Segment s1, Segment s2)
		{
			// Algorithmic equality
			if (s1 == s2) 
				throw new SelfCrossingException("Trying to find selfcrossing");
			
			// TODO: Add rough estimation here (i.e. rectangle estimation)

			if (s1._Crossings.ContainsKey(s2)) 
				return s1._Crossings[s2];
			// TODO: Check integrity
			
			// Parallelism and zero-length
			Vector AB1 = new Vector(s1._B.X - s1._A.X, s1._B.Y - s1._A.Y);
			Vector AB2 = new Vector(s2._B.X - s2._A.X, s2._B.Y - s2._A.Y);
			double D = Vector.CrossProduct(AB2, AB1);
			if (Math.Abs(D) == 0) return null;
			
			if (s1._A == s2._A || s1._A == s2._B) return s1._A;
			if (s1._B == s2._A || s1._B == s2._B) return s1._B;
		
			// Calculating crossing between lines
			double m = Vector.CrossProduct((Vector)s1._A, AB1);
			double n = Vector.CrossProduct((Vector)s2._A, AB2);
			
			double x = (m * AB2.X - n * AB1.X) / D;
			double y = (m * AB2.Y - n * AB1.Y) / D;

			// Checking if an endpoint of one segment is contained by other
			if (s1._A.X == x && s1._A.Y == y) return s1._A; 
			if (s1._B.X == x && s1._B.Y == y) return s1._B; 
			if (s2._A.X == x && s2._A.Y == y) return s2._A; 
			if (s2._B.X == x && s2._B.Y == y) return s2._B; 
			
			// Checking if the crossing point is in both segments
			Vector AC1 = new Vector(x - s1._A.X, y - s1._A.Y);
			Vector BC1 = new Vector(x - s1._B.X, y - s1._B.Y);
			if ((AB1 * AC1) * (-AB1 * BC1) < 0) return null;
			
			Vector AC2 = new Vector(x - s2._A.X, y - s2._A.Y);
			Vector BC2 = new Vector(x - s2._B.X, y - s2._B.Y);
			if ((AB2 * AC2) * (-AB2 * BC2) < 0) return null;
			
			// Adding the point to crossing dictionaries
			Point crs = new Point(x, y);
			s1._Crossings.Add(s2, crs);
			s2._Crossings.Add(s1, crs);
			
			return crs;
		}
	}
	
	public class ConvexPolygon
	{
		private Point[] mVertex;
		private Segment[] mSides;
		private double mXMin, mXMax, mYMin, mYMax;
		
		public double XMin { get { return mXMin; } }
		public double XMax { get { return mXMax; } }
		public double YMin { get { return mYMin; } }
		public double YMax { get { return mYMax; } }
		
		public Point[] Vertex
		{
			get { return mVertex; } 
		}
		public Segment[] Sides
		{
			get { return mSides; } 
		}
		
		
		public ConvexPolygon(Point[] points)
		{
			if (points.Length < 3)
			{
				throw new ArgumentException("Too few points (" + points.Length + "). There should be at least 3 points to make a polygon");
			}
			// Checking convexity
			double sign_value = Vector.CrossProduct(
				(Vector)points[0] - (Vector)points[points.Length - 1],
				(Vector)points[1] - (Vector)points[points.Length - 1]);
			
			double sign_value2 = Vector.CrossProduct(
				(Vector)points[points.Length - 1] - (Vector)points[points.Length - 2],
				(Vector)points[0] - (Vector)points[points.Length - 2]);
			if (Math.Sign(sign_value) * Math.Sign(sign_value2) < 0) 
				throw new PolygonNotConvexException("Polygon isn't convex");
			
			for (int i = 0; i < points.Length - 2; i++)
			{
				double sign_value_i = Vector.CrossProduct(
					(Vector)points[i + 1] - (Vector)points[i],
					(Vector)points[i + 2] - (Vector)points[i]);
				if (Math.Sign(sign_value) * Math.Sign(sign_value_i) < 0) 
					throw new PolygonNotConvexException("Polygon isn't convex");
			}
			
			// Sorting points clockwise
			if (sign_value > 0)
				mVertex = points;
			else
			{
				mVertex = new Point[points.Length];
				for (int i = 0; i < points.Length; i++)
				{
					mVertex[i] = points[points.Length - 1 - i];
				}
			}
			
			// Constructing sides
			mSides = new Segment[mVertex.Length];
			mSides[mVertex.Length - 1] = new Segment(mVertex[mVertex.Length - 1], mVertex[0]);
			for (int i = 0; i < mVertex.Length - 1; i++)
				mSides[i] = new Segment(mVertex[i], mVertex[i + 1]);
			
			// Calculating bounding box
			mXMin = mVertex[0].X; mXMax = mVertex[0].X;
			mYMin = mVertex[0].Y; mYMax = mVertex[0].Y;
			
			for (int i = 1; i < mVertex.Length; i++)
			{
				if (mXMin > mVertex[i].X) mXMin = mVertex[i].X;
				if (mYMin > mVertex[i].Y) mYMin = mVertex[i].Y;

				if (mXMax < mVertex[i].X) mXMax = mVertex[i].X;
				if (mYMax < mVertex[i].Y) mYMax = mVertex[i].Y;
			}
		}
		
		public int IndexOfSideWithAEnd(Point A)
		{
			for (int i = 0; i < mSides.Length; i++)
				if (A == mSides[i].A) return i;
			return -1;
		}
		public int IndexOfSideWithBEnd(Point B)
		{
			for (int i = 0; i < mSides.Length; i++)
				if (B == mSides[i].B) return i;
			return -1;
		}
		public int IndexOfSide(Segment side)
		{
			for (int i = 0; i < mSides.Length; i++)
				if (mSides[i] == side) return i;
			return -1;
		}
		
		public bool HasCrossing(Point crossing, out Segment mySide, out Segment other)
		{
			mySide = null; other = null;
			for (int i = 0; i < mSides.Length; i++)
			{
				Segment res = mSides[i].HasCrossing(crossing);
				if (res != null)
				{
					mySide = mSides[i];
					other = res;
					return true;
				}
			}
			return false;
		}
		
		public double GetArea()
		{
			double res = 0;
			for (int i = 1; i < mVertex.Length - 1; i++)
			{
				Vector a = new Vector(mVertex[i].X - mVertex[0].X,
									  mVertex[i].Y - mVertex[0].Y);
				Vector b = new Vector(mVertex[i + 1].X - mVertex[0].X,
									  mVertex[i + 1].Y - mVertex[0].Y);
				res += Vector.CrossProduct(a, b);
			}
			res /= 2;
			return res;
		}
		
		public static bool BoundingBoxesAreCrossed(ConvexPolygon p1, ConvexPolygon p2)
		{
			if (p1.mXMin > p2.mXMax) return false;
			if (p2.mXMin > p1.mXMax) return false;
			
			if (p1.mYMin > p2.mYMax) return false;
			if (p2.mYMin > p1.mYMax) return false;
			return true;
		}
		
		public bool Contains(Point p)
		{
			Vector a0 = new Vector(
				mVertex[0].X - mVertex[mVertex.Length - 1].X,
				mVertex[0].Y - mVertex[mVertex.Length - 1].Y);
			Vector b0 = new Vector(
				p.X - mVertex[mVertex.Length - 1].X,
				p.Y - mVertex[mVertex.Length - 1].Y);
			if (Vector.CrossProduct(a0, b0) <= 0) return false;
			
			for (int i = 1; i < mVertex.Length; i++)
			{
				Vector a = new Vector(
					mVertex[i].X - mVertex[i - 1].X,
					mVertex[i].Y - mVertex[i - 1].Y);
				Vector b = new Vector(
					p.X - mVertex[i - 1].X,
					p.Y - mVertex[i - 1].Y);
				if (Vector.CrossProduct(a, b) <= 0) return false;
			}
			return true;
		}
		
		public double CalcProjectionToPixel(int i, int j, double quality)
		{
			double bloha = 0.00001;
			double part = 0;
			for (int qx = 0; qx < quality; qx++)
				for (int qy = 0; qy < quality; qy++)
			{
				double px = i + bloha + (double)qx / quality;
				double py = j + bloha + (double)qy / quality;
				if (Contains(new Point(px, py)))
				{
					part += 1.0;
				}
			}
			part /= quality * quality;
			return part;
		}
		
	}
	
    public class Vector
    {
        private double m_x, m_y;
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public Vector(double x, double y)
        {
            m_x = x;
            m_y = y;

        }

        /// <summary>
        /// "Длина" -- модуль вектора
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(m_x * m_x + m_y * m_y);
            }
        }

        /// <summary>
        /// Нуль-вектор
        /// </summary>
        public static Vector Zero
        {
            get
            {
                return new Vector(0, 0);
            }
        }

        /// <summary>
        /// Умножение вектора на число
        /// </summary>
        public static Vector operator *(Vector a, double k)
        {
            Vector res = a;
            res.m_x *= k;
            res.m_y *= k;
            return res;
        }

        /// <summary>
        /// Деление вектора на число
        /// </summary>
        public static Vector operator /(Vector a, double k)
        {
            return a * (1.0 / k);
        }

        /// <summary>
        /// Умножение числа на вектор
        /// </summary>
        public static Vector operator *(double k, Vector a)
        {
            return a * k;
        }

        /// <summary>
        /// Скалярное произведение
        /// </summary>
        public static double operator *(Vector a, Vector b)
        {
            return a.m_x * b.m_x + a.m_y * b.m_y;
        }

        /// <summary>
        /// Сложение двух векторов
        /// </summary>
        public static Vector operator +(Vector a, Vector b)
        {
            Vector res = a;
            res.m_x += b.m_x;
            res.m_y += b.m_y;
            return res;

        }

        /// <summary>
        /// Вычитание двух векторов
        /// </summary>
        public static Vector operator -(Vector a, Vector b)
        {
            Vector res = a;
            res.m_x -= b.m_x;
            res.m_y -= b.m_y;
            return res;

        }

        /// <summary>
        /// Унарный минус
        /// </summary>
        public static Vector operator -(Vector a)
        {
            return a * (-1.0);
        }

        /// <summary>
        /// Векторное произведение
        /// </summary>
        public static double CrossProduct(Vector a, Vector b)
        {
            return a.m_x * b.m_y - a.m_y * b.m_x;
        }

        /// <summary>
        /// Угол между векторами
        /// </summary>
        public static double Angle(Vector a, Vector b, double epsilon)
        {
            double q = Math.Asin(CrossProduct(a, b) / (a.Length * b.Length));
            if (a.Length < epsilon || b.Length < epsilon)
                throw new ZeroVectorsAngleException("Trying to find angle between zero-length vectors (epsilon = " + epsilon + ")");
            else
                return q;
        }

		public Vector(Point begin, Point end)
		{
			this.m_x = end.X - begin.X;
			this.m_y = end.Y - begin.Y;
		}
		
        public static implicit operator Point(Vector a)
        {
            return new Point(a.m_x, a.m_y);
        }

    }

}

